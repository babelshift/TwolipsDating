using System;

namespace TwolipsDating.Utilities
{
    public static class EmailTextHelper
    {
        public static class ReviewEmail
        {
            private const string Subject = "{0} wrote a review about you on Twolips!";
            public static string GetSubject(string followerUserName)
            {
                return String.Format(Subject, followerUserName);
            }

            private const string Body = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD HTML 4.01//EN"" ""http://www.w3.org/TR/html4/strict.dtd"">
<html>
	<head>
		<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
		<meta name=""viewport"" content=""width=device-width, minimum-scale=1.0, maximum-scale=1.0, user-scalable=0"" />
	</head>
	<body style=""margin: 0; padding: 0; background: #fff"">
		<table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""100%"" style=""background: #ddd"">
		<tbody>
			<tr>
				<td>
					<table align=""center"" cellpadding=""0"" cellspacing=""0"" border=""0"" width=""670"" style=""background: #fff; border-left: 1px solid #ccc; border-right: 1px solid #ccc;"">
						<tbody>
							<tr>
								<td style=""background: #eee; padding: 15px"">
									<table cellpadding=""0"" cellspacing=""0"" border=""0"">
										<tr>
											<td>
												<img src=""https://az732589.vo.msecnd.net/twolipsdatingcdn/twolipsicon-white-32x32.png"" width=""32"" height=""32"" />
											</td>
											<td style=""padding-left: 5px;"">
												<div style=""font-family: Helvetica,Arial,sans-serif; font-size: 20px;"">
													twolips dating
												</div>
											</td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td style=""border-top: 1px solid #ccc; padding: 15px;"">
									<div style=""font-family: Helvetica,Arial,sans-serif; margin-bottom: 15px; text-align: center"">
										<p>{0}, someone wrote a review about you on Twolips.</p>
										<img src=""{1}"" />
										<h2 style=""color: #4b4b4b; margin-top: 5px; margin-bottom: 5px;"">{2}</h2>
										<p>""{3}""</p>
										<a href=""{4}"" style=""font-size: 18px"">View Profile</a>
									</div>
								</td>
							</tr>
							<tr>
								<td style=""background: #eee; padding: 15px; border-top: 1px solid #ccc"">
									<div style=""font-family: Helvetica,Arial,sans-serif; font-size: 12px; margin-left: 25px; width: 500px; margin-top: 7px;"">
										<a href=""https://www.twolipsdating.com/"">Home</a> | <a href=""https://www.twolipsdating.com/about/privacy"">Privacy Policy</a> | <a href=""mailto:info@twolipsdating.com"">Contact Us</a> | <a href=""http://twitter.com/twolipsdating/"">Follow Us on Twitter</a> | <a href=""http://facebook.com/twolipsdating"">Like Us on Facebook</a>
										<p><small>Twolips Dating - PO Box 780835, Orlando, FL, 32878-0835</small></p>
									</div>
								</td>
							</tr>
						</tbody>
					</table>
				</td>
			</tr>
		</table>
	</body>
</html>";

