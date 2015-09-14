using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using TwolipsDating.Models;
using TwolipsDating.Utilities;
using TwolipsDating.ViewModels;

namespace TwolipsDating.Business
{
    public class TriviaService : BaseService, ITriviaService
    {
        public TriviaService(ApplicationDbContext db, IIdentityMessageService emailService)
            : base(db, emailService)
        {
        }

        internal static TriviaService Create(IdentityFactoryOptions<TriviaService> options, IOwinContext context)
        {
            var service = new TriviaService(context.Get<ApplicationDbContext>(), new EmailService());
            return service;
        }

        public async Task<IReadOnlyCollection<Quiz>> GetSimilarQuizzes(int quizId)
        {
            Debug.Assert(quizId > 0);

            var thisQuiz = db.Quizzes.Find(quizId);
            int quizCategoryId = thisQuiz.QuizCategoryId;
            var quizzesInCategory = (from quiz in db.Quizzes
                                     where quiz.QuizCategoryId == quizCategoryId
                                     where quiz.Id != quizId
                                     orderby Guid.NewGuid()
                                     select quiz).Take(4);
            return (await quizzesInCategory.ToListAsync()).AsReadOnly();
        }

        public async Task<Quiz> GetQuizAsync(int quizId)
        {
            Debug.Assert(quizId > 0);

            var quizResult = from quiz in db.Quizzes.Include(t => t.Questions)
                             where quiz.Id == quizId
                             select quiz;

            return await quizResult.FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyCollection<Quiz>> GetQuizzesAsync()
        {
            var quizzes = from quiz in db.Quizzes
                          where quiz.IsActive
                          select quiz;

            var results = await quizzes.ToListAsync();

            return results.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<Quiz>> GetNewQuizzesAsync()
        {
            var quizzes = (from quiz in db.Quizzes
                           where quiz.IsActive
                           orderby quiz.DateCreated descending
                           select quiz).Take(10);

            var results = await quizzes.ToListAsync();

            return results.AsReadOnly();
        }

        public async Task<ReadOnlyDictionary<int, IReadOnlyCollection<Quiz>>> GetDailyQuizzesAsync(int daysAgo)
        {
            var quizzes = from quiz in db.Quizzes
                          where quiz.IsActive
                          select quiz;

            var allQuizzes = await quizzes.ToDictionaryAsync(x => x.Id, x => x);

            SortedDictionary<int, IReadOnlyCollection<Quiz>> randomQuizzesResult = new SortedDictionary<int, IReadOnlyCollection<Quiz>>();

            if (allQuizzes == null || allQuizzes.Count == 0)
            {
                return new ReadOnlyDictionary<int, IReadOnlyCollection<Quiz>>(randomQuizzesResult);
            }

            int numberOfQuizzesToRetrieve = 5;
            // if there are less profiles than we want to collect, only collect that amount
            if (allQuizzes.Count < numberOfQuizzesToRetrieve)
            {
                numberOfQuizzesToRetrieve = allQuizzes.Count;
            }

            for (int i = 0; i <= daysAgo; i++)
            {
                List<Quiz> randomQuizzes = new List<Quiz>();
                int seed = DateTime.Today.AddDays(i * -1).DayOfYear;
                foreach (var quiz in DictionaryHelper.UniqueRandomValues(allQuizzes, seed).Take(numberOfQuizzesToRetrieve))
                {
                    randomQuizzes.Add(quiz);
                }
                randomQuizzesResult.Add(i, randomQuizzes);
            }

            return new ReadOnlyDictionary<int, IReadOnlyCollection<Quiz>>(randomQuizzesResult);
        }

        public async Task<IReadOnlyCollection<TrendingQuizViewModel>> GetTrendingQuizzesAsync()
        {
            // we have to group on the max date completed so that we can order by that date after the group by is complete
            var completedQuizzes = (from completedQuiz in db.CompletedQuizzes
                                    group completedQuiz by new { completedQuiz.QuizId, completedQuiz.Quiz.Name } into g
                                    select new
                                    {
                                        QuizId = g.Key.QuizId,
                                        QuizName = g.Key.Name,
                                        DateCompleted = g.Max(x => x.DateCompleted),
                                        CompletedCount = g.Count()
                                    })
                                   .OrderByDescending(x => x.DateCompleted)
                                   .Select(x => new TrendingQuizViewModel()
                                   {
                                       QuizId = x.QuizId,
                                       QuizName = x.QuizName,
                                       CompletedCount = x.CompletedCount
                                   }).Take(10);

            var result = await completedQuizzes.ToListAsync();

            int totalTrendingQuizzes = 0;

            foreach (var quiz in result)
            {
                totalTrendingQuizzes += quiz.CompletedCount;
            }

            foreach (var quiz in result)
            {
                quiz.PercentageOfTrending = (int)Math.Round(((double)quiz.CompletedCount / (double)totalTrendingQuizzes) * 100);
            }

            return result.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<MostPopularQuizViewModel>> GetPopularQuizzesAsync()
        {
            var quizzes = (from quiz in db.Quizzes
                           where quiz.IsActive
                           where quiz.CompletedByUsers.Count() > 0
                           orderby quiz.CompletedByUsers.Count() descending
                           select new MostPopularQuizViewModel()
                           {
                               QuizId = quiz.Id,
                               QuizName = quiz.Name,
                               CompletedCount = quiz.CompletedByUsers.Count()
                           }).Take(10);

            var result = await quizzes.ToListAsync();
            return result.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<Quiz>> GetUnfinishedQuizzesAsync(string userId)
        {
            // get completed for user
            var completedQuizIds = from completedQuiz in db.CompletedQuizzes
                                   where completedQuiz.UserId == userId
                                   select completedQuiz.QuizId;

            // get from quizzes where not completed by user
            var quizzes = from quiz in db.Quizzes
                          where !completedQuizIds.Contains(quiz.Id)
                          select quiz;

            var result = await quizzes.ToListAsync();
            return result.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<Question>> GetQuizQuestionsAsync(int quizId)
        {
            Debug.Assert(quizId > 0);

            var questionList = from questions in db.Questions
                               from quiz in questions.Quizzes
                               where questions.QuestionTypeId.HasValue
                               where questions.QuestionTypeId.Value == (int)QuestionTypeValues.Quiz
                               where quiz.Id == quizId
                               select questions;

            var results = await questionList.ToListAsync();

            return results.AsReadOnly();
        }

        /// <summary>
        /// Returns a random question that the user has not answered yet
        /// </summary>
        /// <returns></returns>
        public async Task<Question> GetRandomQuestionAsync(string userId, int questionTypeId)
        {
            //Debug.Assert(!String.IsNullOrEmpty(userId));
            Debug.Assert(questionTypeId > 0);

            var questionsAlreadyAnswered = await (from questions in db.AnsweredQuestions
                                                  where questions.UserId == userId
                                                  select questions.QuestionId).ToListAsync();

            var questionList = await (from questions in db.Questions
                                      where !questionsAlreadyAnswered.Contains(questions.Id)
                                      where questions.QuestionTypeId.HasValue
                                      where questions.QuestionTypeId.Value == questionTypeId
                                      select questions).ToListAsync();

            // there are no more questions that can be answered
            if (questionList.Count == 0)
            {
                return null;
            }

            Random random = new Random();
            int randomQuestionIndex = random.Next(0, questionList.Count);
            Question randomQuestion = questionList[randomQuestionIndex];

            return randomQuestion;
        }

        public async Task<int> GetCorrectAnswerAsync(int questionId, int answerId)
        {
            Debug.Assert(questionId > 0);
            Debug.Assert(answerId > 0);

            int? correctAnswerId = await (from questions in db.Questions
                                          where questions.Id == questionId
                                          select questions.CorrectAnswerId).FirstOrDefaultAsync();

            if (correctAnswerId.HasValue)
            {
                return correctAnswerId.Value;
            }
            else
            {
                throw new ArgumentException("There is no Question with that ID and Answer ID.");
            }
        }

        public async Task<AnsweredQuestionServiceResult> RecordAnsweredQuestionAsync(string userId, int profileId, int questionId, int answerId, int questionTypeId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));
            Debug.Assert(profileId > 0);
            Debug.Assert(questionId > 0);
            Debug.Assert(answerId > 0);
            Debug.Assert(questionTypeId > 0);

            bool success = false;
            int correctAnswerId = 0;

            try
            {
                // save the user's answer to the database
                AddAnsweredQuestion(userId, questionId, answerId);

                // check if the supplied answer is correct
                correctAnswerId = await GetCorrectAnswerAsync(questionId, answerId);

                // give the player the correct number of points for the question if they got it correct
                if (answerId == correctAnswerId)
                {
                    var user = db.Users.Find(userId);
                    int questionPoints = await GetQuestionPointsAsync(questionId);
                    IncreaseUserPoints(user, questionPoints);
                }

                // award the user any tags for question-related points
                await HandleTagAwards(userId, profileId);

                // save the changes regarding the answered question
                int changes = await db.SaveChangesAsync();

                // only award if there are changes
                if (changes > 0)
                {
                    // award the user any question-related milestones if they have enough question-related points
                    await MilestoneService.AwardAchievedMilestonesAsync(userId, (int)MilestoneTypeValues.QuestionsAnsweredCorrectly);

                    // save the milestone and tag award changes
                    changes = await db.SaveChangesAsync();

                    success = true;
                }
            }
            catch (DbUpdateException ex)
            {
                Log.Error("TriviaService.RecordAnsweredQuestionAsync", ex, new { userId, profileId, questionId, answerId, questionTypeId });
                ValidationDictionary.AddError(Guid.NewGuid().ToString(), ErrorMessages.AnswerNotSubmitted);
            }

            return success ? AnsweredQuestionServiceResult.Success(correctAnswerId) : AnsweredQuestionServiceResult.Failed(ErrorMessages.AnswerNotSubmitted);
        }

        private async Task<int> GetQuestionPointsAsync(int questionId)
        {
            int questionPoints = await (from question in db.Questions
                                        where question.Id == questionId
                                        select question.Points).FirstAsync();

            return questionPoints;
        }

        private async Task HandleTagAwards(string userId, int profileId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));
            Debug.Assert(profileId > 0);

            // get a collection of all tags and associated point sums for the user
            var tagsForAnsweredQuestions = await GetTagsForAnsweredQuestionsAsync(userId);

            // for each tag, check how many tags he should have and award the missing
            foreach (var tag in tagsForAnsweredQuestions)
            {
                // user gets a tag award for every 25 points per tag-related question, this is how many should be present
                int supposedTagAwardCount = tag.Points / 25;

                // get the actual number of awarded tags of this type for the user
                int actualTagAwardCount = await GetUsersAwardedTagCountForTag(profileId, tag.TagId);

                // the number of tags to award is the difference between the supposed to have and the actual have
                int numberOfTagsToAward = supposedTagAwardCount - actualTagAwardCount;

                // award the proper number of tags the user deserves
                for (int i = 0; i < numberOfTagsToAward; i++)
                {
                    AwardTagToProfile(profileId, tag.TagId);
                }
            }
        }

        private void AddAnsweredQuestion(string userId, int questionId, int answerId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));
            Debug.Assert(questionId > 0);
            Debug.Assert(answerId > 0);

