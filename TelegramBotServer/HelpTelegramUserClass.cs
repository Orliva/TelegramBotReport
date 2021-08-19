using System;
using TelegramBotServer.Enum;
using System.Collections.Generic;
using TelegramBotServer.CurrentDateTimes;

namespace TelegramBotServer.Users
{
    /// <summary>
    /// Данные для таблички с переработками
    /// </summary>
    [Serializable]
    public class ExtraWorkData : ICloneable
    {
        public DateTime Date { get; set; }
        public int CountHour { get; set; }
        public string Comment { get; set; }

        public ExtraWorkData(DateTime date_, int countHour, string comment)
        {
            Date = date_;
            CountHour = countHour;
            Comment = comment;
        }

        public object Clone()
        {
            return new ExtraWorkData(this.Date, this.CountHour, this.Comment.Clone() as string);
        }
    }

    /// <summary>
    /// Заметки к заявкам по крупным мероприятиям и индивидуальным задачам
    /// </summary>
    [Serializable]
    public class Description : ICloneable
    {
        public Answer CodeBlock { get; set; }
        public string Descript { get; set; }

        public Description(Answer codeBlock, string description)
        {
            CodeBlock = codeBlock;
            Descript = description;
        }

        public object Clone() => new Description(this.CodeBlock, this.Descript.Clone() as string);
    }

    /// <summary>
    /// Недельная порция заявок по одной категории
    /// </summary>
    [Serializable]
    public struct WeekStoryData : ICloneable
    {
        public (DateTime, DateTime) Date { get; set; }
        public Queue<int> TicketStory { get; private set; }

        public WeekStoryData(DateTime date_, Queue<int> ticketsOnWeek)
        {
            Date = GetWeek(date_);
            TicketStory = ticketsOnWeek;
        }

        public object Clone()
        {
            return new WeekStoryData(DateTime.Now, new Queue<int>(this.TicketStory));
        }

        static private (DateTime, DateTime) GetWeek(DateTime date)
        {
            (int, int) week = CurrentDateTime.CurrentWeek(date);
             return (new DateTime(DateTime.Now.Year, CurrentDateTime.GetMonth(date, week).Item1, week.Item1),
                     new DateTime(DateTime.Now.Year, CurrentDateTime.GetMonth(date, week).Item2, week.Item2));
        }
    }
}
