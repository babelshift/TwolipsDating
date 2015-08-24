using Microsoft.AspNet.Identity;
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
        public MilestoneService(ApplicationDbContext db, IIdentityMessageService emailService)
            : base(db, emailService)
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

            // look up the "amount" of whatever milestone type requirement calls for
            int amount = await GetAmountForMilestoneTypeAsync(userId, milestoneTypeId);

            foreach (var milestone in milestones)
            {
                // only bother with this milestone if the user hasn't already achieved it
                if (!HasUserAlreadyAchievedMilestone(userId, milestone.Id, milestonesAchieved))
                {
                    // if the user has met or exceeded the milestone's requirements, award them the milestone
                    if (amount >= milestone.AmountRequired)
                    {
                        AwardMilestoneToUser(userId, milestone.Id);
                    }
                }
            }
        }

        private async Task<int> GetAmountForMilestoneTypeAsync(string userId, int milestoneTypeId)
        {
            int amount = 0;

            TriviaService triviaService = new TriviaService(db, EmailService);
            ProfileService profileService = new ProfileService(db, EmailService);

            // get the number of questions answered correctly
            if (milestoneTypeId == (int)MilestoneTypeValues.QuestionsAnsweredCorrectly)
            {
                amount = await triviaService.GetQuestionsAnsweredCorrectlyCountAsync(userId);
            }
            // get the number of quizzes completed successfully
            else if (milestoneTypeId == (int)MilestoneTypeValues.QuizzesCompletedSuccessfully)
            {
                var completedQuizzes = await triviaService.GetCompletedQuizzesForUserAsync(userId);
                amount = completedQuizzes.Count;
            }
            // get the number of gifts sent
            else if (milestoneTypeId == (int)MilestoneTypeValues.GiftSent)
            {
                amount = await profileService.GetSentGiftCountForUserAsync(userId);
            }
            // get the number of gifts purchased
            else if (milestoneTypeId == (int)MilestoneTypeValues.GiftsPurchased)
            {
                amount = await profileService.GetPurchasedItemCountForUserAsync(userId, (int)StoreItemTypeValues.Gift);
            }
            // get the number of points obtained
            else if (milestoneTypeId == (int)MilestoneTypeValues.PointsObtained)
            {
                amount = await profileService.GetPointsForUserAsync(userId);
            }
            // get the number of profiles reviewed
            else if (milestoneTypeId == (int)MilestoneTypeValues.ProfileReviewsWritten)
            {
                amount = await profileService.GetReviewsWrittenCountByUserAsync(userId);
            }
            // get the number of images uploaded
            else if (milestoneTypeId == (int)MilestoneTypeValues.ProfileImagesUploaded)
            {
                amount = await profileService.GetImagesUploadedCountByUserAsync(userId);
            }
            // get the number of titles purchased
            else if (milestoneTypeId == (int)MilestoneTypeValues.TitlesPurchased)
            {
                amount = await profileService.GetPurchasedItemCountForUserAsync(userId, (int)StoreItemTypeValues.Title);
            }
            // get the number of tags awarded
            else if (milestoneTypeId == (int)MilestoneTypeValues.TagsAwarded)
            {
                amount = await profileService.GetTagAwardCountForUserAsync(userId);
            }

            return amount;
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