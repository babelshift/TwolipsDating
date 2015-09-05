using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TwolipsDating.Models;
using TwolipsDating.ViewModels;

namespace TwolipsDating.Business
{
    public class SearchService : BaseService
    {
        public SearchService(IValidationDictionary validationDictionary) : base(validationDictionary) { }

        internal async Task<IReadOnlyCollection<Profile>> GetProfilesByUserNameAsync(string userName)
        {
            var results = await (from profiles in db.Profiles
                                 where profiles.ApplicationUser.UserName.Contains(userName)
                                 where profiles.ApplicationUser.IsActive
                                 select profiles).ToListAsync();

            return results.AsReadOnly();
        }

        internal async Task<IReadOnlyCollection<Profile>> GetProfilesByTagNameAsync(string userName)
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

        internal async Task<IReadOnlyCollection<Profile>> GetProfilesByTagNamesAsync(string[] tags)
        {
            var results = await (from profiles in db.Profiles
                                 join tagSuggestions in db.TagSuggestions on profiles.Id equals tagSuggestions.ProfileId
                                 where tags.Contains(tagSuggestions.Tag.Name)
                                 where profiles.ApplicationUser.IsActive
                                 select profiles)
                                 .Distinct()
                                 .ToListAsync();

            return results.AsReadOnly();
        }

        internal async Task<IReadOnlyCollection<QuizSearchResultViewModel>> GetQuizzesByTagsAsync(string tag)
        {
            Debug.Assert(!String.IsNullOrEmpty(tag));

            string sql = @"
                select
	                q.Id as QuizId,
	                q.Name as QuizName,
	                q.Description as QuizDescription,
	                qqc.QuestionCount as QuestionCount,
	                qqc.QuestionPointAverage as AveragePoints
                from 
	                dbo.Quizs q
	                inner join 
	                (
		                select 
		                qq.Quiz_Id as QuizId,
                        round(avg(cast(qu.Points as float)), 0) as QuestionPointAverage,
		                count(qq.Question_Id) as QuestionCount
		                from dbo.QuizQuestions qq
		                inner join dbo.questions qu on qu.Id = qq.Question_Id
		                group by qq.Quiz_Id
	                ) qqc on qqc.QuizId = q.Id
	                inner join dbo.QuizQuestions as qq on qq.Quiz_Id = q.Id
	                inner join dbo.questions qu on qu.Id = qq.Question_Id
	                inner join dbo.TagQuestions tq on tq.Question_Id = qu.Id
	                inner join dbo.Tags t on t.TagId = tq.Tag_TagId
                where 
	                t.Name in (@tagList)
                group by 
	                q.Id,
	                q.Name,
	                q.Description,
	                qqc.QuestionCount,
	                qqc.QuestionPointAverage
            ";

            var results = await QueryAsync<QuizSearchResultViewModel>(sql, new { tagList = tag });

            return results.ToList().AsReadOnly();
        }
    }
}