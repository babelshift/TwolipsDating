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
            await SetNotificationsAsync();

            var quizzes = await triviaService.GetQuizzesAsync();

            TriviaMenuViewModel viewModel = new TriviaMenuViewModel();
            viewModel.Quizzes = Mapper.Map<IReadOnlyCollection<Quiz>, IReadOnlyCollection<QuizOverviewViewModel>>(quizzes);

            var currentUserId = await GetCurrentUserIdAsync();
            var completedQuizzes = await triviaService.GetCompletedQuizzesForUserAsync(currentUserId);

            foreach (var quiz in viewModel.Quizzes)
            {
                if(completedQuizzes.Any(q => q.Key == quiz.Id))
                {
                    quiz.IsComplete = true;
                }
            }

            //var currentUserProfile = await ProfileService.GetUserProfileAsync(currentUserId);
            viewModel.UserStats = await ProfileService.GetUserStatsAsync(currentUserId);
            viewModel.IsCurrentUserEmailConfirmed = String.IsNullOrEmpty(currentUserId) ? false : await UserManager.IsEmailConfirmedAsync(currentUserId);

            return View(viewModel);
        }

        public async Task<JsonResult> RandomJson()
        {
            QuestionViewModel viewModel = await GetRandomQuestionViewModel();

            return Json(new 
            { 
                QuestionId = viewModel.QuestionId,
                Content = viewModel.Content,
                Points = viewModel.Points,
                IsAlreadyAnswered = viewModel.IsAlreadyAnswered,
                CorrectAnswerId = viewModel.CorrectAnswerId,
                Answers = viewModel.Answers
            }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public async Task<ActionResult> Random()
        {
            QuestionViewModel viewModel = await GetRandomQuestionViewModel();

            return View(viewModel);
        }

        private async Task<QuestionViewModel> GetRandomQuestionViewModel()
        {
            if(User.Identity.IsAuthenticated)
            {
                await SetNotificationsAsync();
            }

            var currentUserId = await GetCurrentUserIdAsync();

            // generate a random question with its answers to view
            var randomQuestion = await triviaService.GetRandomQuestionAsync(currentUserId, (int)QuestionTypeValues.Random);

            QuestionViewModel viewModel = Mapper.Map<Question, QuestionViewModel>(randomQuestion);

            if (viewModel != null)
            {
                viewModel.QuestionTypeId = (int)QuestionTypeValues.Random;
                viewModel.UsersAnsweredCorrectly = await GetUsersAnsweredCorrectlyAsync(randomQuestion);
            }

            return viewModel;
        }

        [AllowAnonymous]
        public async Task<ActionResult> Timed()
        {
            if (User.Identity.IsAuthenticated)
            {
                await SetNotificationsAsync();
            }

            var currentUserId = await GetCurrentUserIdAsync();

            // generate a random question with its answers to view
            Question randomQuestion = await triviaService.GetRandomQuestionAsync(currentUserId, (int)QuestionTypeValues.Timed);

            QuestionViewModel viewModel = Mapper.Map<Question, QuestionViewModel>(randomQuestion);

            if (viewModel != null)
            {
                viewModel.QuestionTypeId = (int)QuestionTypeValues.Timed;
                viewModel.UsersAnsweredCorrectly = await GetUsersAnsweredCorrectlyAsync(randomQuestion);
            }

            return View(viewModel);
        }

        private async Task<IReadOnlyCollection<UserAnsweredQuestionCorrectlyViewModel>> GetUsersAnsweredCorrectlyAsync(Question randomQuestion)
        {
            if (randomQuestion != null)
            {
                var usersAnsweredCorrectly = await triviaService.GetUsersAnsweredCorrectlyAsync(randomQuestion.Id);
                return Mapper.Map<IReadOnlyCollection<AnsweredQuestion>, IReadOnlyCollection<UserAnsweredQuestionCorrectlyViewModel>>(usersAnsweredCorrectly);
            }

            return null;
        }

        [HttpPost]
        public async Task<JsonResult> SubmitAnswer(int questionId, int answerId)
        {
            try
            {
                var currentUserId = await GetCurrentUserIdAsync();

                var currentUserProfile = await ProfileService.GetUserProfileAsync(currentUserId);

                // log the answer for this user's question history
                int correctAnswerId = await triviaService.RecordAnsweredQuestionAsync(currentUserId, currentUserProfile.Id, questionId, answerId, (int)QuestionTypeValues.Random);

                return Json(new { success = true, correctAnswerId = correctAnswerId });
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

                        // log the answer for this user's question history
                        int correctAnswerId = await triviaService.RecordAnsweredQuestionAsync(currentUserId, currentUserProfile.Id, questionId, answerId, (int)QuestionTypeValues.Timed);

                        return Json(new { success = true, correctAnswerId = correctAnswerId });
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

        public async Task<ActionResult> Quiz(int id)
        {
            await SetNotificationsAsync();

            var currentUserId = await GetCurrentUserIdAsync();

            var quiz = await triviaService.GetQuizAsync(id);

            // check if the quiz has already been completed by the user
            bool isAlreadyCompleted = await triviaService.IsQuizAlreadyCompletedAsync(currentUserId, quiz.Id);

            List<QuestionViewModel> questionListViewModel = new List<QuestionViewModel>();
            // if it has already been completed, get answered questions for quiz and user
            if (isAlreadyCompleted)
            {
                // get the already answered questions for this quiz
                var answeredQuizQuestions = await triviaService.GetAnsweredQuizQuestions(currentUserId, id);

                // get the list of all possible questions for this quiz
                var quizQuestions = await triviaService.GetQuizQuestionsAsync(id);
                questionListViewModel = Mapper.Map<IReadOnlyCollection<Question>, List<QuestionViewModel>>(quizQuestions);

                // match up the already selected answers with the questions for this quiz
                foreach (var questionViewModel in questionListViewModel)
                {
                    var answeredQuizQuestion = answeredQuizQuestions[questionViewModel.QuestionId];
                    questionViewModel.SelectedAnswerId = answeredQuizQuestion.AnswerId;
                    questionViewModel.IsAlreadyAnswered = true;

                    // mark the correct answer to show the user
                    foreach (var answer in questionViewModel.Answers)
                    {
                        answer.IsCorrect = (answer.AnswerId == questionViewModel.CorrectAnswerId);
                    }
                }
            }
            // if it hasn't been completed, get unanswered questions for quiz
            else
            {
                var quizQuestions = await triviaService.GetQuizQuestionsAsync(id);
                questionListViewModel = Mapper.Map<IReadOnlyCollection<Question>, List<QuestionViewModel>>(quizQuestions);
            }

            QuizViewModel viewModel = new QuizViewModel()
            {
                Questions = questionListViewModel,
                QuizName = quiz.Name,
                QuizId = id,
                IsAlreadyCompleted = isAlreadyCompleted
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Quiz(QuizViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            string currentUserId = await GetCurrentUserIdAsync();

            var profile = await ProfileService.GetUserProfileAsync(currentUserId);

            // loop through questions and record answers
            int numberOfCorrectAnswers = 0;
            foreach (var question in viewModel.Questions)
            {
                int correctAnswerId = await triviaService.RecordAnsweredQuestionAsync(
                    currentUserId,
                    profile.Id,
                    question.QuestionId,
                    question.SelectedAnswerId.Value,
                    (int)QuestionTypeValues.Quiz);

                if(question.SelectedAnswerId.Value == correctAnswerId)
                {
                    numberOfCorrectAnswers++;
                }
            }

            int count = await triviaService.SetQuizAsCompleted(currentUserId, viewModel.QuizId, numberOfCorrectAnswers);

            return RedirectToAction("quiz", new { id = viewModel.QuizId });
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