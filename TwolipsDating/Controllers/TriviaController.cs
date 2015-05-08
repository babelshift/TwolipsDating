using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TwolipsDating.Business;
using TwolipsDating.Models;
using TwolipsDating.Utilities;
using TwolipsDating.ViewModels;

namespace TwolipsDating.Controllers
{
    public class TriviaController : BaseController
    {
        TriviaService triviaService = new TriviaService();

        // GET: Trivia
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Random()
        {
            var currentUserId = await GetCurrentUserIdAsync();

            // generate a random question with its answers to view
            Question randomQuestion = await triviaService.GetRandomQuestionAsync(currentUserId);

            QuestionViewModel viewModel = Mapper.Map<Question, QuestionViewModel>(randomQuestion);

            return View(viewModel);
        }

        [HttpPost]
        public async Task<JsonResult> SubmitAnswer(int questionId, int answerId)
        {
            try
            {
                var currentUserId = await GetCurrentUserIdAsync();

                var currentUserProfile = await ProfileService.GetUserProfileAsync(currentUserId);

                // check if the supplied answer is correct
                bool isAnswerCorrect = await triviaService.IsAnswerCorrectAsync(questionId, answerId);

                // log the answer for this user's question history
                await triviaService.RecordAnsweredQuestionAsync(currentUserId, currentUserProfile.Id, questionId, answerId, (int)QuestionTypeValues.Random);

                return Json(new { success = true, isAnswerCorrect = isAnswerCorrect });
            }
            catch (Exception e)
            {
                Log.Error("SubmitAnswer", e,
                    parameters: new { questionId = questionId, answerId = answerId }
                );

                return Json(new { success = false, error = ErrorMessages.AnswerNotSubmitted });
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && triviaService != null)
            {
                triviaService.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}