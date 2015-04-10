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

        public async Task<IReadOnlyCollection<ViolationType>> GetViolationTypesAsync()
        {
            var violationTypeResults = from violationTypes in db.ViolationTypes
                          select violationTypes;

            var results = await violationTypeResults.ToListAsync();

            return results.AsReadOnly();
        }
    }
}