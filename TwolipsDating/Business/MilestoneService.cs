using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TwolipsDating.Models;

namespace TwolipsDating.Business
{
    public class MilestoneService : BaseService
    {
        public MilestoneService(ApplicationDbContext db)
            : base(db)
        {
        }

        /// <summary>
        /// Based on the milestone type, performs a lookup to see if the user has met the requirements of any milestones of that type. If the user
        /// has met the requirements, the milestone is awarded to the user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="milestoneTypeId"></param>
        /// <returns></returns>
        public async Task AwardAchievedMilestonesAsync(string userId, int milestoneTypeId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));
            Debug.Assert(milestoneTypeId > 0);

            var milestones = await GetMilestonesByTypeAsync(milestoneTypeId); // all milestones
            var milestonesAchieved = await GetMilestonesAchievedByUserAsync(userId, milestoneTypeId); // milestones achieved by user

            foreach (var milestone in milestones)
            {
                // only bother with this milestone if the user hasn't already achieved it
                if (!HasUserAlreadyAchievedMilestone(userId, milestone.Id, milestonesAchieved))
                {
                    int amount = 0;

                    TriviaService triviaService = new TriviaService(db);
                    ProfileService profileService = new ProfileService(db);

                    if (milestone.MilestoneTypeId == (int)MilestoneTypeValues.QuestionsAnsweredCorrectly)
                    {
                        amount = await triviaService.GetQuestionsAnsweredCorrectlyCountAsync(userId);
                    }
                    else if (milestone.MilestoneTypeId == (int)MilestoneTypeValues.QuizzesCompletedSuccessfully)
                    {
                        var completedQuizzes = await triviaService.GetCompletedQuizzesForUserAsync(userId);
                        amount = completedQuizzes.Count;
                    }
                    else if (milestone.MilestoneTypeId == (int)MilestoneTypeValues.GiftSent)
                    {
                        amount = await profileService.GetSentGiftCountForUserAsync(userId);
                    }
                    else if (milestone.MilestoneTypeId == (int)MilestoneTypeValues.GiftsPurchased)
                    {
                        amount = await profileService.GetPurchasedItemCountForUserAsync(userId, (int)StoreItemTypeValues.Gift);
                    }
                    else if (milestone.MilestoneTypeId == (int)MilestoneTypeValues.PointsObtained)
                    {
                        amount = await profileService.GetPointsForUserAsync(userId);
                    }
                    else if (milestone.MilestoneTypeId == (int)MilestoneTypeValues.ProfileReviewsWritten)
                    {
                        amount = await profileService.GetReviewsWrittenCountByUserAsync(userId);
                    }
                    else if (milestone.MilestoneTypeId == (int)MilestoneTypeValues.ProfileImagesUploaded)
                    {
                        amount = await profileService.GetImagesUploadedCountByUserAsync(userId);
                    }
                    else if(milestone.MilestoneTypeId == (int)MilestoneTypeValues.TitlesPurchased)
                    {
                        amount = await profileService.GetPurchasedItemCountForUserAsync(userId, (int)StoreItemTypeValues.Title);
                    }
                    else if(milestone.MilestoneTypeId == (int)MilestoneTypeValues.TagsAwarded)
                    {
                        amount = await profileService.GetTagAwardCountForUserAsync(userId);
                    }

                    if (amount >= milestone.AmountRequired)
                    {
                        AwardMilestoneToUser(userId, milestone.Id);
                    }
                }
            }
        }

        /// <summary>
        /// Returns true if the user has already achieved the milestone, otherwise false.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="milestoneId"></param>
        /// <param name="milestonesAchieved"></param>
        /// <returns></returns>
        private bool HasUserAlreadyAchievedMilestone(string userId, int milestoneId, IReadOnlyDictionary<int, int> milestonesAchieved)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));
            Debug.Assert(milestoneId > 0);
            Debug.Assert(milestonesAchieved != null);

            int milestoneIdAchieved = 0;
            if (milestonesAchieved.TryGetValue(milestoneId, out milestoneIdAchieved))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a collection of all milestones of a certain type.
        /// </summary>
        /// <param name="milestoneTypeId"></param>
        /// <returns></returns>
        private async Task<IReadOnlyCollection<Milestone>> GetMilestonesByTypeAsync(int milestoneTypeId)
        {
            Debug.Assert(milestoneTypeId > 0);

            var milestones = from milestone in db.Milestones
                             where milestone.MilestoneTypeId == milestoneTypeId
                             select milestone;

            var results = await milestones.ToListAsync();

            return results.AsReadOnly();
        }

        /// <summary>
        /// Returns a dictionary of all milestones already achieved by a user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="milestoneTypeId"></param>
        /// <returns></returns>
        private async Task<IReadOnlyDictionary<int, int>> GetMilestonesAchievedByUserAsync(string userId, int milestoneTypeId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            var milestonesAchieved = from milestoneAchieved in db.MilestoneAchievements
                                     where milestoneAchieved.UserId == userId
                                     where milestoneAchieved.Milestone.MilestoneTypeId == milestoneTypeId
                                     select milestoneAchieved.MilestoneId;

            var results = await milestonesAchieved.ToDictionaryAsync(t => t);

            return new ReadOnlyDictionary<int, int>(results);
        }

        /// <summary>
        /// Awards a milestone to a user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="milestoneId"></param>
        private void AwardMilestoneToUser(string userId, int milestoneId)
        {
            Debug.Assert(milestoneId > 0);
            Debug.Assert(!String.IsNullOrEmpty(userId));

            MilestoneAchievement achievement = new MilestoneAchievement()
            {
                UserId = userId,
                MilestoneId = milestoneId,
                DateAchieved = DateTime.Now
            };

            db.MilestoneAchievements.Add(achievement);
        }
    }
}