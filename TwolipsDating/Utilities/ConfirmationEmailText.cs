using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.Utilities
{
    public static class ConfirmationEmailText
    {
        public const string Subject = "Confirm your twolips dating account";
        private const string Body = @"<img src=""http://www.twolipsdating.com/Content/twolipsicon-white-180x180.png"" width=""32"" height=""32"" style=""float: left;"" />
<div style=""font-family: Helvetica,Arial,sans-serif; font-size: 24px; margin-left: 35px;"">
    Twolips Dating - Find better dates and have a better time.
</div>
<br />
<div style=""border-top: 1px solid #4b4b4b; width: 650px;""></div>
<div style=""font-family: Helvetica,Arial,sans-serif; margin-left: 25px; width: 500px; margin-bottom: 15px;"">
    <p style=""font-weight: bold;"">Hello and thank you for registering for Twolips Dating!</p>

    <div style=""font-size: 12px; color: #4b4b4b;"">
        <p>We're sending you this message to confirm your new account. If you think that this message is not intended for you, please ignore it.</p>

        <p>
            If you're the right person, please <a href=""{0}"">click here to confirm your account</a> as soon as possible to begin contributing to the community.
            Please be aware that you will be unable to login and perform certain activities until your account is confirmed.
        </p>
    </div>
</div>
<div style=""border-top: 1px solid #4b4b4b; width: 650px;""></div>
<div style=""font-family: Helvetica,Arial,sans-serif; font-size: 12px; margin-left: 25px; width: 500px; margin-top: 7px;"">
    <a href=""https://www.twolipsdating.com/about/privacy"">Privacy Policy</a> | <a href=""mailto:info@twolipsdating.com"">Contact Us</a> | <a href=""https://www.twolipsdating.com/"">Home</a>
    <p><small>Twolips Dating, Orlando, FL, USA</small></p>
</div>";

        public static string GetBody(string callbackUrl)
        {
            return String.Format(Body, callbackUrl);
        }
    }
}