            public static string GetBody(string receiverUserName, string senderProfileImagePath, string senderUserName, string reviewText, string senderProfileUrl)
            {
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

            private const string Body = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD HTML 4.01//EN"" ""http://www.w3.org/TR/html4/strict.dtd"">
<html>
	<head>
		<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
		<meta name=""viewport"" content=""width=device-width, minimum-scale=1.0, maximum-scale=1.0, user-scalable=0"" />
	</head>
	<body style=""margin: 0; padding: 0; background: #fff"">
		<table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""100%"" style=""background: #ddd"">
		<tbody>
			<tr>
				<td>
					<table align=""center"" cellpadding=""0"" cellspacing=""0"" border=""0"" width=""670"" style=""background: #fff; border-left: 1px solid #ccc; border-right: 1px solid #ccc;"">
						<tbody>
							<tr>
								<td style=""background: #eee; padding: 15px"">
									<table cellpadding=""0"" cellspacing=""0"" border=""0"">
										<tr>
											<td>
												<img src=""https://az732589.vo.msecnd.net/twolipsdatingcdn/twolipsicon-white-32x32.png"" width=""32"" height=""32"" />
											</td>
											<td style=""padding-left: 5px;"">
												<div style=""font-family: Helvetica,Arial,sans-serif; font-size: 20px;"">
													twolips dating
												</div>
											</td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td style=""border-top: 1px solid #ccc; padding: 15px;"">
									<div style=""font-family: Helvetica,Arial,sans-serif; margin-bottom: 15px; text-align: center"">
										<p>{0}, you have a new gift on Twolips.</p>
										<table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""500"" style=""margin: 0 auto"">
											<tr>
												<td style=""text-align:center;"" width=""225"">
													<img src=""{1}"" />
												</td>
												<td>
												</td>
												<td style=""text-align:center;"" width=""225"">
													<img src=""{2}"" />
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
										<a href=""{5}"" style=""font-size: 18px"">View Profile</a>
									</div>
								</td>
							</tr>
							<tr>
								<td style=""background: #eee; padding: 15px; border-top: 1px solid #ccc"">
									<div style=""font-family: Helvetica,Arial,sans-serif; font-size: 12px; margin-left: 25px; width: 500px; margin-top: 7px;"">
										<a href=""https://www.twolipsdating.com/"">Home</a> | <a href=""https://www.twolipsdating.com/about/privacy"">Privacy Policy</a> | <a href=""mailto:info@twolipsdating.com"">Contact Us</a> | <a href=""http://twitter.com/twolipsdating/"">Follow Us on Twitter</a> | <a href=""http://facebook.com/twolipsdating"">Like Us on Facebook</a>
										<p><small>Twolips Dating - PO Box 780835, Orlando, FL, 32878-0835</small></p>
									</div>
								</td>
							</tr>
						</tbody>
					</table>
				</td>
			</tr>
		</table>
	</body>
</html>";

            public static string GetBody(string receiverUserName, string giftImagePath, string giftName, string senderProfileImagePath, string senderUserName, string senderProfileUrl)
            {
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

            private const string Body = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD HTML 4.01//EN"" ""http://www.w3.org/TR/html4/strict.dtd"">
<html>
	<head>
		<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
		<meta name=""viewport"" content=""width=device-width, minimum-scale=1.0, maximum-scale=1.0, user-scalable=0"" />
	</head>
	<body style=""margin: 0; padding: 0; background: #fff"">
		<table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""100%"" style=""background: #ddd"">
		<tbody>
			<tr>
				<td>
					<table align=""center"" cellpadding=""0"" cellspacing=""0"" border=""0"" width=""670"" style=""background: #fff; border-left: 1px solid #ccc; border-right: 1px solid #ccc;"">
						<tbody>
							<tr>
								<td style=""background: #eee; padding: 15px"">
									<table cellpadding=""0"" cellspacing=""0"" border=""0"">
										<tr>
											<td>
												<img src=""https://az732589.vo.msecnd.net/twolipsdatingcdn/twolipsicon-white-32x32.png"" width=""32"" height=""32"" />
											</td>
											<td style=""padding-left: 5px;"">
												<div style=""font-family: Helvetica,Arial,sans-serif; font-size: 20px;"">
													twolips dating
												</div>
											</td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td style=""border-top: 1px solid #ccc; padding: 15px;"">
									<div style=""font-family: Helvetica,Arial,sans-serif; margin-bottom: 15px; text-align: center"">
										<p>{0}, you have a new message on Twolips.</p>
										<img src=""{1}"" />
										<h2 style=""color: #4b4b4b; margin-top: 5px; margin-bottom: 5px;"">{2}</h2>
										<p>""{3}""</p>
										<a href=""{4}"" style=""font-size: 18px"">View Conversation</a>
									</div>
								</td>
							</tr>
							<tr>
								<td style=""background: #eee; padding: 15px; border-top: 1px solid #ccc"">
									<div style=""font-family: Helvetica,Arial,sans-serif; font-size: 12px; margin-left: 25px; width: 500px; margin-top: 7px;"">
										<a href=""https://www.twolipsdating.com/"">Home</a> | <a href=""https://www.twolipsdating.com/about/privacy"">Privacy Policy</a> | <a href=""mailto:info@twolipsdating.com"">Contact Us</a> | <a href=""http://twitter.com/twolipsdating/"">Follow Us on Twitter</a> | <a href=""http://facebook.com/twolipsdating"">Like Us on Facebook</a>
										<p><small>Twolips Dating - PO Box 780835, Orlando, FL, 32878-0835</small></p>
									</div>
								</td>
							</tr>
						</tbody>
					</table>
				</td>
			</tr>
		</table>
	</body>
</html>";

            public static string GetBody(string receiverUserName, string senderProfileImagePath, string senderUserName, string messageText, string conversationUrl)
            {
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

            private const string Body = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD HTML 4.01//EN"" ""http://www.w3.org/TR/html4/strict.dtd"">
<html>
	<head>
		<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
		<meta name=""viewport"" content=""width=device-width, minimum-scale=1.0, maximum-scale=1.0, user-scalable=0"" />
	</head>
	<body style=""margin: 0; padding: 0; background: #fff"">
		<table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""100%"" style=""background: #ddd"">
		<tbody>
			<tr>
				<td>
					<table align=""center"" cellpadding=""0"" cellspacing=""0"" border=""0"" width=""670"" style=""background: #fff; border-left: 1px solid #ccc; border-right: 1px solid #ccc;"">
						<tbody>
							<tr>
								<td style=""background: #eee; padding: 15px"">
									<table cellpadding=""0"" cellspacing=""0"" border=""0"">
										<tr>
											<td>
												<img src=""https://az732589.vo.msecnd.net/twolipsdatingcdn/twolipsicon-white-32x32.png"" width=""32"" height=""32"" />
											</td>
											<td style=""padding-left: 5px;"">
												<div style=""font-family: Helvetica,Arial,sans-serif; font-size: 20px;"">
													twolips dating
												</div>
											</td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td style=""border-top: 1px solid #ccc; padding: 15px;"">
									<div style=""font-family: Helvetica,Arial,sans-serif; margin-bottom: 15px; text-align: center"">
										<p>{0}, you have a new follower on Twolips.</p>
										<img src=""{1}"" />
										<h2 style=""color: #4b4b4b; margin-top: 5px; margin-bottom: 5px;"">{2}</h2>
										<a href=""{3}"" style=""font-size: 18px"">View Profile</a>
									</div>
								</td>
							</tr>
							<tr>
								<td style=""background: #eee; padding: 15px; border-top: 1px solid #ccc"">
									<div style=""font-family: Helvetica,Arial,sans-serif; font-size: 12px; margin-left: 25px; width: 500px; margin-top: 7px;"">
										<a href=""https://www.twolipsdating.com/"">Home</a> | <a href=""https://www.twolipsdating.com/about/privacy"">Privacy Policy</a> | <a href=""mailto:info@twolipsdating.com"">Contact Us</a> | <a href=""http://twitter.com/twolipsdating/"">Follow Us on Twitter</a> | <a href=""http://facebook.com/twolipsdating"">Like Us on Facebook</a>
										<p><small>Twolips Dating - PO Box 780835, Orlando, FL, 32878-0835</small></p>
									</div>
								</td>
							</tr>
						</tbody>
					</table>
				</td>
			</tr>
		</table>
	</body>
</html>";

            public static string GetBody(string followingUserName, string followerProfileImagePath, string followerUserName, string profileUrl)
            {
                return String.Format(Body, followingUserName, followerProfileImagePath, followerUserName, profileUrl);
            }
        }

        public static class ConfirmationEmail
        {
            public const string Subject = "Confirm your twolips dating account";

            private const string Body = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD HTML 4.01//EN"" ""http://www.w3.org/TR/html4/strict.dtd"">
<html>
	<head>
		<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
		<meta name=""viewport"" content=""width=device-width, minimum-scale=1.0, maximum-scale=1.0, user-scalable=0"" />
	</head>
	<body style=""margin: 0; padding: 0; background: #fff"">
		<table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""100%"" style=""background: #ddd"">
		<tbody>
			<tr>
				<td>
					<table align=""center"" cellpadding=""0"" cellspacing=""0"" border=""0"" width=""670"" style=""background: #fff; border-left: 1px solid #ccc; border-right: 1px solid #ccc;"">
						<tbody>
							<tr>
								<td style=""background: #eee; padding: 15px"">
									<table cellpadding=""0"" cellspacing=""0"" border=""0"">
										<tr>
											<td>
												<img src=""https://az732589.vo.msecnd.net/twolipsdatingcdn/twolipsicon-white-32x32.png"" width=""32"" height=""32"" />
											</td>
											<td style=""padding-left: 5px;"">
												<div style=""font-family: Helvetica,Arial,sans-serif; font-size: 20px;"">
													twolips dating
												</div>
											</td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td style=""border-top: 1px solid #ccc; padding: 15px;"">
									<div style=""font-family: Helvetica,Arial,sans-serif; margin-bottom: 15px;"">
										<h2>Hello and thank you for registering!</h2>

										<div style=""font-size: 12px; color: #4b4b4b;"">
											<p>We're sending you this message to confirm your new account. If you think that this message is not intended for you, please ignore it.</p>

											<h3><a href=""{0}"">Click to confirm your account</a></h3>
											
											<p>Please be aware that you will be unable to login and perform certain activities until your account is confirmed.
											</p>
										</div>
									</div>
								</td>
							</tr>
							<tr>
								<td style=""background: #eee; padding: 15px; border-top: 1px solid #ccc"">
									<div style=""font-family: Helvetica,Arial,sans-serif; font-size: 12px; margin-left: 25px; width: 500px; margin-top: 7px;"">
										<a href=""https://www.twolipsdating.com/"">Home</a> | <a href=""https://www.twolipsdating.com/about/privacy"">Privacy Policy</a> | <a href=""mailto:info@twolipsdating.com"">Contact Us</a> | <a href=""http://twitter.com/twolipsdating/"">Follow Us on Twitter</a> | <a href=""http://facebook.com/twolipsdating"">Like Us on Facebook</a>
										<p><small>Twolips Dating - PO Box 780835, Orlando, FL, 32878-0835</small></p>
									</div>
								</td>
							</tr>
						</tbody>
					</table>
				</td>
			</tr>
		</table>
	</body>
</html>";

            public static string GetBody(string callbackUrl)
            {
                return String.Format(Body, callbackUrl);
            }
        }

        public static class WelcomeEmail
        {
            public const string Subject = "Welcome to twolips dating";

            private const string Body = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD HTML 4.01//EN"" ""http://www.w3.org/TR/html4/strict.dtd"">
<html>
	<head>
		<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
		<meta name=""viewport"" content=""width=device-width, minimum-scale=1.0, maximum-scale=1.0, user-scalable=0"" />
	</head>
	<body style=""margin: 0; padding: 0; background: #fff"">
		<table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""100%"" style=""background: #ddd"">
		<tbody>
			<tr>
				<td>
					<table align=""center"" cellpadding=""0"" cellspacing=""0"" border=""0"" width=""670"" style=""background: #fff; border-left: 1px solid #ccc; border-right: 1px solid #ccc;"">
						<tbody>
							<tr>
								<td style=""background: #eee; padding: 15px"">
									<table cellpadding=""0"" cellspacing=""0"" border=""0"">
										<tr>
											<td>
												<img src=""https://az732589.vo.msecnd.net/twolipsdatingcdn/twolipsicon-white-32x32.png"" width=""32"" height=""32"" />
											</td>
											<td style=""padding-left: 5px;"">
												<div style=""font-family: Helvetica,Arial,sans-serif; font-size: 20px;"">
													twolips dating
												</div>
											</td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td style=""border-top: 1px solid #ccc; padding: 15px;"">
									<div style=""font-family: Helvetica,Arial,sans-serif;"">
										<h2>Welcome!</h2>
										<p>Thank you for confirming your email address. Please connect with us on Twitter, Facebook, and Google+ to stay updated.</p>
										<a href=""http://twitter.com/twolipsdating""><img src=""https://az732589.vo.msecnd.net/twolipsdatingcdn/tw-icon-32.png"" alt=""Twitter""/></a>
										&nbsp;
										<a href=""http://facebook.com/twolipsdating""><img src=""https://az732589.vo.msecnd.net/twolipsdatingcdn/fb-icon-32.png"" alt=""Facebook""/></a>
										&nbsp;
										<a href=""http://plus.google.com/+twolipsdating""><img src=""https://az732589.vo.msecnd.net/twolipsdatingcdn/gp-icon-32.png"" alt=""Google+""/></a>
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
										</div>
									</div>
								</td>
							</tr>
							<tr>
								<td style=""background: #eee; padding: 15px; border-top: 1px solid #ccc"">
									<div style=""font-family: Helvetica,Arial,sans-serif; font-size: 12px; margin-left: 25px; width: 500px; margin-top: 7px;"">
										<a href=""https://www.twolipsdating.com/"">Home</a> | <a href=""https://www.twolipsdating.com/about/privacy"">Privacy Policy</a> | <a href=""mailto:info@twolipsdating.com"">Contact Us</a> | <a href=""http://twitter.com/twolipsdating/"">Follow Us on Twitter</a> | <a href=""http://facebook.com/twolipsdating"">Like Us on Facebook</a>
										<p><small>Twolips Dating - PO Box 780835, Orlando, FL, 32878-0835</small></p>
									</div>
								</td>
							</tr>
						</tbody>
					</table>
				</td>
			</tr>
		</table>
	</body>
</html>";

            public static string GetBody()
            {
                return String.Format(Body);
            }
        }
    }
}