using System;
using TelegramBotServer.Users;
using TelegramBotServer.Keyboard;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBotServer.Enum;
using TelegramBotServer.Configuration;
using ReportConstructorWord;
using System.IO;
using System.Linq;
using TelegramBotServer.CurrentDateTimes;

namespace TelegramBotServer
{
    partial class Program
    {

        #region ImpNumericCommand

        /// <summary>
        /// Реализуем поведение, необходимое для выполнения цифровых команд от 1 до 7
        /// </summary>
        /// <param name="message"></param>
        /// <param name="user"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static async Task<Message> SwitcherAsync(Message message, TelegramUser user)
        {
            CustomKeyboard kb;

            switch (user.Status)
            {
                case Answer.Hardware:
                case Answer.Software:
                case Answer.Cartridge:
                    await ParseJiraRequestAsync(user, message.Text, user.Status);
                    break;
                case Answer.BigEvent:
                case Answer.PersonTasks:
                    if (user.IsMultiProccessStart)
                    {
                        if (user.IsJiraResponce)
                        {
                            Task task = new Task(async () => await ParseJiraRequestAsync(user, message.Text, user.Status));
                            Task task2 = task.ContinueWith(async (Task t) =>
                            {
                                lock (locker)
                                    kb = MyKeyboard;
                                user.InlineButtonMessage = await SendTextAsync(message: message,
                                                                                    text: Config.PushYesOrInputTextStr,
                                                                                    keyboard: kb.DescrKeyboard);
                                user.IsJiraResponce = false;
                            });
                            task.Start();
                            task2.Wait();
                            return message;
                        }
                        else
                        {
                            try
                            {
                                await bot.EditMessageReplyMarkupAsync(user.Id, user.InlineButtonMessage.MessageId);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            user.DescriptionsList.Add(new Description(user.Status, message.Text));
                            user.IsMultiProccessStart = false;
                        }
                    }
                    else
                    {
                        await bot.EditMessageReplyMarkupAsync(user.Id, user.InlineButtonMessage.MessageId);
                        await SendTextAsync(message, "Ошибка! Необходимо нажать на кнопку!");
                    }
                    break;
                case Answer.ExtraWork:
                    switch (user.IterationFlag)
                    {
                        case 0:
                            GetExtraWorkDate(user, message.Text);
                            await SendTextAsync(message, Config.InputCountHourExtraWorkStr);
                            user.IterationFlag++;
                            return message;
                        case 1:
                            GetExtraWorkCountHour(user, message.Text);
                            await SendTextAsync(message, Config.InputCommentStr);
                            user.IterationFlag++;
                            return message;
                        case 2:
                            GetExtraWorkComment(user, message.Text);
                            user.IterationFlag = -1;
                            user.Status = Answer.None;
                            break;
                        default:
                            user.IterationFlag = -1;
                            user.Status = Answer.None;
                            throw new Exception("Invalid value: flagExtraWork");
                    }
                    break;
                case Answer.EndReport:
                    await SendTextAsync(message, "Ваш отчет:");
                    (int, int) weekDay = CurrentDateTime.CurrentWeek(DateTime.Now);
                    (int, int) weekMonth = CurrentDateTime.GetMonth(DateTime.Now, weekDay);

                    await SendFile(message,
                             await CreateReportAsync(user, Path.Combine(Config.ReportFilesPath,
                                                                    Config.StartFileName +
                                                                    (user.NickName ?? message.From.Username) +
                                                                    weekDay.Item1 + "_" + weekMonth.Item1 + "-" + weekDay.Item2 + "_" +
                                                                    weekMonth.Item2.ToString() + Config.ReportDefaultExtensionFile)));
                    user.InputInStory(DateTime.Now);
                    break;
            }
            return await EndTaskAsync(message, user);
        }

        /// <summary>
        /// Готовим экземпляр пользователя к выполнению цифровых команд от 1 до 7
        /// </summary>
        /// <param name="message"></param>
        /// <param name="user"></param>
        /// <param name="status"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static async Task<Message> BeforeSwitcherAsync(Message message, TelegramUser user, Answer status)
        {
            CustomKeyboard kb;
            lock (locker)
                swithcerMode = true;
            user.Status = status;

            switch (status)
            {
                case Answer.Hardware:
                case Answer.Software:
                case Answer.Cartridge:
                    return await SendTextAsync(message: message,
                                           text: "Введите номера заявок");
                case Answer.BigEvent:
                case Answer.PersonTasks:
                    lock (locker)
                        kb = MyKeyboard;
                    return user.InlineButtonMessage = await SendTextAsync(message: message,
                                                    text: Config.IsTicketExistStr,
                                                    keyboard: kb.JiraRespKeyboard);
                case Answer.ExtraWork:
                    user.ExtraWorkList.Add(new ExtraWorkData(DateTime.Now, 0, string.Empty));
                    user.IterationFlag++;
                    return await SendTextAsync(message: message,
                                           text: Config.DateExtraWorkStr);
                case Answer.EndReport:
                    return await SwitcherAsync(message, user);
                default:
                    return message;
            }
        }
        #endregion

        #region ImpCommand

        /// <summary>
        /// Реализуем команду вывода заявок для ВСЕХ пользователей
        /// </summary>
        /// <param name="message"></param>
        /// <param name="SrcUser"></param>
        /// <param name="index"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        private static async Task<Message> GetAllStatAsync(Message message, TelegramUser SrcUser, TimeInterval interval)
        {
            if (SrcUser.Profile.IsAdminAcc)
            {
                switch (interval)
                {
                    case TimeInterval.Week:
                        foreach (TelegramUser user in UserList)
                        {
                            await SendTextAsync(SrcUser.Id,
                                user.Profile.GetFullName() + ":");
                            await user.Story.PrintStoryInTelegram(SrcUser.Id,
                                TimeInterval.Week);
                        }
                        break;
                    case TimeInterval.All:
                        foreach (TelegramUser user in UserList)
                        {
                            await SendTextAsync(SrcUser.Id,
                                user.Profile.GetFullName() + ":");
                            await user.Story.PrintStoryInTelegram(SrcUser.Id,
                                TimeInterval.All);
                        }
                        break;
                    default:
                        throw new Exception("TimeInterval isn't valid!");
                }
            }
            else
            {
                await SendTextAsync(message, "Только УЗ администратора может просматривать всех пользователей");
            }
            return message;
        }

        /// <summary>
        /// Реализуем вывод заявок для КОНКРЕТНОГО пользователя
        /// </summary>
        /// <param name="message"></param>
        /// <param name="user"></param>
        /// <param name="index"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        private static async Task<Message> GetStatUserAsync(Message message, TelegramUser user, TimeInterval interval)
        {
            try
            {
                switch (interval)
                {
                    case TimeInterval.Week:
                        await user.Story.PrintStoryInTelegram(TimeInterval.Week);
                        break;
                    case TimeInterval.All:
                        await user.Story.PrintStoryInTelegram(TimeInterval.All);
                        break;
                    default:
                        throw new Exception("TimeInterval isn't valid!");
                }
            }
            catch
            {
                await SendTextAsync(message, "Сейчас у Вас нет выполненных заявок и/или по ним не составлен отчет!");
            }
            return message;
        }

        /// <summary>
        /// Изменить данные о профиле пользователя
        /// </summary>
        /// <param name="message"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private static async Task<Message> EditProfileUserAsync(Message message, TelegramUser user)
        {
            user.Status = Answer.EditProfile;
            switch (user.IterationFlag)
            {
                case -1:
                    await SendTextAsync(message, "Введите Вашу фамилию (Пример: Иванов):");
                    user.IterationFlag++;
                    return message;
                case 0:
                    user.Profile.FamilyName = message.Text;
                    user.IterationFlag++;
                    await SendTextAsync(message, "Введите Ваше имя сокращенно (Пример: И.):");
                    return message;
                case 1:
                    user.Profile.FirstName = message.Text;
                    user.IterationFlag++;
                    await SendTextAsync(message, "Введите Ваше отчество сокращенно (Пример: И.):");
                    return message;
                case 2:
                    user.Profile.SecondName = message.Text;
                    user.IterationFlag++;
                    await SendTextAsync(message, "Введите Вашу занимаемую должность:");
                    return message;
                case 3:
                    user.Profile.UserPost = message.Text;
                    user.IterationFlag = -1;
                    break;
                default:
                    user.IterationFlag = -1;
                    break;
            }
            return await EndTaskAsync(message, user);
        }

        /// <summary>
        /// Полностью очистить профиль пользователя, включая права администратора
        /// </summary>
        /// <param name="message"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private static async Task<Message> ClearProfileAsync(Message message, TelegramUser user)
        {
            user.Profile = new ProfileUser();
            return await EndTaskAsync(message, user);
        }

        /// <summary>
        /// Пробуем изменить права одного пользователя другим пользователем
        /// </summary>
        /// <param name="message"></param>
        /// <param name="index"></param>
        /// <param name="isAdmin"></param>
        /// <returns></returns>
        private static async Task<Message> TryChangeAdminRightsAsync(Message message, int index, bool isAdmin)
        {
            string familyNameUser = message.Text.Split(' ').Last();
            if (familyNameUser == "Non")
                return message;
            lock (locker)
            {
                foreach (TelegramUser user in UserList)
                {
                    if (user.Profile.FamilyName == familyNameUser)
                    {
                        if (UserList[index].TryChangeAdminRights(destAccount: user, isAdmin: isAdmin))
                            SendTextAsync(message, "Успешно!");
                        else
                            SendTextAsync(message, "Не удалось выдать права!");
                        return message;
                    }
                }
            }
            await SendTextAsync(message, "Не найден пользователь с такой фамилией");
            return message;
        }
        #endregion

        private static async Task<Message> EndTaskAsync(Message message, TelegramUser user)
        {
            lock (locker)
                swithcerMode = false;
            user.Status = Answer.None;
            await SendTextAsync(message, "Готово");
            await RuleSendAsync(message);
            return message;
        }
    }
}
