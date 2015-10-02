using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TwolipsDating.Models;
using TwolipsDating.ViewModels;
namespace TwolipsDating.Business
{
    public interface ITriviaService : IBaseService
    {
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyDictionary<int, TwolipsDating.Models.AnsweredQuestion>> GetAnsweredQuizQuestions(string userId, int quizId);
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyDictionary<int, TwolipsDating.Models.CompletedQuiz>> GetCompletedQuizzesForUserAsync(string currentUserId);
        System.Threading.Tasks.Task<int> GetCorrectAnswerAsync(int questionId, int answerId);
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.Quiz>> GetNewQuizzesAsync(int takeCount = 10);
        System.Threading.Tasks.Task<int> GetQuestionsAnsweredCorrectlyCountAsync(string userId);
        System.Threading.Tasks.Task<TwolipsDating.Models.Quiz> GetQuizAsync(int quizId);
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.Question>> GetQuizQuestionsAsync(int quizId);
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.Quiz>> GetQuizzesAsync();
        System.Threading.Tasks.Task<TwolipsDating.Models.Question> GetRandomQuestionAsync(string userId, int questionTypeId);
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.Tag>> GetTagsForQuestionAsync(int questionId);
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.ViewModels.TagViewModel>> GetTagsForQuizAsync(int quizId);
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.AnsweredQuestion>> GetUsersAnsweredCorrectlyAsync(int questionId);
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.ViewModels.UserCompletedQuizViewModel>> GetUsersCompletedQuizAsync(int? quizId = null, string currentUserId = null);
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.ViewModels.UserCompletedQuizViewModel>> GetUsersCompletedQuizzesAsync(string currentUserId = null);
        System.Threading.Tasks.Task<int> GetUsersQuestionPointsForTypeAsync(string userId, int questionTypeId);
        System.Threading.Tasks.Task<bool> IsQuizCompletedByUserAsync(string userId, int quizId);
        System.Threading.Tasks.Task<AnsweredQuestionServiceResult> RecordAnsweredQuestionAsync(string userId, int profileId, int questionId, int answerId, int questionTypeId);
        System.Threading.Tasks.Task<int> SetQuizAsCompleted(string userId, int quizId, int numberOfCorrectAnswers);
        Task<ReadOnlyDictionary<int, IReadOnlyCollection<Quiz>>> GetDailyQuizzesAsync(int daysAgo);

        Task<IReadOnlyCollection<TrendingQuizViewModel>> GetTrendingQuizzesAsync();
        Task<IReadOnlyCollection<MostPopularQuizViewModel>> GetPopularQuizzesAsync();
        Task<IReadOnlyCollection<Quiz>> GetUnfinishedQuizzesAsync(string userId);
        Task<IReadOnlyCollection<Quiz>> GetSimilarQuizzesAsync(int quizId);

        Task<IReadOnlyCollection<Quiz>> GetQuizzesInCategoryAsync(int id);
        Task<QuizCategory> GetQuizCategoryAsync(int id);

        Task<IReadOnlyCollection<QuizCategory>> GetQuizCategoriesAsync();

        Task<IReadOnlyCollection<Models.Profile>> GetTopPlayersAsync();

        Task<double> GetQuizScoreAsync(string userId, int quizId);

        Task<int> CountOfQuizzesCompletedAsync(string userId, int daysAgo);

        Task<int> GetQuizCategoriesTouchedByUserCountAsync(string userId);

        Task<ServiceResult> AddQuestionToQuizAsync(int quizId, string question, int points, IReadOnlyList<string> answers, int correctAnswer, IReadOnlyCollection<int> tags);

        Task<IReadOnlyCollection<UserWithSimilarQuizScoreViewModel>> GetUsersWithSimilarScoresAsync(string userId, int quizId, int numRecords);
    }
}
