using System;
using Microsoft.Office.Interop.Word;
using TelegramBotServer.CurrentDateTimes;

namespace ReportConstructorWord
{
    class ConstructorWord : IDisposable
    {
        private readonly Application app;
        private readonly Document doc;
        private readonly string path;
        private readonly Microsoft.Office.Interop.Word.Range range;
        private int countParagraph = 1;
        private bool disposedValue = false;


        public ConstructorWord(string path_)
        {
            app = new Application();
            doc = app.Documents.Add(Visible: true);
            range = doc.Range(0, 0);
            path = path_;
        }

        public void CreateHeader()
        {
            range.Font.Name = "Times New Roman";
            range.Font.Size = 14;
            range.Paragraphs.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

            doc.Paragraphs[countParagraph].Range.Text = "Начальнику отдела";
            doc.Paragraphs[countParagraph].Range.Font.Name = "Times New Roman";
            doc.Paragraphs[countParagraph].Range.Font.Size = 14;
            doc.Paragraphs[countParagraph].SpaceAfter = 0;
            doc.Paragraphs[countParagraph].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
            NextLine();

            doc.Paragraphs[countParagraph].Range.Text = "поддержки пользователей";
            doc.Paragraphs[countParagraph].Range.Font.Name = "Times New Roman";
            doc.Paragraphs[countParagraph].Range.Font.Size = 14;
            doc.Paragraphs[countParagraph].SpaceAfter = 0;
            doc.Paragraphs[countParagraph].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
            NextLine();

            doc.Paragraphs[countParagraph].Range.Text = "корпоративной сети";
            doc.Paragraphs[countParagraph].Range.Font.Name = "Times New Roman";
            doc.Paragraphs[countParagraph].Range.Font.Size = 14;
            doc.Paragraphs[countParagraph].SpaceAfter = 0;
            doc.Paragraphs[countParagraph].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
            NextLine();

            doc.Paragraphs[countParagraph].Range.Text = "Д. Д. Белоусу";
            doc.Paragraphs[countParagraph].Range.Font.Name = "Times New Roman";
            doc.Paragraphs[countParagraph].Range.Font.Size = 14;
            doc.Paragraphs[countParagraph].SpaceAfter = 0;
            doc.Paragraphs[countParagraph].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
            NextLine(3);

            doc.Paragraphs[countParagraph].Range.Text = "ОТЧЕТ О РЕЗУЛЬТАТАХ ТРУДА";
            doc.Paragraphs[countParagraph].Range.Font.Name = "Times New Roman";
            doc.Paragraphs[countParagraph].Range.Font.Size = 14;
            doc.Paragraphs[countParagraph].Range.Font.Bold = 1;
            doc.Paragraphs[countParagraph].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            NextLine();

            doc.Paragraphs[countParagraph].Range.Font.Bold = 0;
            doc.Paragraphs[countParagraph].Range.Text = DateTime.Now.Date.ToShortDateString();
            doc.Paragraphs[countParagraph].Range.Font.Name = "Times New Roman";
            doc.Paragraphs[countParagraph].Range.Font.Size = 14;
            doc.Paragraphs[countParagraph].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
            NextLine();

        }

        public void CreatePeriod(DateTime date)
        {
            (int, int) weekStr = CurrentDateTime.CurrentWeek(date);
            (int, int) weekMonth = CurrentDateTime.GetMonth(date, weekStr);

            DateTime d1 = new DateTime(date.Year, weekMonth.Item1, weekStr.Item1);
            DateTime d2 = new DateTime(date.Year, weekMonth.Item2, weekStr.Item2);

            doc.Paragraphs[countParagraph].Range.Text = $"\tЗа период с {d1.ToShortDateString()} по {d2.ToShortDateString()} мной были выполнены следующие работы следующие работы:";
            doc.Paragraphs[countParagraph].Range.Font.Name = "Times New Roman";
            doc.Paragraphs[countParagraph].Range.Font.Size = 14;
            doc.Paragraphs[countParagraph].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
            NextLine();
        }


