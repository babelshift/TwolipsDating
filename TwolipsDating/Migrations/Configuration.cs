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
                new Gender() { Id = 11, Name = "Elf" }
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
                new Tag() { TagId = (int)TagValues.intellectual, Name = "intellectual", Description = "a smarty pants" },
                new Tag() { TagId = (int)TagValues.neckbeard, Name = "neckbeard", Description = "an unshaven mongoloid" },
                new Tag() { TagId = (int)TagValues.jock, Name = "jock", Description = "an enthusiast" },
                new Tag() { TagId = (int)TagValues.player, Name = "player", Description = "will take advantage of you" },
                new Tag() { TagId = (int)TagValues.wizard, Name = "wizard", Description = "has magical powers" },
                new Tag() { TagId = (int)TagValues.simple, Name = "simple", Description = "nothing too surprising about this one" },
                new Tag() { TagId = (int)TagValues.hot, Name = "hot", Description = "a face and body like fire, but not literally" },
                new Tag() { TagId = (int)TagValues.arrogant, Name = "arrogant", Description = "probably will not stop talking about self" },
                new Tag() { TagId = (int)TagValues.intense, Name = "intense", Description = "usually operates in a mode of extreme focus at the expense of everything else" },
                new Tag() { TagId = (int)TagValues.hobbit, Name = "hobbit", Description = "short and stumpy, but in a good way" },
                new Tag() { TagId = (int)TagValues.insecure, Name = "insecure", Description = "deflects weaknesses through negative projection" },
                new Tag() { TagId = (int)TagValues.bookworm, Name = "bookworm", Description = "prefers the library over a bar" },
                new Tag() { TagId = (int)TagValues.gamer, Name = "gamer", Description = "maybe a gambler, but most likely just likes video games" },
                new Tag() { TagId = (int)TagValues.foody, Name = "foody", Description = "likes food, makes food, eats food, a lot" },
                new Tag() { TagId = (int)TagValues.creative, Name = "creative", Description = "someone who can draw the door and show you through it" },
                new Tag() { TagId = (int)TagValues.film_critic, Name = "film-critic", Description = "able to find flaws in absolutely everything" }
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
                new StoreItem() { Id = 1, Name = "Rose (red)", IconFileName = "RedRose.png", Description = "A red rose", PointPrice = 10, ItemTypeId = (int)StoreItemTypeValues.Gift },
                new StoreItem() { Id = 2, Name = "Rose (white)", IconFileName = "WhiteRose.png", Description = "A white rose", PointPrice = 15, ItemTypeId = (int)StoreItemTypeValues.Gift },
                new StoreItem() { Id = 3, Name = "Dog bone", IconFileName = "DogBone.png", Description = "A tasty dog bone", PointPrice = 15, ItemTypeId = (int)StoreItemTypeValues.Gift },
                new StoreItem() { Id = 4, Name = "Candy", IconFileName = "Candy.png", Description = "A delicious piece of candy", PointPrice = 20, ItemTypeId = (int)StoreItemTypeValues.Gift },
                new StoreItem() { Id = 5, Name = "Lizard Lord", IconFileName = "LizardLord1.png", Description = "King of the lizard creatures.", PointPrice = 100, ItemTypeId = (int)StoreItemTypeValues.Title },
                new StoreItem() { Id = 6, Name = "Robot Cop", IconFileName = "RobotCop2.png", Description = "A robotic police officer different from the popular copyrighted version.", PointPrice = 100, ItemTypeId = (int)StoreItemTypeValues.Title },
                new StoreItem() { Id = 7, Name = "High Warlord", IconFileName = "HighWarlord2.png", Description = "Played way too much World of Warcraft.", PointPrice = 200, ItemTypeId = (int)StoreItemTypeValues.Title },
                new StoreItem() { Id = 8, Name = "Grand Marshal", IconFileName = "GrandMarshal1.png", Description = "Played way too much World of Warcraft.", PointPrice = 200, ItemTypeId = (int)StoreItemTypeValues.Title },
                new StoreItem() { Id = 9, Name = "General", IconFileName = "General1.png", Description = "Commands a presence on the battlefield.", PointPrice = 200, ItemTypeId = (int)StoreItemTypeValues.Title },
                new StoreItem() { Id = 10, Name = "Psycho Dentist", IconFileName = "PsychoDentist.png", Description = "Straps people to a chair and drills for cavities.", PointPrice = 200, ItemTypeId = (int)StoreItemTypeValues.Title },
                new StoreItem() { Id = 11, Name = "Crazy Cat Lover", IconFileName = "CrazyCatLover.png", Description = "Owns 20 cats and probably doesn't clean up after them.", PointPrice = 200, ItemTypeId = (int)StoreItemTypeValues.Title },
                new StoreItem() { Id = 12, Name = "The President", IconFileName = "President.png", Description = "The most powerful person in the world.", PointPrice = 200, ItemTypeId = (int)StoreItemTypeValues.Title },
                new StoreItem() { Id = 13, Name = "El Guapo", IconFileName = "ElGuapo.png", Description = "Devilishly handsome.", PointPrice = 200, ItemTypeId = (int)StoreItemTypeValues.Title });

            context.QuestionTypes.AddOrUpdate(q => q.Id,
                new QuestionType() { Id = (int)QuestionTypeValues.Random, Name = "Random" },
                new QuestionType() { Id = (int)QuestionTypeValues.Timed, Name = "Timed" },
                new QuestionType() { Id = (int)QuestionTypeValues.Quiz, Name = "Quiz" });

            context.MilestoneTypes.AddOrUpdate(m => m.Id,
                new MilestoneType() { Id = (int)MilestoneTypeValues.QuestionAnsweredCorrectly, Name = "Question Answered Correctly" },
                new MilestoneType() { Id = (int)MilestoneTypeValues.QuizCompletedSuccessfully, Name = "Quiz Completed Successfully" });

            SaveChanges(context);
        }
    }
}
