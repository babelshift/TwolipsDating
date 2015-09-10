using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TwolipsDating.Models;
using TwolipsDating.ViewModels;

namespace TwolipsDating.Business
{
    public class MilestoneService : BaseService
    {
        public TriviaService TriviaService { private get; set; }
        public ProfileService ProfileService { private get; set; }

        private MilestoneService(ApplicationDbContext db, IIdentityMessageService emailService)
            : base(db, emailService)
        {
        }

        internal static MilestoneService Create(IdentityFactoryOptions<MilestoneService> options, IOwinContext context)
        {
            var service = new MilestoneService(context.Get<ApplicationDbContext>(), new EmailService());
            return service;
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

            // get the number of questions answered correctly
            if (milestoneTypeId == (int)MilestoneTypeValues.QuestionsAnsweredCorrectly)
            {
                amount = await TriviaService.GetQuestionsAnsweredCorrectlyCountAsync(userId);
            }
            // get the number of quizzes completed successfully
            else if (milestoneTypeId == (int)MilestoneTypeValues.QuizzesCompletedSuccessfully)
            {
                var completedQuizzes = await TriviaService.GetCompletedQuizzesForUserAsync(userId);
                amount = completedQuizzes.Count;
            }
            // get the number of gifts sent
            else if (milestoneTypeId == (int)MilestoneTypeValues.GiftSent)
            {
                amount = await ProfileService.GetSentGiftCountForUserAsync(userId);
            }
            // get the number of gifts purchased
            else if (milestoneTypeId == (int)MilestoneTypeValues.GiftsPurchased)
            {
                amount = await ProfileService.GetPurchasedItemCountForUserAsync(userId, (int)StoreItemTypeValues.Gift);
            }
            // get the number of points obtained
            else if (milestoneTypeId == (int)MilestoneTypeValues.PointsObtained)
            {
                amount = await ProfileService.GetPointsForUserAsync(userId);
            }
            // get the number of profiles reviewed
            else if (milestoneTypeId == (int)MilestoneTypeValues.ProfileReviewsWritten)
            {
                amount = await ProfileService.GetReviewsWrittenCountByUserAsync(userId);
            }
            // get the number of images uploaded
            else if (milestoneTypeId == (int)MilestoneTypeValues.ProfileImagesUploaded)
            {
                amount = await ProfileService.GetImagesUploadedCountByUserAsync(userId);
            }
            // get the number of titles purchased
            else if (milestoneTypeId == (int)MilestoneTypeValues.TitlesPurchased)
            {
                amount = await ProfileService.GetPurchasedItemCountForUserAsync(userId, (int)StoreItemTypeValues.Title);
            }
            // get the number of tags awarded
            else if (milestoneTypeId == (int)MilestoneTypeValues.TagsAwarded)
            {
                amount = await ProfileService.GetTagAwardCountForUserAsync(userId);
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

        private async Task<int> GetUserProgressTowardsAchievement(string userId, int milestoneTypeId)
        {
            int count = 0;

            if (milestoneTypeId == (int)MilestoneTypeValues.GiftsPurchased)
            {
                count = await (from storeTransaction in db.StoreTransactions
                               where storeTransaction.UserId == userId
                               where storeTransaction.StoreItem.ItemTypeId == (int)StoreItemTypeValues.Gift
                               select storeTransaction).CountAsync();
            }
            else if (milestoneTypeId == (int)MilestoneTypeValues.GiftSent)
            {
                count = await (from giftTransaction in db.GiftTransactions
                               where giftTransaction.FromUserId == userId
                               select giftTransaction).CountAsync();
            }
            else if (milestoneTypeId == (int)MilestoneTypeValues.PointsObtained)
            {
                int pointsSpent = (await (from storeTransaction in db.StoreTransactions
                                          where storeTransaction.UserId == userId
                                          select (int?)storeTransaction.PointPrice).SumAsync()) ?? 0;

                int currentPoints = (await (from user in db.Users
                                            where user.Id == userId
                                            select (int?)user.Points).FirstOrDefaultAsync()) ?? 0;

                count = pointsSpent + currentPoints;
            }
            else if (milestoneTypeId == (int)MilestoneTypeValues.ProfileImagesUploaded)
            {
                count = await (from userImage in db.UserImages
                               where userImage.ApplicationUserId == userId
                               select userImage).CountAsync();
            }
            else if (milestoneTypeId == (int)MilestoneTypeValues.ProfileReviewsWritten)
            {
                count = await (from review in db.Reviews
                               where review.AuthorUserId == userId
                               select review).CountAsync();
            }
            else if (milestoneTypeId == (int)MilestoneTypeValues.QuestionsAnsweredCorrectly)
            {
                count = await (from answeredQuestion in db.AnsweredQuestions
                               join question in db.Questions on answeredQuestion.AnswerId equals question.CorrectAnswerId
                               where answeredQuestion.UserId == userId
                               select answeredQuestion).CountAsync();
            }
            else if (milestoneTypeId == (int)MilestoneTypeValues.QuizzesCompletedSuccessfully)
            {
                // need to calculate if the quiz was successful, come up with a % threshold
                count = await (from quizCompletions in db.CompletedQuizzes
                               where quizCompletions.UserId == userId
                               select quizCompletions).CountAsync();

                // need to get correct answer count for quiz and total answer count

                // there's probably a query somewhere which calculates this for us already
            }
            else if (milestoneTypeId == (int)MilestoneTypeValues.TagsAwarded)
            {
                count = await (from tagAward in db.TagAwards
                               where tagAward.Profile.ApplicationUser.Id == userId
                               select tagAward).CountAsync();
            }
            else if (milestoneTypeId == (int)MilestoneTypeValues.TitlesPurchased)
            {
                count = await (from storeTransaction in db.StoreTransactions
                               where storeTransaction.UserId == userId
                               where storeTransaction.StoreItem.ItemTypeId == (int)StoreItemTypeValues.Title
                               select storeTransaction).CountAsync();
            }

            return count;
        }

        internal async Task<IReadOnlyCollection<AchievementOverviewViewModel>> GetAchievementsAndStatusForUserAsync(string userId)
        {
            var milestones = from milestone in db.Milestones
                             orderby new { milestone.MilestoneTypeId, milestone.AmountRequired }
                             select milestone;

            var results = await milestones.ToListAsync();

            List<AchievementOverviewViewModel> achievementOverviews = new List<AchievementOverviewViewModel>();
            List<AchievementStatusViewModel> currentAchievementStatuses = new List<AchievementStatusViewModel>();
            AchievementOverviewViewModel currentAchievementOverView = new AchievementOverviewViewModel();

            int currentMilestoneTypeId = 0;
            int previousMilestoneTypeId = 0;
            Milestone previousMilestone = null;
            foreach (var milestone in results)
            {
                previousMilestoneTypeId = currentMilestoneTypeId;
                currentMilestoneTypeId = milestone.MilestoneTypeId;

                // we hit a new milestone type, create a new collection
                if (currentMilestoneTypeId != previousMilestoneTypeId)
                {
                    // save off the current group before starting over but not if this is the first item
                    if (previousMilestoneTypeId > 0)
                    {
                        currentAchievementOverView.AchievementStatuses = currentAchievementStatuses;
                        currentAchievementOverView.AchievementTypeName = previousMilestone.MilestoneType.Name;
                        currentAchievementOverView.AchievementDescription = previousMilestone.MilestoneType.Description;
                        achievementOverviews.Add(currentAchievementOverView);
                    }

                    // start over for the next group
                    currentAchievementOverView = new AchievementOverviewViewModel();
                    currentAchievementStatuses = new List<AchievementStatusViewModel>();
                }

                // look up the user's status on this milestone
                int amountAchieved = await GetUserProgressTowardsAchievement(userId, milestone.MilestoneTypeId);

                currentAchievementStatuses.Add(new AchievementStatusViewModel()
                    {
                        RequiredCount = milestone.AmountRequired,
                        AchievedCount = amountAchieved <= milestone.AmountRequired ? amountAchieved : milestone.AmountRequired,
                        AchievementStatus = (amountAchieved >= milestone.AmountRequired) ? AchievementStatusType.Complete : AchievementStatusType.Incomplete
                    });

                previousMilestone = milestone;
            }

            return achievementOverviews.AsReadOnly(); ;
        }

        internal async Task<int> GetCompletedAchievementCount(string userId)
        {
            int completedCount = await (from achievements in db.MilestoneAchievements
                                        where achievements.UserId == userId
                                        select achievements).CountAsync();

            return completedCount;
        }

        internal async Task<int> GetPossibleAchievementCount()
        {
            int possibleCount = await (from achievements in db.Milestones
                                       select achievements).CountAsync();

            return possibleCount;
        }
    }
}