using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TwolipsDating.Models;

namespace TwolipsDating.Business
{
    internal class TriviaService : BaseService
    {
        internal async Task<Quiz> GetQuizAsync(int quizId)
        {
            Debug.Assert(quizId > 0);

            var quizResult = from quiz in db.Quizzes
                             where quiz.Id == quizId
                             select quiz;

            return await quizResult.FirstOrDefaultAsync();
        }

        internal async Task<IReadOnlyCollection<Quiz>> GetQuizzesAsync()
        {
            var quizzes = from quiz in db.Quizzes
                          select quiz;

            var results = await quizzes.ToListAsync();

            return results.AsReadOnly();
        }

        internal async Task<IReadOnlyCollection<Question>> GetQuizQuestionsAsync(int quizId)
        {
            Debug.Assert(quizId > 0);

            var questionList = from questions in db.Questions
                               from quiz in questions.Quizzes
                               where questions.QuestionTypeId.HasValue
                               where questions.QuestionTypeId.Value == (int)QuestionTypeValues.Quiz
                               where quiz.Id == quizId
                               select questions;

            var results = await questionList.ToListAsync();

            return results.AsReadOnly();
        }

        /// <summary>
        /// Returns a random question that the user has not answered yet
        /// </summary>
        /// <returns></returns>
        internal async Task<Question> GetRandomQuestionAsync(string userId, int questionTypeId)
        {
            //Debug.Assert(!String.IsNullOrEmpty(userId));
            Debug.Assert(questionTypeId > 0);

            var questionsAlreadyAnswered = await (from questions in db.AnsweredQuestions
                                                  where questions.UserId == userId
                                                  select questions.QuestionId).ToListAsync();

            var questionList = await (from questions in db.Questions
                                      where !questionsAlreadyAnswered.Contains(questions.Id)
                                      where questions.QuestionTypeId.HasValue
                                      where questions.QuestionTypeId.Value == questionTypeId
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

        internal async Task<int> GetCorrectAnswerAsync(int questionId, int answerId)
        {
            Debug.Assert(questionId > 0);
            Debug.Assert(answerId > 0);

            int? correctAnswerId = await (from questions in db.Questions
                                          where questions.Id == questionId
                                          select questions.CorrectAnswerId).FirstOrDefaultAsync();

            if (correctAnswerId.HasValue)
            {
                return correctAnswerId.Value;
            }
            else
            {
                throw new ArgumentException("There is no Question with that ID and Answer ID.");
            }
        }

        internal async Task<int> RecordAnsweredQuestionAsync(string userId, int profileId, int questionId, int answerId, int questionTypeId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));
            Debug.Assert(profileId > 0);
            Debug.Assert(questionId > 0);
            Debug.Assert(answerId > 0);
            Debug.Assert(questionTypeId > 0);

            // check if the supplied answer is correct
            int correctAnswerId = await GetCorrectAnswerAsync(questionId, answerId);

            // save the user's answer to the database
            SaveAnswer(userId, questionId, answerId);

            // give the player the correct number of points for the question if they got it correct
            if (answerId == correctAnswerId)
            {
                var user = db.Users.Find(userId);
                int questionPoints = await GetQuestionPointsAsync(questionId);
                IncreaseUserPointsAsync(user, questionPoints);
            }

            // get the user's points for all questions
            int pointsForAllQuestions = await GetUsersQuestionPointsAsync(userId);

            // award the user any question-related milestones if they have enough question-related points
            await HandleQuestionMilestonesAsync(userId, pointsForAllQuestions);

            // award the user any tags for question-related points
            await HandleTagAwards(userId, profileId);

            // get the user's total points (questions, quizzes, games)
            // do something if they hit a total milestone

            // save our current transaction for recording the answer
            int count = await db.SaveChangesAsync();

            return correctAnswerId;
        }

        private async Task<int> GetQuestionPointsAsync(int questionId)
        {
            int questionPoints = await (from question in db.Questions
                                        where question.Id == questionId
                                        select question.Points).FirstAsync();

            return questionPoints;
        }

