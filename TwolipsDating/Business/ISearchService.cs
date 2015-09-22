using System.Collections.Generic;
using TwolipsDating.ViewModels;

namespace TwolipsDating.Business
{
    public interface ISearchService : IBaseService
    {
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.Profile>> GetProfilesByTagNameAsync(string userName);

        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.Profile>> GetProfilesByTagNamesAsync(string[] tags);

        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.Profile>> GetProfilesByUserNameAsync(string userName);

        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.Profile>> GetProfilesByUserNameAsync(string userName, string currentUserId);

        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.ViewModels.QuizSearchResultViewModel>> GetQuizzesByTagsAsync(string tag);

        System.Threading.Tasks.Task<IReadOnlyCollection<SearchResultProfileViewModel>> GetProfilesAsync(string userId);
    }
}