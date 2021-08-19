using System;
using System.Collections.Generic;
using TelegramBotServer.Enum;
using System.Threading.Tasks;

namespace TelegramBotServer.Users
{
    /// <summary>
    /// История заявок
    /// </summary>
    [Serializable]
    public class Story : ICloneable
    {
        private readonly long userId;
        public Stack<WeekStoryData> HardwareTicketStory { get; set; }
        public Stack<WeekStoryData> SoftwareTicketStory { get; set; }
        public Stack<WeekStoryData> CartridgeTicketStory { get; set; }
        public Stack<WeekStoryData> BigEventTicketStory { get; set; }
        public Stack<WeekStoryData> PersonTicketStory { get; set; }

        public Story(long Id)
        {
            userId = Id;
            HardwareTicketStory = new Stack<WeekStoryData>();
            SoftwareTicketStory = new Stack<WeekStoryData>();
            CartridgeTicketStory = new Stack<WeekStoryData>();
            BigEventTicketStory = new Stack<WeekStoryData>();
            PersonTicketStory = new Stack<WeekStoryData>();
        }

        public object Clone()
        {
            Story cloneObj = new Story(this.userId)
            {
                HardwareTicketStory = new Stack<WeekStoryData>(this.HardwareTicketStory),
                SoftwareTicketStory = new Stack<WeekStoryData>(this.SoftwareTicketStory),
                CartridgeTicketStory = new Stack<WeekStoryData>(this.CartridgeTicketStory),
                BigEventTicketStory = new Stack<WeekStoryData>(this.BigEventTicketStory),
                PersonTicketStory = new Stack<WeekStoryData>(this.PersonTicketStory),
            };
            return cloneObj;
        }

        private async Task WeekTour(long chatId)
        {
            await PrintWeekTicketCategory(chatId, HardwareTicketStory.Peek(), "Hardware");
            await PrintWeekTicketCategory(chatId, SoftwareTicketStory.Peek(), "Software");
            await PrintWeekTicketCategory(chatId, CartridgeTicketStory.Peek(), "Cartridge");
            await PrintWeekTicketCategory(chatId, BigEventTicketStory.Peek(), "BigEvent");
            await PrintWeekTicketCategory(chatId, PersonTicketStory.Peek(), "Person task");
        }

        private async Task PrintWeekTicketCategory(long chatId, WeekStoryData weekTickets, string text)
        {
            await Program.SendTextAsync(chatId, text + "\nКоличество: " + weekTickets.TicketStory.Count);
            foreach (int ticket in weekTickets.TicketStory)
                await Program.SendTextAsync(chatId, Configuration.Config.BodyJiraUrl + ticket.ToString());
        }

        private async Task AllTour(long chatId)
        {
            await PrintAllTicketCategory(chatId, HardwareTicketStory, "Hardware");
            await PrintAllTicketCategory(chatId, SoftwareTicketStory, "Software");
            await PrintAllTicketCategory(chatId, CartridgeTicketStory, "Cartridge");
            await PrintAllTicketCategory(chatId, BigEventTicketStory, "BigEvent");
            await PrintAllTicketCategory(chatId, PersonTicketStory, "Person task");
        }

        private async Task PrintAllTicketCategory(long chatId, Stack<WeekStoryData> ticketsStory, string text)
        {
            long count = 0;
            foreach (WeekStoryData week in ticketsStory)
                count += week.TicketStory.Count;
            await Program.SendTextAsync(chatId, text + "\nКоличество: " + count.ToString());
        }

        private async Task ImplPrintStoryInTelegram(long chatId, TimeInterval interval)
        {
            switch (interval)
            {
                case TimeInterval.Week:
                    await WeekTour(chatId);
                    break;
                case TimeInterval.All:
                    await AllTour(chatId);
                    break;
            }
        }

        public async Task PrintStoryInTelegram(long chatId, TimeInterval interval)
        {
            await ImplPrintStoryInTelegram(chatId, interval);
        }

        public async Task PrintStoryInTelegram(TimeInterval interval)
        {
            await ImplPrintStoryInTelegram(userId, interval);
        }
    }
}
