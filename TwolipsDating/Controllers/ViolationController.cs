using Microsoft.AspNet.Identity;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using System.Web.Mvc;
using TwolipsDating.Business;
using TwolipsDating.Utilities;

namespace TwolipsDating.Controllers
{
    public class ViolationController : BaseController
    {
        /// <summary>
        /// Adds a review violation to the database for the currently logged in user. Does nothing if the user has already reported this review.
        /// </summary>
        /// <param name="reviewId"></param>
        /// <param name="violationTypeId"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> AddReviewViolation(int reviewId, int violationTypeId, string content)
        {
            var authorUserId = User.Identity.GetUserId();

            try
            {
                // don't allow the user to report a review that they've already reported
                if (await ViolationService.HasUserAlreadyReportedReview(reviewId, authorUserId))
                {
                    return Json(new { success = false, error = ErrorMessages.UserAlreadyReportedReview });
                }

                int changes = await ViolationService.AddReviewViolation(reviewId, violationTypeId, content, authorUserId);

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

                    return Json(new { success = false, error = ErrorMessages.ReviewViolationNotSaved });
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

        /// <summary>
        /// Adds a question violation to the database for the currently logged in user. Does nothing if the user has already reported this review.
        /// </summary>
        /// <param name="questionId"></param>
        /// <param name="violationTypeId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> AddQuestionViolation(int questionId, int violationTypeId)
        {
            var currentUserId = User.Identity.GetUserId();

            try
            {
                // don't allow the user to report a question that they've already reported
                if (await ViolationService.HasUserAlreadyReportedQuestion(questionId, currentUserId))
                {
                    return Json(new { success = false, error = ErrorMessages.UserAlreadyReportedQuestion });
                }

                int changes = await ViolationService.AddQuestionViolation(questionId, violationTypeId, currentUserId);

                if (changes > 0)
                {
                    return Json(new { success = true });
                }
                else
                {
                    Log.Warn(
                        "AddQuestionViolation",
                        ErrorMessages.QuestionViolationNotSaved,
                        parameters: new { violationTypeId = violationTypeId, authorUserId = currentUserId }
                    );

                    return Json(new { success = false, error = ErrorMessages.QuestionViolationNotSaved });
                }
            }
            catch (DbUpdateException e)
            {
                Log.Error(
                    "AddQuestionViolation",
                    e,
                    parameters: new { violationTypeId = violationTypeId, authorUserId = currentUserId }
                );

                return Json(new { success = false, error = ErrorMessages.QuestionViolationNotSaved });
            }
        }

        /// <summary>
        /// Disposes all services.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && ViolationService != null)
            {
                ViolationService.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}