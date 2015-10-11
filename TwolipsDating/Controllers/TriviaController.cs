using AutoMapper;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
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
        [AllowAnonymous, RequireProfileIfAuthenticated, RequireConfirmedEmailIfAuthenticated]
        public async Task<ActionResult> Index()
        {
            await SetNotificationsAsync();

            var currentUserId = User.Identity.GetUserId();

            TriviaDashboardViewModel viewModel = new TriviaDashboardViewModel();

            var newQuizzes = await TriviaService.GetNewQuizzesAsync();
            viewModel.NewQuizzes = Mapper.Map<IReadOnlyCollection<Quiz>, IReadOnlyCollection<QuizOverviewViewModel>>(newQuizzes);

            var dailyQuizzes = await TriviaService.GetDailyQuizzesAsync(5);
            viewModel.DailyQuizzes = dailyQuizzes.ToDictionary(
                x => x.Key,
                x => Mapper.Map<IReadOnlyCollection<Quiz>, IReadOnlyCollection<QuizOverviewViewModel>>(x.Value));

            viewModel.TrendingQuizzes = await GetTrendingQuizzesAsync();

            viewModel.PopularQuizzes = await GetPopularQuizzesAsync();

            // unauthenticated users don't have any quizzes to track
            if (User.Identity.IsAuthenticated)
            {
                var unfinishedQuizzes = await TriviaService.GetUnfinishedQuizzesAsync(User.Identity.GetUserId());
                viewModel.UnfinishedQuizzes = Mapper.Map<IReadOnlyCollection<Quiz>, IReadOnlyCollection<QuizOverviewViewModel>>(unfinishedQuizzes);
            }

            // alters the viewmodel's list of quizzes to indicate if the quiz has already been completed by the currently logged in user
            await SetQuizzesCompletedByCurrentUser(currentUserId, viewModel);

            viewModel.RecentlyCompletedQuizzes = await TriviaService.GetUsersCompletedQuizzesAsync(currentUserId);

            var quizCategories = await TriviaService.GetQuizCategoriesAsync();
            viewModel.QuizCategories = Mapper.Map<IReadOnlyCollection<QuizCategory>, IReadOnlyCollection<QuizCategoryViewModel>>(quizCategories);

            return View(viewModel);
        }

        private async Task<IReadOnlyCollection<MostPopularQuizViewModel>> GetPopularQuizzesAsync()
        {
            var popularQuizzes = await TriviaService.GetPopularQuizzesAsync();
            return popularQuizzes;
        }

        private async Task<IReadOnlyCollection<TrendingQuizViewModel>> GetTrendingQuizzesAsync()
        {
            var trendingQuizzes = await TriviaService.GetTrendingQuizzesAsync();
            return trendingQuizzes;
        }

        /// <summary>
        /// Alters the viewmodel's list of quizzes to indicate if the quiz has already been completed by the currently logged in user
        /// </summary>
        /// <param name="currentUserId"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        private async Task SetQuizzesCompletedByCurrentUser(string currentUserId, TriviaDashboardViewModel viewModel)
        {
            var completedQuizzes = await TriviaService.GetCompletedQuizzesForUserAsync(currentUserId);

            foreach (var quiz in viewModel.NewQuizzes)
            {
                if (completedQuizzes.Any(q => q.Key == quiz.Id))
                {
                    quiz.IsComplete = true;
                }
            }

            foreach (var quizCollection in viewModel.DailyQuizzes.Values)
            {
                foreach (var quiz in quizCollection)
                {
                    if (completedQuizzes.Any(q => q.Key == quiz.Id))
                    {
                        quiz.IsComplete = true;
                    }
                }
            }
        }

        [AllowAnonymous, RequireProfileIfAuthenticated, RequireConfirmedEmailIfAuthenticated]
        public async Task<ActionResult> Category(int id, string seoName)
        {
            var quizCategory = await TriviaService.GetQuizCategoryAsync(id);
            if (quizCategory == null)
            {
                return new HttpNotFoundResult();
            }

            string expectedSeoName = quizCategory.GetSEOName();
            if (seoName != expectedSeoName)
            {
                return RedirectToAction("category", new { id = id, seoName = expectedSeoName });
            }

            await SetNotificationsAsync();
            var currentUserId = User.Identity.GetUserId();

            TriviaCategoryViewModel viewModel = new TriviaCategoryViewModel();

            var quizzesInCategory = await TriviaService.GetQuizzesInCategoryAsync(id);
            viewModel.Quizzes = Mapper.Map<IReadOnlyCollection<Quiz>, IReadOnlyCollection<QuizOverviewViewModel>>(quizzesInCategory);

            var quizCategories = await TriviaService.GetQuizCategoriesAsync();
            viewModel.QuizCategories = Mapper.Map<IReadOnlyCollection<QuizCategory>, IReadOnlyCollection<QuizCategoryViewModel>>(quizCategories);

            viewModel.TrendingQuizzes = await GetTrendingQuizzesAsync();
            viewModel.PopularQuizzes = await GetPopularQuizzesAsync();

            viewModel.RecentlyCompletedQuizzes = await TriviaService.GetUsersCompletedQuizzesAsync(currentUserId);

            viewModel.ActiveQuizCategoryId = quizCategory.Id;
            viewModel.ActiveQuizCategoryName = quizCategory.Name;
            viewModel.ActiveQuizCategoryIcon = quizCategory.FontAwesomeIconName;

            return View(viewModel);
        }

        [AllowAnonymous, RequireProfileIfAuthenticated, RequireConfirmedEmailIfAuthenticated]
        public async Task<ActionResult> Top()
        {
            await SetNotificationsAsync();
            var currentUserId = User.Identity.GetUserId();

            TriviaTopPlayersViewModel viewModel = new TriviaTopPlayersViewModel();

            var topPlayers = await TriviaService.GetTopPlayersAsync();
            viewModel.Players = Mapper.Map<IReadOnlyCollection<Models.Profile>, IReadOnlyCollection<ProfileViewModel>>(topPlayers);

            var quizCategories = await TriviaService.GetQuizCategoriesAsync();
            viewModel.QuizCategories = Mapper.Map<IReadOnlyCollection<QuizCategory>, IReadOnlyCollection<QuizCategoryViewModel>>(quizCategories);

            viewModel.TrendingQuizzes = await GetTrendingQuizzesAsync();
            viewModel.PopularQuizzes = await GetPopularQuizzesAsync();

            viewModel.RecentlyCompletedQuizzes = await TriviaService.GetUsersCompletedQuizzesAsync(currentUserId);

            return View(viewModel);
        }

        #endregion Dashboard

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
        [AllowAnonymous, RequireProfileIfAuthenticated, RequireConfirmedEmailIfAuthenticated]
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

            var quizCategories = await TriviaService.GetQuizCategoriesAsync();
            viewModel.QuizCategories = Mapper.Map<IReadOnlyCollection<QuizCategory>, IReadOnlyCollection<QuizCategoryViewModel>>(quizCategories);

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
            var currentUserId = User.Identity.GetUserId();

            int? currentUserProfileId = await UserService.GetProfileIdAsync(currentUserId);

            // only allow submit answer if the user has a profile
            if (currentUserProfileId.HasValue)
            {
                var result = await TriviaService.RecordAnsweredQuestionAsync(
                    currentUserId,
                    currentUserProfileId.Value,
                    questionId,
                    answerId,
                    (int)QuestionTypeValues.Random);

                if (result.Succeeded)
                {
                    return Json(new { success = true, correctAnswerId = result.CorrectAnswerId });
                }
            }
            else
            {
                Log.Error("TriviaController/SubmitAnswer", ErrorMessages.AnswerNotSubmitted,
                    parameters: new { questionId, answerId }
                );
            }

            return Json(new { success = false, error = ErrorMessages.AnswerNotSubmitted });
        }

        #endregion Random Question

        #region Timed Question

        /// <summary>
        /// Returns a view containing a timed random question. Redirects to the user's create profile if no profile exists for the user.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous, RequireProfileIfAuthenticated, RequireConfirmedEmailIfAuthenticated]
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

            var quizCategories = await TriviaService.GetQuizCategoriesAsync();
            viewModel.QuizCategories = Mapper.Map<IReadOnlyCollection<QuizCategory>, IReadOnlyCollection<QuizCategoryViewModel>>(quizCategories);

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
            DateTime utcNow = DateTime.UtcNow;
            string responseMessage = String.Empty;

            if (QuestionStartTime.HasValue)
            {
                TimeSpan timeSpentOnQuestion = utcNow.Subtract(QuestionStartTime.Value);
                bool didUserAnswerInTime = timeSpentOnQuestion.TotalSeconds <= 10 ? true : false;
                QuestionStartTime = null;

                if (didUserAnswerInTime)
                {
                    var currentUserId = User.Identity.GetUserId();

                    int? currentUserProfileId = await UserService.GetProfileIdAsync(currentUserId);

                    if (currentUserProfileId.HasValue)
                    {
                        // log the answer for this user's question history
                        var result = await TriviaService.RecordAnsweredQuestionAsync(
                            currentUserId,
                            currentUserProfileId.Value,
                            questionId,
                            answerId,
                            (int)QuestionTypeValues.Timed);

                        if (result.Succeeded)
                        {
                            return Json(new { success = true, correctAnswerId = result.CorrectAnswerId });
                        }
                    }
                    else
                    {
                        Log.Error("SubmitTimedAnswer", ErrorMessages.AnswerNotSubmitted,
                            parameters: new { questionId = questionId, answerId = answerId }
                        );

                        responseMessage = ErrorMessages.AnswerNotSubmitted;
                    }
                }
                else
                {
                    responseMessage = ErrorMessages.TimedQuestionOutOfTime;
                }
            }
            else
            {
                responseMessage = ErrorMessages.TimedQuestionNoStart;
            }

            return Json(new { success = false, error = responseMessage });
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
        [AllowAnonymous, RequireProfileIfAuthenticated, ImportModelStateFromTempData]
        public async Task<ActionResult> Quiz(int id, string seoName)
        {
            await SetNotificationsAsync();

            var currentUserId = User.Identity.GetUserId();

            // if there is no quiz by this id, return not found
            var quiz = await TriviaService.GetQuizAsync(id);
            if (quiz == null)
            {
                return new HttpNotFoundResult();
            }

            // redirect to the SEO name if not provided
            string expectedSeoName = quiz.GetSEOName();
            if (seoName != expectedSeoName)
            {
                return RedirectToAction("quiz", new { id = id, seoName = expectedSeoName });
            }

            bool isAlreadyCompleted = false;
            List<QuestionViewModel> questionListViewModel = new List<QuestionViewModel>();
            MinefieldQuestionViewModel minefieldQuestion = new MinefieldQuestionViewModel();

            if (User.Identity.IsAuthenticated)
            {
                // check if the quiz has already been completed by the user
                isAlreadyCompleted = await TriviaService.IsQuizCompletedByUserAsync(currentUserId, quiz.Id);

                // if it has already been completed, get answered questions for quiz and user
                if (isAlreadyCompleted)
                {
                    if (quiz.QuizTypeId == (int)QuizTypeValues.Individual)
                    {
                        questionListViewModel = await GetQuestionsForIndividualQuizAsync(currentUserId, quiz.Id, quiz.Questions);
                    }
                    else
                    {
                        minefieldQuestion = await GetQuestionForMinefieldQuizAsync(currentUserId, quiz.Id, quiz.MinefieldQuestion);
                    }
                }
                // if it hasn't been completed, get unanswered questions for quiz
                else
                {
                    if (quiz.QuizTypeId == (int)QuizTypeValues.Individual)
                    {
                        questionListViewModel = Mapper.Map<ICollection<Question>, List<QuestionViewModel>>(quiz.Questions);
                    }
                    else
                    {
                        minefieldQuestion = Mapper.Map<MinefieldQuestion, MinefieldQuestionViewModel>(quiz.MinefieldQuestion);
                    }
                }
            }
            // user isn't authenticated, so anonymous users the questions
            else
            {
                if (quiz.QuizTypeId == (int)QuizTypeValues.Individual)
                {
                    questionListViewModel = Mapper.Map<ICollection<Question>, List<QuestionViewModel>>(quiz.Questions);
                }
                else
                {
                    minefieldQuestion = Mapper.Map<MinefieldQuestion, MinefieldQuestionViewModel>(quiz.MinefieldQuestion);
                }
            }

            var usersCompletedQuiz = await TriviaService.GetUsersCompletedQuizAsync(id, currentUserId);
            var tagsForQuiz = await GetTagsForQuizAsync(id);
            var questionViolationViewModel = await GetQuestionViolationViewModelAsync();
            var similarQuizzes = await TriviaService.GetSimilarQuizzesAsync(id);
            var quizCategories = await TriviaService.GetQuizCategoriesAsync();
            var usersWithSimilarScores = await TriviaService.GetUsersWithSimilarScoresAsync(currentUserId, id, 4);

            QuizViewModel viewModel = new QuizViewModel()
            {
                Questions = questionListViewModel,
                MinefieldQuestion = minefieldQuestion,
                QuizName = quiz.Name,
                QuizId = id,
                QuizTypeId = quiz.QuizTypeId,
                IsAlreadyCompleted = isAlreadyCompleted,
                QuizDescription = quiz.Description,
                UsersCompletedQuiz = usersCompletedQuiz,
                Tags = tagsForQuiz,
                QuestionViolation = questionViolationViewModel,
                ThumbnailImageUrl = quiz.GetThumbnailImagePath(),
                ImageUrl = quiz.GetImagePath(),
                SimilarQuizzes = Mapper.Map<IReadOnlyCollection<Quiz>, IReadOnlyCollection<QuizOverviewViewModel>>(similarQuizzes),
                QuizCategories = Mapper.Map<IReadOnlyCollection<QuizCategory>, IReadOnlyCollection<QuizCategoryViewModel>>(quizCategories),
                UsersWithSimilarScores = usersWithSimilarScores
            };

            if (TempData["DidUserJustCompleteQuiz"] != null)
            {
                viewModel.PointsObtainedProgress = await MilestoneService.GetAchievementProgressForUserAsync(currentUserId, (int)MilestoneTypeValues.PointsObtained);
                viewModel.PointsObtainedProgress.IncreaseAchievedCount = viewModel.PointsEarned;

                viewModel.QuizzesCompletedProgress = await MilestoneService.GetAchievementProgressForUserAsync(currentUserId, (int)MilestoneTypeValues.QuizzesCompletedSuccessfully);
                viewModel.QuizzesCompletedProgress.IncreaseAchievedCount = 1;

                viewModel.TagsAwardedProgress = await MilestoneService.GetAchievementProgressForUserAsync(currentUserId, (int)MilestoneTypeValues.TagsAwarded);
                viewModel.TagsAwardedProgress.IncreaseAchievedCount = TempData["TagsAwardedCount"] != null ? Int32.Parse(TempData["TagsAwardedCount"].ToString()) : 0;
            }

            return View(viewModel);
        }

        private async Task<List<QuestionViewModel>> GetQuestionsForIndividualQuizAsync(string currentUserId, int quizId, ICollection<Question> questionsEntity)
        {
            // get the already answered questions for this quiz
            var answeredQuizQuestions = await TriviaService.GetAnsweredQuizQuestionsAsync(currentUserId, quizId);

            // get the list of all possible questions for this quiz
            //var quizQuestions = await TriviaService.GetQuizQuestionsAsync(quizId);
            var questionListViewModel = Mapper.Map<ICollection<Question>, List<QuestionViewModel>>(questionsEntity);

            // match up the already selected answers with the questions for this quiz
            foreach (var questionViewModel in questionListViewModel)
            {
                var answeredQuizQuestion = answeredQuizQuestions[questionViewModel.QuestionId];
                questionViewModel.SelectedAnswerId = answeredQuizQuestion.AnswerId;
                questionViewModel.IsAlreadyAnswered = true;

                // mark the correct answer to show the user on the UI that it was correct
                foreach (var answer in questionViewModel.Answers)
                {
                    answer.IsCorrect = (answer.AnswerId == questionViewModel.CorrectAnswerId);
                }
            }

            return questionListViewModel;
        }

        private async Task<MinefieldQuestionViewModel> GetQuestionForMinefieldQuizAsync(string currentUserId, int quizId, MinefieldQuestion minefieldQuestionEntity)
        {
            // get the already answered questions for this quiz
            var answeredQuizQuestions = await TriviaService.GetSelectedMinefieldAnswersAsync(currentUserId, quizId);

            var minefieldQuestion = Mapper.Map<MinefieldQuestion, MinefieldQuestionViewModel>(minefieldQuestionEntity);

            // mark up the answers that were selected and are correct for the user's quiz score
            foreach (var answer in minefieldQuestion.Answers)
            {
                AnsweredMinefieldQuestion answeredQuizQuestion = null;
                bool selectedByUser = answeredQuizQuestions.TryGetValue(answer.AnswerId, out answeredQuizQuestion);
                if (selectedByUser)
                {
                    answer.IsSelected = true;
                    answer.IsCorrect = answeredQuizQuestion.Answer.IsCorrect;
                }
            }

            return minefieldQuestion;
        }

        /// <summary>
        /// Returns a collection of tag associated with the questions of a quiz.
        /// </summary>
        /// <param name="quizId"></param>
        /// <returns></returns>
        private async Task<IReadOnlyCollection<TagViewModel>> GetTagsForQuizAsync(int quizId)
        {
            var tags = await TriviaService.GetTagsForQuizAsync(quizId);
            return tags;
        }

        #endregion Quiz

        #region Individual Quiz

        /// <summary>
        /// Submits the completion of a quiz for the currently logged in user.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost, ExportModelStateToTempData, ValidateAntiForgeryToken]
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
            int numberOfTagsAwarded = 0;
            foreach (var question in viewModel.Questions)
            {
                var result = await TriviaService.RecordAnsweredQuestionAsync(
                    currentUserId,
                    profile.Id,
                    question.QuestionId,
                    question.SelectedAnswerId.Value,
                    (int)QuestionTypeValues.Quiz);

                if (result.Succeeded)
                {
                    if (question.SelectedAnswerId.Value == result.CorrectAnswerId)
                    {
                        numberOfCorrectAnswers++;
                    }

                    numberOfTagsAwarded += result.TagsAwardedCount;
                }
            }

            TempData["TagsAwardedCount"] = numberOfTagsAwarded;
            TempData["DidUserJustCompleteQuiz"] = true;

            int count = await TriviaService.SetQuizAsCompletedAsync(currentUserId, viewModel.QuizId, numberOfCorrectAnswers);

            return RedirectToAction("quiz", new { id = viewModel.QuizId, seoName = viewModel.SEOName });
        }

        #endregion Individual Quiz

        #region Minefield Quiz

        [HttpPost, ExportModelStateToTempData, ValidateAntiForgeryToken]
        public async Task<ActionResult> MinefieldQuiz(QuizViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("quiz", new { id = viewModel.QuizId, seoName = viewModel.SEOName });
            }

            string currentUserId = User.Identity.GetUserId();

            var profile = await ProfileService.GetProfileAsync(currentUserId);

            // loop through questions and record answers
            var result = await TriviaService.RecordAnsweredMinefieldQuestionAsync(
                currentUserId,
                viewModel.MinefieldQuestion.MinefieldQuestionId,
                viewModel.MinefieldQuestion.Answers);

            if (result.Succeeded)
            {
                int count = await TriviaService.SetQuizAsCompletedAsync(currentUserId, viewModel.QuizId, result.CorrectAnswerCount);
            }

            TempData["TagsAwardedCount"] = 0;
            TempData["DidUserJustCompleteQuiz"] = true;

            return RedirectToAction("quiz", new { id = viewModel.QuizId, seoName = viewModel.SEOName });
        }

        #endregion Minefield Quiz

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
            var randomQuestion = await TriviaService.GetRandomQuestionAsync(currentUserId, questionTypeId);

            QuestionViewModel viewModel = Mapper.Map<Question, QuestionViewModel>(randomQuestion);

            if (viewModel != null)
            {
                viewModel.QuestionTypeId = questionTypeId;
                viewModel.UsersAnsweredCorrectly = await GetUsersAnsweredCorrectlyAsync(randomQuestion.Id);
                viewModel.Tags = await GetTagsForQuestionAsync(randomQuestion.Id);

                foreach (var user in viewModel.UsersAnsweredCorrectly)
                {
                    user.IsFavoritedByCurrentUser = await ProfileService.IsProfileFavoritedByUserAsync(user.ProfileId, currentUserId);
                }
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
            var tags = await TriviaService.GetTagsForQuestionAsync(questionId);
            return Mapper.Map<IReadOnlyCollection<Tag>, IReadOnlyCollection<TagViewModel>>(tags);
        }

        /// <summary>
        /// Returns a collection of users who answered a question correctly.
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        private async Task<IReadOnlyCollection<UserAnsweredQuestionCorrectlyViewModel>> GetUsersAnsweredCorrectlyAsync(int questionId)
        {
            var usersAnsweredCorrectly = await TriviaService.GetUsersAnsweredCorrectlyAsync(questionId);
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
                var violationTypes = await ViolationService.GetQuestionViolationTypesAsync();
                viewModel.ViolationTypes = violationTypes.ToDictionary(v => v.Id, v => v.Name);
            }

            return viewModel;
        }

        #region Add Questions to Quiz (Admin Page)

        [Authorize(Users = "justin")]
        public async Task<ActionResult> AddQuizQuestions(int id)
        {
            AddQuizQuestionsViewModel viewModel = new AddQuizQuestionsViewModel();
            viewModel.QuizId = id;
            viewModel.Questions = new List<CreateQuestionViewModel>();
            SetupQuestions(viewModel);

            viewModel.Tags = await GetSearchableTagsAsync();

            return View(viewModel);
        }

        [ValidateAntiForgeryToken, HttpPost, Authorize(Users = "justin")]
        public async Task<ActionResult> AddQuizQuestions(AddQuizQuestionsViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                foreach (var question in viewModel.Questions)
                {
                    var answerContents = question.Answers.Select(x => x.Content).ToList();
                    var result = await TriviaService.AddQuestionToQuizAsync(
                        viewModel.QuizId,
                        question.Content,
                        question.Points,
                        answerContents.AsReadOnly(),
                        question.CorrectAnswer,
                        question.SelectedTags.AsReadOnly());
                }
            }

            viewModel.Tags = await GetSearchableTagsAsync();

            return View(viewModel);
        }

        private static void SetupQuestions(AddQuizQuestionsViewModel viewModel)
        {
            for (int i = 0; i < 10; i++)
            {
                CreateQuestionViewModel q = new CreateQuestionViewModel();
                List<CreateAnswerViewModel> answers = new List<CreateAnswerViewModel>();
                for (int j = 0; j < 4; j++)
                {
                    CreateAnswerViewModel a = new CreateAnswerViewModel();
                    answers.Add(a);
                }
                q.Answers = answers;
                viewModel.Questions.Add(q);
            }
        }

        private async Task<Dictionary<int, string>> GetSearchableTagsAsync()
        {
            var allTags = await ProfileService.GetAllTagsAsync();
            Dictionary<int, string> d = new Dictionary<int, string>();

            foreach (var tag in allTags)
            {
                d.Add(tag.TagId, tag.Name);
            }

            return d;
        }

        #endregion Add Questions to Quiz (Admin Page)
    }
}