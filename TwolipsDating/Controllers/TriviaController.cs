using AutoMapper;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using TwolipsDating.Business;
using TwolipsDating.Models;
using TwolipsDating.Utilities;
using TwolipsDating.ViewModels;

namespace TwolipsDating.Controllers
{
    public class TriviaController : BaseController
    {
        #region Services

        private TriviaService triviaService = new TriviaService();
        private ViolationService violationService = new ViolationService();
        private UserService userService = new UserService();

        #endregion

        /// <summary>
        /// Get or set the date and time at which the current user has started viewing a question.
        /// </summary>
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

        #region Dashboard

        /// <summary>
        /// Returns a view to display the trivia dashboard
        /// </summary>
        /// <returns></returns>
        [RequireProfile]
        public async Task<ActionResult> Index()
        {
            await SetNotificationsAsync();

            var currentUserId = User.Identity.GetUserId();

            var newQuizzes = await triviaService.GetNewQuizzesAsync();

            TriviaMenuViewModel viewModel = new TriviaMenuViewModel();
            viewModel.NewQuizzes = Mapper.Map<IReadOnlyCollection<Quiz>, IReadOnlyCollection<QuizOverviewViewModel>>(newQuizzes);

            // alters the viewmodel's list of quizzes to indicate if the quiz has already been completed by the currently logged in user
            await SetQuizzesCompletedByCurrentUser(currentUserId, viewModel);

            viewModel.UserStats = await ProfileService.GetUserStatsAsync(currentUserId);
            viewModel.RecentlyCompletedQuizzes = await GetUsersCompletedQuizzesAsync();

            return View(viewModel);
        }

        /// <summary>
        /// Alters the viewmodel's list of quizzes to indicate if the quiz has already been completed by the currently logged in user
        /// </summary>
        /// <param name="currentUserId"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        private async Task SetQuizzesCompletedByCurrentUser(string currentUserId, TriviaMenuViewModel viewModel)
        {
            var completedQuizzes = await triviaService.GetCompletedQuizzesForUserAsync(currentUserId);

            foreach (var quiz in viewModel.NewQuizzes)
            {
                if (completedQuizzes.Any(q => q.Key == quiz.Id))
                {
                    quiz.IsComplete = true;
                }
            }
        }

        /// <summary>
        /// Returns a collection of quizzes that have been completed by various users
        /// </summary>
        /// <returns></returns>
        private async Task<IReadOnlyCollection<UserCompletedQuizViewModel>> GetUsersCompletedQuizzesAsync()
        {
            return await triviaService.GetUsersCompletedQuizzesAsync();
        }

        #endregion

        #region Random Question

