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

        internal async Task<int> RecordAnsweredQuestionAsync(string userId, int questionId, int answerId, int questionTypeId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));
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

            // check if the user has hit an achievement or milestone

            int points = await CalculateUserPointsAsync(userId, questionTypeId);
            // if user has over 10 points, give them 'Junior' achievement
            // if user has over 25 points, give them 'Average' achievement
            // if user has over 50 points, give them 'Senior' achievement

            // if user has 5 'intellectual' tag questions answered, give them the 'intellectual' badge
            var tagsForAnsweredQuestions = await GetTagsForAnsweredQuestionsAsync(userId, questionTypeId);

            return changes;
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

        internal async Task<int> CalculateUserPointsAsync(string userId, int questionTypeId)
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