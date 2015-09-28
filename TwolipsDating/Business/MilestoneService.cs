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
using TwolipsDating.Utilities;

namespace TwolipsDating.Business
{
    public class MilestoneService : BaseService, IMilestoneService
    {
        public ITriviaService TriviaService { get; set; }
        public IProfileService ProfileService { get; set; }

        public MilestoneService(ApplicationDbContext db, IIdentityMessageService emailService)
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

            var allMilestonesForType = await GetMilestonesByTypeAsync(milestoneTypeId); // all milestones
            var milestonesAchievedForType = await GetMilestonesAchievedByUserAsync(userId, milestoneTypeId); // milestones achieved by user

            // look up how much the user has achieved for this milestone
            int amountAchieved = await GetAchievedAmountForUser(userId, milestoneTypeId);

            foreach (var milestone in allMilestonesForType)
            {
                // only bother with this milestone if the user hasn't already achieved it
                if (!HasUserAchievedMilestone(userId, milestone.Id, milestonesAchievedForType))
                {
                    // if the user has met or exceeded the milestone's requirements, award them the milestone
                    if (amountAchieved >= milestone.AmountRequired)
                    {
                        AwardMilestoneToUser(userId, milestone.Id);
                    }
                }
            }
        }

        private async Task<int> GetAchievedAmountForUser(string userId, int milestoneTypeId)
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
                amount = await ProfileService.GetLifetimeForUserAsync(userId);
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
            else if (milestoneTypeId == (int)MilestoneTypeValues.Trekkie
                || milestoneTypeId == (int)MilestoneTypeValues.RebelAlliance
                || milestoneTypeId == (int)MilestoneTypeValues.HighWarlord)
            {
                int quizId = 0;
                if (milestoneTypeId == (int)MilestoneTypeValues.Trekkie)
                {
                    quizId = (int)QuizValues.StarTrek_TOS;
                }
                else if (milestoneTypeId == (int)MilestoneTypeValues.RebelAlliance)
                {
                    quizId = (int)QuizValues.StarWarsCharacters;
                }
                else if (milestoneTypeId == (int)MilestoneTypeValues.HighWarlord)
                {
                    quizId = (int)QuizValues.WorldOfWarcraft_HighWarlord;
                }

                bool hasUserCompletedQuiz = await TriviaService.IsQuizCompletedByUserAsync(userId, quizId);

                amount = hasUserCompletedQuiz ? 1 : 0;
            }
            else if (milestoneTypeId == (int)MilestoneTypeValues.GoldMedalist)
            {
                bool hasUserCompletedQuiz1 = await TriviaService.IsQuizCompletedByUserAsync(userId, (int)QuizValues.SummerOlympics);
                bool hasUserCompletedQuiz2 = await TriviaService.IsQuizCompletedByUserAsync(userId, (int)QuizValues.WinterOlympics);

                amount = (hasUserCompletedQuiz1 && hasUserCompletedQuiz2) ? 1 : 0;
            }
            else if (milestoneTypeId == (int)MilestoneTypeValues.HighFive)
            {
                int daysAgo = 1;
                int countOfQuizzesCompletedInLastDay = await TriviaService.CountOfQuizzesCompletedAsync(userId, daysAgo);
                amount = countOfQuizzesCompletedInLastDay >= 5 ? 1 : 0;
            }
            else if (milestoneTypeId == (int)MilestoneTypeValues.MultiTalented)
            {
                // if the categories "touched" by the user (completed a quiz in that category) are equal to the number of total categories, achievement!
                int quizCategoryCount = (await TriviaService.GetQuizCategoriesAsync()).Count;
                int quizCategoriesTouchedByUserCount = await TriviaService.GetQuizCategoriesTouchedByUserCountAsync(userId);
                amount = quizCategoriesTouchedByUserCount == quizCategoryCount ? 1 : 0;
            }
            else if (milestoneTypeId == (int)MilestoneTypeValues.FriendlyExchange)
            {
                TimeSpan duration = TimeSpan.FromMinutes(30);

                // has anyone even sent us something in the last 30 minutes?
                var giftsSentToCurrentUser = await ProfileService.GetGiftsSentToUserAsync(userId, duration);

                // someone sent us something in the last 30 minutes! yay, we're popular
                if (giftsSentToCurrentUser.Count > 0)
                {
                    // these are the user ids of the people who sent us gifts
                    var userIdsForGiftsSent = giftsSentToCurrentUser.Select(x => x.FromUserId);

                    // now check if we sent any gifts to anyone who sent us a gift in the last 30 minutes
                    var giftsSentToUsers = await ProfileService.GetGiftsSentToUsersFromUserAsync(userId, userIdsForGiftsSent, duration);

                    // we sent someone a gift that sent us a gift in the last 30 minutes
                    if (giftsSentToUsers.Count > 0)
                    {
                        amount = 1;
                    }
                }
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
        private bool HasUserAchievedMilestone(string userId, int milestoneId, IReadOnlyDictionary<int, int> milestonesAchieved)
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

        private async Task<IReadOnlyDictionary<int, int>> GetUserProgressTowardsAchievements(string userId, IEnumerable<int> milestoneTypeIds)
        {
            Dictionary<int, int> achievementProgress = new Dictionary<int, int>();
            foreach (int milestoneTypeId in milestoneTypeIds)
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
                                                select (int?)user.CurrentPoints).FirstOrDefaultAsync()) ?? 0;

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
                else if (milestoneTypeId == (int)MilestoneTypeValues.Trekkie)
                {
                    count = await HasUserAchievedSoloMilestone(userId, (int)MilestoneValues.Trekkie);
                }
                else if (milestoneTypeId == (int)MilestoneTypeValues.RebelAlliance)
                {
                    count = await HasUserAchievedSoloMilestone(userId, (int)MilestoneValues.RebelAlliance);
                }
                else if (milestoneTypeId == (int)MilestoneTypeValues.HighWarlord)
                {
                    count = await HasUserAchievedSoloMilestone(userId, (int)MilestoneValues.HighWarlord);
                }
                else if (milestoneTypeId == (int)MilestoneTypeValues.GoldMedalist)
                {
                    count = await HasUserAchievedSoloMilestone(userId, (int)MilestoneValues.GoldMedalist);
                }
                else if (milestoneTypeId == (int)MilestoneTypeValues.HighFive)
                {
                    count = await HasUserAchievedSoloMilestone(userId, (int)MilestoneValues.HighFive);
                }