        private async Task HandleTagAwards(string userId, int profileId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));
            Debug.Assert(profileId > 0);

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
                    AwardTagToProfile(profileId, tag.TagId);
                }
            }
        }

        private void SaveAnswer(string userId, int questionId, int answerId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));
            Debug.Assert(questionId > 0);
            Debug.Assert(answerId > 0);

            // log the answered question
            AnsweredQuestion answeredQuestion = new AnsweredQuestion()
            {
                AnswerId = answerId,
                DateAnswered = DateTime.Now,
                QuestionId = questionId,
                UserId = userId
            };

            db.AnsweredQuestions.Add(answeredQuestion);
        }

        private async Task<int> GetUsersAwardedTagCountForTag(int profileId, int tagId)
        {
            Debug.Assert(tagId > 0);
            Debug.Assert(profileId > 0);

            var tagAwards = from tagAward in db.TagAwards
                            where tagAward.TagId == tagId
                            where tagAward.ProfileId == profileId
                            select tagAward;

            return await tagAwards.CountAsync();
        }

        private async Task HandleQuestionMilestonesAsync(string userId, int points)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

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
                        AwardMilestoneToUser(userId, milestone.Id);
                    }
                }
            }
        }

        private void AwardMilestoneToUser(string userId, int milestoneId)
        {
            Debug.Assert(milestoneId > 0);
            Debug.Assert(!String.IsNullOrEmpty(userId));

            MilestoneAchievement achievement = new MilestoneAchievement()
            {
                UserId = userId,
                MilestoneId = milestoneId,
                DateAchieved = DateTime.Now
            };

            db.MilestoneAchievements.Add(achievement);
        }

        private async Task<IReadOnlyDictionary<int, int>> GetMilestoneIdsAchievedAsync(string userId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

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

        private void AwardTagToProfile(int profileId, int tagId)
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

            int points = await (from users in db.Users
                                select users.Points).FirstAsync();

            return points;
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

        internal async Task<IReadOnlyDictionary<int, AnsweredQuestion>> GetAnsweredQuizQuestions(string userId, int quizId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));
            Debug.Assert(quizId > 0);

            var answeredQuestions = from answeredQuestion in db.AnsweredQuestions
                                    join question in db.Questions on answeredQuestion.QuestionId equals question.Id
                                    from quiz in db.Quizzes
                                    where question.QuestionTypeId.HasValue
                                    where question.QuestionTypeId.Value == (int)QuestionTypeValues.Quiz
                                    where answeredQuestion.UserId == userId
                                    where quiz.Id == quizId
                                    select answeredQuestion;

            var result = (await answeredQuestions.ToListAsync())
                .ToDictionary(a => a.QuestionId, a => a);

            return new ReadOnlyDictionary<int, AnsweredQuestion>(result);
        }

        internal async Task<int> SetQuizAsCompleted(string userId, int quizId, int numberOfCorrectAnswers)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));
            Debug.Assert(quizId > 0);

            CompletedQuiz completedQuiz = new CompletedQuiz()
            {
                DateCompleted = DateTime.Now,
                QuizId = quizId,
                UserId = userId
            };

            db.CompletedQuizzes.Add(completedQuiz);

            // if the user got enough questions right, give them points
            var quiz = await GetQuizAsync(quizId);
            double percentageOfCorrectQuestions = numberOfCorrectAnswers / quiz.Questions.Count;
            if (percentageOfCorrectQuestions >= 0.8)
            {
                var user = db.Users.Find(userId);
                IncreaseUserPointsAsync(user, quiz.Points);
            }

            return await db.SaveChangesAsync();
        }

        private static void IncreaseUserPointsAsync(ApplicationUser user, int points)
        {
            user.Points += points;
        }

        private async Task<int> GetQuizPointsAsync(int quizId)
        {
            int points = await (from quizzes in db.Quizzes
                                where quizzes.Id == quizId
                                select quizzes.Points).FirstAsync();

            return points;
        }

        internal async Task<bool> IsQuizAlreadyCompletedAsync(string userId, int quizId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));
            Debug.Assert(quizId > 0);

            var completedQuiz = await (from completedQuizzes in db.CompletedQuizzes
                                       where completedQuizzes.UserId == userId
                                       where completedQuizzes.QuizId == quizId
                                       select completedQuizzes).FirstOrDefaultAsync();

            return completedQuiz != null;
        }

        internal async Task<IReadOnlyDictionary<int, CompletedQuiz>> GetCompletedQuizzesForUserAsync(string currentUserId)
        {
            var completedQuizzes = await (from quizzes in db.CompletedQuizzes
                                          where quizzes.UserId == currentUserId
                                          select quizzes).ToDictionaryAsync(q => q.QuizId, q => q);

            return new ReadOnlyDictionary<int, CompletedQuiz>(completedQuizzes);
        }

        internal async Task<IReadOnlyCollection<AnsweredQuestion>> GetUsersAnsweredCorrectlyAsync(int questionId)
        {
            var usersAnsweredCorrectly = from questionsAnswered in db.AnsweredQuestions
                                         where questionsAnswered.QuestionId == questionId
                                         join questions in db.Questions on questionsAnswered.AnswerId equals questions.CorrectAnswerId
                                         join users in db.Users on questionsAnswered.UserId equals users.Id
                                         where users.IsActive
                                         orderby questionsAnswered.DateAnswered descending
                                         select questionsAnswered;

            return await usersAnsweredCorrectly.ToListAsync();
        }

        internal async Task<IReadOnlyCollection<CompletedQuiz>> GetUsersCompletedQuizAsync(int quizId)
        {
            var usersCompletedQuiz = from completedQuizzes in db.CompletedQuizzes
                                     where completedQuizzes.QuizId == quizId
                                     join users in db.Users on completedQuizzes.UserId equals users.Id
                                     where users.IsActive
                                     orderby completedQuizzes.DateCompleted descending
                                     select completedQuizzes;

            return await usersCompletedQuiz.ToListAsync();
        }
    }
}