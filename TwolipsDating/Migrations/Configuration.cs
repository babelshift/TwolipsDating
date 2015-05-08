namespace TwolipsDating.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using TwolipsDating.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<TwolipsDating.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(TwolipsDating.Models.ApplicationDbContext context)
        {
            context.Genders.AddOrUpdate(g => g.Id,
                new Gender() { Id = 1, Name = "Unknown" },
                new Gender() { Id = 2, Name = "Man" },
                new Gender() { Id = 3, Name = "Woman" }
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

            context.Gifts.AddOrUpdate(g => g.Id,
                new Gift() { Id = 1, Name = "Rose (red)", IconFileName = "RedRose.png", Description = "A red rose" },
                new Gift() { Id = 2, Name = "Rose (white)", IconFileName = "WhiteRose.png", Description = "A white rose" },
                new Gift() { Id = 3, Name = "Dog bone", IconFileName = "DogBone.png", Description = "A tasty dog bone" },
                new Gift() { Id = 4, Name = "Candy", IconFileName = "Candy.png", Description = "A delicious piece of candy" });

            context.QuestionTypes.AddOrUpdate(q => q.Id,
                new QuestionType() { Id = (int)QuestionTypeValues.Random, Name = "Random" },
                new QuestionType() { Id = (int)QuestionTypeValues.Timed, Name = "Timed" });

            context.MilestoneTypes.AddOrUpdate(m => m.Id,
                new MilestoneType() { Id = (int)MilestoneTypeValues.QuestionAnsweredCorrectly, Name = "Question Answered Correctly" },
                new MilestoneType() { Id = (int)MilestoneTypeValues.QuizCompletedSuccessfully, Name = "Quiz Completed Successfully" });
        }
    }
}
