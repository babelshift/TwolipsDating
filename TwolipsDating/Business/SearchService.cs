using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using TwolipsDating.Models;
using TwolipsDating.ViewModels;

namespace TwolipsDating.Business
{
    public class SearchService : BaseService
    {
        internal async Task<IReadOnlyCollection<Profile>> SearchProfilesByUserName(string userName)
        {
            var results = await (from profiles in db.Profiles
                                 where profiles.ApplicationUser.UserName.Contains(userName)
                                 where profiles.ApplicationUser.IsActive
                                 select profiles).ToListAsync();

            return results.AsReadOnly();
        }

        internal async Task<IReadOnlyCollection<Profile>> SearchProfilesByTagName(string userName)
        {
            var results = await (from profiles in db.Profiles
                                 join tagSuggestions in db.TagSuggestions on profiles.Id equals tagSuggestions.ProfileId
                                 where tagSuggestions.Tag.Name == userName
                                 where profiles.ApplicationUser.IsActive
                                 select profiles)
                                 .Distinct()
                                 .ToListAsync();

            return results.AsReadOnly();
        }

        internal async Task<IReadOnlyCollection<QuizSearchResultViewModel>> GetQuizzesByTagAsync(string tag)
        {
            var result = from quizzes in db.Quizzes
                         from questions in quizzes.Questions
                         group quizzes by new { quizzes.Id, quizzes.Name, quizzes.Description } into g
                         select new QuizSearchResultViewModel()
                         {
                             QuizId = g.Key.Id,
                             QuizName = g.Key.Name,
                             QuizDescription = g.Key.Description,
                             AveragePoints = (int)g.Average(x => x.Questions.Average(y => y.Points)),
                             QuestionCount = g.Count()
                         };

            return (await result.ToListAsync()).AsReadOnly();
        }
    }
}