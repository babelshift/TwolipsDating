namespace TwolipsDating.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Validation;
    using System.Linq;
    using System.Text;
    using TwolipsDating.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<TwolipsDating.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }
        
        /// <summary>
        /// Wrapper for SaveChanges adding the Validation Messages to the generated exception
        /// </summary>
        /// <param name="context">The context.</param>
        private void SaveChanges(DbContext context)
        {
            try
            {
                context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                StringBuilder sb = new StringBuilder();

                foreach (var failure in ex.EntityValidationErrors)
                {
                    sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                    foreach (var error in failure.ValidationErrors)
                    {
                        sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                        sb.AppendLine();
                    }
                }

                throw new DbEntityValidationException(
                    "Entity Validation Failed - errors follow:\n" +
                    sb.ToString(), ex
                ); // Add the original exception as the innerException
            }
        }

        protected override void Seed(TwolipsDating.Models.ApplicationDbContext context)
        {
            context.Genders.AddOrUpdate(g => g.Id,
                new Gender() { Id = 1, Name = "Unknown" },
                new Gender() { Id = 2, Name = "Man" },
                new Gender() { Id = 3, Name = "Woman" },
                new Gender() { Id = 4, Name = "Genderless" },
                new Gender() { Id = 5, Name = "Hobbit" },
                new Gender() { Id = 6, Name = "Robot" },
                new Gender() { Id = 7, Name = "Fembot" },
                new Gender() { Id = 8, Name = "Troll" },
                new Gender() { Id = 9, Name = "Ogre" },
                new Gender() { Id = 10, Name = "Orc" },
                new Gender() { Id = 11, Name = "Elf" },
                new Gender() { Id = 13, Name = "Fluid" },
                new Gender() { Id = 14, Name = "Undead" },
                new Gender() { Id = 15, Name = "Mystic" },
                new Gender() { Id = 16, Name = "Augmented" },
                new Gender() { Id = 17, Name = "Dog" },
                new Gender() { Id = 18, Name = "Cat" }
            );

            context.MessageStatuses.AddOrUpdate(m => m.Id,
                new MessageStatus() { Id = 1, Name = "Unread" },
                new MessageStatus() { Id = 2, Name = "Read" },
                new MessageStatus() { Id = 3, Name = "Deleted" }
            );

            context.ReviewRatings.AddOrUpdate(m => m.Value,
                new ReviewRating() { Value = 1 },
                new ReviewRating() { Value = 2 },
                new ReviewRating() { Value = 3 },
                new ReviewRating() { Value = 4 },
                new ReviewRating() { Value = 5 }
            );

            context.Tags.AddOrUpdate(m => m.TagId,
                new Tag() { TagId = (int)TagValues.intellectual, Name = "intellectual", Description = "Engages in critical study, thought, and reflection about the nature of being." },
                new Tag() { TagId = (int)TagValues.neckbeard, Name = "neckbeard", Description = "A growth of hair on a man's neck, especially when regarded as indicative of poor grooming." },
                new Tag() { TagId = (int)TagValues.jock, Name = "jock", Description = "A stereotype of an athlete generally attributed mostly to high school and college athletics participants who form a distinct youth subculture." },
                new Tag() { TagId = (int)TagValues.player, Name = "player", Description = "Someone who is skilled at manipulating others, and especially at seducing others by pretending to care about them." },
                new Tag() { TagId = (int)TagValues.wizard, Name = "wizard", Description = "Someone who has magical powers, especially in legends and fairy tales." },
                new Tag() { TagId = (int)TagValues.simple, Name = "simpleton", Description = "A foolish or gullible person." },
                new Tag() { TagId = (int)TagValues.hot, Name = "hot", Description = "A face and body like fire, but not literally." },
                new Tag() { TagId = (int)TagValues.arrogant, Name = "arrogant", Description = "Having or revealing an exaggerated sense of one's own importance or abilities." },
                new Tag() { TagId = (int)TagValues.intense, Name = "intense", Description = "Usually operates in a mode of extreme focus at the expense of everything else." },
                new Tag() { TagId = (int)TagValues.hobbit, Name = "hobbit", Description = "Short and stumpy, but in a good way. Usually eats 3 breakfasts in a single day." },
                new Tag() { TagId = (int)TagValues.insecure, Name = "insecure", Description = "Not confident and deflects weaknesses through negative projection." },
                new Tag() { TagId = (int)TagValues.bookworm, Name = "bookworm", Description = "Devoted to reading and prefers the library over a bar." },
                new Tag() { TagId = (int)TagValues.gamer, Name = "gamer", Description = "Maybe a gambler, but most likely just likes video games." },
                new Tag() { TagId = (int)TagValues.foody, Name = "foody", Description = "Likes food, makes food, eats food, a lot" },
                new Tag() { TagId = (int)TagValues.creative, Name = "creative", Description = "Someone who can draw the door and show you through it." },
                new Tag() { TagId = (int)TagValues.film_critic, Name = "film-critic", Description = "Able to find flaws in absolutely every movie and beyond." },
                new Tag() { TagId = (int)TagValues.technocratic, Name = "technocratic", Description = "Probably part of the technological elitist revolution. Beware of augmentations." },
                new Tag() { TagId = (int)TagValues.drone, Name = "drone", Description = "A person who does monotonous work day in and day out. Usually works in a cubicle." },
                new Tag() { TagId = (int)TagValues.fashionista, Name = "fashionista", Description = "Truly devoted to all fashion-related causes, especially unique or high fashion." },
                new Tag() { TagId = (int)TagValues.gossip, Name = "gossip", Description = "Absolutely unable to keep any secrets. Anything you tell them will most likely end up on Facebook within minutes." },
                new Tag() { TagId = (int)TagValues.linguist, Name = "linguist", Description = "Interested in the study of spelling, grammar, and languages. Look to this person in case you forgot your dictionary or thesaurus." },
                new Tag() { TagId = (int)TagValues.philosopher, Name = "philosopher", Description = "Often ponders age old questions such as the nature of self, the origin of the universe, and why cats are so cute." },
                new Tag() { TagId = (int)TagValues.quiet, Name = "quiet", Description = "When tasked with needing to stay silent, they will excel to a professional level." },
                new Tag() { TagId = (int)TagValues.scientist, Name = "scientist", Description = "Engaged in the systemitc activity to acquire knowledge through observation and experimentation." },
                new Tag() { TagId = (int)TagValues.top_chef, Name = "top-chef", Description = "Could handily defeat Alton Brown and Anthony Bourdain in a cook off." },
                new Tag() { TagId = (int)TagValues.chief_executive, Name = "chief-executive", Description = "Likes to run the show, lead the people, and then take the golden parachute when the company explodes." }
            );

            context.ViolationTypes.AddOrUpdate(v => v.Id,
                new ViolationType() { Id = 1, Name = "Illegal content" },
                new ViolationType() { Id = 2, Name = "Spam" },
                new ViolationType() { Id = 3, Name = "Advertising" },
                new ViolationType() { Id = 4, Name = "Harassment" });

            context.QuestionViolationTypes.AddOrUpdate(v => v.Id,
                new QuestionViolationType() { Id = 1, Name = "Wrong answer" },
                new QuestionViolationType() { Id = 2, Name = "Poor question" },
                new QuestionViolationType() { Id = 3, Name = "Typo in question or answer" });

            context.StoreItemTypes.AddOrUpdate(g => g.Id,
                new StoreItemType() { Id = (int)StoreItemTypeValues.Gift, Name = StoreItemTypeValues.Gift.ToString(), Description = "A gift for a friend." },
                new StoreItemType() { Id = (int)StoreItemTypeValues.Title, Name = StoreItemTypeValues.Title.ToString(), Description = "A title to show off." });

            context.StoreItems.AddOrUpdate(g => g.Id,
                new StoreItem() { Id = 1, Name = "Rose (red)", IconFileName = "RedRose.png", Description = "A red rose with curiously missing thorns", PointPrice = 10, ItemTypeId = (int)StoreItemTypeValues.Gift },
                new StoreItem() { Id = 2, Name = "Rose (white)", IconFileName = "WhiteRose.png", Description = "A white rose with some seriously sharp thorns", PointPrice = 15, ItemTypeId = (int)StoreItemTypeValues.Gift },
                new StoreItem() { Id = 3, Name = "Dog bone", IconFileName = "DogBone.png", Description = "A tasty dog bone perfect for chewing", PointPrice = 15, ItemTypeId = (int)StoreItemTypeValues.Gift },
                new StoreItem() { Id = 4, Name = "Candy", IconFileName = "Candy.png", Description = "A delicious piece of candy straight from grandma's candy bowl", PointPrice = 20, ItemTypeId = (int)StoreItemTypeValues.Gift },
                new StoreItem() { Id = 5, Name = "Lizard Lord", IconFileName = "LizardLord1.png", Description = "King of the lizard creatures.", PointPrice = 100, ItemTypeId = (int)StoreItemTypeValues.Title },
                new StoreItem() { Id = 6, Name = "Robot Cop", IconFileName = "RobotCop2.png", Description = "A robotic police officer different from the popular copyrighted version.", PointPrice = 100, ItemTypeId = (int)StoreItemTypeValues.Title },
                new StoreItem() { Id = 7, Name = "High Warlord", IconFileName = "HighWarlord2.png", Description = "Played way too much World of Warcraft.", PointPrice = 200, ItemTypeId = (int)StoreItemTypeValues.Title },
                new StoreItem() { Id = 8, Name = "Grand Marshal", IconFileName = "GrandMarshal1.png", Description = "Played way too much World of Warcraft.", PointPrice = 200, ItemTypeId = (int)StoreItemTypeValues.Title },
                new StoreItem() { Id = 9, Name = "General", IconFileName = "General1.png", Description = "Commands a presence on the battlefield.", PointPrice = 200, ItemTypeId = (int)StoreItemTypeValues.Title },
                new StoreItem() { Id = 10, Name = "Psycho Dentist", IconFileName = "PsychoDentist.png", Description = "Straps people to a chair and drills for cavities.", PointPrice = 200, ItemTypeId = (int)StoreItemTypeValues.Title },
                new StoreItem() { Id = 11, Name = "Crazy Cat Lover", IconFileName = "CrazyCatLover.png", Description = "Owns 20 cats and probably doesn't clean up after them.", PointPrice = 200, ItemTypeId = (int)StoreItemTypeValues.Title },
                new StoreItem() { Id = 12, Name = "The President", IconFileName = "President.png", Description = "The most powerful person in the world.", PointPrice = 200, ItemTypeId = (int)StoreItemTypeValues.Title },
                new StoreItem() { Id = 13, Name = "El Guapo", IconFileName = "ElGuapo.png", Description = "Devilishly handsome.", PointPrice = 200, ItemTypeId = (int)StoreItemTypeValues.Title },
                new StoreItem() { Id = 14, Name = "Banana Bunch", IconFileName = "Bananas.png", Description = "A perfectly sized bunch of bananas full of potassium.", PointPrice = 25, ItemTypeId = (int)StoreItemTypeValues.Gift });

            context.QuestionTypes.AddOrUpdate(q => q.Id,
                new QuestionType() { Id = (int)QuestionTypeValues.Random, Name = "Random" },
                new QuestionType() { Id = (int)QuestionTypeValues.Timed, Name = "Timed" },
                new QuestionType() { Id = (int)QuestionTypeValues.Quiz, Name = "Quiz" });

            context.MilestoneTypes.AddOrUpdate(m => m.Id,
                new MilestoneType() { Id = (int)MilestoneTypeValues.QuestionsAnsweredCorrectly, Name = "Questions answered correctly" },
                new MilestoneType() { Id = (int)MilestoneTypeValues.QuizzesCompletedSuccessfully, Name = "Quizzes completed successfully" },
                new MilestoneType() { Id = (int)MilestoneTypeValues.GiftSent, Name = "Gifts sent to a user" },
                new MilestoneType() { Id = (int)MilestoneTypeValues.GiftsPurchased, Name = "Gifts purchased" },
                new MilestoneType() { Id = (int)MilestoneTypeValues.PointsObtained, Name = "Points obtained" },
                new MilestoneType() { Id = (int)MilestoneTypeValues.ProfileImagesUploaded, Name = "Profile images uploaded" },
                new MilestoneType() { Id = (int)MilestoneTypeValues.ProfileReviewsWritten, Name = "Profile reviews written" },
                new MilestoneType() { Id = (int)MilestoneTypeValues.TagsAwarded, Name = "Tags awarded" },
                new MilestoneType() { Id = (int)MilestoneTypeValues.TitlesPurchased, Name = "Titles purchased" });

            context.Milestones.AddOrUpdate(m => m.Id,
                new Milestone() { Id = (int)MilestoneValues.QuestionsAnsweredCorrectly1, MilestoneTypeId = (int)MilestoneTypeValues.QuestionsAnsweredCorrectly, AmountRequired = 25 },
                new Milestone() { Id = (int)MilestoneValues.QuestionsAnsweredCorrectly2, MilestoneTypeId = (int)MilestoneTypeValues.QuestionsAnsweredCorrectly, AmountRequired = 50 },
                new Milestone() { Id = (int)MilestoneValues.QuestionsAnsweredCorrectly3, MilestoneTypeId = (int)MilestoneTypeValues.QuestionsAnsweredCorrectly, AmountRequired = 100 },
                new Milestone() { Id = (int)MilestoneValues.QuestionsAnsweredCorrectly4, MilestoneTypeId = (int)MilestoneTypeValues.QuestionsAnsweredCorrectly, AmountRequired = 200 },
                new Milestone() { Id = (int)MilestoneValues.QuizzesCompletedSuccessfully1, MilestoneTypeId = (int)MilestoneTypeValues.QuizzesCompletedSuccessfully, AmountRequired = 5 },
                new Milestone() { Id = (int)MilestoneValues.QuizzesCompletedSuccessfully2, MilestoneTypeId = (int)MilestoneTypeValues.QuizzesCompletedSuccessfully, AmountRequired = 15 },
                new Milestone() { Id = (int)MilestoneValues.QuizzesCompletedSuccessfully3, MilestoneTypeId = (int)MilestoneTypeValues.QuizzesCompletedSuccessfully, AmountRequired = 30 },
                new Milestone() { Id = (int)MilestoneValues.QuizzesCompletedSuccessfully4, MilestoneTypeId = (int)MilestoneTypeValues.QuizzesCompletedSuccessfully, AmountRequired = 60 },
                new Milestone() { Id = (int)MilestoneValues.ProfileReviewsWritten1, MilestoneTypeId = (int)MilestoneTypeValues.ProfileReviewsWritten, AmountRequired = 5 },
                new Milestone() { Id = (int)MilestoneValues.ProfileReviewsWritten2, MilestoneTypeId = (int)MilestoneTypeValues.ProfileReviewsWritten, AmountRequired = 15 },
                new Milestone() { Id = (int)MilestoneValues.ProfileReviewsWritten3, MilestoneTypeId = (int)MilestoneTypeValues.ProfileReviewsWritten, AmountRequired = 30 },
                new Milestone() { Id = (int)MilestoneValues.ProfileReviewsWritten4, MilestoneTypeId = (int)MilestoneTypeValues.ProfileReviewsWritten, AmountRequired = 60 },
                new Milestone() { Id = (int)MilestoneValues.ProfileImagesUploaded1, MilestoneTypeId = (int)MilestoneTypeValues.ProfileImagesUploaded, AmountRequired = 5 },
                new Milestone() { Id = (int)MilestoneValues.ProfileImagesUploaded2, MilestoneTypeId = (int)MilestoneTypeValues.ProfileImagesUploaded, AmountRequired = 10 },
                new Milestone() { Id = (int)MilestoneValues.ProfileImagesUploaded3, MilestoneTypeId = (int)MilestoneTypeValues.ProfileImagesUploaded, AmountRequired = 15 },
                new Milestone() { Id = (int)MilestoneValues.ProfileImagesUploaded4, MilestoneTypeId = (int)MilestoneTypeValues.ProfileImagesUploaded, AmountRequired = 20 },
                new Milestone() { Id = (int)MilestoneValues.GiftsSent1, MilestoneTypeId = (int)MilestoneTypeValues.GiftSent, AmountRequired = 5 },
                new Milestone() { Id = (int)MilestoneValues.GiftsSent2, MilestoneTypeId = (int)MilestoneTypeValues.GiftSent, AmountRequired = 10 },
                new Milestone() { Id = (int)MilestoneValues.GiftsSent3, MilestoneTypeId = (int)MilestoneTypeValues.GiftSent, AmountRequired = 15 },
                new Milestone() { Id = (int)MilestoneValues.GiftsSent4, MilestoneTypeId = (int)MilestoneTypeValues.GiftSent, AmountRequired = 20 },
                new Milestone() { Id = (int)MilestoneValues.GiftsPurchased1, MilestoneTypeId = (int)MilestoneTypeValues.GiftsPurchased, AmountRequired = 5 },
                new Milestone() { Id = (int)MilestoneValues.GiftsPurchased2, MilestoneTypeId = (int)MilestoneTypeValues.GiftsPurchased, AmountRequired = 10 },
                new Milestone() { Id = (int)MilestoneValues.GiftsPurchased3, MilestoneTypeId = (int)MilestoneTypeValues.GiftsPurchased, AmountRequired = 15 },
                new Milestone() { Id = (int)MilestoneValues.GiftsPurchased4, MilestoneTypeId = (int)MilestoneTypeValues.GiftsPurchased, AmountRequired = 20 },
                new Milestone() { Id = (int)MilestoneValues.TitlesPurchased1, MilestoneTypeId = (int)MilestoneTypeValues.TitlesPurchased, AmountRequired = 5 },
                new Milestone() { Id = (int)MilestoneValues.TitlesPurchased2, MilestoneTypeId = (int)MilestoneTypeValues.TitlesPurchased, AmountRequired = 10 },
                new Milestone() { Id = (int)MilestoneValues.TitlesPurchased3, MilestoneTypeId = (int)MilestoneTypeValues.TitlesPurchased, AmountRequired = 15 },
                new Milestone() { Id = (int)MilestoneValues.TitlesPurchased4, MilestoneTypeId = (int)MilestoneTypeValues.TitlesPurchased, AmountRequired = 20 },
                new Milestone() { Id = (int)MilestoneValues.TagsAwarded1, MilestoneTypeId = (int)MilestoneTypeValues.TagsAwarded, AmountRequired = 5 },
                new Milestone() { Id = (int)MilestoneValues.TagsAwarded2, MilestoneTypeId = (int)MilestoneTypeValues.TagsAwarded, AmountRequired = 10 },
                new Milestone() { Id = (int)MilestoneValues.TagsAwarded3, MilestoneTypeId = (int)MilestoneTypeValues.TagsAwarded, AmountRequired = 15 },
                new Milestone() { Id = (int)MilestoneValues.TagsAwarded4, MilestoneTypeId = (int)MilestoneTypeValues.TagsAwarded, AmountRequired = 20 },
                new Milestone() { Id = (int)MilestoneValues.PointsObtained1, MilestoneTypeId = (int)MilestoneTypeValues.PointsObtained, AmountRequired = 100 },
                new Milestone() { Id = (int)MilestoneValues.PointsObtained2, MilestoneTypeId = (int)MilestoneTypeValues.PointsObtained, AmountRequired = 250 },
                new Milestone() { Id = (int)MilestoneValues.PointsObtained3, MilestoneTypeId = (int)MilestoneTypeValues.PointsObtained, AmountRequired = 500 },
                new Milestone() { Id = (int)MilestoneValues.PointsObtained4, MilestoneTypeId = (int)MilestoneTypeValues.PointsObtained, AmountRequired = 1000 });

            context.Quizzes.AddOrUpdate(m => m.Id,
                new Quiz() { Id = 1, Name = "Technical Guru", Description = "If you can finish this, you're probably good with gadgets.", DateCreated = new DateTime(2015, 8, 1), Points = 20, IsActive = true },
                new Quiz() { Id = 2, Name = "Gamer God", Description = "Pull out your Playstation and gear up to claim victory.", DateCreated = new DateTime(2015, 8, 1), Points = 20, IsActive = true },
                new Quiz() { Id = 3, Name = "Creative Genius", Description = "Try to study up on your color theory and sewing terminology.", DateCreated = new DateTime(2015, 8, 1), Points = 20, IsActive = true },
                new Quiz() { Id = 4, Name = "Business Sense", Description = "The former CEO of Enron would probably fail this one.", DateCreated = new DateTime(2015, 8, 1), Points = 15, IsActive = true },
                new Quiz() { Id = 5, Name = "Language Linguist", Description = "If you're interested in the science of language, then this is the quiz for you.", DateCreated = new DateTime(2015, 8, 1), Points = 20, IsActive = true },
                new Quiz() { Id = 6, Name = "World of Warcraft (Scout)", Description = "This entry level quiz will require you to know the basics of World of Warcraft.", DateCreated = new DateTime(2015, 8, 12), Points = 10, IsActive = true },
                new Quiz() { Id = 7, Name = "World of Warcraft (Champion)", Description = "This intermediate level quiz should only be completed by regulars of Azeroth.", DateCreated = new DateTime(2015, 8, 12), Points = 15, IsActive = true },
                new Quiz() { Id = 8, Name = "World of Warcraft (High Warlord)", Description = "Can you master the most difficult trivia only for seasoned veterans?", DateCreated = new DateTime(2015, 8, 12), Points = 25, IsActive = true },
                new Quiz() { Id = 9, Name = "Movies and Monsters", Description = "Giant spiders, spooky skeletons, swarms of ants, and a whole lot of stop motion animation.", DateCreated = new DateTime(2015, 8, 14), Points = 20, IsActive = true });

            SaveChanges(context);
        }
    }
}
