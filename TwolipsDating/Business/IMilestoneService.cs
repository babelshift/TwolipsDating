using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwolipsDating.ViewModels;
namespace TwolipsDating.Business
{
    public interface IMilestoneService : IBaseService
    {
        System.Threading.Tasks.Task<AwardAchievementServiceResult> AwardAchievedMilestonesAsync(string userId, int milestoneTypeId);
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.ViewModels.AchievementOverviewViewModel>> GetAchievementsAndStatusForUserAsync(string userId);
        System.Threading.Tasks.Task<int> GetCompletedAchievementCountAsync(string userId);
        System.Threading.Tasks.Task<int> GetPossibleAchievementCountAsync();
        IProfileService ProfileService { set; }
        ITriviaService TriviaService { set; }
        IUserService UserService { get; set; }
        Task<AchievementProgressViewModel> GetAchievementProgressForUserAsync(string userId, int milestoneTypeId);

        Task<IReadOnlyCollection<AchievementUnlockedViewModel>> GetMilestoneDetailsAsync(System.Collections.Generic.IReadOnlyCollection<int> milestoneIdsUnlocked);
        Task<IReadOnlyCollection<AchievementCompletedViewModel>> GetCompletedAchievementsAsync(string userId);
    }
}
