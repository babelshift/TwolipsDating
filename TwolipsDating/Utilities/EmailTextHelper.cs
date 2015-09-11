using System;
using System.Web;

namespace TwolipsDating.Utilities
{
    public static class EmailTextHelper
    {
        public static class ReviewEmail
        {
            private const string Subject = "{0} wrote a review about you on Twolips!";
            public static string GetSubject(string followerUserName)
            {
                followerUserName = HttpUtility.HtmlEncode(followerUserName);
                return String.Format(Subject, followerUserName);
            }

            private const string Body = @"
<div style=""text-align: center"">
<table border=""0"" cellpadding=""0"" cellspacing=""0"">
<tr><td style=""font-family: Helvetica,Arial,sans-serif; text-align: center; padding-bottom: 5px"">
<p>{0}, someone wrote a review about you on Twolips.</p>
</td></tr>
<tr><td style=""font-family: Helvetica,Arial,sans-serif; text-align: center;"">
<img src=""{1}"" width=""128"" height=""128"" />
</td></tr>
<tr><td style=""padding-top: 5px; padding-bottom: 5px; font-family: Helvetica,Arial,sans-serif; text-align: center;"">
<h2 style=""color: #4b4b4b;"">{2}</h2>
</td></tr>
<tr><td style=""font-family: Helvetica,Arial,sans-serif; text-align: center;"">
<p>""{3}""</p>
</td></tr>
<tr><td style=""font-family: Helvetica,Arial,sans-serif; text-align: center; padding-top: 5px; padding-bottom: 5px"">
<a href=""{4}"" style=""font-size: 18px"">View Profile</a>
</td></tr>
</table>
</div>";

            public static string GetBody(string receiverUserName, string senderProfileImagePath, string senderUserName, string reviewText, string senderProfileUrl)
            {
                receiverUserName = HttpUtility.HtmlEncode(receiverUserName);
                senderUserName = HttpUtility.HtmlEncode(senderUserName);
                reviewText = HttpUtility.HtmlEncode(reviewText);

                return String.Format(Body, receiverUserName, senderProfileImagePath, senderUserName, reviewText, senderProfileUrl);
            }
        }

        public static class GiftEmail
        {
            private const string Subject = "{0} sent you a gift on Twolips!";
            public static string GetSubject(string followerUserName)
            {
                return String.Format(Subject, followerUserName);
            }

            private const string Body = @"
<div style=""text-align: center"">
<table border=""0"" cellpadding=""0"" cellspacing=""0"">
<tr><td style=""font-family: Helvetica,Arial,sans-serif; text-align: center; padding-bottom: 5px"">
<p>{0}, you have a new gift on Twolips.</p>
</td></tr>
<tr><td style=""font-family: Helvetica,Arial,sans-serif; text-align: center;"">
<table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""500"" style=""margin: 0 auto"">
<tr>
<td style=""text-align:center;"" width=""225"">
<img src=""{1}"" />
</td>
<td>
</td>
<td style=""text-align:center;"" width=""225"">
<img src=""{2}"" width=""128"" height=""128"" />
</td>
</tr>
<tr>
<td style=""text-align:center;"">
<h2 style=""font-family: Helvetica,Arial,sans-serif; color: #4b4b4b;"">{3}</h2>
</td>
<td style=""text-align:center;"">
<p style=""font-family: Helvetica,Arial,sans-serif; color: #4b4b4b;"">from</p>
</td style=""text-align:center;"">
<td style=""text-align:center;"">
<h2 style=""font-family: Helvetica,Arial,sans-serif; color: #4b4b4b;"">{4}</h2>
</td>
</tr>
</table>
</td></tr>
<tr><td style=""font-family: Helvetica,Arial,sans-serif; text-align: center; padding-top: 5px; padding-bottom: 5px"">
										<a href=""{5}"" style=""font-size: 18px"">View Profile</a>
</td></tr>
</table>
</div>";

            public static string GetBody(string receiverUserName, string giftImagePath, string giftName, string senderProfileImagePath, string senderUserName, string senderProfileUrl)
            {
                receiverUserName = HttpUtility.HtmlEncode(receiverUserName);
                senderUserName = HttpUtility.HtmlEncode(senderUserName);
                giftName = HttpUtility.HtmlEncode(giftName);
                return String.Format(Body, receiverUserName, giftImagePath, senderProfileImagePath, giftName, senderUserName, senderProfileUrl);
            }
        }

        public static class MessageEmail
        {
            private const string Subject = "{0} sent you a message on Twolips!";
            public static string GetSubject(string followerUserName)
            {
                return String.Format(Subject, followerUserName);
            }

            private const string Body = @"
<div style=""text-align: center"">
<table border=""0"" cellpadding=""0"" cellspacing=""0"">
<tr><td style=""font-family: Helvetica,Arial,sans-serif; text-align: center; padding-bottom: 5px"">
<p>{0}, you have a new message on Twolips.</p>
</td></tr>
<tr><td style=""font-family: Helvetica,Arial,sans-serif; text-align: center;"">
<img src=""{1}"" width=""128"" height=""128"" />
</td></tr>
<tr><td style=""padding-top: 5px; padding-bottom: 5px; font-family: Helvetica,Arial,sans-serif; text-align: center;"">
<h2 style=""color: #4b4b4b;"">{2}</h2>
</td></tr>
<tr><td style=""font-family: Helvetica,Arial,sans-serif; text-align: center;"">
<p>""{3}""</p>
</td></tr>
<tr><td style=""font-family: Helvetica,Arial,sans-serif; text-align: center; padding-top: 5px; padding-bottom: 5px"">
<a href=""{4}"" style=""font-size: 18px"">View Conversation</a>
</td></tr>
</table>
</div>";

