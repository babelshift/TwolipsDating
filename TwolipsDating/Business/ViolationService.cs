using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TwolipsDating.Models;

namespace TwolipsDating.Business
{
    public class ViolationService : BaseService
    {
        private ViolationService(ApplicationDbContext db)
            : base(db)
        {
        }

        internal static ViolationService Create(IdentityFactoryOptions<ViolationService> options, IOwinContext context)
        {
            var service = new ViolationService(context.Get<ApplicationDbContext>());
            service.EmailService = new EmailService();
            return service;
        }

        /// <summary>
        /// Returns a boolean indicating if a user has already reported a review violation.
        /// </summary>
        /// <param name="reviewId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> HasUserAlreadyReportedReview(int reviewId, string userId)
        {
            Debug.Assert(reviewId > 0);
            Debug.Assert(!String.IsNullOrEmpty(userId));

            var reviewViolations = from violation in db.ReviewViolations
                                   where violation.ReviewId == reviewId
                                   where violation.AuthorUserId == userId
                                   select violation;

            return (await reviewViolations.CountAsync()) == 1;
        }

        /// <summary>
        /// Adds a review violation report to the database. Does nothing if the user put nothing in the content.
        /// </summary>
        /// <param name="reviewId"></param>
        /// <param name="violationTypeId"></param>
        /// <param name="content"></param>
        /// <param name="authorUserId"></param>
        /// <returns></returns>
        public async Task<int> AddReviewViolation(int reviewId, int violationTypeId, string content, string authorUserId)
        {
            if (String.IsNullOrEmpty(content))
            {
                throw new ArgumentNullException("content", "Review violation content cannot be empty.");
            }

            Debug.Assert(reviewId > 0);
            Debug.Assert(violationTypeId > 0);
            Debug.Assert(!String.IsNullOrEmpty(authorUserId));

            ReviewViolation violation = db.ReviewViolations.Create();
            violation.ReviewId = reviewId;
            violation.ViolationTypeId = violationTypeId;
            violation.Content = content;
            violation.AuthorUserId = authorUserId;
            violation.DateCreated = DateTime.Now;

            db.ReviewViolations.Add(violation);
            return await db.SaveChangesAsync();
        }

        /// <summary>
        /// Returns a collection of types that users can select for question violations.
        /// </summary>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<QuestionViolationType>> GetQuestionViolationTypesAsync()
        {
            var violationTypeResults = from violationTypes in db.QuestionViolationTypes
                                       select violationTypes;

            var results = await violationTypeResults.ToListAsync();

            return results.AsReadOnly();
        }

        /// <summary>
        /// Returns a collection of types that users can select for review violations.
        /// </summary>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<ViolationType>> GetViolationTypesAsync()
        {
            var violationTypeResults = from violationTypes in db.ViolationTypes
                                       select violationTypes;

            var results = await violationTypeResults.ToListAsync();

            return results.AsReadOnly();
        }

        /// <summary>
        /// Returns a boolean indicating if a user has alraedy reported a question violation.
        /// </summary>
        /// <param name="questionId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> HasUserAlreadyReportedQuestion(int questionId, string userId)
        {
            Debug.Assert(questionId > 0);
            Debug.Assert(!String.IsNullOrEmpty(userId));

            var questionViolations = from violation in db.QuestionViolations
                                     where violation.QuestionId == questionId
                                     where violation.AuthorUserId == userId
                                     select violation;

            return (await questionViolations.CountAsync()) == 1;
        }

        /// <summary>
        /// Adds a question violation authored by a user.
        /// </summary>
        /// <param name="questionId"></param>
        /// <param name="violationTypeId"></param>
        /// <param name="authorUserId"></param>
        /// <returns></returns>
        internal async Task<int> AddQuestionViolation(int questionId, int violationTypeId, string authorUserId)
        {
            Debug.Assert(questionId > 0);
            Debug.Assert(violationTypeId > 0);
            Debug.Assert(!String.IsNullOrEmpty(authorUserId));

            QuestionViolation violation = db.QuestionViolations.Create();
            violation.QuestionId = questionId;
            violation.QuestionViolationTypeId = violationTypeId;
            violation.AuthorUserId = authorUserId;
            violation.DateCreated = DateTime.Now;

            db.QuestionViolations.Add(violation);
            return await db.SaveChangesAsync();
        }
    }
}