using System;
using System.Collections.Generic;
using TelegramBotServer.Enum;
using TelegramBotServer.Extensions;

namespace TelegramBotServer
{
    namespace Users
    {
        [Serializable()]
        public class TelegramUser : IEquatable<TelegramUser>, ICloneable
        {
            [field: NonSerialized]
            public Telegram.Bot.Types.Message InlineButtonMessage { get; set; }
            public int IterationFlag { get; set; }
            public ProfileUser Profile { get; set; }
            public Story Story { get; set; }
            public bool IsJiraResponce { get; set; }
            public bool IsMultiProccessStart { get; set; }
            public Answer Status { get; set; }
            public List<Description> DescriptionsList { get; set; }
            public List<ExtraWorkData> ExtraWorkList { get; set; }
            public Queue<int> HardwareTicket { get; set; }
            public Queue<int> SoftwareTicket { get; set; }
            public Queue<int> CartridgeTicket { get; set; }
            public Queue<int> BigEventTicket { get; set; }
            public Queue<int> PersonTicket { get; set; }
            public long Id { get; private set; }
            public string NickName { get; set; }

            public TelegramUser(string UserName, long IdChat)
            {
                IterationFlag = -1;
                Id = IdChat;
                NickName = UserName;
                IsJiraResponce = false;
                IsMultiProccessStart = false;
                InlineButtonMessage = null;
                DescriptionsList = new List<Description>();
                ExtraWorkList = new List<ExtraWorkData>();
                HardwareTicket = new Queue<int>();
                SoftwareTicket = new Queue<int>();
                CartridgeTicket = new Queue<int>();
                BigEventTicket = new Queue<int>();
                PersonTicket = new Queue<int>();
                Story = new Story(Id);
                Profile = new ProfileUser();
                Status = Answer.None;
            }

            public void ClearCurrentTickets()
            {
                this.HardwareTicket.Clear();
                this.SoftwareTicket.Clear();
                this.CartridgeTicket.Clear();
                this.BigEventTicket.Clear();
                this.PersonTicket.Clear();
            }

            public bool Equals(TelegramUser other)
            {
                if (other != null)
                {
                    if (this.Id == other.Id)
                        return true;
                }
                return false;
            }

            public object Clone()
            {
                return new TelegramUser(this.NickName, this.Id)
                {
                    IterationFlag = this.IterationFlag,
                    IsJiraResponce = this.IsJiraResponce,
                    IsMultiProccessStart = this.IsMultiProccessStart,
                    InlineButtonMessage = this.InlineButtonMessage,
                    DescriptionsList = this.DescriptionsList.Clone<Description>() as List<Description>,
                    ExtraWorkList = this.ExtraWorkList.Clone<ExtraWorkData>() as List<ExtraWorkData>,
                    HardwareTicket = new Queue<int>(this.HardwareTicket),
                    SoftwareTicket = new Queue<int>(this.SoftwareTicket),
                    CartridgeTicket = new Queue<int>(this.CartridgeTicket),
                    BigEventTicket = new Queue<int>(this.BigEventTicket),
                    PersonTicket = new Queue<int>(this.PersonTicket),
                    Story = this.Story.Clone() as Story,
                    Profile = this.Profile.Clone() as ProfileUser,
                    Status = this.Status,
                };
            }

            public void InputInStory(DateTime date)
            {
                Story.HardwareTicketStory.Push(new WeekStoryData(date, new Queue<int>(HardwareTicket)));
                Story.SoftwareTicketStory.Push(new WeekStoryData(date, new Queue<int>(SoftwareTicket)));
                Story.CartridgeTicketStory.Push(new WeekStoryData(date, new Queue<int>(CartridgeTicket)));
                Story.BigEventTicketStory.Push(new WeekStoryData(date, new Queue<int>(BigEventTicket)));
                Story.PersonTicketStory.Push(new WeekStoryData(date, new Queue<int>(PersonTicket)));
                ClearCurrentTickets();
            }

            public bool TryChangeAdminRights(TelegramUser destAccount, bool isAdmin)
            {
                if (this.Profile != null && this.Profile.IsAdminAcc == true)
                {
                    this.Profile.TryChangeAdminRights(this, destAccount, isAdmin);
                    return true;
                }
                return false;
            }
        }
    }
}
