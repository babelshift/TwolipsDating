using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TwolipsDating.Models;
using System.Data.Entity;
using System.Threading.Tasks;

namespace TwolipsDating.Business
{
    internal class TriviaService : BaseService
    {
        internal async Task<Question> GetRandomQuestionAsync()
        {
            var questionList = await (from questions in db.Questions
                                      select questions).ToListAsync();

            Random random = new Random();
            int randomQuestionIndex = random.Next(0, questionList.Count);

            Question randomQuestion = questionList[randomQuestionIndex];

            return randomQuestion;
        }

        internal async Task<bool> IsAnswerCorrectAsync(int questionId, int answerId)
        {
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
    }
}