using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotServer
{
    namespace Keyboard
    {
        class CustomKeyboard : IReplyMarkup
        {
            private List<InlineKeyboardButton> descrKBList;
            private List<InlineKeyboardButton> jiraRespKBList;
            public InlineKeyboardMarkup DescrKeyboard { get; private set; }
            public InlineKeyboardMarkup JiraRespKeyboard { get; private set; }
            public InlineKeyboardButton YesInlineDescrButton { get; private set; }
            public InlineKeyboardButton NoInlineDescrButton { get; private set; }
            public InlineKeyboardButton YesInlineJiraRespButton { get; private set; }
            public InlineKeyboardButton NoInlineJiraRespButton { get; private set; }

            public CustomKeyboard()
            {

                YesInlineDescrButton = new InlineKeyboardButton();
                YesInlineDescrButton.CallbackData = "_YesDescription";
                YesInlineDescrButton.Text = "Да";

                NoInlineDescrButton = new InlineKeyboardButton();
                NoInlineDescrButton.CallbackData = "_NoDescription";
                NoInlineDescrButton.Text = "Нет";

                descrKBList = new List<InlineKeyboardButton>(2);
                descrKBList.Add(YesInlineDescrButton);
                descrKBList.Add(NoInlineDescrButton);

                DescrKeyboard = new InlineKeyboardMarkup(descrKBList);


                YesInlineJiraRespButton = new InlineKeyboardButton();
                YesInlineJiraRespButton.CallbackData = "_YesJiraResp";
                YesInlineJiraRespButton.Text = "Да";

                NoInlineJiraRespButton = new InlineKeyboardButton();
                NoInlineJiraRespButton.CallbackData = "_NoJiraResp";
                NoInlineJiraRespButton.Text = "Нет";

                jiraRespKBList = new List<InlineKeyboardButton>(2);
                jiraRespKBList.Add(YesInlineJiraRespButton);
                jiraRespKBList.Add(NoInlineJiraRespButton);

                JiraRespKeyboard = new InlineKeyboardMarkup(jiraRespKBList);
            }

            public CustomKeyboard(string callBackDataYesDescrB, string callBackDataNoDescrB,
                string callBackDataYesJiraRespB, string callBackDataNoJiraRespB)
            {

                YesInlineDescrButton = new InlineKeyboardButton();
                YesInlineDescrButton.CallbackData = callBackDataYesDescrB;
                YesInlineDescrButton.Text = "Да";

                NoInlineDescrButton = new InlineKeyboardButton();
                NoInlineDescrButton.CallbackData = callBackDataNoDescrB;
                NoInlineDescrButton.Text = "Нет";

                descrKBList = new List<InlineKeyboardButton>(2);
                descrKBList.Add(YesInlineDescrButton);
                descrKBList.Add(NoInlineDescrButton);

                DescrKeyboard = new InlineKeyboardMarkup(descrKBList);


                YesInlineJiraRespButton = new InlineKeyboardButton();
                YesInlineJiraRespButton.CallbackData = callBackDataYesJiraRespB;
                YesInlineJiraRespButton.Text = "Да";

                NoInlineJiraRespButton = new InlineKeyboardButton();
                NoInlineJiraRespButton.CallbackData = callBackDataNoJiraRespB;
                NoInlineJiraRespButton.Text = "Нет";

                jiraRespKBList = new List<InlineKeyboardButton>(2);
                jiraRespKBList.Add(YesInlineJiraRespButton);
                jiraRespKBList.Add(NoInlineJiraRespButton);

                JiraRespKeyboard = new InlineKeyboardMarkup(jiraRespKBList);
            }
        }
    }
}
