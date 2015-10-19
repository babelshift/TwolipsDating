using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
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

        public async Task<IReadOnlyCollection<Quiz>> GetSimilarQuizzesAsync(int quizId)
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

            int quizTypeId = await (from quizzes in db.Quizzes
                                    where quizzes.Id == quizId
                                    select quizzes.QuizTypeId).FirstAsync();

            if (quizTypeId == (int)QuizTypeValues.Individual)
            {
                var quiz = from quizzes in db.Quizzes
                           .Include(x => x.Questions)
                           .Include(x => x.Questions.Select(y => y.PossibleAnswers))
                           .Include(x => x.Questions.Select(y => y.Tags))
                           where quizzes.Id == quizId
                           select quizzes;

                return await quiz.FirstOrDefaultAsync();
            }
            else
            {
                var quiz = from quizzes in db.Quizzes
                           .Include(x => x.MinefieldQuestion)
                           where quizzes.Id == quizId
                           select quizzes;

                return await quiz.FirstOrDefaultAsync();
            }
        }

        public async Task<IReadOnlyCollection<Quiz>> GetQuizzesAsync()
        {
            var quizzes = from quiz in db.Quizzes
                          where quiz.IsActive
                          select quiz;

            var results = await quizzes.ToListAsync();

            return results.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<Quiz>> GetNewQuizzesAsync(int takeCount = 10)
        {
            var quizzes = (from quiz in db.Quizzes
                           where quiz.IsActive
                           orderby quiz.DateCreated descending
                           select quiz).Take(takeCount);

            var results = await quizzes.ToListAsync();

            return results.AsReadOnly();
        }

        public async Task<ReadOnlyDictionary<int, IReadOnlyCollection<Quiz>>> GetDailyQuizzesAsync(int daysAgo)
        {
            var quizzes = from quiz in db.Quizzes.Include(x => x.MinefieldQuestion)
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
                                    group completedQuiz
                                    by new
                                    {
                                        completedQuiz.QuizId,
                                        completedQuiz.Quiz.Name,
                                        completedQuiz.Quiz.ImageFileName
                                    } into g
                                    select new
                                    {
                                        QuizId = g.Key.QuizId,
                                        QuizName = g.Key.Name,
                                        DateCompleted = g.Max(x => x.DateCompleted),
                                        ThumbnailImagePath = g.Key.ImageFileName,
                                        CompletedCount = g.Count()
                                    })
                                   .OrderByDescending(x => x.DateCompleted)
                                   .Select(x => new TrendingQuizViewModel()
                                   {
                                       QuizId = x.QuizId,
                                       QuizName = x.QuizName,
                                       CompletedCount = x.CompletedCount,
                                       ThumbnailImagePath = x.ThumbnailImagePath
                                   }).Take(10);

            var result = await completedQuizzes.ToListAsync();

            //int totalTrendingQuizzes = 0;

            foreach (var quiz in result)
            {
                quiz.ThumbnailImagePath = QuizExtensions.GetThumbnailImagePath(quiz.ThumbnailImagePath);
            }

            //foreach (var quiz in result)
            //{
            //    quiz.PercentageOfTrending = (int)Math.Round(((double)quiz.CompletedCount / (double)totalTrendingQuizzes) * 100);
            //}

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
                               CompletedCount = quiz.CompletedByUsers.Count(),
                               ThumbnailImagePath = quiz.ImageFileName
                           }).Take(10);

            var result = await quizzes.ToListAsync();

            foreach (var quiz in result)
            {
                quiz.ThumbnailImagePath = QuizExtensions.GetThumbnailImagePath(quiz.ThumbnailImagePath);
            }

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

        public async Task<AnsweredQuestionServiceResult> RecordAnsweredQuestionAsync(string userId, int questionId, int answerId, int questionTypeId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));
            Debug.Assert(questionId > 0);
            Debug.Assert(answerId > 0);
            Debug.Assert(questionTypeId > 0);

            bool success = false;
            int correctAnswerId = 0;
            int tagsAwardedCount = 0;
            List<AwardAchievementServiceResult> awardedAchievements = new List<AwardAchievementServiceResult>();

            try
            {
                // save the user's answer to the database
                AddAnsweredQuestion(userId, questionId, answerId);

                // check if the supplied answer is correct
                correctAnswerId = await GetCorrectAnswerAsync(questionId, answerId);

                var user = db.Users.Find(userId);

                // give the player the correct number of points for the question if they got it correct
                if (answerId == correctAnswerId)
                {
                    int questionPoints = await GetQuestionPointsAsync(questionId);
                    IncreaseUserPoints(user, questionPoints);
                }

                // award the user any tags for question-related points
                tagsAwardedCount = await HandleTagAwardsAsync(userId, user.Profile.Id, (int)QuizTypeValues.Individual);

                // save the changes regarding the answered question
                int changes = await db.SaveChangesAsync();

                // only award if there are changes
                if (changes > 0)
                {
                    awardedAchievements = await AwardAchievementsAsync(userId);

                    success = true;
                }
            }
            catch (DbUpdateException ex)
            {
                Log.Error("TriviaService.RecordAnsweredQuestionAsync", ex, new { userId, questionId, answerId, questionTypeId });
                ValidationDictionary.AddError(Guid.NewGuid().ToString(), ErrorMessages.AnswerNotSubmitted);
            }

            return success
                ? AnsweredQuestionServiceResult.Success(correctAnswerId, tagsAwardedCount, awardedAchievements)
                : AnsweredQuestionServiceResult.Failed(ErrorMessages.AnswerNotSubmitted);
        }

        private async Task<int> GetQuestionPointsAsync(int questionId)
        {
            int questionPoints = await (from question in db.Questions
                                        where question.Id == questionId
                                        select question.Points).FirstAsync();

            return questionPoints;
        }

        private async Task<int> HandleTagAwardsAsync(string userId, int profileId, int quizTypeId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));
            Debug.Assert(profileId > 0);

            // get a collection of all tags and associated point sums for the user
            var tagsForAnsweredQuestions = await GetTagsForAnsweredQuestionsAsync(userId, quizTypeId);

            int totalNumberOfTagsAwarded = 0;

            // for each tag, check how many tags he should have and award the missing
            foreach (var tag in tagsForAnsweredQuestions)
            {
                // user gets a tag award for every 25 points per tag-related question, this is how many should be present
                int supposedTagAwardCount = tag.Points / 25;

                // get the actual number of awarded tags of this type for the user
                int actualTagAwardCount = await GetUsersAwardedTagCountForTag(profileId, tag.TagId);

                // the number of tags to award is the difference between the supposed to have and the actual have
                int numberOfTagsToAward = supposedTagAwardCount - actualTagAwardCount;

                // if the user has been awarded tags, count it up, we will display / log this count somewhere for the user to be informed
                if (numberOfTagsToAward > 0)
                {
                    totalNumberOfTagsAwarded += numberOfTagsToAward;
                }

                // award the proper number of tags the user deserves
                for (int i = 0; i < numberOfTagsToAward; i++)
                {
                    AwardTagToProfile(profileId, tag.TagId);
                }
            }

            return totalNumberOfTagsAwarded;
        }

        private void AddAnsweredMinefieldQuestion(string userId, int minefieldQuestionId, IReadOnlyCollection<int> selectedMinefieldAnswerIds)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));
            Debug.Assert(minefieldQuestionId > 0);
            Debug.Assert(selectedMinefieldAnswerIds != null);
            Debug.Assert(selectedMinefieldAnswerIds.Count > 0);

            foreach (var selectedMinefieldAnswerId in selectedMinefieldAnswerIds)
            {
                // log the answered question
                AnsweredMinefieldQuestion answeredMinefieldQuestion = new AnsweredMinefieldQuestion()
                {
                    MinefieldAnswerId = selectedMinefieldAnswerId,
                    DateAnswered = DateTime.Now,
                    MinefieldQuestionId = minefieldQuestionId,
                    UserId = userId
                };

                db.AnsweredMinefieldQuestions.Add(answeredMinefieldQuestion);
            }
        }

        private void AddAnsweredQuestion(string userId, int minefieldQuestionId, int minefieldAnswerId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));
            Debug.Assert(minefieldQuestionId > 0);
            Debug.Assert(minefieldAnswerId > 0);

            // log the answered question
            AnsweredQuestion answeredQuestion = new AnsweredQuestion()
            {
                AnswerId = minefieldAnswerId,
                DateAnswered = DateTime.Now,
                QuestionId = minefieldQuestionId,
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

        private async Task<IReadOnlyCollection<QuestionTagPoints>> GetTagsForAnsweredQuestionsAsync(string userId, int quizTypeId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));
            Debug.Assert(quizTypeId > 0);

            string sql = "";

            if (quizTypeId == (int)QuizTypeValues.Individual)
            {
                sql = @"
                    select t.TagId, sum(q.Points) as Points
                    from dbo.AnsweredQuestions as aq
                    inner join dbo.Questions as q on aq.QuestionId = q.Id
                    inner join dbo.TagQuestions as tq on q.Id = tq.Question_Id
                    inner join dbo.Tags as t on tq.Tag_TagId = t.TagId
                    where aq.UserId = @userId
                    group by t.TagId
                ";
            }
            else
            {
                sql = @"
                    select t.TagId, sum(q.Points) as Points
                    from dbo.AnsweredMinefieldQuestions as aq
                    inner join dbo.MinefieldQuestions q on q.MinefieldQuestionId = aq.MinefieldQuestionId
                    inner join dbo.TagMinefieldQuestions as tq on q.MinefieldQuestionId = tq.MinefieldQuestion_MinefieldQuestionId
                    inner join dbo.Tags as t on tq.Tag_TagId = t.TagId
                    where aq.UserId = @userId
                    group by t.TagId
                ";
            }

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

        public async Task<IReadOnlyDictionary<int, AnsweredMinefieldQuestion>> GetSelectedMinefieldAnswersAsync(string userId, int quizId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));
            Debug.Assert(quizId > 0);

            var answeredQuestions = await (from answeredQuestion in db.AnsweredMinefieldQuestions
                                           .Include(x => x.Answer)
                                           where answeredQuestion.Question.Quiz.Id == quizId
                                           where answeredQuestion.UserId == userId
                                           select answeredQuestion)
                                    .ToDictionaryAsync(x => x.MinefieldAnswerId, x => x);

            return new ReadOnlyDictionary<int, AnsweredMinefieldQuestion>(answeredQuestions);
        }

        public async Task<IReadOnlyDictionary<int, AnsweredQuestion>> GetAnsweredQuizQuestionsAsync(string userId, int quizId)
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

        public async Task<int> SetQuizAsCompletedAsync(string userId, int quizId, int numberOfCorrectAnswers)
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

            double percentageOfCorrectQuestions = 0;
            if (quiz.QuizTypeId == (int)QuizTypeValues.Individual)
            {
                percentageOfCorrectQuestions = (double)numberOfCorrectAnswers / (double)quiz.Questions.Count;
            }
            else
            {
                percentageOfCorrectQuestions = (double)numberOfCorrectAnswers / (double)quiz.MinefieldQuestion.PossibleAnswers.Count;
            }

            if (percentageOfCorrectQuestions >= 0.8)
            {
                var user = db.Users.Find(userId);
                IncreaseUserPoints(user, quiz.Points);
            }

            int count = await db.SaveChangesAsync();

            await HandleMilestonesAsync(userId, quizId);

            return count;
        }

        private async Task HandleMilestonesAsync(string userId, int quizId)
        {
            await AwardAchievedMilestonesForUserAsync(userId, (int)MilestoneTypeValues.QuizzesCompletedSuccessfully);
            await AwardAchievedMilestonesForUserAsync(userId, (int)MilestoneTypeValues.HighFive);
            await AwardAchievedMilestonesForUserAsync(userId, (int)MilestoneTypeValues.MultiTalented);

            if (quizId == (int)QuizValues.StarTrek_TOS)
            {
                await AwardAchievedMilestonesForUserAsync(userId, (int)MilestoneTypeValues.Trekkie);
            }
            else if (quizId == (int)QuizValues.StarWarsCharacters)
            {
                await AwardAchievedMilestonesForUserAsync(userId, (int)MilestoneTypeValues.RebelAlliance);
            }
            else if (quizId == (int)QuizValues.WorldOfWarcraft_HighWarlord)
            {
                await AwardAchievedMilestonesForUserAsync(userId, (int)MilestoneTypeValues.HighWarlord);
            }
            else if (quizId == (int)QuizValues.SummerOlympics || quizId == (int)QuizValues.WinterOlympics)
            {
                await AwardAchievedMilestonesForUserAsync(userId, (int)MilestoneTypeValues.GoldMedalist);
            }
        }

        public async Task<double> GetQuizScoreAsync(string userId, int quizId, int quizTypeId)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));
            Debug.Assert(quizId > 0);

            if (quizTypeId == (int)QuizTypeValues.Individual)
            {
                var query = from completedQuizzes in db.CompletedQuizzes
                            where completedQuizzes.UserId == userId
                            where completedQuizzes.QuizId == quizId
                            from answers in completedQuizzes.User.AnsweredQuestions
                            from questions in completedQuizzes.Quiz.Questions
                            where answers.AnswerId == questions.CorrectAnswerId
                            group completedQuizzes by completedQuizzes.Quiz.Questions.Count
                                into g
                                select new
                                {
                                    Score = (double)g.Count() / (double)g.Key
                                };

                var result = await query.FirstOrDefaultAsync();

                return result != null ? result.Score : 0;
            }
            else
            {
                var correctAnswerQuery = from completedQuizzes in db.CompletedQuizzes
                                         where completedQuizzes.UserId == userId
                                         where completedQuizzes.QuizId == quizId
                                         join selectedAnswers in db.AnsweredMinefieldQuestions on
                                             new { QuizId = completedQuizzes.QuizId, UserId = completedQuizzes.UserId }
                                             equals new { QuizId = selectedAnswers.MinefieldQuestionId, UserId = selectedAnswers.UserId }
                                         select completedQuizzes;

                int correctAnswerCount = await correctAnswerQuery.CountAsync();

                var totalPossibleCorrectAnswerQuery = from minefieldAnswers in db.MinefieldAnswers
                                                      where minefieldAnswers.MinefieldQuestionId == quizId
                                                      where minefieldAnswers.IsCorrect == true
                                                      select minefieldAnswers;

                int totalPossibleCorrectAnswerCount = await totalPossibleCorrectAnswerQuery.CountAsync();

                double userScore = (double)correctAnswerCount / (double)totalPossibleCorrectAnswerCount;

                return userScore;
            }
        }

        private static void IncreaseUserPoints(ApplicationUser user, int points)
        {
            user.CurrentPoints += points;
            user.LifetimePoints += points;
        }

        private async Task<int> GetQuizPointsAsync(int quizId)
        {
            int points = await (from quizzes in db.Quizzes
                                where quizzes.Id == quizId
                                select quizzes.Points).FirstAsync();

            return points;
        }

        public async Task<bool> IsQuizCompletedByUserAsync(string userId, int quizId)
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
					q.QuizTypeId,
	                q.Name QuizName,
	                u.UserName,
                    u.Id UserId,
	                cq.DateCompleted,
	                ui.FileName ProfileImagePath,
	                p.Id ProfileId,
					""IsFavoritedByCurrentUser"" =
						CASE
							WHEN fp.UserId is null THEN 0
							ELSE 1
						END,
	                (
						case
							when q.QuizTypeId = 1
							then (
								select count(*)
								from dbo.QuestionQuizs qq2
								inner join dbo.AnsweredQuestions aq on aq.QuestionId = qq2.Question_Id
								inner join dbo.Questions qu on qu.Id = aq.QuestionId
								where qq2.Quiz_Id = q.Id
								and aq.UserId = u.Id
								and aq.AnswerId = qu.CorrectAnswerId
							)
							else (
								select count(*)
								from dbo.answeredminefieldquestions amq
								inner join dbo.MinefieldAnswers ma on ma.Id = amq.MinefieldAnswerId
								inner join dbo.MinefieldQuestions mq on mq.MinefieldQuestionId = amq.MinefieldQuestionId
								where mq.MinefieldQuestionId = q.Id
								and ma.IsCorrect = 1
				                and amq.UserId = u.id
							)
						end
					) CorrectAnswerCount,
					(
						case
							when q.QuizTypeId = 1
							then (
								select count(*)
								from dbo.QuestionQuizs qq2
								where qq2.Quiz_Id = q.Id
							)
							else (
								select count(*)
								from dbo.MinefieldAnswers ma
								where MinefieldQuestionId = q.Id
								and ma.IsCorrect = 1
							)
						end
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
                    left join dbo.FavoriteProfiles fp on fp.ProfileId = p.Id and fp.UserId = '{0}'
					inner join dbo.GeoCities gc on gc.Id = p.GeoCityId
					inner join dbo.GeoStates gs on gs.Id = gc.GeoStateId
                where
                    u.IsActive = 1
                    {1}
                group by
	                q.Id,
					q.QuizTypeId,
	                q.Name,
	                u.UserName,
                    u.Id,
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

            if (quizId.HasValue)
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

        public async Task<IReadOnlyCollection<Quiz>> GetQuizzesInCategoryAsync(int id)
        {
            var quizzes = from category in db.QuizCategories
                          where category.Id == id
                          from quiz in category.Quizzes
                          select quiz;

            return (await quizzes.ToListAsync()).AsReadOnly();
        }

        public async Task<QuizCategory> GetQuizCategoryAsync(int id)
        {
            return await db.QuizCategories.FindAsync(id);
        }

        public async Task<IReadOnlyCollection<QuizCategory>> GetQuizCategoriesAsync()
        {
            var categories = from category in db.QuizCategories
                             select category;

            return (await categories.ToListAsync()).AsReadOnly();
        }

        public async Task<IReadOnlyCollection<Profile>> GetTopPlayersAsync()
        {
            var profiles = from user in db.Users
                           orderby user.LifetimePoints descending
                           where user.IsActive
                           where user.Profile != null
                           select user.Profile;

            return (await profiles.ToListAsync()).AsReadOnly();
        }

        public async Task<int> CountOfQuizzesCompletedAsync(string userId, int daysAgo)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));
            Debug.Assert(daysAgo > 0);

            DateTime start = DateTime.Now.Subtract(TimeSpan.FromDays(daysAgo));
            DateTime end = DateTime.Now;

            int completedQuizzesCount = await (from completedQuizzes in db.CompletedQuizzes
                                               where completedQuizzes.UserId == userId
                                               where completedQuizzes.DateCompleted >= start
                                               where completedQuizzes.DateCompleted <= end
                                               select completedQuizzes).CountAsync();

            return completedQuizzesCount;
        }

        public async Task<int> GetQuizCategoriesTouchedByUserCountAsync(string userId)
        {
            int count = await (from completedQuizzes in db.CompletedQuizzes
                               where completedQuizzes.UserId == userId
                               select completedQuizzes.Quiz.QuizCategoryId)
                                                     .Distinct()
                                                     .CountAsync();

            return count;
        }

        #region Question Creation

        public async Task<ServiceResult> AddQuestionToQuizAsync(int quizId, string questionContent, int points, IReadOnlyList<string> answers, int correctAnswer, IReadOnlyCollection<int> tags)
        {
            Debug.Assert(quizId > 0);
            Debug.Assert(!String.IsNullOrEmpty(questionContent));
            Debug.Assert(points >= 1 && points <= 5);
            Debug.Assert(answers != null && answers.Count > 0);
            Debug.Assert(correctAnswer >= 1 && correctAnswer <= 4);

            Log.Info(String.Format("QuizId: {0}, Question: {1}, Points: {2}", quizId, questionContent, points));
            Log.Info(String.Format("Correct Answer: {0}", correctAnswer));

            bool success = false;

            try
            {
                List<object> parameters = new List<object>();
                StringBuilder sb = new StringBuilder();

                BuildSqlToAddQuestionToQuiz(quizId, questionContent, points, answers, correctAnswer, parameters, sb);

                var output = AddOutputParameters(parameters);

                string sql = sb.ToString();
                Log.Info(sql);

                int count = await db.Database.ExecuteSqlCommandAsync(sql, parameters.ToArray());
                success = count > 0;

                if (success)
                {
                    if (tags != null)
                    {
                        var newQuestion = GetNewQuestionFromOutputParameter(output);

                        AttachTagsToQuestion(tags, newQuestion);

                        await db.SaveChangesAsync();
                    }
                }
            }
            catch (DbUpdateException ex)
            {
                Log.Error("TriviaService.AddQuestionToQuizAsync", ex);
                ValidationDictionary.AddError(Guid.NewGuid().ToString(), String.Format("Failed while inserting quiz question: \"{0}\"", questionContent));
            }

            return success ? ServiceResult.Success : ServiceResult.Failed("Failed while inserting quiz question.");
        }

        private void BuildSqlToAddQuestionToQuiz(int quizId, string questionContent, int points, IReadOnlyList<string> answers, int correctAnswer, List<object> parameters, StringBuilder sb)
        {
            sb.Append("declare @answers as dbo.AnswerType;");

            parameters.Add(new SqlParameter("@quizId", quizId));
            parameters.Add(new SqlParameter("@question", questionContent));
            parameters.Add(new SqlParameter("@points", points));

            for (int i = 1; i <= answers.Count; i++)
            {
                string content = answers[i - 1];

                // don't insert blank answers
                if (String.IsNullOrEmpty(content))
                {
                    continue;
                }

                sb.AppendFormat("insert into @answers(Content, IsCorrect) values(@answer{0}, @answer{0}Correct);", i);

                parameters.Add(new SqlParameter(String.Format("@answer{0}", i), content));
                parameters.Add(new SqlParameter(String.Format("@answer{0}Correct", i), correctAnswer == i ? 1 : 0));

                Log.Info(String.Format("Answer{0}: {1}", i, content));
            }

            sb.Append("exec dbo.InsertQuizQuestion @question, @points, @quizId, @answers, @latestQuestionId output;");
        }

        private static SqlParameter AddOutputParameters(List<object> parameters)
        {
            var output = new SqlParameter("@latestQuestionId", System.Data.SqlDbType.Int)
            {
                Direction = System.Data.ParameterDirection.Output
            };
            parameters.Add(output);
            return output;
        }

        private void AttachTagsToQuestion(IReadOnlyCollection<int> tags, Question newQuestion)
        {
            foreach (var tagId in tags)
            {
                var tag = SetupTagForQuestion(tagId);
                newQuestion.Tags.Add(tag);
            }
        }

        private Tag SetupTagForQuestion(int tagId)
        {
            var tag = db.Tags.Local.FirstOrDefault(x => x.TagId == tagId);
            if (tag == null)
            {
                tag = new Tag()
                {
                    TagId = tagId
                };
                db.Tags.Attach(tag);
            }
            return tag;
        }

        private Question GetNewQuestionFromOutputParameter(SqlParameter output)
        {
            var newQuestion = new Question()
            {
                Id = Convert.ToInt32(output.Value),
                Tags = new List<Tag>()
            };
            db.Questions.Attach(newQuestion);
            return newQuestion;
        }

        #endregion Question Creation

        /// <summary>
        /// Returns a collection of users and their scores who are similar to the passed user and his score for the passed quiz. For example, if a user
        /// scored 50% on a Star Trek quiz, anyone else that scored within 25% of that score will be returned.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="quizId"></param>
        /// <param name="numRecords"></param>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<UserWithSimilarQuizScoreViewModel>> GetUsersWithSimilarScoresAsync(string userId, int quizId, int numRecords)
        {
            if (String.IsNullOrEmpty(userId))
            {
                return new List<UserWithSimilarQuizScoreViewModel>().AsReadOnly();
            }

            Debug.Assert(quizId > 0);

            // the score calculation depends on the type of quiz
            int quizTypeId = (await db.Quizzes.FindAsync(quizId)).QuizTypeId;

            // we want the user's score on this quiz so we can find the similar scores
            double userScore = await GetQuizScoreAsync(userId, quizId, quizTypeId);

            IQueryable<UserWithSimilarQuizScoreViewModel> scoreQuery = null;

            if (quizTypeId == (int)QuizTypeValues.Individual)
            {
                // quizzes with individual questions (multiple choice answers) have to calculate their score based on:
                // users correct answer count (where the answer they chose is equal to the correct answer)
                // total question count for the quiz (for example, a quiz with 10 questions has a total question count of 10)
                // score is UsersCorrectAnswerCount divided by TotalQuestionCount
                scoreQuery = from completedQuizzes in db.CompletedQuizzes
                             where completedQuizzes.UserId != userId
                             where completedQuizzes.QuizId == quizId
                             from answers in completedQuizzes.User.AnsweredQuestions
                             from questions in completedQuizzes.Quiz.Questions
                             where answers.AnswerId == questions.CorrectAnswerId
                             group completedQuizzes by new
                             {
                                 UserName = completedQuizzes.User.UserName,
                                 ProfileId = completedQuizzes.User.Profile.Id,
                                 ProfileThumbnailImagePath = completedQuizzes.User.Profile.UserImage.FileName,
                                 completedQuizzes.Quiz.Questions.Count
                             } into g
                             select new UserWithSimilarQuizScoreViewModel()
                             {
                                 UserName = g.Key.UserName,
                                 ProfileId = g.Key.ProfileId,
                                 ProfileThumbnailImagePath = g.Key.ProfileThumbnailImagePath,
                                 Score = (double)g.Count() / (double)g.Key.Count
                             };
            }
            else
            {
                // quizzes with minefield questions (many possible answers for a single question) have to calculate their score based on:
                // users correct answer count (where the answers they chose are marked as "correct" answers)
                // total possible correct answer count (for example, a question with 3 correct answers and 5 incorrect answers has a total possible correct answer count of 3)
                // score is UsersCorrectAnswerCount divided by TotalPossibleCorrectAnswerCount

                var totalPossibleCorrectAnswerQuery = from minefieldAnswers in db.MinefieldAnswers
                                                      where minefieldAnswers.MinefieldQuestionId == quizId
                                                      where minefieldAnswers.IsCorrect == true
                                                      select minefieldAnswers;

                int totalPossibleCorrectAnswerCount = await totalPossibleCorrectAnswerQuery.CountAsync();

                scoreQuery = from completedQuizzes in db.CompletedQuizzes
                             where completedQuizzes.UserId != userId
                             where completedQuizzes.QuizId == quizId
                             join selectedAnswers in db.AnsweredMinefieldQuestions on
                                 new { QuizId = completedQuizzes.QuizId, UserId = completedQuizzes.UserId }
                                 equals new { QuizId = selectedAnswers.MinefieldQuestionId, UserId = selectedAnswers.UserId }
                             group completedQuizzes by new
                             {
                                 UserName = completedQuizzes.User.UserName,
                                 ProfileId = completedQuizzes.User.Profile.Id,
                                 ProfileThumbnailImagePath = completedQuizzes.User.Profile.UserImage.FileName
                             } into g
                             select new UserWithSimilarQuizScoreViewModel()
                            {
                                UserName = g.Key.UserName,
                                ProfileId = g.Key.ProfileId,
                                ProfileThumbnailImagePath = g.Key.ProfileThumbnailImagePath,
                                Score = (double)g.Count() / (double)totalPossibleCorrectAnswerCount
                            };
            }

            // filter down the results to only include scores in a certain range of the user's score
            var resultQuery = scoreQuery
                .Where((x => (x.Score <= userScore * 1.25) && (x.Score >= userScore * .75)))
                .OrderBy(x => Guid.NewGuid())
                .Select(x => new UserWithSimilarQuizScoreViewModel()
                {
                    UserName = x.UserName,
                    ProfileId = x.ProfileId,
                    ProfileThumbnailImagePath = x.ProfileThumbnailImagePath,
                    Score = x.Score
                })
                .Take(numRecords);

            var results = await resultQuery.ToListAsync();

            // we have to do this outside of the query because Linq to Entities doesn't support custom extensions
            foreach (var result in results)
            {
                result.ProfileThumbnailImagePath = ProfileExtensions.GetProfileThumbnailImagePath(result.ProfileThumbnailImagePath);
            }

            return results.AsReadOnly();
        }

        public async Task<AnsweredMinefieldQuestionServiceResult> RecordAnsweredMinefieldQuestionAsync(string userId, int minefieldQuestionId, IList<MinefieldAnswerViewModel> minefieldAnswers)
        {
            Debug.Assert(!String.IsNullOrEmpty(userId));
            Debug.Assert(minefieldQuestionId > 0);
            Debug.Assert(minefieldAnswers != null);
            Debug.Assert(minefieldAnswers.Count > 0);

            bool success = false;
            int correctAnswerCount = 0;
            List<AwardAchievementServiceResult> awardedAchievements = new List<AwardAchievementServiceResult>();

            try
            {
                // find all the answers that the user got correct
                List<int> selectedAnswerIds = minefieldAnswers
                    .Where(x => x.IsSelected)
                    .Select(x => x.AnswerId)
                    .ToList();

                // these are the answers that the user selected
                var selectedAnswers = await (from minefieldAnswer in db.MinefieldAnswers
                                             where selectedAnswerIds.Contains(minefieldAnswer.Id)
                                             where minefieldAnswer.MinefieldQuestionId == minefieldQuestionId
                                             select minefieldAnswer).ToListAsync();

                // save the user's answers to the database
                AddAnsweredMinefieldQuestion(userId, minefieldQuestionId, selectedAnswerIds);

                // this is the user who answered the question
                var user = db.Users.Find(userId);

                // award points for each correct answer
                foreach (var selectedAnswer in selectedAnswers)
                {
                    if (selectedAnswer.IsCorrect)
                    {
                        IncreaseUserPoints(user, 1);
                        correctAnswerCount++;
                    }
                }

                // tag awards?
                int tagsAwardedCount = await HandleTagAwardsAsync(userId, user.Profile.Id, (int)QuizTypeValues.Minefield);

                // save the changes regarding the answered question
                int changes = await db.SaveChangesAsync();

                // only award if there are changes
                if (changes > 0)
                {
                    awardedAchievements = await AwardAchievementsAsync(userId);

                    success = true;
                }
            }
            catch (DbUpdateException ex)
            {
                Log.Error("TriviaService.RecordAnsweredMinefieldQuestionAsync", ex, new { userId, minefieldQuestionId });
                ValidationDictionary.AddError(Guid.NewGuid().ToString(), ErrorMessages.AnswerNotSubmitted);
            }

            return success
                ? AnsweredMinefieldQuestionServiceResult.Success(correctAnswerCount)
                : AnsweredMinefieldQuestionServiceResult.Failed(ErrorMessages.AnswerNotSubmitted);
        }

        private async Task<List<AwardAchievementServiceResult>> AwardAchievementsAsync(string userId)
        {
            List<AwardAchievementServiceResult> awardedAchievements = new List<AwardAchievementServiceResult>();

            // award the user any question-related milestones if they have enough question-related points
            var result = await MilestoneService.AwardAchievedMilestonesAsync(userId, (int)MilestoneTypeValues.QuestionsAnsweredCorrectly);
            var result2 = await MilestoneService.AwardAchievedMilestonesAsync(userId, (int)MilestoneTypeValues.PointsObtained);

            if (result.Succeeded && result.MilestoneIdsUnlocked != null && result.MilestoneIdsUnlocked.Count > 0)
            {
                awardedAchievements.Add(result);
            }

            if (result2.Succeeded && result2.MilestoneIdsUnlocked != null && result2.MilestoneIdsUnlocked.Count > 0)
            {
                awardedAchievements.Add(result2);
            }

            // save the milestone and tag award changes
            await db.SaveChangesAsync();

            return awardedAchievements;
        }
    }
}