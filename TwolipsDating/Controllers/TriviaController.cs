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
using Microsoft.AspNet.Identity;
using System.Net;

namespace TwolipsDating.Controllers
{
    public class TriviaController : BaseController
    {
        TriviaService triviaService = new TriviaService();
        ViolationService violationService = new ViolationService();
        UserService userService = new UserService();

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

            var currentUserId = User.Identity.GetUserId();
            var completedQuizzes = await triviaService.GetCompletedQuizzesForUserAsync(currentUserId);

            foreach (var quiz in viewModel.Quizzes)
            {
                if (completedQuizzes.Any(q => q.Key == quiz.Id))
                {
                    quiz.IsComplete = true;
                }
            }

            //var currentUserProfile = await ProfileService.GetUserProfileAsync(currentUserId);
            viewModel.UserStats = await ProfileService.GetUserStatsAsync(currentUserId);
            viewModel.IsCurrentUserEmailConfirmed = String.IsNullOrEmpty(currentUserId) ? false : await UserManager.IsEmailConfirmedAsync(currentUserId);

            return View(viewModel);
        }

        #region Random Question

        public async Task<JsonResult> RandomJson()
        {
            QuestionViewModel viewModel = await GetRandomQuestionViewModelAsync((int)QuestionTypeValues.Random);

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
            var currentUserId = User.Identity.GetUserId();

            // if the user is logged in but doesn't have a profile, redirect to profile
            if (User.Identity.IsAuthenticated
                && !(await userService.DoesUserHaveProfileAsync(currentUserId)))
                return RedirectToProfileIndex();

            QuestionViewModel viewModel = await GetRandomQuestionViewModelAsync((int)QuestionTypeValues.Random);

            viewModel.QuestionViolation = await GetQuestionViolationViewModelAsync();

            return View(viewModel);
        }

        [HttpPost]
        public async Task<JsonResult> SubmitAnswer(int questionId, int answerId)
        {
            try
            {
                var currentUserId = User.Identity.GetUserId();

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

        #endregion

        #region Timed Question

        [AllowAnonymous]
        public async Task<ActionResult> Timed()
        {
            var currentUserId = User.Identity.GetUserId();

            // if the user is logged in but doesn't have a profile, redirect to profile
            if (User.Identity.IsAuthenticated
                && !(await userService.DoesUserHaveProfileAsync(currentUserId)))
                return RedirectToProfileIndex();

            var viewModel = await GetRandomQuestionViewModelAsync((int)QuestionTypeValues.Timed);

            viewModel.QuestionViolation = await GetQuestionViolationViewModelAsync();

            return View(viewModel);
        }

        private async Task<QuestionViolationViewModel> GetQuestionViolationViewModelAsync()
        {
            QuestionViolationViewModel viewModel = new QuestionViolationViewModel();

            // anonymous viewers can't report violations so don't look any of the types up
            if (User.Identity.IsAuthenticated)
            {
                // setup violation types
                var violationTypes = await violationService.GetQuestionViolationTypesAsync();
                viewModel.ViolationTypes = violationTypes.ToDictionary(v => v.Id, v => v.Name);
            }

            return viewModel;
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
                        var currentUserId = User.Identity.GetUserId();

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

        #endregion

        #region Quiz

        [AllowAnonymous]
        public async Task<ActionResult> Quiz(int id)
        {
            await SetNotificationsAsync();

            var currentUserId = User.Identity.GetUserId();

            // if the user is logged in but doesn't have a profile, redirect to profile
            if (User.Identity.IsAuthenticated
                && !(await userService.DoesUserHaveProfileAsync(currentUserId)))
                return RedirectToProfileIndex();

            var quiz = await triviaService.GetQuizAsync(id);
            if (quiz == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            bool isAlreadyCompleted = false;
            List<QuestionViewModel> questionListViewModel = new List<QuestionViewModel>();

            if (User.Identity.IsAuthenticated)
            {
                // check if the quiz has already been completed by the user
                isAlreadyCompleted = await triviaService.IsQuizAlreadyCompletedAsync(currentUserId, quiz.Id);

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
                    questionListViewModel = await GetQuizQuestions(id, questionListViewModel);
                }
            }
            // user isn't authenticated, so anonymous users the questions
            else
            {
                questionListViewModel = await GetQuizQuestions(id, questionListViewModel);
            }

            QuizViewModel viewModel = new QuizViewModel()
            {
                Questions = questionListViewModel,
                QuizName = quiz.Name,
                QuizId = id,
                IsAlreadyCompleted = isAlreadyCompleted,
                QuizDescription = quiz.Description,
                UsersCompletedQuiz = await GetUsersCompletedQuizAsync(id)
            };

            viewModel.QuestionViolation = await GetQuestionViolationViewModelAsync();

            return View(viewModel);
        }

        private async Task<List<QuestionViewModel>> GetQuizQuestions(int id, List<QuestionViewModel> questionListViewModel)
        {
            var quizQuestions = await triviaService.GetQuizQuestionsAsync(id);
            questionListViewModel = Mapper.Map<IReadOnlyCollection<Question>, List<QuestionViewModel>>(quizQuestions);
            return questionListViewModel;
        }

        [HttpPost]
        public async Task<ActionResult> Quiz(QuizViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            string currentUserId = User.Identity.GetUserId();

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

                if (question.SelectedAnswerId.Value == correctAnswerId)
                {
                    numberOfCorrectAnswers++;
                }
            }

            int count = await triviaService.SetQuizAsCompleted(currentUserId, viewModel.QuizId, numberOfCorrectAnswers);

            return RedirectToAction("quiz", new { id = viewModel.QuizId });
        }

        #endregion

        private async Task<QuestionViewModel> GetRandomQuestionViewModelAsync(int questionTypeId)
        {
            await SetNotificationsAsync();

            var currentUserId = User.Identity.GetUserId();

            // generate a random question with its answers to view
            var randomQuestion = await triviaService.GetRandomQuestionAsync(currentUserId, questionTypeId);

            QuestionViewModel viewModel = Mapper.Map<Question, QuestionViewModel>(randomQuestion);

            if (viewModel != null)
            {
                viewModel.QuestionTypeId = questionTypeId;
                viewModel.UsersAnsweredCorrectly = await GetUsersAnsweredCorrectlyAsync(randomQuestion.Id);
            }

            return viewModel;
        }

        private async Task<IReadOnlyCollection<UserAnsweredQuestionCorrectlyViewModel>> GetUsersAnsweredCorrectlyAsync(int questionId)
        {
            var usersAnsweredCorrectly = await triviaService.GetUsersAnsweredCorrectlyAsync(questionId);
            return Mapper.Map<IReadOnlyCollection<AnsweredQuestion>, IReadOnlyCollection<UserAnsweredQuestionCorrectlyViewModel>>(usersAnsweredCorrectly);
        }

        private async Task<IReadOnlyCollection<UserCompletedQuizViewModel>> GetUsersCompletedQuizAsync(int quizId)
        {
            var usersCompletedQuiz = await triviaService.GetUsersCompletedQuizAsync(quizId);
            return Mapper.Map<IReadOnlyCollection<CompletedQuiz>, IReadOnlyCollection<UserCompletedQuizViewModel>>(usersCompletedQuiz);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (triviaService != null)
                {
                    triviaService.Dispose();
                    triviaService = null;
                }

                if (violationService != null)
                {
                    violationService.Dispose();
                    violationService = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}