            // log the answered question
            AnsweredQuestion answeredQuestion = new AnsweredQuestion()
            {
                AnswerId = answerId,
                DateAnswered = DateTime.Now,
                QuestionId = questionId,
                UserId = userId
            };

            db.AnsweredQuestions.Add(answeredQuestion);
        }

        private async Task<int> GetUsersAwardedTagCountForTag(int profileId, int tagId)
        {
            Debug.Assert(tagId > 0);
            Debug.Assert(profileId > 0);

            var tagAwards = from tagAward in db.TagAwards
                            where tagAward.TagId == tagId
                            where tagAward.ProfileId == profileId
                            select tagAward;

            return await tagAwards.CountAsync();
        }

        private void AwardTagToProfile(int profileId, int tagId)
        {
            Debug.Assert(profileId > 0);
            Debug.Assert(tagId > 0);

            TagAward tagAward = new TagAward()
            {
                DateAwarded = DateTime.Now,
                ProfileId = profileId,
                TagId = tagId
            };

            db.TagAwards.Add(tagAward);
        }

        private async Task<IReadOnlyCollection<QuestionTagPoints>> GetTagsForAnsweredQuestionsAsync(string userId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));

            string sql = @"
                select t.TagId, sum(q.Points) as Points
                from dbo.AnsweredQuestions as aq
                inner join dbo.Questions as q on aq.QuestionId = q.Id
                inner join dbo.TagQuestions as tq on q.Id = tq.Question_Id
                inner join dbo.Tags as t on tq.Tag_TagId = t.TagId
                where aq.UserId = @userId
                group by t.TagId
            ";

            var results = await QueryAsync<QuestionTagPoints>(sql, new { userId = userId });

            return results.ToList().AsReadOnly();
        }

        public async Task<int> GetUsersQuestionPointsForTypeAsync(string userId, int questionTypeId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));
            Debug.Assert(questionTypeId > 0);

            string sql = @"
                select sum(points) as points
                from dbo.AnsweredQuestions as aq
                inner join dbo.Questions as q on aq.QuestionId = q.Id
                where q.QuestionTypeId = @questionTypeId
                and aq.UserId = @userId
            ";

            int? points = 0;

            var results = await QueryAsync<int>(sql, new { questionTypeId = questionTypeId, userId = userId });

            points = results.FirstOrDefault();

            if (!points.HasValue)
            {
                points = 0;
            }

            return points.Value;
        }

        public async Task<IReadOnlyDictionary<int, AnsweredQuestion>> GetAnsweredQuizQuestions(string userId, int quizId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));
            Debug.Assert(quizId > 0);

            var answeredQuestions = from answeredQuestion in db.AnsweredQuestions
                                    join question in db.Questions on answeredQuestion.QuestionId equals question.Id
                                    from quiz in db.Quizzes
                                    where question.QuestionTypeId.HasValue
                                    where question.QuestionTypeId.Value == (int)QuestionTypeValues.Quiz
                                    where answeredQuestion.UserId == userId
                                    where quiz.Id == quizId
                                    select answeredQuestion;

            var result = (await answeredQuestions.ToListAsync())
                .ToDictionary(a => a.QuestionId, a => a);

            return new ReadOnlyDictionary<int, AnsweredQuestion>(result);
        }

        public async Task<int> SetQuizAsCompleted(string userId, int quizId, int numberOfCorrectAnswers)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));
            Debug.Assert(quizId > 0);

            CompletedQuiz completedQuiz = new CompletedQuiz()
            {
                DateCompleted = DateTime.Now,
                QuizId = quizId,
                UserId = userId
            };

            db.CompletedQuizzes.Add(completedQuiz);

            // if the user got enough questions right, give them points
            var quiz = await GetQuizAsync(quizId);
            double percentageOfCorrectQuestions = (double)numberOfCorrectAnswers / (double)quiz.Questions.Count;
            if (percentageOfCorrectQuestions >= 0.8)
            {
                var user = db.Users.Find(userId);
                IncreaseUserPoints(user, quiz.Points);
            }

            int count = await db.SaveChangesAsync();

            await AwardAchievedMilestonesForUserAsync(userId, (int)MilestoneTypeValues.QuizzesCompletedSuccessfully);

            return count;
        }

        private static void IncreaseUserPoints(ApplicationUser user, int points)
        {
            user.CurrentPoints += points;
        }

        private async Task<int> GetQuizPointsAsync(int quizId)
        {
            int points = await (from quizzes in db.Quizzes
                                where quizzes.Id == quizId
                                select quizzes.Points).FirstAsync();

            return points;
        }

        public async Task<bool> IsQuizAlreadyCompletedAsync(string userId, int quizId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));
            Debug.Assert(quizId > 0);

            var completedQuiz = await (from completedQuizzes in db.CompletedQuizzes
                                       where completedQuizzes.UserId == userId
                                       where completedQuizzes.QuizId == quizId
                                       select completedQuizzes).FirstOrDefaultAsync();

            return completedQuiz != null;
        }

        public async Task<IReadOnlyDictionary<int, CompletedQuiz>> GetCompletedQuizzesForUserAsync(string currentUserId)
        {
            var completedQuizzes = await (from quizzes in db.CompletedQuizzes
                                          where quizzes.UserId == currentUserId
                                          select quizzes).ToDictionaryAsync(q => q.QuizId, q => q);

            return new ReadOnlyDictionary<int, CompletedQuiz>(completedQuizzes);
        }

        public async Task<IReadOnlyCollection<AnsweredQuestion>> GetUsersAnsweredCorrectlyAsync(int questionId)
        {
            var usersAnsweredCorrectly = from questionsAnswered in db.AnsweredQuestions
                                         where questionsAnswered.QuestionId == questionId
                                         join questions in db.Questions on questionsAnswered.AnswerId equals questions.CorrectAnswerId
                                         join users in db.Users on questionsAnswered.UserId equals users.Id
                                         where users.IsActive
                                         orderby questionsAnswered.DateAnswered descending
                                         select questionsAnswered;

            return await usersAnsweredCorrectly.ToListAsync();
        }

        public async Task<IReadOnlyCollection<UserCompletedQuizViewModel>> GetUsersCompletedQuizAsync(int? quizId = null, string currentUserId = null)
        {
            string sql = String.Format(@"
                select top 10
	                q.Id,
	                q.Name QuizName,
	                u.UserName,
	                cq.DateCompleted,
	                ui.FileName ProfileImagePath,
	                p.Id ProfileId,
					""IsFavoritedByCurrentUser"" = 
						CASE
							WHEN fp.UserId is null THEN 0
							ELSE 1
						END,
	                count(*) CorrectAnswerCount,
	                (
		                select count(*)
		                from dbo.QuizQuestions qq2
		                where qq2.Quiz_Id = q.Id
	                ) TotalAnswerCount,
                    p.Birthday,
					gc.Name CityName,
					gs.Abbreviation StateName
                from
	                dbo.CompletedQuizs cq
	                inner join dbo.AspNetUsers u on u.Id = cq.UserId
	                inner join dbo.Profiles p on p.ApplicationUser_Id = u.Id
	                left join dbo.UserImages ui on ui.Id = p.UserImageId
	                inner join dbo.Quizs q on q.Id = cq.QuizId
	                inner join dbo.QuizQuestions qq on qq.Quiz_Id = q.Id
	                inner join dbo.AnsweredQuestions aq on aq.UserId = cq.UserId and aq.QuestionId = qq.Question_Id
	                inner join dbo.Questions qu on qu.Id = aq.QuestionId
                    left join dbo.FavoriteProfiles fp on fp.ProfileId = p.Id and fp.UserId = '{0}'
					inner join dbo.GeoCities gc on gc.Id = p.GeoCityId
					inner join dbo.GeoStates gs on gs.Id = gc.GeoStateId
                where
	                aq.AnswerId = qu.CorrectAnswerId
                    and u.IsActive = 1
                    {1}
                group by
	                q.Id,
	                q.Name,
	                u.UserName,
	                cq.DateCompleted,
	                ui.FileName,
	                p.Id,
                    fp.UserId,
					p.Birthday,
					gc.Name,
					gs.Abbreviation
                order by
                    cq.DateCompleted desc"
                , !String.IsNullOrEmpty(currentUserId) ? currentUserId : String.Empty
                , quizId.HasValue ? "and q.Id = @quizId" : String.Empty);

            dynamic parameters = new ExpandoObject();

            if(quizId.HasValue)
            {
                parameters.quizId = quizId.Value;
            }

            if (!String.IsNullOrEmpty(currentUserId))
            {
                parameters.currentUserId = currentUserId;
            }
            
            var results = await QueryAsync<UserCompletedQuizViewModel>(sql, (object)parameters);

            foreach (var viewModel in results)
            {
                viewModel.ProfileImagePath = ProfileExtensions.GetProfileThumbnailImagePath(viewModel.ProfileImagePath);
                viewModel.Age = DateTimeExtensions.GetAge(viewModel.Birthday);
                viewModel.Location = CityExtensions.ToFullLocationString(viewModel.CityName, viewModel.StateName);
            }

            return results.ToList().AsReadOnly();
        }

        public async Task<IReadOnlyCollection<UserCompletedQuizViewModel>> GetUsersCompletedQuizzesAsync(string currentUserId)
        {
            return await GetUsersCompletedQuizAsync(null, currentUserId);
        }

        public async Task<IReadOnlyCollection<Tag>> GetTagsForQuestionAsync(int questionId)
        {
            var tagsForQuestion = from questions in db.Questions
                                  where questions.Id == questionId
                                  select questions.Tags;

            return (await tagsForQuestion.SingleAsync())
                .ToList()
                .AsReadOnly();
        }

        public async Task<IReadOnlyCollection<TagViewModel>> GetTagsForQuizAsync(int quizId)
        {
            var tagsForQuiz = from quizzes in db.Quizzes
                              where quizzes.Id == quizId
                              from questions in quizzes.Questions
                              from tags in questions.Tags
                              group tags by new { tags.TagId, tags.Name, tags.Description }
                                  into g
                                  select new TagViewModel()
                                  {
                                      TagId = g.Key.TagId,
                                      Name = g.Key.Name,
                                      Description = g.Key.Description
                                  };

            return await tagsForQuiz.ToListAsync();
        }

        public async Task<int> GetQuestionsAnsweredCorrectlyCountAsync(string userId)
        {
            var questionsAnsweredCorrectly = from answeredQuestion in db.AnsweredQuestions
                                             where answeredQuestion.UserId == userId
                                             where answeredQuestion.AnswerId == answeredQuestion.Question.CorrectAnswerId
                                             select answeredQuestion;

            return await questionsAnsweredCorrectly.CountAsync();
        }
    }
}