        /// <summary>
        /// Returns a JSON object which contains a random question and its contents. Returns "success: false" if no questions remain unanswered for the currently logged in user.
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> RandomJson()
        {
            QuestionViewModel viewModel = await GetRandomQuestionViewModelAsync((int)QuestionTypeValues.Random);

            // there is a random question
            if (viewModel != null)
            {
                return Json(new
                {
                    Success = true,
                    QuestionId = viewModel.QuestionId,
                    Content = viewModel.Content,
                    Points = viewModel.Points,
                    IsAlreadyAnswered = viewModel.IsAlreadyAnswered,
                    CorrectAnswerId = viewModel.CorrectAnswerId,
                    Answers = viewModel.Answers
                }, JsonRequestBehavior.AllowGet);
            }
            // there are no more random questions
            else
            {
                return Json(new
                {
                    Success = false
                }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Returns a view containing a random question. Redirects to the user's create profile if no profile exists for the user.
        /// </summary>
        /// <returns></returns>
        [RequireProfile]
        [AllowAnonymous]
        public async Task<ActionResult> Random()
        {
            QuestionViewModel viewModel = new QuestionViewModel();

            viewModel = await GetRandomQuestionViewModelAsync((int)QuestionTypeValues.Random);

            if (viewModel != null)
            {
                viewModel.QuestionViolation = await GetQuestionViolationViewModelAsync();
            }
            else
            {
                viewModel = new QuestionViewModel();
                viewModel.Tags = new List<TagViewModel>();
                viewModel.UsersAnsweredCorrectly = new List<UserAnsweredQuestionCorrectlyViewModel>();
            }

            return View(viewModel);
        }

        /// <summary>
        /// Submits an answer for a question and records the action for the currently logged in user.
        /// </summary>
        /// <param name="questionId"></param>
        /// <param name="answerId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> SubmitAnswer(int questionId, int answerId)
        {
            try
            {
                var currentUserId = User.Identity.GetUserId();

                int? currentUserProfileId = await userService.GetProfileIdAsync(currentUserId);

                // only allow submit answer if the user has a profile
                if (currentUserProfileId.HasValue)
                {
                    int correctAnswerId = await triviaService.RecordAnsweredQuestionAsync(
                        currentUserId,
                        currentUserProfileId.Value,
                        questionId,
                        answerId,
                        (int)QuestionTypeValues.Random);

                    return Json(new { success = true, correctAnswerId = correctAnswerId });
                }
                else
                {
                    Log.Warn("SubmitAnswer", ErrorMessages.AnswerNotSubmitted,
                        parameters: new { questionId = questionId, answerId = answerId }
                    );

                    return Json(new { success = false, error = ErrorMessages.AnswerNotSubmitted });
                }
            }
            catch (DbUpdateException e)
            {
                Log.Error("SubmitAnswer", e,
                    parameters: new { questionId = questionId, answerId = answerId }
                );

                return Json(new { success = false, error = ErrorMessages.AnswerNotSubmitted });
            }
        }

        #endregion Random Question

        #region Timed Question

        /// <summary>
        /// Returns a view containing a timed random question. Redirects to the user's create profile if no profile exists for the user.
        /// </summary>
        /// <returns></returns>
        [RequireProfile]
        [AllowAnonymous]
        public async Task<ActionResult> Timed()
        {
            QuestionViewModel viewModel = new QuestionViewModel();

            viewModel = await GetRandomQuestionViewModelAsync((int)QuestionTypeValues.Timed);

            if (viewModel != null)
            {
                viewModel.QuestionViolation = await GetQuestionViolationViewModelAsync();
            }
            else
            {
                viewModel = new QuestionViewModel();
                viewModel.Tags = new List<TagViewModel>();
                viewModel.UsersAnsweredCorrectly = new List<UserAnsweredQuestionCorrectlyViewModel>();
            }

            return View(viewModel);
        }

        /// <summary>
        /// Submits an answer for a timed question and records the action for the currently logged in user.
        /// </summary>
        /// <param name="questionId"></param>
        /// <param name="answerId"></param>
        /// <returns></returns>
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

                        int? currentUserProfileId = await userService.GetProfileIdAsync(currentUserId);

                        if (currentUserProfileId.HasValue)
                        {
                            // log the answer for this user's question history
                            int correctAnswerId = await triviaService.RecordAnsweredQuestionAsync(
                                currentUserId,
                                currentUserProfileId.Value,
                                questionId,
                                answerId,
                                (int)QuestionTypeValues.Timed);

                            return Json(new { success = true, correctAnswerId = correctAnswerId });
                        }
                        else
                        {
                            Log.Warn("SubmitTimedAnswer", ErrorMessages.AnswerNotSubmitted,
                                parameters: new { questionId = questionId, answerId = answerId }
                            );

                            return Json(new { success = false, error = ErrorMessages.AnswerNotSubmitted });
                        }
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
            catch (DbUpdateException e)
            {
                Log.Error("SubmitAnswer", e,
                    parameters: new { questionId = questionId, answerId = answerId }
                );

                return Json(new { success = false, error = ErrorMessages.AnswerNotSubmitted });
            }
        }

        /// <summary>
        /// Starts a timer on a timed question and returns a JSON object containing the time components.
        /// </summary>
        /// <returns></returns>
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

        #endregion Timed Question

        #region Quiz

        /// <summary>
        /// Returns a view containing quiz questions and optionally the correct answers and the selected answers by the user if they've completed the quiz.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous, RequireProfile, RequireConfirmedEmail, ImportModelStateFromTempData]
        public async Task<ActionResult> Quiz(int id, string seoName)
        {
            await SetNotificationsAsync();

            var currentUserId = User.Identity.GetUserId();

            // if there is no quiz by this id, return not found
            var quiz = await triviaService.GetQuizAsync(id);
            if (quiz == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            string expectedSeoName = quiz.GetSEOName();
            if (seoName != expectedSeoName)
            {
                return RedirectToAction("quiz", new { id = id, seoName = expectedSeoName });
            }

            bool isAlreadyCompleted = false;
            List<QuestionViewModel> questionListViewModel = new List<QuestionViewModel>();
            int correctAnswerCount = 0;

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

                        // if the user selected the correct answer
                        if (questionViewModel.SelectedAnswerId == questionViewModel.CorrectAnswerId)
                        {
                            correctAnswerCount++;
                        }

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
                    questionListViewModel = Mapper.Map<ICollection<Question>, List<QuestionViewModel>>(quiz.Questions);
                }
            }
            // user isn't authenticated, so anonymous users the questions
            else
            {
                questionListViewModel = Mapper.Map<ICollection<Question>, List<QuestionViewModel>>(quiz.Questions);
            }

            QuizViewModel viewModel = new QuizViewModel()
            {
                Questions = questionListViewModel,
                QuizName = quiz.Name,
                QuizId = id,
                IsAlreadyCompleted = isAlreadyCompleted,
                QuizDescription = quiz.Description,
                UsersCompletedQuiz = await GetUsersCompletedQuizAsync(id),
                Tags = await GetTagsForQuizAsync(id),
                QuestionViolation = await GetQuestionViolationViewModelAsync(),
                AveragePoints = questionListViewModel != null && questionListViewModel.Count > 0
                    ? (int)Math.Round(questionListViewModel.Average(q => q.Points))
                    : 0,
                ImageUrl = quiz.GetImageUrl(),
                UserScorePercent = (int)Math.Round(((double)correctAnswerCount / (double)questionListViewModel.Count) * 100)
            };

            return View(viewModel);
        }

        /// <summary>
        /// Submits the completion of a quiz for the currently logged in user.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost, ExportModelStateToTempData]
        public async Task<ActionResult> Quiz(QuizViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("quiz", new { id = viewModel.QuizId, seoName = viewModel.SEOName });
            }

            string currentUserId = User.Identity.GetUserId();

            var profile = await ProfileService.GetProfileAsync(currentUserId);

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

        /// <summary>
        /// Returns a collection of users who have completed a quiz.
        /// </summary>
        /// <param name="quizId"></param>
        /// <returns></returns>
        private async Task<IReadOnlyCollection<UserCompletedQuizViewModel>> GetUsersCompletedQuizAsync(int quizId)
        {
            var usersCompletedQuiz = await triviaService.GetUsersCompletedQuizAsync(quizId);
            return usersCompletedQuiz;
        }

        /// <summary>
        /// Returns a collection of tag associated with the questions of a quiz.
        /// </summary>
        /// <param name="quizId"></param>
        /// <returns></returns>
        private async Task<IReadOnlyCollection<TagViewModel>> GetTagsForQuizAsync(int quizId)
        {
            var tags = await triviaService.GetTagsForQuizAsync(quizId);
            return tags;
        }

        #endregion Quiz

        /// <summary>
        /// Returns a view model containing a random question of a certain type with its tags and users who have already answered it correctly.
        /// </summary>
        /// <param name="questionTypeId"></param>
        /// <returns></returns>
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
                viewModel.Tags = await GetTagsForQuestionAsync(randomQuestion.Id);
            }

            return viewModel;
        }

        /// <summary>
        /// Returns a collection of tags for a certain question.
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        private async Task<IReadOnlyCollection<TagViewModel>> GetTagsForQuestionAsync(int questionId)
        {
            var tags = await triviaService.GetTagsForQuestionAsync(questionId);
            return Mapper.Map<IReadOnlyCollection<Tag>, IReadOnlyCollection<TagViewModel>>(tags);
        }

        /// <summary>
        /// Returns a collection of users who answered a question correctly.
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        private async Task<IReadOnlyCollection<UserAnsweredQuestionCorrectlyViewModel>> GetUsersAnsweredCorrectlyAsync(int questionId)
        {
            var usersAnsweredCorrectly = await triviaService.GetUsersAnsweredCorrectlyAsync(questionId);
            return Mapper.Map<IReadOnlyCollection<AnsweredQuestion>, IReadOnlyCollection<UserAnsweredQuestionCorrectlyViewModel>>(usersAnsweredCorrectly);
        }

        /// <summary>
        /// Returns a view model containing question violation objects.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Disposes all services.
        /// </summary>
        /// <param name="disposing"></param>
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