            public static string GetBody(string receiverUserName, string senderProfileImagePath, string senderUserName, string messageText, string conversationUrl)
            {
                receiverUserName = HttpUtility.HtmlEncode(receiverUserName);
                senderUserName = HttpUtility.HtmlEncode(senderUserName);
                messageText = HttpUtility.HtmlEncode(messageText);
                return String.Format(Body, receiverUserName, senderProfileImagePath, senderUserName, messageText, conversationUrl);
            }
        }

        public static class NewFollowerEmail
        {
            private const string Subject = "{0} is now following you on Twolips!";
            public static string GetSubject(string followerUserName)
            {
                return String.Format(Subject, followerUserName);
            }

            private const string Body = @"
<div style=""text-align: center"">
<table border=""0"" cellpadding=""0"" cellspacing=""0"">
<tr><td style=""font-family: Helvetica,Arial,sans-serif; text-align: center; padding-bottom: 5px"">
<p>{0}, you have a new follower on Twolips.</p>
</td></tr>
<tr><td style=""font-family: Helvetica,Arial,sans-serif; text-align: center;"">
<img src=""{1}"" width=""128"" height=""128"" />
</td></tr>
<tr><td style=""padding-top: 5px; padding-bottom: 5px; font-family: Helvetica,Arial,sans-serif; text-align: center;"">
<h2 style=""color: #4b4b4b;"">{2}</h2>
</td></tr>
<tr><td style=""font-family: Helvetica,Arial,sans-serif; text-align: center; padding-top: 5px; padding-bottom: 5px"">
<a href=""{3}"" style=""font-size: 18px"">View Profile</a>
</td></tr>
</table>
</div>";

            public static string GetBody(string followingUserName, string followerProfileImagePath, string followerUserName, string profileUrl)
            {
                followerUserName = HttpUtility.HtmlEncode(followerUserName);
                followingUserName = HttpUtility.HtmlEncode(followingUserName);
                return String.Format(Body, followingUserName, followerProfileImagePath, followerUserName, profileUrl);
            }
        }

        public static class PasswordChangeEmail
        {
            public const string Subject = "Your twolips password has been changed";

            private const string Body = @"
<div style=""text-align: left"">
<h3>We wanted to let you know that your password has been changed.</h3>

<div style=""font-size: 14px; color: #4b4b4b;"">
	<p>If you didn't do this, please let us know immediately. Otherwise, click below to confirm that this email address is still accurate.</p>

	<h3><a href=""{0}"">Click to confirm your email</a></h3>
</div>
</div>
</div>";

            public static string GetBody(string callbackUrl)
            {
                return String.Format(Body, callbackUrl);
            }
        }

        public static class EmailChangedEmail
        {
            public const string Subject = "Your twolips email has changed";

            private const string Body = @"
<div style=""text-align: left"">
<h3>We wanted to let you know that your email address has been changed.</h3>

<div style=""font-size: 14px; color: #4b4b4b;"">
	<p>If you didn't do this, please let us know immediately. Otherwise, click below to confirm that this email address is accurate.</p>

	<h3><a href=""{0}"">Click to confirm your email</a></h3>
</div>
</div>
</div>";

            public static string GetBody(string callbackUrl)
            {
                return String.Format(Body, callbackUrl);
            }
        }

        public static class ConfirmationEmail
        {
            public const string Subject = "Confirm your twolips dating account";

            private const string Body = @"
<div style=""text-align: left"">
<h3>Congrats! Your account is all setup.</h3>

<div style=""font-size: 14px; color: #4b4b4b;"">
	<p>Now we just need you to confirm your email address to complete the process.</p>

	<h3><a href=""{0}"">Click to confirm your account</a></h3>
</div>
</div>
</div>";

            public static string GetBody(string callbackUrl)
            {
                return String.Format(Body, callbackUrl);
            }
        }

        public static class WelcomeEmail
        {
            public const string Subject = "Welcome to twolips dating";

            private const string Body = @"
<h2>Welcome!</h2>
<p>Thank you for confirming your email address.</p>
<div style=""color: #4b4b4b;"">
	<h3>Get Started</h3>
	<div style=""font-size: 12px"">
		<ul>
			<li><a href=""https://www.twolipsdating.com/profile"">Create your profile</a> by selecting what you are, when you were born, and where you live.</li>
			<li>Consider being silly with what you are. Are you an Ogre? How about a Robot? Have fun!</li>
			<li>Use the <a href=""https://www.twolipsdating.com/search"">search section</a> to find other users.</li>
		</ul>
	</div>
	<h3>Customize Your Profile</h3>
	<div style=""font-size: 12px"">
		<ul>
			<li>Upload some images to your <a href=""https://www.twolipsdating.com/profile"">profile</a> and change your profile picture.</li>
			<li>Display unique titles that you earn with points or rewards.</li>
			<li>Raise your profile rank by being a positive influence.</li>
		</ul>
	</div>
	<h3>Answer Trivia with Friends</h3>
	<div style=""font-size: 12px"">
		<ul>
			<li>Answer questions and complete quizzes from your <a href=""https://www.twolipsdating.com/dashboard"">trivia dashboard</a>.</li>
			<li>Visit the <a href=""https://www.twolipsdating.com/trivia"">trivia section</a> to answer random and timed questions.</li>
			<li>Earn points and tags by answering questions correctly and completing quizzes.</li>
			<li>Show off your knowledge by earning profile tags based on the questions you answer.</li>
		</ul>
	</div>
</div>";

            public static string GetBody()
            {
                return String.Format(Body);
            }
        }
    }
}