        public void CreateHardwareTasks(TelegramBotServer.Users.TelegramUser user)
        {
            doc.Paragraphs[countParagraph].Range.Text = "Выполнено (кол-во) заявок по ремонту и настройке компьютерной техники (Ссылки на Jiru).";
            doc.Paragraphs[countParagraph].Range.Font.Name = "Times New Roman";
            doc.Paragraphs[countParagraph].Range.Font.Size = 14;
            doc.Paragraphs[countParagraph].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
            doc.Paragraphs[countParagraph].Range.ListFormat.ApplyNumberDefault(WdDefaultListBehavior.wdWord10ListBehavior);
            NextLine();

            doc.Paragraphs[countParagraph].Range.Text = $"\tКоличество = {user.HardwareTicket.Count}";
            doc.Paragraphs[countParagraph].Range.Font.Name = "Times New Roman";
            doc.Paragraphs[countParagraph].Range.Font.Size = 14;
            doc.Paragraphs[countParagraph].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
            doc.Paragraphs[countParagraph].Range.ListFormat.RemoveNumbers(WdNumberType.wdNumberParagraph);
            NextLine();

            doc.Paragraphs[countParagraph].Indent();
            foreach (int str in user.HardwareTicket)
            {
                doc.Paragraphs[countParagraph].Range.Text = $"{TelegramBotServer.Configuration.Config.BodyJiraUrl + str.ToString()}";
                doc.Paragraphs[countParagraph].Range.Font.Name = "Times New Roman";
                doc.Paragraphs[countParagraph].Range.Font.Size = 14;
                doc.Paragraphs[countParagraph].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
                doc.Paragraphs[countParagraph].Range.Hyperlinks.Add(doc.Paragraphs[countParagraph].Range, $"{TelegramBotServer.Configuration.Config.BodyJiraUrl + str}");
                doc.Paragraphs[countParagraph].Range.InsertParagraphAfter();
                countParagraph++;
            }

    }

        public void CreateSoftwareTasks(TelegramBotServer.Users.TelegramUser user)
        {
            doc.Paragraphs[countParagraph].Range.Text = "Выполнены (кол-во) заявки по настройке ПО (Ссылки на Jiru).";
            doc.Paragraphs[countParagraph].Range.Font.Name = "Times New Roman";
            doc.Paragraphs[countParagraph].Range.Font.Size = 14;
            doc.Paragraphs[countParagraph].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
            doc.Paragraphs[countParagraph].Range.ListFormat.ApplyNumberDefault(WdDefaultListBehavior.wdWord10ListBehavior);
            NextLine();

            doc.Paragraphs[countParagraph].Range.Text = $"\tКоличество = {user.SoftwareTicket.Count}";
            doc.Paragraphs[countParagraph].Range.Font.Name = "Times New Roman";
            doc.Paragraphs[countParagraph].Range.Font.Size = 14;
            doc.Paragraphs[countParagraph].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
            doc.Paragraphs[countParagraph].Range.ListFormat.RemoveNumbers(WdNumberType.wdNumberParagraph);
            NextLine();

            doc.Paragraphs[countParagraph].Indent();
            foreach (int str in user.SoftwareTicket)
            {
                doc.Paragraphs[countParagraph].Range.Text = $"{TelegramBotServer.Configuration.Config.BodyJiraUrl + str.ToString()}";
                doc.Paragraphs[countParagraph].Range.Font.Name = "Times New Roman";
                doc.Paragraphs[countParagraph].Range.Font.Size = 14;
                doc.Paragraphs[countParagraph].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
                doc.Paragraphs[countParagraph].Range.Hyperlinks.Add(doc.Paragraphs[countParagraph].Range, $"{TelegramBotServer.Configuration.Config.BodyJiraUrl + str}");
                doc.Paragraphs[countParagraph].Range.InsertParagraphAfter();
                countParagraph++;
            }
        }

