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
                new Gender() { Id = 2, Name = "Male" },
                new Gender() { Id = 3, Name = "Female" },
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
                new Gender() { Id = 18, Name = "Cat" },
                new Gender() { Id = 19, Name = "Klingon" },
                new Gender() { Id = 20, Name = "Romulan" }
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
                new Tag() { TagId = (int)TagValues.chief_executive, Name = "chief-executive", Description = "Likes to run the show, lead the people, and then take the golden parachute when the company explodes." },
                new Tag() { TagId = (int)TagValues.health_nut, Name = "health-nut", Description = "Carefully measures each and every molecule that enters their body just in case one of them might be unhealthy." },
                new Tag() { TagId = (int)TagValues.doctor, Name = "doctor", Description = "Managed to endure many years of unresponsive advisors and whining students without going insane." },
                new Tag() { TagId = (int)TagValues.athlete, Name = "athlete", Description = "May or may not have competed in the Olympics. Regardless, he or she is probably fit." },
                new Tag() { TagId = (int)TagValues.rockstar, Name = "rockstar", Description = "Anyone unlucky enough to get this tag will forever be stuck with an awful business buzzword." },
                new Tag() { TagId = (int)TagValues.princess, Name = "princess", Description = "Enjoys colored dresses, slippers, and hairstyles. Also constantly bothered by evil family members." },
                new Tag() { TagId = (int)TagValues.prince, Name = "prince", Description = "Will become the ruler of his family's kingdom regardless of his credentials." }
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
                new MilestoneType() { Id = (int)MilestoneTypeValues.QuestionsAnsweredCorrectly, Name = "Questions answered correctly", Description = "Answering questions correctly will award you tags, points, and build up your profile with your knowledge." },
                new MilestoneType() { Id = (int)MilestoneTypeValues.QuizzesCompletedSuccessfully, Name = "Quizzes completed successfully", Description = "Completing quizzes shows others that you know a bundle of knowledge about a specific topic." },
                new MilestoneType() { Id = (int)MilestoneTypeValues.GiftSent, Name = "Gifts sent to a user", Description = "Sending gifts to others can begin a conversation about the gift or any messages sent with that gift." },
                new MilestoneType() { Id = (int)MilestoneTypeValues.GiftsPurchased, Name = "Gifts purchased", Description = "Purchasing gifts and items from the store can be used to show off your profile treasures." },
                new MilestoneType() { Id = (int)MilestoneTypeValues.PointsObtained, Name = "Points obtained", Description = "Points are obtained through trivia, quizzes, achievements, and more in order to obtain items, gifts, and titles." },
                new MilestoneType() { Id = (int)MilestoneTypeValues.ProfileImagesUploaded, Name = "Profile images uploaded", Description = "Upload pictures of yourself and things that you enjoy to customize your profile appearance." },
                new MilestoneType() { Id = (int)MilestoneTypeValues.ProfileReviewsWritten, Name = "Profile reviews written", Description = "Reviewing your friends and people that you meet can help others know what they're getting into." },
                new MilestoneType() { Id = (int)MilestoneTypeValues.TagsAwarded, Name = "Tags awarded", Description = "Tags are awarded from questions, quizzes, and achievements to tailor your profile around your knowledge and actions." },
                new MilestoneType() { Id = (int)MilestoneTypeValues.TitlesPurchased, Name = "Titles purchased", Description = "Purchasing unique titles from the store allows you to select and display a title before your user name around the community." });

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

            context.QuizCategories.AddOrUpdate(m => m.Id,
                new QuizCategory() { Id = (int)QuizCategoryValues.Uncategorized, Name = "Uncategorized", FontAwesomeIconName = "fa-question" },
                new QuizCategory() { Id = (int)QuizCategoryValues.Business, Name = "Business", FontAwesomeIconName = "fa-building" },
                new QuizCategory() { Id = (int)QuizCategoryValues.Creative, Name = "Creative", FontAwesomeIconName = "fa-magic" },
                new QuizCategory() { Id = (int)QuizCategoryValues.Food, Name = "Food", FontAwesomeIconName = "fa-cutlery" },
                new QuizCategory() { Id = (int)QuizCategoryValues.History, Name = "History", FontAwesomeIconName = "fa-book" },
                new QuizCategory() { Id = (int)QuizCategoryValues.Literature, Name = "Literature", FontAwesomeIconName = "fa-language" },
                new QuizCategory() { Id = (int)QuizCategoryValues.Movies, Name = "Movies", FontAwesomeIconName = "fa-film" },
                new QuizCategory() { Id = (int)QuizCategoryValues.Science, Name = "Science", FontAwesomeIconName = "fa-flask" },
                new QuizCategory() { Id = (int)QuizCategoryValues.Technical, Name = "Technical", FontAwesomeIconName = "fa-laptop" },
                new QuizCategory() { Id = (int)QuizCategoryValues.Television, Name = "Television", FontAwesomeIconName = "fa-tv" },
                new QuizCategory() { Id = (int)QuizCategoryValues.VideoGames, Name = "Video Games", FontAwesomeIconName = "fa-gamepad" },
                new QuizCategory() { Id = (int)QuizCategoryValues.Sports, Name = "Sports", FontAwesomeIconName = "fa-futbol-o" }
            );

            context.Quizzes.AddOrUpdate(m => m.Id,
                new Quiz() { Id = 1, QuizCategoryId = (int)QuizCategoryValues.Technical, Name = "Technical Guru", Description = "If you can finish this, you're probably good with gadgets.", DateCreated = new DateTime(2015, 8, 1), Points = 20, IsActive = true, ImageFileName = "TechnicalGuru.jpg" },
                new Quiz() { Id = 2, QuizCategoryId = (int)QuizCategoryValues.VideoGames, Name = "Gamer God", Description = "Pull out your Playstation and gear up to claim victory.", DateCreated = new DateTime(2015, 8, 1), Points = 20, IsActive = true, ImageFileName = "GamerGod.jpg" },
                new Quiz() { Id = 3, QuizCategoryId = (int)QuizCategoryValues.Creative, Name = "Creative Genius", Description = "Try to study up on your color theory and sewing terminology.", DateCreated = new DateTime(2015, 8, 1), Points = 20, IsActive = true, ImageFileName = "CreativeGenius.jpg" },
                new Quiz() { Id = 4, QuizCategoryId = (int)QuizCategoryValues.Business, Name = "Business Sense", Description = "The former CEO of Enron would probably fail this one.", DateCreated = new DateTime(2015, 8, 1), Points = 15, IsActive = true, ImageFileName = "BusinessSense.jpg" },
                new Quiz() { Id = 5, QuizCategoryId = (int)QuizCategoryValues.Science, Name = "Language Linguist", Description = "If you're interested in the science of language, then this is the quiz for you.", DateCreated = new DateTime(2015, 8, 1), Points = 20, IsActive = true, ImageFileName = "LanguageLinguist.jpg" },
                new Quiz() { Id = 6, QuizCategoryId = (int)QuizCategoryValues.VideoGames, Name = "World of Warcraft (Scout)", Description = "This entry level quiz will require you to know the basics of World of Warcraft.", DateCreated = new DateTime(2015, 8, 12), Points = 10, IsActive = true, ImageFileName = "WoW.jpg" },
                new Quiz() { Id = 7, QuizCategoryId = (int)QuizCategoryValues.VideoGames, Name = "World of Warcraft (Champion)", Description = "This intermediate level quiz should only be completed by regulars of Azeroth.", DateCreated = new DateTime(2015, 8, 12), Points = 15, IsActive = true, ImageFileName = "WoW.jpg" },
                new Quiz() { Id = 8, QuizCategoryId = (int)QuizCategoryValues.VideoGames, Name = "World of Warcraft (High Warlord)", Description = "Can you master the most difficult trivia only for seasoned veterans?", DateCreated = new DateTime(2015, 8, 12), Points = 25, IsActive = true, ImageFileName = "WoW.jpg" },
                new Quiz() { Id = 9, QuizCategoryId = (int)QuizCategoryValues.Movies, Name = "Movies and Monsters", Description = "Giant spiders, spooky skeletons, swarms of ants, and a whole lot of stop motion animation.", DateCreated = new DateTime(2015, 8, 14), Points = 20, IsActive = true, ImageFileName = "MonstersAndMovies.jpg" },
                new Quiz() { Id = 10, QuizCategoryId = (int)QuizCategoryValues.VideoGames, Name = "Legend of Zelda", Description = "The triforce can be yours with the completion of this one. OK, so the triforce isn't real, but this quiz is.", DateCreated = new DateTime(2015, 8, 17), Points = 20, IsActive = true, ImageFileName = "Zelda.jpg" },
                new Quiz() { Id = 11, QuizCategoryId = (int)QuizCategoryValues.Movies, Name = "Movie Quotes", Description = "\"The force will be with you Harry.\" -Gandalf", DateCreated = new DateTime(2015, 8, 18), Points = 20, IsActive = true, ImageFileName = "MovieQuotes.jpg" },
                new Quiz() { Id = 12, QuizCategoryId = (int)QuizCategoryValues.Literature, Name = "Harry Potter", Description = "Get out your wands and head to Hogwarts. There's a foul smell in the air. Only you can solve this problem.", DateCreated = new DateTime(2015, 8, 18), Points = 20, IsActive = true, ImageFileName = "HarryPotter.jpg" },
                new Quiz() { Id = 13, QuizCategoryId = (int)QuizCategoryValues.Movies, Name = "Spaceballs: The Quiz", Description = "Brought to you by the makers of Spaceballs: The Blanket and Spaceballs: The Toilet Paper.", DateCreated = new DateTime(2015, 8, 19), Points = 25, IsActive = true, ImageFileName = "Spaceballs.jpg" },
                new Quiz() { Id = 14, QuizCategoryId = (int)QuizCategoryValues.Food, Name = "The Great Food Truck Quiz", Description = "All you could ever want to answer about food trucks.", DateCreated = new DateTime(2015, 8, 30), Points = 20, IsActive = true, ImageFileName = "Foodtruck.jpg" },
                new Quiz() { Id = 15, QuizCategoryId = (int)QuizCategoryValues.Television, Name = "The Walking Dead", Description = "If you can pass this quiz, you would probably survive a zombie apocalypse.", DateCreated = new DateTime(2015, 8, 30), Points = 20, IsActive = true, ImageFileName = "WalkingDead.jpg" },
                new Quiz() { Id = 16, QuizCategoryId = (int)QuizCategoryValues.Movies, Name = "Star Wars Characters", Description = "There's a lot of them, but we managed to fit the important ones into this quiz.", DateCreated = new DateTime(2015, 9, 12), Points = 20, IsActive = true, ImageFileName = "StarWarsCharacters.jpg" },
                new Quiz() { Id = 17, QuizCategoryId = (int)QuizCategoryValues.Movies, Name = "Movies of the 90s", Description = "The 90s had a lot of grunge rock, cable television, and the Internet. Oh, and some really good movies.", DateCreated = new DateTime(2015, 9, 12, 11, 0, 0), Points = 15, IsActive = true, ImageFileName = "Movies90s.jpg" },
                new Quiz() { Id = 18, QuizCategoryId = (int)QuizCategoryValues.Science, Name = "Ocean Depths", Description = "It is said that more is known about the Moon than Earth's oceans. What do you know about them?", DateCreated = new DateTime(2015, 9, 13, 15, 0, 0), Points = 25, IsActive = true, ImageFileName = "OceanDepths.jpg" },
                new Quiz() { Id = 19, QuizCategoryId = (int)QuizCategoryValues.Sports, Name = "Summer Olympics", Description = "Which obscure olympics sport do you prefer? Handball? Or maybe equestrian-related? Let's find out.", DateCreated = new DateTime(2015, 9, 13, 19, 0, 0), Points = 20, IsActive = true, ImageFileName = "SummerOlympics.jpg" },
                new Quiz() { Id = 20, QuizCategoryId = (int)QuizCategoryValues.Science, Name = "Exotic Animals", Description = "Watch out for alligators and eagles. You might just be their next target.", DateCreated = new DateTime(2015, 9, 13, 22, 0, 0), Points = 20, IsActive = true, ImageFileName = "ExoticAnimals.jpg" },
                new Quiz() { Id = 21, QuizCategoryId = (int)QuizCategoryValues.Television, Name = "Star Trek: The Original Series", Description = "Where no quiz has gone before. You don't have to be a trekkie to know these answers.", DateCreated = new DateTime(2015, 9, 19, 23, 0, 0), Points = 15, IsActive = true, ImageFileName = "StarTrekTOS.jpg" });

            context.LookingForLocations.AddOrUpdate(m => m.Id,
                new LookingForLocation() { Id = 1, Range = "Anywhere" },
                new LookingForLocation() { Id = 2, Range = "In my city" },
                new LookingForLocation() { Id = 3, Range = "In my state" },
                new LookingForLocation() { Id = 4, Range = "In my country" });

            context.LookingForTypes.AddOrUpdate(m => m.Id,
                new LookingForType() { Id = 1, Name = "Nothing particular" },
                new LookingForType() { Id = 2, Name = "New friends (in person)" },
                new LookingForType() { Id = 3, Name = "New friends (online)" },
                new LookingForType() { Id = 4, Name = "Short-term dating" },
                new LookingForType() { Id = 5, Name = "Long-term dating" },
                new LookingForType() { Id = 6, Name = "Long distance relationship" });

            context.RelationshipStatuses.AddOrUpdate(m => m.Id,
                new RelationshipStatus() { Id = 1, Name = "Single", Description = "Actively looking." },
                new RelationshipStatus() { Id = 2, Name = "Dating", Description = "Happily dating." },
                new RelationshipStatus() { Id = 3, Name = "Married", Description = "Happily married." },
                new RelationshipStatus() { Id = 4, Name = "It's complicated", Description = "Unknown." });

            SaveChanges(context);
        }
    }
}
