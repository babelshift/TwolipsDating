using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TwolipsDating.Business;
using TwolipsDating.Utilities;
using Microsoft.AspNet.Identity;

namespace TwolipsDating.Controllers
{
    public class ViolationController : BaseController
    {
        private ViolationService violationService = new ViolationService();

        // todo, don't accept author user id, user could pretend to be someone else
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

        [HttpPost]
        public async Task<JsonResult> AddQuestionViolation(int questionId, int violationTypeId)
        {
            var currentUserId = User.Identity.GetUserId();

            try
            {
                if (await violationService.HasUserAlreadyReportedQuestion(questionId, currentUserId))
                {
                    return Json(new { success = false, error = ErrorMessages.UserAlreadyReportedQuestion });
                }

                int changes = await violationService.AddQuestionViolation(questionId, violationTypeId, currentUserId);

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


        protected override void Dispose(bool disposing)
        {
            if (disposing && violationService != null)
            {
                violationService.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}