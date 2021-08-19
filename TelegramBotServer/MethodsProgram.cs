using System;
using TelegramBotServer.Users;
using ReportConstructorWord;
using System.Threading.Tasks;
using TelegramBotServer.Enum;
using TelegramBotServer.Configuration;
using Telegram.Bot.Types;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace TelegramBotServer
{
    partial class Program
    {
        #region CreateReport
        /// <summary>
        /// Генерирует отчет в Word
        /// </summary>
        /// <param name="user">Пользователь для которого генерируется отчет</param>
        /// <param name="path">Путь где будет сохранен отчет на сервере</param>
        /// <returns></returns>
        public static async Task<String> CreateReportAsync(TelegramUser user, string path)
        {
            if (UserList.Count < 1)
                throw new Exception("UserList.Count < 1");
            path = ValidationFilePath(path);

            using (ConstructorWord constructor = new ConstructorWord(path))
            {
                constructor.CreateHeader();
                constructor.CreatePeriod(DateTime.Now);
                constructor.CreateHardwareTasks(user);
                constructor.CreateSoftwareTasks(user);
                constructor.CreateCartridgeTasks(user);
                constructor.CreateBitEventTasks(user);
                constructor.CreatePersonTasks(user);
                constructor.CreateExtraWorkTasks(user);
                constructor.CreateFooter(user);
            }
            return path;
        }

        /// <summary>
        /// Проверяем путь и модифицируем имя файла при необходимости
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string ValidationFilePath(string path)
        {
            if (!Directory.Exists(Config.ReportFilesPath))
                Directory.CreateDirectory(Config.ReportFilesPath);
            while (System.IO.File.Exists(path))
            {
                var fileName = Path.GetFileNameWithoutExtension(path.Split(Path.DirectorySeparatorChar).Last());
                if (fileName[^1] != ')')
                    fileName += "(1)" + Config.ReportDefaultExtensionFile;
                else
                {
                    string tmpStr = "";
                    int count = fileName.Length - 2;
                    while (fileName[count] != '(')
                    {
                        tmpStr += fileName[count];
                        count--;
                    }
                    tmpStr = (int.Parse(new String(tmpStr.Reverse().ToArray())) + 1).ToString();
                    fileName = fileName.Remove(count) + $"({tmpStr})" + Config.ReportDefaultExtensionFile;
                }
                path = Config.ReportFilesPath + fileName;
            }
            return path;
        }
        #endregion

        #region ParseText
        public static async Task ParseJiraRequestAsync(TelegramUser user, string answerUser, Answer status)
        {
            string[] str = answerUser.Split(Config.separator, StringSplitOptions.RemoveEmptyEntries);
            foreach (string str_ in str)
            {
                if (int.TryParse(str_, out int ticket))
                {
                    switch (status)
                    {
                        case Answer.Hardware:
                            user.HardwareTicket.Enqueue(ticket);
                            break;
                        case Answer.Software:
                            user.SoftwareTicket.Enqueue(ticket);
                            break;
                        case Answer.Cartridge:
                            user.CartridgeTicket.Enqueue(ticket);
                            break;
                        case Answer.BigEvent:
                            user.BigEventTicket.Enqueue(ticket);
                            break;
                        case Answer.PersonTasks:
                            user.PersonTicket.Enqueue(ticket);
                            break;
                        case Answer.ExtraWork:
                            throw new Exception("Exception of arg: Answer.ExtraWork!");
                        case Answer.EndReport:
                            throw new Exception("Exception of arg: Answer.EndReport!");
                        case Answer.None:
                            throw new Exception("Exception of arg: Answer.None!");
                        case Answer.EditProfile:
                            throw new Exception("Exception of arg: Answer.EditProfile");
                    }
                }
                else
                    await SendTextAsync(user.Id, str_ + Config.NotFoundTicketStr);
            }
        }
        #endregion

        /// <summary>
        /// Отправляем документ
        /// </summary>
        /// <param name="message"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static async Task<Message> SendFile(Message message, string filePath)
        {
            await bot.SendChatActionAsync(message.Chat.Id, ChatAction.UploadDocument);

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var fileName = filePath.Split(Path.DirectorySeparatorChar).Last();
                return await bot.SendDocumentAsync(chatId: message.Chat.Id,
                                                   document: new InputOnlineFile(fileStream, fileName),
                                                   caption: "Этот отчет можно найти на Вашем сервере");
            }
        }

        #region SendText
        /// <summary>
        /// Вывод цифровых команд от 1 до 7
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static async Task<Message> RuleSendAsync(Message message) =>
            await bot.SendTextMessageAsync(chatId: message.Chat.Id, text: Config.RuleMessageStr);

        public static async Task SendTextAsync(long id, string text) =>
            await bot.SendTextMessageAsync(chatId: id, text: text);

        public static async Task<Message> SendTextAsync(Message message, string text) =>
            await bot.SendTextMessageAsync(chatId: message.Chat.Id, text: text);

        /// <summary>
        /// Отправляем Inline кнопку к сообщению
        /// </summary>
        /// <param name="message"></param>
        /// <param name="text"></param>
        /// <param name="keyboard"></param>
        /// <returns></returns>
        public static async Task<Message> SendTextAsync(Message message, string text, InlineKeyboardMarkup keyboard)
        {
            await bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
            return await bot.SendTextMessageAsync(chatId: message.Chat.Id,
                                                  text: text,
                                                  replyMarkup: keyboard);
        }
        #endregion

        #region ExtraWorkImp
        private static void GetExtraWorkComment(TelegramUser user, string comment)
        {
            if (comment != null)
            {
                if (comment.Length > 30)
                {
                    string[] words = comment.Split(' ');
                    int tmp = 0;
                    for (int i = 0; i < words.Length; i++)
                    {
                        tmp += words[i].Length;
                        if (tmp < 30)
                            user.ExtraWorkList[^1].Comment += words[i] + ' ';
                        else
                        {
                            tmp = words[i].Length;
                            user.ExtraWorkList[^1].Comment += '\n' + words[i] + ' ';
                        }
                    }
                }
                else
                    user.ExtraWorkList[^1].Comment = comment;
            }
            else
                user.ExtraWorkList[^1].Comment = string.Empty;
        }

        private static void GetExtraWorkCountHour(TelegramUser user, string countHourString)
        {
            if (int.TryParse(countHourString, out int hour))
                user.ExtraWorkList[^1].CountHour = hour;
            else
                user.ExtraWorkList[^1].CountHour = hour;
        }

        private static void GetExtraWorkDate(TelegramUser user, string dateString)
        {
            if (DateTime.TryParse(dateString, out DateTime date))
                user.ExtraWorkList[^1].Date = date;
            else
                user.ExtraWorkList[^1].Date = date;
        }
        #endregion
        //#region SendText
        ///// <summary>
        ///// Вывод цифровых команд от 1 до 7
        ///// </summary>
        ///// <param name="message"></param>
        ///// <returns></returns>
        //public static async Task<Message> RuleSendAsync(Message message) =>
        //    await bot.SendTextMessageAsync(chatId: message.Chat.Id, text: Config.RuleMessageStr);

        //public static async Task SendTextAsync(long id, string text) =>
        //    await bot.SendTextMessageAsync(chatId: id, text: text);

        //public static async Task<Message> SendTextAsync(Message message, string text) =>
        //    await bot.SendTextMessageAsync(chatId: message.Chat.Id, text: text);

        ///// <summary>
        ///// Отправляем Inline кнопку к сообщению
        ///// </summary>
        ///// <param name="message"></param>
        ///// <param name="text"></param>
        ///// <param name="keyboard"></param>
        ///// <returns></returns>
        //public static async Task<Message> SendText(Message message, string text, InlineKeyboardMarkup keyboard)
        //{
        //    await bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
        //    return await bot.SendTextMessageAsync(chatId: message.Chat.Id,
        //                                          text: text,
        //                                          replyMarkup: keyboard);
        //}
        //#endregion

        //#region Serializer

        ///// <summary>
        ///// Бинарная сериализация
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="objectToSerialize"></param>
        ///// <param name="formatter"></param>
        ///// <param name="path"></param>
        ///// <returns></returns>
        //public static async Task SerializedAsync<T>(T objectToSerialize, BinaryFormatter formatter, string path)
        //{
        //    // получаем поток, куда будем записывать сериализованный объект
        //    using (FileStream fs = System.IO.File.Create(path))
        //    {
        //        formatter.Serialize(fs, objectToSerialize);
        //        Console.WriteLine($"Объект:{objectToSerialize} сериализован");
        //    }
        //}

        ///// <summary>
        ///// Бинарная десериализация
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="objectToSerialize"></param>
        ///// <param name="formatter"></param>
        ///// <param name="path"></param>
        ///// <returns></returns>
        //public static async Task DeserializedAsync<T>(T objectToSerialize, BinaryFormatter formatter, string path)
        //{
        //    // десериализация из файла
        //    using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
        //    {
        //        UserList = (List<TelegramUser>)formatter.Deserialize(fs);
        //        Console.WriteLine($"Объект:{objectToSerialize} десериализован");
        //    }
        //}
        //#endregion

        //#region ParseText
        //public static async Task ParseJiraRequestAsync(TelegramUser user, string answerUser, Answer status)
        //{
        //    string[] str = answerUser.Split(Config.separator, StringSplitOptions.RemoveEmptyEntries);
        //    foreach (string str_ in str)
        //    {
        //        if (int.TryParse(str_, out int ticket))
        //        {
        //            switch (status)
        //            {
        //                case Answer.Hardware:
        //                    user.HardwareTicket.Enqueue(ticket);
        //                    break;
        //                case Answer.Software:
        //                    user.SoftwareTicket.Enqueue(ticket);
        //                    break;
        //                case Answer.Cartridge:
        //                    user.CartridgeTicket.Enqueue(ticket);
        //                    break;
        //                case Answer.BigEvent:
        //                    user.BigEventTicket.Enqueue(ticket);
        //                    break;
        //                case Answer.PersonTasks:
        //                    user.PersonTicket.Enqueue(ticket);
        //                    break;
        //                case Answer.ExtraWork:
        //                    throw new Exception("Exception of arg: Answer.ExtraWork!");
        //                case Answer.EndReport:
        //                    throw new Exception("Exception of arg: Answer.EndReport!");
        //                case Answer.None:
        //                    throw new Exception("Exception of arg: Answer.None!");
        //                case Answer.EditProfile:
        //                    throw new Exception("Exception of arg: Answer.EditProfile");
        //            }
        //        }
        //        else
        //            await SendTextAsync(user.Id, str_ + Config.NotFoundTicketStr);
        //    }
        //}
        //#endregion

        //#region CreateReport
        ///// <summary>
        ///// Генерирует отчет в Word
        ///// </summary>
        ///// <param name="user">Пользователь для которого генерируется отчет</param>
        ///// <param name="path">Путь где будет сохранен отчет на сервере</param>
        ///// <returns></returns>
        //public static async Task<String> CreateReportAsync(TelegramUser user, string path)
        //{
        //    if (UserList.Count < 1)
        //        throw new Exception("UserList.Count < 1");
        //    path = ValidationFilePath(path);

        //    using (ConstructorWord constructor = new ConstructorWord(path))
        //    {
        //        constructor.CreateHeader();
        //        constructor.CreatePeriod(DateTime.Now);
        //        constructor.CreateHardwareTasks(user);
        //        constructor.CreateSoftwareTasks(user);
        //        constructor.CreateCartridgeTasks(user);
        //        constructor.CreateBitEventTasks(user);
        //        constructor.CreatePersonTasks(user);
        //        constructor.CreateExtraWorkTasks(user);
        //        constructor.CreateFooter(user);
        //    }
        //    return path;
        //}

        ///// <summary>
        ///// Проверяем путь и модифицируем имя файла при необходимости
        ///// </summary>
        ///// <param name="path"></param>
        ///// <returns></returns>
        //private static string ValidationFilePath(string path)
        //{
        //    if (!Directory.Exists(Config.ReportFilesPath))
        //        Directory.CreateDirectory(Config.ReportFilesPath);
        //    while (System.IO.File.Exists(path))
        //    {
        //        var fileName = Path.GetFileNameWithoutExtension(path.Split(Path.DirectorySeparatorChar).Last());
        //        if (fileName[^1] != ')')
        //            fileName += "(1)" + Config.ReportDefaultExtensionFile;
        //        else
        //        {
        //            string tmpStr = "";
        //            int count = fileName.Length - 2;
        //            while (fileName[count] != '(')
        //            {
        //                tmpStr += fileName[count];
        //                count--;
        //            }
        //            tmpStr = (int.Parse(new String(tmpStr.Reverse().ToArray())) + 1).ToString();
        //            fileName = fileName.Remove(count) + $"({tmpStr})" + Config.ReportDefaultExtensionFile;
        //        }
        //        path = Config.ReportFilesPath + fileName;
        //    }
        //    return path;
        //}
        //#endregion

        //#region ExtraWorkImp
        //private static void GetExtraWorkComment(TelegramUser user, string comment)
        //{
        //    if (comment != null)
        //    {
        //        if (comment.Length > 30)
        //        {
        //            string[] words = comment.Split(' ');
        //            int tmp = 0;
        //            for (int i = 0; i < words.Length; i++)
        //            {
        //                tmp += words[i].Length;
        //                if (tmp < 30)
        //                    user.ExtraWorkList[^1].Comment += words[i] + ' ';
        //                else
        //                {
        //                    tmp = words[i].Length;
        //                    user.ExtraWorkList[^1].Comment += '\n' + words[i] + ' ';
        //                }
        //            }
        //        }
        //        else
        //            user.ExtraWorkList[^1].Comment = comment;
        //    }
        //    else
        //        user.ExtraWorkList[^1].Comment = string.Empty;
        //}

        //private static void GetExtraWorkCountHour(TelegramUser user, string countHourString)
        //{
        //    if (int.TryParse(countHourString, out int hour))
        //        user.ExtraWorkList[^1].CountHour = hour;
        //    else
        //        user.ExtraWorkList[^1].CountHour = hour;
        //}

        //private static void GetExtraWorkDate(TelegramUser user, string dateString)
        //{
        //    if (DateTime.TryParse(dateString, out DateTime date))
        //        user.ExtraWorkList[^1].Date = date;
        //    else
        //        user.ExtraWorkList[^1].Date = date;
        //}
        //#endregion
    }
}
