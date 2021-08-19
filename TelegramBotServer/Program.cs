using System;
using System.Collections.Generic;
using Telegram.Bot;
using TelegramBotServer.Users;
using TelegramBotServer.Keyboard;
using FileSys = System.IO.File;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Exceptions;
using TelegramBotServer.Enum;
using TelegramBotServer.Configuration;
using System.IO;
using System.Linq;


using System.Runtime.Serialization.Formatters.Binary;

namespace TelegramBotServer
{
    /// <summary>
    /// Класс состоит из трех частей
    /// Разнести по классам
    /// </summary>
    partial class Program
    {
        private static bool swithcerMode = false;
        private static readonly object locker = new object();
        private static List<TelegramUser> UserList = new List<TelegramUser>();
        private static TelegramBotClient bot;
        private static readonly CustomKeyboard MyKeyboard = new CustomKeyboard();

        public static async Task Main(string[] args)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            string path = Path.Combine(Environment.CurrentDirectory, "UserList.dat");
            string token = FileSys.ReadAllText(Config.TokenPath);
            bot = new TelegramBotClient(token);
            var cts = new CancellationTokenSource();

            var me = await bot.GetMeAsync();
            Console.Title = me.Username;
            await DeserializedAsync(UserList, formatter, path);

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            bot.StartReceiving(new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync), cts.Token);

            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            await SerializedAsync(UserList, formatter, path);