        public void CreateCartridgeTasks(TelegramBotServer.Users.TelegramUser user)
        {
            doc.Paragraphs[countParagraph].Range.Text = "Выполнено (кол-во) заявок по замене картриджей (Ссылки на Jiru).";
            doc.Paragraphs[countParagraph].Range.Font.Name = "Times New Roman";
            doc.Paragraphs[countParagraph].Range.Font.Size = 14;
            doc.Paragraphs[countParagraph].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
            doc.Paragraphs[countParagraph].Range.ListFormat.ApplyNumberDefault(WdDefaultListBehavior.wdWord10ListBehavior);
            NextLine();

            doc.Paragraphs[countParagraph].Range.Text = $"\tКоличество = {user.CartridgeTicket.Count}";
            doc.Paragraphs[countParagraph].Range.Font.Name = "Times New Roman";
            doc.Paragraphs[countParagraph].Range.Font.Size = 14;
            doc.Paragraphs[countParagraph].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
            doc.Paragraphs[countParagraph].Range.ListFormat.RemoveNumbers(WdNumberType.wdNumberParagraph);
            NextLine();

            doc.Paragraphs[countParagraph].Indent();
            foreach (int str in user.CartridgeTicket)
            {
                doc.Paragraphs[countParagraph].Range.Text = $"{TelegramBotServer.Configuration.Config.BodyJiraUrl + str.ToString()}";
                doc.Paragraphs[countParagraph].Range.Font.Name = "Times New Roman";
                doc.Paragraphs[countParagraph].Range.Font.Size = 14;
                doc.Paragraphs[countParagraph].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
                doc.Paragraphs[countParagraph].Range.Hyperlinks.Add(doc.Paragraphs[countParagraph].Range, $"{TelegramBotServer.Configuration.Config.BodyJiraUrl + str}");
                doc.Paragraphs[countParagraph].Range.InsertParagraphAfter();
                countParagraph++;
            }
        }

        public void CreateBitEventTasks(TelegramBotServer.Users.TelegramUser user)
        {
            doc.Paragraphs[countParagraph].Range.Text = "Принял участие в подготовке, проведение …. крупного мероприятия, проекте и т.п. : ФИЭБ, день карьеры и т.п.";
            doc.Paragraphs[countParagraph].Range.Font.Name = "Times New Roman";
            doc.Paragraphs[countParagraph].Range.Font.Size = 14;
            doc.Paragraphs[countParagraph].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
            doc.Paragraphs[countParagraph].Range.ListFormat.ApplyNumberDefault(WdDefaultListBehavior.wdWord10ListBehavior);
            NextLine();

            doc.Paragraphs[countParagraph].Range.Text = $"\tКоличество = {user.BigEventTicket.Count}";
            doc.Paragraphs[countParagraph].Range.Font.Name = "Times New Roman";
            doc.Paragraphs[countParagraph].Range.Font.Size = 14;
            doc.Paragraphs[countParagraph].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
            doc.Paragraphs[countParagraph].Range.ListFormat.RemoveNumbers(WdNumberType.wdNumberParagraph);
            NextLine();

            doc.Paragraphs[countParagraph].Indent();
            foreach (int str in user.BigEventTicket)
            {
                doc.Paragraphs[countParagraph].Range.Text = $"{TelegramBotServer.Configuration.Config.BodyJiraUrl + str.ToString()}";
                doc.Paragraphs[countParagraph].Range.Font.Name = "Times New Roman";
                doc.Paragraphs[countParagraph].Range.Font.Size = 14;
                doc.Paragraphs[countParagraph].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
                doc.Paragraphs[countParagraph].Range.Hyperlinks.Add(doc.Paragraphs[countParagraph].Range, $"{TelegramBotServer.Configuration.Config.BodyJiraUrl + str}");
                doc.Paragraphs[countParagraph].Range.InsertParagraphAfter();
                countParagraph++;
            }
            foreach (TelegramBotServer.Users.Description descr in user?.DescriptionsList)
            {
                if (descr.CodeBlock == TelegramBotServer.Enum.Answer.BigEvent)
                {
                    foreach (string str in descr.Descript.Split('\n'))
                    {
                        doc.Paragraphs[countParagraph].Range.Text = $"{str}";
                        doc.Paragraphs[countParagraph].Range.Font.Name = "Times New Roman";
                        doc.Paragraphs[countParagraph].Range.Font.Size = 14;
                        doc.Paragraphs[countParagraph].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
                        doc.Paragraphs[countParagraph].Range.ListFormat.RemoveNumbers(WdNumberType.wdNumberParagraph);
                        doc.Paragraphs[countParagraph].Range.InsertParagraphAfter();
                        doc.Paragraphs[countParagraph].TabIndent(1);

                        countParagraph++;
                    }
                }
            }
        }