                achievementProgress.Add(milestoneTypeId, count);
            }

            return new ReadOnlyDictionary<int, int>(achievementProgress);
        }

        private async Task<int> HasUserAchievedSoloMilestone(string userId, int milestoneId)
        {
            bool achieved = await (from milestoneAchievement in db.MilestoneAchievements
                                   where milestoneAchievement.UserId == userId
                                   where milestoneAchievement.MilestoneId == milestoneId
                                   select milestoneAchievement).AnyAsync();

            return achieved ? 1 : 0;
        }

        public async Task<IReadOnlyCollection<AchievementOverviewViewModel>> GetAchievementsAndStatusForUserAsync(string userId)
        {
            // we are going to hack here
            // if amount required > 1, then we are assuming this is a collection milestone
            var milestones = from milestone in db.Milestones.Include(x => x.MilestoneType)
                             orderby new { milestone.MilestoneTypeId, milestone.AmountRequired }
                             select milestone;

            var results = await milestones.ToListAsync();

            // split the milestone list into collection and solo milestones
            List<Milestone> collectionMilestones = new List<Milestone>();
            List<Milestone> soloMilestones = new List<Milestone>();
            foreach (var milestone in results)
            {
                if (milestone.AmountRequired == 1)
                {
                    soloMilestones.Add(milestone);
                }
                else if (milestone.AmountRequired > 1)
                {
                    collectionMilestones.Add(milestone);
                }
            }

            // collection milestones are those with multiple milestones in a bundle like "Collect 5 images, collection 10 images, collect 20 images"
            // for example, "Points Obtained" is a collection achievement with many milestones under it
            var achievementOverviews = await GetCollectionAchievementOverviewsAsync(userId, collectionMilestones);

            // solo milestones are those with only a single requirement such as "complete this quiz" or "complete 5 quizzes in a day"
            // for example, "Rebel Alliance" is a solo achievement with only a single requirement to get the badge
            var soloMilestoneOverviews = await GetSoloAchievementOverviewsAsync(userId, soloMilestones);

            // combine both collection and solo milestones together
            achievementOverviews.AddRange(soloMilestoneOverviews);

            return achievementOverviews.AsReadOnly();
        }

        private async Task<IReadOnlyCollection<AchievementOverviewViewModel>> GetSoloAchievementOverviewsAsync(string userId, IEnumerable<Milestone> soloMilestones)
        {
            var soloMilestoneIds = soloMilestones.Select(x => x.Id).ToList();

            var completedMilestones = await (from achievements in db.MilestoneAchievements
                                             where achievements.UserId == userId
                                             where soloMilestoneIds.Contains(achievements.MilestoneId)
                                             select achievements)
                                             .ToDictionaryAsync(x => x.MilestoneId, x => x.Milestone);

            List<AchievementOverviewViewModel> achievementOverviews = new List<AchievementOverviewViewModel>();

            foreach (var soloMilestone in soloMilestones)
            {
                var achievementOverview = new AchievementOverviewViewModel()
                {
                    AchievementTypeName = soloMilestone.MilestoneType.Name,
                    AchievementDescription = soloMilestone.MilestoneType.Description,
                    AchievementStatuses = new List<AchievementStatusViewModel>()
                };

                int achievedCount = 0;
                Milestone completedMilestone = null;
                // user has completed this milestone
                if (completedMilestones.TryGetValue(soloMilestone.Id, out completedMilestone))
                {
                    achievedCount = 1;
                }

                achievementOverview.AchievementStatuses.Add(new AchievementStatusViewModel()
                {
                    AchievedCount = achievedCount,
                    RequiredCount = soloMilestone.AmountRequired,
                    AchievementIconPath = soloMilestone.IconFileName
                });

                achievementOverviews.Add(achievementOverview);
            }

            foreach (var achievementOverview in achievementOverviews)
            {
                // this will always only be a single item for solo achievements
                foreach (var achievementStatus in achievementOverview.AchievementStatuses)
                {
                    achievementStatus.AchievementIconPath = MilestoneExtensions.GetIconPath(achievementStatus.AchievementIconPath);
                }
            }

            return achievementOverviews.AsReadOnly();
        }

        private async Task<List<AchievementOverviewViewModel>> GetCollectionAchievementOverviewsAsync(string userId, IList<Milestone> collectionMilestones)
        {
            List<AchievementOverviewViewModel> achievementOverviews = new List<AchievementOverviewViewModel>();
            List<AchievementStatusViewModel> currentAchievementStatuses = new List<AchievementStatusViewModel>();
            AchievementOverviewViewModel currentAchievementOverView = new AchievementOverviewViewModel();

            // get progress towards all valid milestone types at once so we don't have to keep querying for it
            var collectionMilestoneTypeIds = collectionMilestones
                .Select(x => x.MilestoneTypeId)
                .Distinct()
                .ToList();
            var achievementProgress = await GetUserProgressTowardsAchievements(userId, collectionMilestoneTypeIds);

            int currentMilestoneTypeId = 0;
            int previousMilestoneTypeId = 0;
            Milestone previousMilestone = null;
            foreach (var milestone in collectionMilestones)
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

                int amountAchieved = achievementProgress[milestone.MilestoneTypeId];

                currentAchievementStatuses.Add(new AchievementStatusViewModel()
                {
                    RequiredCount = milestone.AmountRequired,
                    AchievedCount = amountAchieved <= milestone.AmountRequired ? amountAchieved : milestone.AmountRequired,
                    AchievementIconPath = milestone.GetIconPath()
                });

                previousMilestone = milestone;
            }

            currentAchievementOverView.AchievementStatuses = currentAchievementStatuses;
            currentAchievementOverView.AchievementTypeName = previousMilestone.MilestoneType.Name;
            currentAchievementOverView.AchievementDescription = previousMilestone.MilestoneType.Description;
            achievementOverviews.Add(currentAchievementOverView);

            return achievementOverviews;
        }

        public async Task<int> GetCompletedAchievementCount(string userId)
        {
            int completedCount = await (from achievements in db.MilestoneAchievements
                                        where achievements.UserId == userId
                                        select achievements).CountAsync();

            return completedCount;
        }

        public async Task<int> GetPossibleAchievementCount()
        {
            int possibleCount = await (from achievements in db.Milestones
                                       select achievements).CountAsync();

            return possibleCount;
        }
    }
}