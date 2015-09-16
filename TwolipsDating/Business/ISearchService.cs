using System;
namespace TwolipsDating.Business
{
    public interface ISearchService : IBaseService
    {
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.Profile>> GetProfilesByTagNameAsync(string userName);
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.Profile>> GetProfilesByTagNamesAsync(string[] tags);
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.Profile>> GetProfilesByUserNameAsync(string userName);
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.Profile>> GetProfilesByUserNameAsync(string userName, string currentUserId);
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.ViewModels.QuizSearchResultViewModel>> GetQuizzesByTagsAsync(string tag);
    }
}
