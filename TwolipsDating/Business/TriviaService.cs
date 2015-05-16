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
        internal async Task<IReadOnlyCollection<Quiz>> GetQuizzesAsync()
        {
            var quizzes = from quiz in db.Quizzes
                          select quiz;

            var results = await quizzes.ToListAsync();

            return results.AsReadOnly();
        }

        internal async Task<Question> GetRandomQuizQuestionAsync(string userId, int quizId)
        {
            return await GetRandomQuestionAsync(userId, (int)QuestionTypeValues.Quiz, quizId);
        }

        /// <summary>
        /// Returns a random question that the user has not answered yet
        /// </summary>
        /// <returns></returns>
        internal async Task<Question> GetRandomQuestionAsync(string userId, int questionTypeId, int? quizId = null)
        {
            List<int> questionsAlreadyAnswered = new List<int>();
            List<Question> questionList = new List<Question>();

            questionsAlreadyAnswered = await (from questions in db.AnsweredQuestions
                                              where questions.UserId == userId
                                              select questions.QuestionId).ToListAsync();

            if (quizId.HasValue)
            {
                questionList = await (from questions in db.Questions
                                      from quiz in db.Quizzes
                                      where !questionsAlreadyAnswered.Contains(questions.Id)
                                      where questions.QuestionTypeId.HasValue
                                      where questions.QuestionTypeId.Value == questionTypeId
                                      where quiz.Id == quizId
                                      select questions).ToListAsync();
            }
            else
            {
                questionList = await (from questions in db.Questions
                                      where !questionsAlreadyAnswered.Contains(questions.Id)
                                      where questions.QuestionTypeId.HasValue
                                      where questions.QuestionTypeId.Value == questionTypeId
                                      select questions).ToListAsync();
            }


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

            int changes = await SaveAnswerAsync(userId, questionId, answerId);

            // get the user's points for all questions
            int points = await GetUsersQuestionPointsAsync(userId);

            // award the user any question-related milestones
            await HandleQuestionMilestonesAsync(userId, points);

            // award the user any tags for question-related points
            await HandleQuestionPointAwards(userId, profileId);

            // get the user's total points (questions, quizzes, games)
            // do something if they hit a total milestone

            return changes;
        }

        private async Task HandleQuestionPointAwards(string userId, int profileId)
        {
            // get a collection of all tags and associated point sums for the user
            var tagsForAnsweredQuestions = await GetTagsForAnsweredQuestionsAsync(userId);

            // for each tag, check how many tags he should have and award the missing
            foreach (var tag in tagsForAnsweredQuestions)
            {
                // user gets a tag award for every 15 points per tag-related question, this is how many should be present
                int supposedTagAwardCount = tag.Points / 15;

                // get the actual number of awarded tags of this type for the user
                int actualTagAwardCount = await GetUsersAwardedTagCountForTag(profileId, tag.TagId);

                // the number of tags to award is the difference between the supposed to have and the actual have
                int numberOfTagsToAward = supposedTagAwardCount - actualTagAwardCount;

                // award the proper number of tags the user deserves
                for (int i = 0; i < numberOfTagsToAward; i++)
                {
                    await AwardTagToProfileAsync(profileId, tag.TagId);
                }
            }
        }

        private async Task<int> SaveAnswerAsync(string userId, int questionId, int answerId)
        {
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
            return changes;
        }

        private async Task<int> GetUsersAwardedTagCountForTag(int profileId, int tagId)
        {
            var tagAwards = from tagAward in db.TagAwards
                            where tagAward.TagId == tagId
                            where tagAward.ProfileId == profileId
                            select tagAward;

            return await tagAwards.CountAsync();
        }

        private async Task HandleQuestionMilestonesAsync(string userId, int points)
        {
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
                        await AwardMilestoneToUserAsync(userId, milestone.Id);
                    }
                }
            }
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

        private async Task<IReadOnlyCollection<QuestionTagPoints>> GetTagsForAnsweredQuestionsAsync(string userId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            string sql = @"
                select t.TagId, sum(q.Points) as Points
                from dbo.AnsweredQuestions as aq
                inner join dbo.Questions as q on aq.QuestionId = q.Id
                inner join dbo.TagQuestions as tq on q.Id = tq.Question_Id
                inner join dbo.Tags as t on tq.Tag_TagId = t.TagId
                where aq.UserId = @userId
                group by t.TagId
            ";

            var results = await QueryAsync<QuestionTagPoints>(sql, new { userId = userId });

            return results.ToList().AsReadOnly();
        }

        internal async Task<int> GetUsersQuestionPointsAsync(string userId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            string sql = @"
                select sum(points) as points
                from dbo.AnsweredQuestions as aq
                inner join dbo.Questions as q on aq.QuestionId = q.Id
                where aq.UserId = @userId
            ";

            int? points = 0;

            var results = await QueryAsync<int>(sql, new { userId = userId });

            points = results.FirstOrDefault();

            if (!points.HasValue)
            {
                points = 0;
            }

            return points.Value;
        }

        internal async Task<int> GetUsersQuestionPointsForTypeAsync(string userId, int questionTypeId)
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
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var results = await db.Database.Connection.QueryAsync<T>(sql, parameters);
                connection.Close();
                return results;
            }
        }
    }
}