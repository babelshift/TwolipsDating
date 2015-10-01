using System;
namespace TwolipsDating.Business
{
    public interface IMilestoneService : IBaseService
    {
        System.Threading.Tasks.Task AwardAchievedMilestonesAsync(string userId, int milestoneTypeId);
        System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyCollection<TwolipsDating.ViewModels.AchievementOverviewViewModel>> GetAchievementsAndStatusForUserAsync(string userId);
        System.Threading.Tasks.Task<int> GetCompletedAchievementCount(string userId);
        System.Threading.Tasks.Task<int> GetPossibleAchievementCount();
        IProfileService ProfileService { set; }
        ITriviaService TriviaService { set; }
        IUserService UserService { get; set; }
    }
}
