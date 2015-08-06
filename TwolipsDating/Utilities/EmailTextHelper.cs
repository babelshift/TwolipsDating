using System;

namespace TwolipsDating.Utilities
{
    public static class EmailTextHelper
    {
        public static class ConfirmationEmail
        {
            public const string Subject = "Confirm your twolips dating account";

            private const string Body = @"<img src=""http://www.twolipsdating.com/Content/twolipsicon-white-180x180.png"" width=""32"" height=""32"" style=""float: left;"" />
<div style=""font-family: Helvetica,Arial,sans-serif; font-size: 24px; margin-left: 35px;"">
    twolips dating - the most entertaining way to meet new people and make new friends.
</div>
<div style=""font-family: Helvetica,Arial,sans-serif; margin-left: 25px; width: 500px; margin-bottom: 15px;"">
    <p style=""font-weight: bold;"">Hello and thank you for registering for <strong>twolips dating</strong>!</p>

    <div style=""font-size: 12px; color: #4b4b4b;"">
        <p>We're sending you this message to confirm your new account. If you think that this message is not intended for you, please ignore it.</p>

        <p>
            If you're the right person, please <a href=""{0}"">click here to confirm your account</a> as soon as possible to begin contributing to the community.
            Please be aware that you will be unable to login and perform certain activities until your account is confirmed.
        </p>
    </div>
</div>
<div style=""font-family: Helvetica,Arial,sans-serif; font-size: 12px; margin-left: 25px; width: 500px; margin-top: 7px;"">
    <a href=""https://www.twolipsdating.com/about/privacy"">Privacy Policy</a> | <a href=""mailto:info@twolipsdating.com"">Contact Us</a> | <a href=""https://www.twolipsdating.com/"">Home</a>
    <p><small>Twolips Dating, Orlando, FL, USA</small></p>
</div>";

            public static string GetBody(string callbackUrl)
            {
                return String.Format(Body, callbackUrl);
            }
        }

        public static class WelcomeEmail
        {
            public const string Subject = "Welcome to twolips dating";

            private const string Body = @"<img src=""http://www.twolipsdating.com/Content/twolipsicon-white-180x180.png"" width=""32"" height=""32"" style=""float: left;"" />
<div style=""font-family: Helvetica,Arial,sans-serif; font-size: 24px; margin-left: 35px;"">
    twolips dating - the most entertaining way to meet new people and make new friends.
</div>
<div style=""font-family: Helvetica,Arial,sans-serif; margin-left: 25px; width: 550px; margin-bottom: 15px;"">
	<p>Thank you for confirming your email address. Read below about how you can begin participating.</p>
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
				<li>Answer random questions directly from your <a href=""https://www.twolipsdating.com/dashboard"">dashboard</a>.</li>
				<li>Visit the <a href=""https://www.twolipsdating.com/trivia"">trivia section</a> to answer random and timed questions</li>
				<li>Earn points by answering questions correctly and completing quizzes</li>
				<li>Show off your knowledge by earning profile tags based on the questions you answer</li>
			</ul>
		</div>
    </div>
</div>
<div style=""font-family: Helvetica,Arial,sans-serif; font-size: 12px; margin-left: 25px; width: 500px; margin-top: 7px;"">
    <a href=""https://www.twolipsdating.com/about/privacy"">Privacy Policy</a> | <a href=""mailto:info@twolipsdating.com"">Contact Us</a> | <a href=""https://www.twolipsdating.com/"">Home</a>
    <p><small>Twolips Dating, Orlando, FL, USA</small></p>
</div>";

            public static string GetBody()
            {
                return String.Format(Body);
            }
        }
    }
}