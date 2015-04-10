using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TwolipsDating.Business;
using TwolipsDating.Utilities;

namespace TwolipsDating.Controllers
{
    public class ViolationController : BaseController
    {
        private ViolationService violationService = new ViolationService();

        [HttpPost]
        public async Task<JsonResult> AddReviewViolation(int reviewId, int violationTypeId, string authorUserId, string content)
        {
            try
            {
                if(await violationService.HasUserAlreadyReportedReview(reviewId, authorUserId))
                {
                    return Json(new { success = false, error = ErrorMessages.UserAlreadyReportedReview });
                }

                int changes = await violationService.AddReviewViolation(reviewId, violationTypeId, content, authorUserId);

                if (changes > 0)
                {
                    return Json(new { success = true });
                }
                else
                {
                    Log.Warn(
                        "AddReviewViolation",
                        ErrorMessages.ReviewViolationNotSaved,
                        parameters: new { violationTypeId = violationTypeId, content = content, authorUserId = authorUserId }
                    );

                    return Json(new { success = false, ErrorMessages.ReviewViolationNotSaved });
                }
            }
            catch (DbUpdateException e)
            {
                Log.Error(
                    "AddReviewViolation",
                    e,
                    parameters: new { violationTypeId = violationTypeId, content = content, authorUserId = authorUserId }
                );

                return Json(new { success = false, error = ErrorMessages.ReviewViolationNotSaved });
            }
        }
    }
}