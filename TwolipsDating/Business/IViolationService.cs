using System;
namespace TwolipsDating.Business
{
    public interface IViolationService : IBaseService
    {
        System.Threading.Tasks.Task<int> AddQuestionViolation(int questionId, int violationTypeId, string authorUserId);
        System.Threading.Tasks.Task<int> AddReviewViolation(int reviewId, int violationTypeId, string content, string authorUserId);
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.QuestionViolationType>> GetQuestionViolationTypesAsync();
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.ViolationType>> GetViolationTypesAsync();
        System.Threading.Tasks.Task<bool> HasUserAlreadyReportedQuestion(int questionId, string userId);
        System.Threading.Tasks.Task<bool> HasUserAlreadyReportedReview(int reviewId, string userId);
    }
}
