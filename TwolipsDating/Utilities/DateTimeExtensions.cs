using System;

namespace TwolipsDating.Utilities
{
    public static class DateTimeExtensions
    {
        public static string GetTimeUntilEnd(this DateTime dateEnd)
        {
            DateTime now = DateTime.Now;

            string format = "{0}:{1}:{2}";

            // in the past, there's no time left
            if (dateEnd < now)
            {
                return String.Format(format, "00", "00", "00");
            }

            TimeSpan timeBetweenNowAndThen = dateEnd - now;

            return String.Format(format, timeBetweenNowAndThen.Days.ToString("00"), timeBetweenNowAndThen.Hours.ToString("00"), timeBetweenNowAndThen.Minutes.ToString("00"));
        }

        public static int GetAge(this DateTime birthday)
        {
            return (int)((DateTime.Now - birthday).TotalDays / 365);
        }

        public static string GetTimeAgo(this DateTime dateReviewCreated)
        {
            string timeAgoNumber = String.Empty;
            string timeAgoSpecifier = String.Empty;

            double totalMinutesAgo = Math.Floor(DateTime.Now.Subtract(dateReviewCreated).TotalMinutes);

            int hoursInDay = 24;
            int minutesInHour = 60;
            int minutesInDay = 60 * 24;
            int minutesInWeek = minutesInDay * 7;

            if (totalMinutesAgo < 1)
            {
                timeAgoNumber = "less than 1";
                timeAgoSpecifier = "minute ago";
            }
            else if (totalMinutesAgo == 1)
            {
                timeAgoNumber = totalMinutesAgo.ToString("F0");
                timeAgoSpecifier = "minute ago";
            }
            else if (totalMinutesAgo > 1 && totalMinutesAgo < minutesInHour)
            {
                timeAgoNumber = totalMinutesAgo.ToString("F0");
                timeAgoSpecifier = "minutes ago";
            }
            else if (totalMinutesAgo == minutesInHour)
            {
                timeAgoNumber = "1";
                timeAgoSpecifier = "hour ago";
            }
            else if (totalMinutesAgo > minutesInHour && totalMinutesAgo < minutesInDay) // between 1 hour and 24 hour
            {
                timeAgoNumber = (totalMinutesAgo / minutesInHour).ToString("F0");
                timeAgoSpecifier = "hours ago";
            }
            else if (totalMinutesAgo == minutesInDay) // exactly 24 hours (1 day)
            {
                timeAgoNumber = "1";
                timeAgoSpecifier = "day ago";
            }
            else if (totalMinutesAgo > minutesInDay && totalMinutesAgo < minutesInWeek) // greater than 1 day
            {
                timeAgoNumber = (totalMinutesAgo / minutesInHour / hoursInDay).ToString("F0");
                timeAgoSpecifier = "days ago";
            }
            else if (totalMinutesAgo >= minutesInWeek)
            {
                timeAgoNumber = dateReviewCreated.ToString("m");
            }

            string timeAgoDisplay = String.Format("{0} {1}", timeAgoNumber, timeAgoSpecifier);
            return timeAgoDisplay;
        }
    }
}