        public void CreatePersonTasks(TelegramBotServer.Users.TelegramUser user)
        {
            doc.Paragraphs[countParagraph].Range.Text = "Выполнено (иные задачи\\личные поручения):";
            doc.Paragraphs[countParagraph].Range.Font.Name = "Times New Roman";
            doc.Paragraphs[countParagraph].Range.Font.Size = 14;
            doc.Paragraphs[countParagraph].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
            doc.Paragraphs[countParagraph].Range.ListFormat.ApplyNumberDefault(WdDefaultListBehavior.wdWord10ListBehavior);
            NextLine();

            //doc.Paragraphs[countParagraph].Indent();
            foreach (int str in user.PersonTicket)
            {
                doc.Paragraphs[countParagraph].Range.Text = $"{TelegramBotServer.Configuration.Config.BodyJiraUrl + str.ToString()}";
                doc.Paragraphs[countParagraph].Range.Font.Name = "Times New Roman";
                doc.Paragraphs[countParagraph].Range.Font.Size = 14;
                doc.Paragraphs[countParagraph].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
                doc.Paragraphs[countParagraph].Range.ListFormat.RemoveNumbers(WdNumberType.wdNumberParagraph);
                doc.Paragraphs[countParagraph].Range.Hyperlinks.Add(doc.Paragraphs[countParagraph].Range, $"{TelegramBotServer.Configuration.Config.BodyJiraUrl + str}");
                doc.Paragraphs[countParagraph].Range.InsertParagraphAfter();
                doc.Paragraphs[countParagraph].TabIndent(1);

                countParagraph++;
            }
            foreach (TelegramBotServer.Users.Description descr in user?.DescriptionsList)
            {
                if (descr.CodeBlock == TelegramBotServer.Enum.Answer.PersonTasks)
                {
                    foreach (string str in descr.Descript.Split('\n'))
                    {
                        doc.Paragraphs[countParagraph].Range.Text = $"{str}";
                        doc.Paragraphs[countParagraph].Range.Font.Name = "Times New Roman";
                        doc.Paragraphs[countParagraph].Range.Font.Size = 14;
                        doc.Paragraphs[countParagraph].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
                        doc.Paragraphs[countParagraph].Range.ListFormat.RemoveNumbers(WdNumberType.wdNumberParagraph);
                        doc.Paragraphs[countParagraph].Range.InsertParagraphAfter();
                        doc.Paragraphs[countParagraph].TabIndent(1);

                        countParagraph++;
                    }
                }
            }
        }

