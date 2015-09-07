using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.Utilities
{
    public enum Months
    {
        January = 1,
        February = 2,
        March = 3,
        April = 4,
        May = 5,
        June = 6,
        July = 7,
        August = 8,
        September = 9,
        October = 10,
        November = 11,
        December = 12
    }

    public class Month
    {
        public string MonthName { get; private set; }
        public int MonthNumber { get; private set; }
        public int DayCount { get; private set; }

        public Month(int month)
        {
            MonthNumber = month;
            
            SetupMonth();
        }

        public Month(Months month)
        {
            MonthNumber = (int)month;

            SetupMonth();
        }

        private void SetupMonth()
        {
            switch(MonthNumber)
            {
                case (int)Months.January:
                    DayCount = 31; MonthName = Months.January.ToString(); break;
                case (int)Months.February:
                    DayCount = 28; MonthName = Months.February.ToString(); break;
                case (int)Months.March:
                    DayCount = 31; MonthName = Months.March.ToString(); break;
                case (int)Months.April:
                    DayCount = 30; MonthName = Months.April.ToString(); break;
                case (int)Months.May:
                    DayCount = 31; MonthName = Months.May.ToString(); break;
                case (int)Months.June:
                    DayCount = 30; MonthName = Months.June.ToString(); break;
                case (int)Months.July:
                    DayCount = 31; MonthName = Months.July.ToString(); break;
                case (int)Months.August:
                    DayCount = 31; MonthName = Months.August.ToString(); break;
                case (int)Months.September:
                    DayCount = 30; MonthName = Months.September.ToString(); break;
                case (int)Months.October:
                    DayCount = 31; MonthName = Months.October.ToString(); break;
                case (int)Months.November:
                    DayCount = 31; MonthName = Months.November.ToString(); break;
                case (int)Months.December:
                    DayCount = 31; MonthName = Months.December.ToString(); break;
                default:
                    DayCount = 31; break;
            }
        }

        public static Month January
        {
            get { return new Month(Months.January); }
        }

        public static Month February
        {
            get { return new Month(Months.February); }
        }

        public static Month March
        {
            get { return new Month(Months.March); }
        }

        public static Month April
        {
            get { return new Month(Months.April); }
        }

        public static Month May
        {
            get { return new Month(Months.May); }
        }

        public static Month June
        {
            get { return new Month(Months.June); }
        }

        public static Month July
        {
            get { return new Month(Months.July); }
        }

        public static Month August
        {
            get { return new Month(Months.August); }
        }

        public static Month September
        {
            get { return new Month(Months.September); }
        }

        public static Month October
        {
            get { return new Month(Months.October); }
        }

        public static Month November
        {
            get { return new Month(Months.November); }
        }

        public static Month December
        {
            get { return new Month(Months.December); }
        }
    }

    public static class CalendarHelper
    {
        public static int GetDaysInMonth(int monthNumber)
        {
            var month = new Month(monthNumber);
            return month.DayCount;
        }

        public static IReadOnlyCollection<int> GetDaysOfMonth(Months month)
        {
            return GetDaysOfMonth((int)month);
        }

        public static IReadOnlyCollection<int> GetDaysOfMonth(int monthNumber)
        {
            var month = new Month(monthNumber);
            int dayCount = month.DayCount;

            List<int> daysInMonth = new List<int>();

            for (int i = 1; i <= dayCount; i++)
            {
                daysInMonth.Add(i);
            }

            return daysInMonth.AsReadOnly();
        }

        public static IReadOnlyCollection<Month> GetMonths()
        {
            List<Month> months = new List<Month>();
            months.Add(Month.January);
            months.Add(Month.February);
            months.Add(Month.March);
            months.Add(Month.April);
            months.Add(Month.May);
            months.Add(Month.June);
            months.Add(Month.July);
            months.Add(Month.August);
            months.Add(Month.September);
            months.Add(Month.October);
            months.Add(Month.November);
            months.Add(Month.December);
            return months.AsReadOnly();
        }

        public static IReadOnlyCollection<int> GetYears()
        {
            int minYear = DateTime.Now.AddYears(-99).Year;
            int maxYear = DateTime.Now.AddYears(-18).Year;
            List<int> years = new List<int>();
            for (int i = minYear; i <= maxYear; i++)
            {
                years.Add(i);
            }
            return years.AsReadOnly();
        }
    }
}