            // Send cancellation request to stop bot
            cts.Cancel();
        }

        #region Serializer

        /// <summary>
        /// Бинарная сериализация
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToSerialize"></param>
        /// <param name="formatter"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async Task SerializedAsync<T>(T objectToSerialize, BinaryFormatter formatter, string path)
        {
            try
            {
                // получаем поток, куда будем записывать сериализованный объект
                using (FileStream fs = System.IO.File.Create(path))
                {
                    formatter.Serialize(fs, objectToSerialize);
                    Console.WriteLine($"Объект:{objectToSerialize} сериализован");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Бинарная десериализация
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToSerialize"></param>
        /// <param name="formatter"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async Task DeserializedAsync<T>(T objectToSerialize, BinaryFormatter formatter, string path)
        {
            try
            {
                // десериализация из файла
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    UserList = (List<TelegramUser>)formatter.Deserialize(fs);
                    Console.WriteLine($"Объект:{objectToSerialize} десериализован");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        /// <summary>
        /// Определяем тип полученного сообщения
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="update"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var handler = update.Type switch
            {
                // UpdateType.Unknown:
                // UpdateType.ChannelPost:
                // UpdateType.EditedChannelPost:
                // UpdateType.ShippingQuery:
                // UpdateType.PreCheckoutQuery:
                // UpdateType.Poll:
                // UpdateType.EditedMessage:
                // UpdateType.InlineQuery:
                // UpdateType.ChosenInlineResult:
                UpdateType.Message => BotOnMessageReceived(update.Message),
                UpdateType.CallbackQuery => BotOnCallbackQueryReceived(update.CallbackQuery),
                _ => UnknownUpdateHandlerAsync(update)
            };

            try
            {
                await handler;
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(botClient, exception, cancellationToken);
            }
        }

        #region ImpBotMethods
        /// <summary>
        /// Реакция на нажатие Inline кнопки
        /// </summary>
        /// <param name="callbackQuery"></param>
        /// <returns></returns>
        private static async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery)
        {
            TelegramUser user = new TelegramUser(callbackQuery.Message.Chat.FirstName, callbackQuery.Message.Chat.Id);
            int index;

            lock (locker)
            {
                index = UserList.IndexOf(user);
                user = UserList[index].Clone() as TelegramUser;
            }

            switch (callbackQuery.Data)
            {
                case "_YesJiraResp":
                    user.IsMultiProccessStart = true;
                    user.IsJiraResponce = true;
                    await SendTextAsync(callbackQuery.Message, Config.InputTicketNumStr);
                    break;
                case "_NoJiraResp":
                    user.IsMultiProccessStart = true;
                    user.IsJiraResponce = false;
                    await SendTextAsync(callbackQuery.Message, Config.InputDescriptionStr);
                    break;
                case "_YesDescription":
                    await SendTextAsync(callbackQuery.Message, Config.InputDescriptionStr);
                    break;
                case "_NoDescription":
                    user.IsMultiProccessStart = false;
                    await EndTaskAsync(callbackQuery.Message, user);
                    break;

            }
            await bot.EditMessageReplyMarkupAsync(callbackQuery.From.Id, callbackQuery.Message.MessageId);
            lock (locker)
                UserList[index] = user;
        }

        /// <summary>
        /// Обработка текстовых сообщений
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private static async Task BotOnMessageReceived(Message message)
        {
            TelegramUser person = new TelegramUser(message.Chat.FirstName, message.Chat.Id);
            int index;
            bool switchMode;

            lock (locker)
            {
                switchMode = swithcerMode;
                if (!UserList.Contains(person))
                {
                    UserList.Add(person);
                    if (UserList.Count == 1)
                        UserList[0].Profile = new ProfileUser("", "", "", "", true);
                }
                index = UserList.IndexOf(person);
                person = UserList[index].Clone() as TelegramUser;
            }

            Console.WriteLine($"Receive message type: {message.Type}");
            if (message.Type != MessageType.Text)
                return;

            Task<Message> action = null;

            if (person.Status == Answer.EditProfile)
            {
                await EditProfileUserAsync(message, person);
                lock (locker)
                    UserList[index] = person;
                return;
            }
            if (switchMode)
            {
                await SwitcherAsync(message, person);
                lock (locker)
                    UserList[index] = person;
                return;
            }
            action = (message.Text.ToLower().Split(Config.separator).First()) switch
            {
                "/start" => Start(message, person),
                "/help" => Help(message),
                "/myweekstat" => GetStatUserAsync(message, person, TimeInterval.Week),
                "/myallstat" => GetStatUserAsync(message, person, TimeInterval.All),
                "/allweekstat" => GetAllStatAsync(message, person, TimeInterval.Week),
                "/allstat" => GetAllStatAsync(message, person, TimeInterval.All),
                "/editconfig" => EditProfileUserAsync(message, person),
                "/clearconfig" => ClearProfileAsync(message, person),
                "/makeadminrights" => TryChangeAdminRightsAsync(message, index, true),
                "/removeadminrights" => TryChangeAdminRightsAsync(message, index, false),

                "1" => BeforeSwitcherAsync(message, person, Answer.Hardware),
                "2" => BeforeSwitcherAsync(message, person, Answer.Software),
                "3" => BeforeSwitcherAsync(message, person, Answer.Cartridge),
                "4" => BeforeSwitcherAsync(message, person, Answer.BigEvent),
                "5" => BeforeSwitcherAsync(message, person, Answer.PersonTasks),
                "6" => BeforeSwitcherAsync(message, person, Answer.ExtraWork),
                "7" => BeforeSwitcherAsync(message, person, Answer.EndReport),
                _ => Help(message)
            };

            var sentMessage = await action;
            Console.WriteLine($"The message was sent with id: {sentMessage.MessageId}");

            lock (locker)
                UserList[index] = person;



            static async Task<Message> Help(Message message)
            {
                await Usage(message);
                await SendTextAsync(id: message.Chat.Id,
                                    text: "Для использования бота напишите в чат цифру:\n");
                await RuleSendAsync(message);
                return message;
            }

            static async Task<Message> Start(Message message, TelegramUser user)
            {
                    user.Status = Answer.None;
                return await EditProfileUserAsync(message, user);
            }

            static async Task<Message> Usage(Message message)
            {
                const string usage = "Использование:\n" +
                                     "/start   - Ввести/исправить данные о себе и начать работу с ботом\n" +
                                     "/help - Вывести полную справку о возможностях\n" +
                                     "/myweekstat   - Вывод моих заявок за неделю\n" +
                                     "/myallstat    - Вывод моих заявок за все время\n" +
                                     "/allweekstat - Вывод заявок всех пользователей за неделю (только админ.)\n" +
                                     "/allstat - Вывод заявок всех пользователей за все время (только админ.)\n" +
                                     "/editconfig - Переделать данные о себе\n" +
                                     "/clearconfig - Полностью очистить данные о себе (включая имеющиеся права администратора)\n" +
                                     "/makeadminrights - Выдать права администратора. Требуется указать фамилию после команды (только админ.)\n" +
                                     "/removeadminrights - Удалить права администратора. Требуется указать фамилию после команды (только админ.)";

                return await SendTextAsync(message: message,
                                      text: usage);
            }
        }

        /// <summary>
        /// Обработка неизвестных видов сообщений
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        private static Task UnknownUpdateHandlerAsync(Update update)
        {
            Console.WriteLine($"Unknown update type: {update.Type}");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Обрабатываем ошибки пойманные на этапе прослушивания сообщений
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="exception"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
        #endregion
    }
}