using System.Collections.Generic;
using TwolipsDating.ViewModels;

namespace TwolipsDating.Business
{
    public interface ISearchService : IBaseService
    {
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.Profile>> GetProfilesByTagNameAsync(string userName);

        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<SearchResultProfileViewModel>> GetProfilesByTagNamesAsync(string[] tags, string userId);

        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.Profile>> GetProfilesByUserNameAsync(string userName);

        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.Models.Profile>> GetProfilesByUserNameAsync(string userName, string currentUserId);

        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.ViewModels.QuizOverviewViewModel>> GetQuizzesByTagsAsync(string tag);

        System.Threading.Tasks.Task<IReadOnlyCollection<SearchResultProfileViewModel>> GetAllProfilesAsync(string userId);
    }
}