        public void CreateExtraWorkTasks(TelegramBotServer.Users.TelegramUser user)
        {
            doc.Paragraphs[countParagraph].Range.Text = "Переработано:";
            doc.Paragraphs[countParagraph].Range.Font.Name = "Times New Roman";
            doc.Paragraphs[countParagraph].Range.Font.Size = 14;
            doc.Paragraphs[countParagraph].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
            doc.Paragraphs[countParagraph].Range.ListFormat.RemoveNumbers(WdNumberType.wdNumberParagraph);
            doc.Paragraphs[countParagraph].Range.ListFormat.ApplyNumberDefault(WdDefaultListBehavior.wdWord10ListBehavior);
            NextLine();

            doc.Paragraphs[countParagraph].Range.ListFormat.RemoveNumbers(WdNumberType.wdNumberParagraph);
            int count = user.ExtraWorkList.Count <= 0 ? 2 : user.ExtraWorkList.Count + 1;
            Table t = doc.Paragraphs[countParagraph].Range.Tables.Add(doc.Paragraphs[countParagraph].Range, count, 4);
            t.Borders.Enable = 1;

            t.AllowAutoFit = true;
            Column firstCol = t.Columns[1];
            t.Columns[1].SetWidth(40, WdRulerStyle.wdAdjustFirstColumn);
            t.Columns[2].SetWidth(90, WdRulerStyle.wdAdjustFirstColumn);
            t.Columns[3].SetWidth(90, WdRulerStyle.wdAdjustFirstColumn);

            t.Rows[1].Cells[1].Range.Text = "№";
            t.Rows[1].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            t.Rows[1].Cells[2].Range.Text = "Дата";
            t.Rows[1].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            t.Rows[1].Cells[3].Range.Text = "Кол-во переработанных часов";
            t.Rows[1].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            t.Rows[1].Cells[4].Range.Text = "Комментарий";
            t.Rows[1].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;


            for (int i = 2; i < count + 1; i++)
            {
                int j = 0;
                foreach (Cell cell in t.Rows[i].Cells)
                {
                    if (j == 0)
                        cell.Range.Text = (i - 1).ToString();
                    if (user.ExtraWorkList.Count > 0)
                    {
                        if (j == 1)
                            cell.Range.Text = user.ExtraWorkList[i - 2].Date.ToShortDateString();
                        else if (j == 2)
                            cell.Range.Text = user.ExtraWorkList[i - 2].CountHour.ToString();
                        else if (j == 3)
                        {
                            foreach (string str in user.ExtraWorkList[i - 2].Comment.Split('\n', '\r'))
                            {
                                if (str.Length > 30)
                                {
                                    for (int k = str.Length; k > 30; k -= 30)
                                        cell.Range.Text += str.Substring(str.Length - k, 30);
                                }
                                else
                                    cell.Range.Text += $"{str}";
                            }
                            cell.Range.Text = cell.Range.Text.Remove(0, 1);
                        }
                    }
                    j++;
                    cell.Range.Font.Name = "Times New Roman";
                    cell.Range.Font.Size = 14;
                }
            }
            t.Range.InsertParagraphAfter();
            countParagraph += t.Range.Paragraphs.Count;
            NextLine();
        }

        public void CreateFooter(TelegramBotServer.Users.TelegramUser user)
        {
            doc.Paragraphs[countParagraph].Range.ListFormat.RemoveNumbers(WdNumberType.wdNumberParagraph);
            Table t = doc.Paragraphs[countParagraph].Range.Tables.Add(doc.Paragraphs[countParagraph].Range, 2, 2);
            t.Borders.Enable = 0;

            t.Rows[1].Cells[1].Range.Text = "Должность:";
            t.Rows[1].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            t.Rows[1].Cells[2].Range.Text = "Ф.И.О.:";
            t.Rows[1].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
            t.Rows[2].Cells[1].Range.Text = user.Profile.UserPost;
            t.Rows[1].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            t.Rows[2].Cells[2].Range.Text = user.Profile.GetFullName();
            t.Rows[2].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
        }

        public void ErrorConstructorWrite()
        {
            doc.Paragraphs[doc.Paragraphs.Count].Range.Text = "Ошибка получения заявок! Пользователь не указан.\n";
            doc.Paragraphs[doc.Paragraphs.Count].Range.Font.Name = "Times New Roman";
            doc.Paragraphs[doc.Paragraphs.Count].Range.Font.Size = 14;
            doc.Paragraphs[doc.Paragraphs.Count].Range.Font.Bold = 1;
            doc.Paragraphs[doc.Paragraphs.Count].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
        }

        protected virtual void NextLine()
        {
            doc.Paragraphs[countParagraph].Range.Text += '\n';
            countParagraph++;
        }

        protected virtual void NextLine(int count)
        {
            for (int i = 0; i < count; i++)
                NextLine();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    try
                    {
                        //doc.SaveAsQuickStyleSet(path);
                        doc.SaveAs2(System.IO.Path.GetFullPath(path),
                        WdSaveFormat.wdFormatDocumentDefault);
                    }
                    catch
                    {
                    }
                }
                doc.Close();
                app.Quit();
                disposedValue = true;
            }
        }

         ~ConstructorWord()
         {
             Dispose(disposing: false);
         }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
