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

        private DateTime? QuestionStartTime
        {
            get
            {
                DateTime? questionStartTime = null;

                if (Session["QuestionStartTime"] != null)
                {
                    DateTime tempTime = DateTime.Now;
                    if (DateTime.TryParse(Session["QuestionStartTime"].ToString(), out tempTime))
                    {
                        questionStartTime = tempTime;
                    }
                }

                return questionStartTime;
            }
            set
            {
                Session["QuestionStartTime"] = value;
            }
        }

        public async Task<ActionResult> Index()
        {
            await SetHeaderCounts();

            var quizzes = await triviaService.GetQuizzesAsync();

            TriviaMenuViewModel viewModel = new TriviaMenuViewModel();
            viewModel.Quizzes = Mapper.Map<IReadOnlyCollection<Quiz>, IReadOnlyCollection<QuizOverviewViewModel>>(quizzes);

            return View(viewModel);
        }

        public async Task<ActionResult> Random()
        {
            await SetHeaderCounts();

            var currentUserId = await GetCurrentUserIdAsync();

            // generate a random question with its answers to view
            Question randomQuestion = await triviaService.GetRandomQuestionAsync(currentUserId, (int)QuestionTypeValues.Random);

            QuestionViewModel viewModel = Mapper.Map<Question, QuestionViewModel>(randomQuestion);

            return View(viewModel);
        }

        public async Task<ActionResult> Timed()
        {
            await SetHeaderCounts();

            var currentUserId = await GetCurrentUserIdAsync();

            // generate a random question with its answers to view
            Question randomQuestion = await triviaService.GetRandomQuestionAsync(currentUserId, (int)QuestionTypeValues.Timed);

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

        [HttpPost]
        public async Task<JsonResult> SubmitTimedAnswer(int questionId, int answerId)
        {
            try
            {
                DateTime utcNow = DateTime.UtcNow;
                if (QuestionStartTime.HasValue)
                {
                    TimeSpan timeSpentOnQuestion = utcNow.Subtract(QuestionStartTime.Value);
                    bool didUserAnswerInTime = timeSpentOnQuestion.TotalSeconds <= 10 ? true : false;
                    QuestionStartTime = null;

                    if (didUserAnswerInTime)
                    {
                        var currentUserId = await GetCurrentUserIdAsync();

                        var currentUserProfile = await ProfileService.GetUserProfileAsync(currentUserId);

                        // check if the supplied answer is correct
                        bool isAnswerCorrect = await triviaService.IsAnswerCorrectAsync(questionId, answerId);

                        // log the answer for this user's question history
                        await triviaService.RecordAnsweredQuestionAsync(currentUserId, currentUserProfile.Id, questionId, answerId, (int)QuestionTypeValues.Timed);

                        return Json(new { success = true, isAnswerCorrect = isAnswerCorrect });
                    }
                    else
                    {
                        return Json(new { success = false, error = "You ran out of time." });
                    }
                }
                else
                {
                    return Json(new { success = false, error = "How did you answer without starting the question?" });
                }
            }
            catch (Exception e)
            {
                Log.Error("SubmitAnswer", e,
                    parameters: new { questionId = questionId, answerId = answerId }
                );

                return Json(new { success = false, error = ErrorMessages.AnswerNotSubmitted });
            }
        }

        public JsonResult StartTimedQuestion()
        {
            DateTime questionStartTime = DateTime.UtcNow;
            QuestionStartTime = questionStartTime;

            return Json(new 
            { 
                day = questionStartTime.Day,
                month = questionStartTime.Month - 1,
                year = questionStartTime.Year,
                hours = questionStartTime.Hour,
                minutes = questionStartTime.Minute,
                seconds = questionStartTime.Second,
                milliseconds = questionStartTime.Millisecond
            }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Quiz(int quizId)
        {
            await SetHeaderCounts();

            return View();
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