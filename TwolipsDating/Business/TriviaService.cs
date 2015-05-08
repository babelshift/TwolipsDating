using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TwolipsDating.Models;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Diagnostics;
using Dapper;
using System.Data.SqlClient;
using System.Collections.ObjectModel;

namespace TwolipsDating.Business
{
    internal class TriviaService : BaseService
    {
        /// <summary>
        /// Returns a random question that the user has not answered yet
        /// </summary>
        /// <returns></returns>
        internal async Task<Question> GetRandomQuestionAsync(string userId)
        {
            var questionsAlreadyAnswered = await (from questions in db.AnsweredQuestions
                                                  where questions.UserId == userId
                                                  select questions.QuestionId).ToListAsync();

            var questionList = await (from questions in db.Questions
                                      where !questionsAlreadyAnswered.Contains(questions.Id)
                                      where questions.QuestionTypeId.HasValue
                                      where questions.QuestionTypeId.Value == (int)QuestionTypeValues.Random
                                      select questions).ToListAsync();

            // there are no more questions that can be answered
            if (questionList.Count == 0)
            {
                return null;
            }

            Random random = new Random();
            int randomQuestionIndex = random.Next(0, questionList.Count);


            Question randomQuestion = questionList[randomQuestionIndex];

            return randomQuestion;
        }

        internal async Task<bool> IsAnswerCorrectAsync(int questionId, int answerId)
        {
            Debug.Assert(questionId > 0);
            Debug.Assert(answerId > 0);

            int? correctAnswerId = await (from questions in db.Questions
                                          where questions.Id == questionId
                                          select questions.CorrectAnswerId).FirstOrDefaultAsync();

            if (correctAnswerId.HasValue)
            {
                return correctAnswerId == answerId;
            }
            else
            {
                return false;
            }
        }

        internal async Task<int> RecordAnsweredQuestionAsync(string userId, int profileId, int questionId, int answerId, int questionTypeId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));
            Debug.Assert(profileId > 0);
            Debug.Assert(questionId > 0);
            Debug.Assert(answerId > 0);
            Debug.Assert(questionTypeId > 0);

            // log the answered question
            AnsweredQuestion answeredQuestion = new AnsweredQuestion()
            {
                AnswerId = answerId,
                DateAnswered = DateTime.Now,
                QuestionId = questionId,
                UserId = userId
            };

            db.AnsweredQuestions.Add(answeredQuestion);
            int changes = await db.SaveChangesAsync();

            // get the user's question points
            int points = await GetUsersQuestionPointsAsync(userId, questionTypeId);

            // get the question milestones
            var milestones = await GetMilestonesAsync();

            // check if user has already achieved the milestone
            var milestoneIdsAchieved = await GetMilestoneIdsAchievedAsync(userId);
            foreach (var milestone in milestones)
            {
                // if the milestone is a question answered type and the user has enough points to satisfy the milestone
                if (milestone.MilestoneTypeId == (int)MilestoneTypeValues.QuestionAnsweredCorrectly 
                    && points > milestone.PointsRequired)
                {
                    // if the user has not achieved this milestone
                    int milestoneIdAchieved = 0;
                    if (!milestoneIdsAchieved.TryGetValue(milestone.Id, out milestoneIdAchieved))
                    {
                        // award the milestone
                        await AwardMilestoneToUserAsync(userId, milestoneIdAchieved);
                    }
                }
            }

            // get the user's total points (questions, quizzes, games)
            // do something if they hit a total milestone

            // if user has 5 tag questions answered, give them the 'intellectual' badge
            var tagsForAnsweredQuestions = await GetTagsForAnsweredQuestionsAsync(userId, questionTypeId);

            foreach (var tag in tagsForAnsweredQuestions)
            {
                if (tag.Count % 5 == 0) // multiple of 5 = award the tag
                {
                    await AwardTagToProfileAsync(profileId, tag.TagId);
                }
            }

            return changes;
        }

        private async Task<int> AwardMilestoneToUserAsync(string userId, int milestoneId)
        {
            MilestoneAchievement achievement = new MilestoneAchievement()
            {
                UserId = userId,
                MilestoneId = milestoneId,
                DateAchieved = DateTime.Now
            };

            db.MilestoneAchievements.Add(achievement);

            return await db.SaveChangesAsync();
        }

        private async Task<IReadOnlyDictionary<int, int>> GetMilestoneIdsAchievedAsync(string userId)
        {
            var milestonesAchieved = from milestoneAchieved in db.MilestoneAchievements
                                     where milestoneAchieved.UserId == userId
                                     select milestoneAchieved.MilestoneId;

            var results = await milestonesAchieved.ToDictionaryAsync(t => t);

            return new ReadOnlyDictionary<int, int>(results);
        }

        private async Task<IReadOnlyCollection<Milestone>> GetMilestonesAsync()
        {
            var milestones = from milestone in db.Milestones
                             select milestone;

            var results = await milestones.ToListAsync();

            return results.AsReadOnly();
        }

        private async Task<int> AwardTagToProfileAsync(int profileId, int tagId)
        {
            Debug.Assert(profileId > 0);
            Debug.Assert(tagId > 0);

            TagAward tagAward = new TagAward()
            {
                DateAwarded = DateTime.Now,
                ProfileId = profileId,
                TagId = tagId
            };

            db.TagAwards.Add(tagAward);
            return await db.SaveChangesAsync();
        }

        private async Task<IReadOnlyCollection<QuestionTagCount>> GetTagsForAnsweredQuestionsAsync(string userId, int questionTypeId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));
            Debug.Assert(questionTypeId > 0);

            string sql = @"
                select t.TagId, count(t.TagId) as Count
                from dbo.AnsweredQuestions as aq
                inner join dbo.Questions as q on aq.QuestionId = q.Id
                inner join dbo.TagQuestions as tq on q.Id = tq.Question_Id
                inner join dbo.Tags as t on tq.Tag_TagId = t.TagId
                where q.QuestionTypeId = @questionTypeId
                and aq.UserId = @userId
                group by t.TagId
            ";

            var results = await QueryAsync<QuestionTagCount>(sql, new { questionTypeId = questionTypeId, userId = userId });

            return results.ToList().AsReadOnly();
        }

        internal async Task<int> GetUsersQuestionPointsAsync(string userId, int questionTypeId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));
            Debug.Assert(questionTypeId > 0);

            string sql = @"
                select sum(points) as points
                from dbo.AnsweredQuestions as aq
                inner join dbo.Questions as q on aq.QuestionId = q.Id
                where q.QuestionTypeId = @questionTypeId
                and aq.UserId = @userId
            ";

            int? points = 0;

            var results = await QueryAsync<int>(sql, new { questionTypeId = questionTypeId, userId = userId });

            points = results.FirstOrDefault();

            if (!points.HasValue)
            {
                points = 0;
            }

            return points.Value;
        }

        private async Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters)
        {
            using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                connection.Open();
                var results = await db.Database.Connection.QueryAsync<T>(sql, parameters);
                connection.Close();
                return results;
            }
        }
    }
}