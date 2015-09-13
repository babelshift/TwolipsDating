﻿using System;
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
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.Quiz>> GetNewQuizzesAsync();
        System.Threading.Tasks.Task<int> GetQuestionsAnsweredCorrectlyCountAsync(string userId);
        System.Threading.Tasks.Task<TwolipsDating.Models.Quiz> GetQuizAsync(int quizId);
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.Question>> GetQuizQuestionsAsync(int quizId);
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.Quiz>> GetQuizzesAsync();
        System.Threading.Tasks.Task<TwolipsDating.Models.Question> GetRandomQuestionAsync(string userId, int questionTypeId);
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.Tag>> GetTagsForQuestionAsync(int questionId);
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.ViewModels.TagViewModel>> GetTagsForQuizAsync(int quizId);
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.AnsweredQuestion>> GetUsersAnsweredCorrectlyAsync(int questionId);
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.ViewModels.UserCompletedQuizViewModel>> GetUsersCompletedQuizAsync(int? quizId = null);
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.ViewModels.UserCompletedQuizViewModel>> GetUsersCompletedQuizzesAsync();
        System.Threading.Tasks.Task<int> GetUsersQuestionPointsForTypeAsync(string userId, int questionTypeId);
        System.Threading.Tasks.Task<bool> IsQuizAlreadyCompletedAsync(string userId, int quizId);
        System.Threading.Tasks.Task<AnsweredQuestionServiceResult> RecordAnsweredQuestionAsync(string userId, int profileId, int questionId, int answerId, int questionTypeId);
        System.Threading.Tasks.Task<int> SetQuizAsCompleted(string userId, int quizId, int numberOfCorrectAnswers);
        Task<ReadOnlyDictionary<int, IReadOnlyCollection<Quiz>>> GetDailyQuizzesAsync(int daysAgo);

        Task<IReadOnlyCollection<TrendingQuizViewModel>> GetTrendingQuizzesAsync();
        Task<IReadOnlyCollection<Quiz>> GetPopularQuizzesAsync();
        Task<IReadOnlyCollection<Quiz>> GetUnfinishedQuizzesAsync(string userId);
    }
}
