namespace TelegramBotServer
{
    namespace Configuration
    {
        static class Config
        {
            public static readonly string TokenPath = "MyToken.token";
            public static readonly string ReportFilesPath = "Reports\\";
            public static readonly string StartFileName = "Отчет";
            public static readonly string ReportDefaultExtensionFile = ".docx";
            public static readonly string BodyJiraUrl = @"https://support.muctr.ru/projects/TO/issues/TO-"; //Получать из API Jira
            public static readonly string RuleMessageStr = "Введите ЦИФРУ, что бы добавить заявку к отчету:\n" +
                "1. По ремонту и настройке компьютерной техники\n" +
                "2. По настройке ПО\n" +
                "3. Замена картриджей\n" +
                "4. Участие в подготовке, проведение крупного мероприятия, проекте и т.п.\n" +
                "5. Иные задачи и/или личные поручения\n" +
                "6. Переработки\n" +
                "7. Сформировать отчет";
            public static readonly string UnderstandStr = "Не понимаю:(\nПожалуйста, попробуй еще раз!";
            public static readonly string NotFoundTicketStr = ": не нашел такой заявки в Jira";
            public static readonly string DateExtraWorkStr = "Введите дату переработки в формате {dd:mm:yy}";
            public static readonly string InputTicketNumStr = "Введите номер(а) заявки из Jira\n";
            public static readonly string IsTicketExistStr = "Есть ли заявка в Jira?";
            public static readonly string InputCommentStr = "Введите коментарий";
            public static readonly string InputCountHourExtraWorkStr = "Введите количество переработанных часов";
            public static readonly string PushYesOrInputTextStr = "Нажмите \"Да\" или введите текст, если неообходимо добавить описание (всё написанное пойдет в отчет)";
            public static readonly string InputDescriptionStr = "Добавьте описание(всё написанное уйдет в отчет):";

            public static readonly char[] separator = { ' ', '\b', '\f', '\n', '\r', '\v', '\t' };
        }
    }
}
