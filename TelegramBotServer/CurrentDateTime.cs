using System;
using System.Collections.Generic;
using System.Text;

namespace TelegramBotServer.CurrentDateTimes
{
    static class CurrentDateTime
    {
        public static (int, int) GetMonth(DateTime date, (int, int) weekStr)
        {
            int month1, month2;
            if (weekStr.Item1 < weekStr.Item2)
            {
                month1 = date.Month;
                month2 = date.Month;
            }
            else if (weekStr.Item1 > weekStr.Item2)
            {
                if (date.Month - 1 > 0)
                    month1 = date.Month - 1;
                else
                    month1 = 12;
                month2 = date.Month;
            }
            else
            {
                month1 = date.Month;
                if (date.Month + 1 < 13)
                    month2 = date.Month + 1;
                else
                    month2 = 1;
            }
            return (month1, month2);
        }

        public static (int, int) CurrentWeek(DateTime date)
        {
            DayOfWeek today = DateTime.Now.DayOfWeek;
            int dayOfMonth = date.Day;
            int month = date.Month;
            int offsetStartWeek = 0;
            int offsetEndWeek = 0;
            bool isOneMonth = true;
            int dayInCurrentMonth = GetDayInMonth(month);
            int tmpMonth = 0;
            (int, int) curWeekDate;

            switch (today)
            {
                case DayOfWeek.Monday:
                    offsetEndWeek = 6;
                    break;
                case DayOfWeek.Tuesday:
                    offsetStartWeek = 1;
                    offsetEndWeek = 5;
                    break;
                case DayOfWeek.Wednesday:
                    offsetStartWeek = 2;
                    offsetEndWeek = 4;
                    break;
                case DayOfWeek.Thursday:
                    offsetStartWeek = 3;
                    offsetEndWeek = 3;
                    break;
                case DayOfWeek.Friday:
                    offsetStartWeek = 4;
                    offsetEndWeek = 2;
                    break;
                case DayOfWeek.Saturday:
                    offsetStartWeek = 5;
                    offsetEndWeek = 1;
                    break;
                case DayOfWeek.Sunday:
                    offsetStartWeek = 6;
                    break;
            }

            if (dayOfMonth - offsetStartWeek < 1)
            {
                if (month - 1 < 1)
                    tmpMonth = 12;
                else
                    tmpMonth = month - 1;
                isOneMonth = false;
            }
            else if (dayOfMonth + offsetEndWeek > dayInCurrentMonth)
            {
                if (month + 1 > 12)
                    tmpMonth = 1;
                else
                    tmpMonth = month + 1;
                isOneMonth = false;
            }

            if (isOneMonth)
            {
                curWeekDate.Item1 = (DateTime.Now.Day - offsetStartWeek);
                curWeekDate.Item2 = (DateTime.Now.Day + offsetEndWeek);
            }
            else
            {
                if (dayOfMonth - offsetStartWeek < 1)
                {
                    curWeekDate.Item1 = (GetDayInMonth(tmpMonth) + (dayOfMonth - offsetStartWeek));
                    curWeekDate.Item2 = (dayInCurrentMonth + offsetEndWeek);
                }
                else
                {
                    curWeekDate.Item1 = (dayInCurrentMonth - offsetStartWeek);
                    curWeekDate.Item2 = (GetDayInMonth(tmpMonth) + (dayOfMonth + offsetEndWeek));
                }
            }
            return curWeekDate;
        }

        static private int GetDayInMonth(int month)
        {
            if (month == 1 || month == 3 || month == 5 || month == 7 ||
                month == 8 || month == 10 || month == 12)
                return 31;
            else if (month == 4 || month == 6 || month == 9 || month == 11)
                return 30;
            if (DateTime.Now.Year % 4 != 0)
                return 28;
            else
            {
                if (DateTime.Now.Year % 100 == 0)
                {
                    if (DateTime.Now.Year % 400 == 0)
                        return 29;
                    else
                        return 28;
                }
                else
                    return 29;
            }
        }
    }
}
