using Microsoft.Office.Interop.Word;
using RefereeHelper.EntityFramework;
using RefereeHelper.Models;
using RefereeHelper.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Word = Microsoft.Office.Interop.Word;

namespace RefereeHelper
{
    internal class WordHelper
    {
        static Object missingObj = System.Reflection.Missing.Value;
        static Object trueObj = true;
        static Object falseObj = false;
        static object missing = Missing.Value;
        static object what = Word.WdGoToItem.wdGoToLine;
        static object which = Word.WdGoToDirection.wdGoToLast;
        static int row;
        static int constrow;
        static int cur;
        static int col;
        static int constcol;
        static TimeOnly buf;
        static DateTime DateTimebuf;

        public WordHelper()
        {
        }

        public bool StarProtocol(Competition competition)
        {
            Word._Application word = new Word.Application();
            Word._Document doc = word.Documents.Add();
            try 
            {
                
                Word.Range wordRange;
                Word.Table table;
                using (var dbContext = new RefereeHelperDbContextFactory().CreateDbContext())
                {
                    List<int> partisipationIds = new List<int>();

                    var partisipations = dbContext.Set<Partisipation>().Select(x => new Partisipation
                    {
                        Id = x.Id,
                        Group = new Group
                        {
                            Id = x.Group.Id,
                        },
                        Competition = new Competition
                        {
                            Id = x.Competition.Id,
                        },
                        Member = new Member
                        {
                            Id = x.Member.Id,
                            FamilyName = x.Member.FamilyName,
                            Name = x.Member.Name,
                            BornDate = x.Member.BornDate,
                            Club = new Club
                            {
                                Name = x.Member.Club.Name
                            },
                            Discharge = new Discharge
                            {
                                Name = x.Member.Discharge.Name
                            }
                        }
                    }).ToList();

                    var groups = dbContext.Set<Group>().Select(x => new Group
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Distance = new Distance
                        {
                            Id = x.Distance.Id,
                            StartTime = x.Distance.StartTime,
                        }
                    }).ToList();

                    var starts = dbContext.Set<Models.Start>().Select(x => new Models.Start
                    {
                        Id = x.Id,
                        Number = x.Number,
                        StartTime = x.StartTime,
                        Partisipation = new Partisipation
                        {
                            Id = x.Partisipation.Id
                        }
                    }).ToList();

                    var timings = dbContext.Set<Timing>().Select(x => new Timing
                    {
                        Id = x.Id,
                        TimeFromStart = x.TimeFromStart,
                        IsFinish = x.IsFinish,
                        Start = new Models.Start
                        {
                            Id = x.Start.Id,
                            Partisipation = new Partisipation
                            {
                                Id = x.Start.Partisipation.Id
                            },
                            Number = x.Start.Number
                        }
                    }).ToList();

                    foreach (Group group in groups)
                    {
                        foreach (Partisipation partisipation in partisipations)
                            if (group.Id == partisipation.Group?.Id && partisipation.Competition?.Id == competition.Id)
                                partisipationIds.Add(partisipation.Id);

                        if (partisipationIds.Count != 0)
                        {
                            wordRange = doc.GoTo(ref what, ref which, ref missing, ref missing);

                            table = doc.Tables.Add(wordRange, 1, 2, ref missingObj, ref missingObj);

                            table.Cell(1, 1).Range.Text = group.Name;
                            table.Cell(1, 2).Range.Text = competition.Place;
                            table.Columns[1].Width = 120;
                            table.Columns[2].Width = 120;
                            table.Range.Font.Size = 20;

                            wordRange = doc.GoTo(ref what, ref which, ref missing, ref missing);
                            wordRange.Text = "\n";
                            wordRange = doc.GoTo(ref what, ref which, ref missing, ref missing);

                            table = doc.Tables.Add(wordRange, partisipationIds.Count + 1, 8, ref missingObj, ref missingObj);

                            table.Cell(1, 1).Range.Text = "№";
                            table.Cell(1, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight;
                            table.Cell(1, 2).Range.Text = "Фамилия, Имя";
                            table.Cell(1, 3).Range.Text = "Коллектив";
                            table.Cell(1, 4).Range.Text = "Квал";
                            table.Cell(1, 5).Range.Text = "Номер";
                            table.Cell(1, 5).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight;
                            table.Cell(1, 6).Range.Text = "ГР";
                            table.Cell(1, 6).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                            table.Cell(1, 7).Range.Text = "Старт";
                            table.Cell(1, 6).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                            table.Cell(1, 8).Range.Text = "Примечание";
                            row = 1;

                            foreach (int partisipationId in partisipationIds)
                                foreach (Partisipation partisipation in partisipations)
                                {
                                    if (partisipation.Id == partisipationId)
                                    {
                                        row++;
                                        table.Cell(row, 1).Range.Text = (row - 1).ToString();
                                        table.Cell(row, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight;
                                        table.Cell(row, 2).Range.Text = partisipation.Member?.FamilyName + ", " + partisipation.Member?.Name;
                                        table.Cell(row, 3).Range.Text = partisipation.Member?.Club?.Name;
                                        table.Cell(row, 4).Range.Text = partisipation.Member?.Discharge?.Name;
                                        foreach (Models.Start start in starts)
                                            if (start.Partisipation.Id == partisipationId)
                                            {
                                                table.Cell(row, 5).Range.Text = start.Number.ToString();
                                                table.Cell(row, 5).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight;
                                                if (start.StartTime != null)
                                                {
                                                    DateTimebuf = (DateTime)start.StartTime;
                                                    table.Cell(row, 7).Range.Text = DateTimebuf.ToShortTimeString();
                                                    table.Cell(1, 7).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                }
                                                else
                                                {
                                                    DateTimebuf = (DateTime)group.Distance.StartTime;
                                                    table.Cell(row, 7).Range.Text = DateTimebuf.ToShortTimeString();
                                                    table.Cell(1, 7).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                }
                                                break;
                                            }
                                        table.Cell(row, 6).Range.Text = partisipation.Member?.BornDate.ToShortDateString();
                                        table.Cell(row, 6).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                    }
                                }
                            table.Range.Font.Size = 8;
                            table.Rows[1].Borders[Word.WdBorderType.wdBorderBottom].LineStyle = Word.WdLineStyle.wdLineStyleSingle;
                            table.Rows[1].Range.Font.Bold = 1;
                            table.Columns.AutoFit();
                            table.Columns[2].Cells.Width = 100;
                            table.Columns[2].Cells.Height = 6;
                            table.Columns[3].Cells.Width = 100;
                            wordRange = doc.GoTo(ref what, ref which, ref missing, ref missing);
                            wordRange.Text = "\n";
                        }
                        else
                        {
                            doc.Close(ref falseObj, ref missingObj, ref missingObj);
                            word.Quit();
                            MessageBox.Show("Невозможно сформировать стратовый протокол", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return false;
                        }

                    }
                    doc.SaveAs(System.IO.Path.Combine(Environment.CurrentDirectory, "temp") + "\\WordSP.docx");
                    doc.Close(ref falseObj, ref missingObj, ref missingObj);
                    word.Quit();
                    return true;
                }
            }
            catch (Exception ex)
            {
                doc.Close(ref falseObj, ref missingObj, ref missingObj);
                word.Quit();
                MessageBox.Show("Произошла ошибка:" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool DistanceProtocol(Competition competition)
        {
            Word._Application word = new Word.Application();
            Word._Document doc = word.Documents.Add();
            try 
            {
                
                Word.Range wordRange;
                Word.Table table;
                using (var dbContext = new RefereeHelperDbContextFactory().CreateDbContext())
                {
                    List<int> partisipationIds = new List<int>();
                    List<int> partisipationDNFIds = new List<int>();
                    List<int> partisipationDNSIds = new List<int>();
                    bool fl = true;

                    var distances = dbContext.Set<Distance>().Select(x => new Distance
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Circles = x.Circles
                    }).ToList();

                    var partisipations = dbContext.Set<Partisipation>().Select(x => new Partisipation
                    {
                        Id = x.Id,
                        Group = new Group
                        {
                            Id = x.Group.Id,
                            Name = x.Group.Name,
                        },
                        Competition = new Competition
                        {
                            Id = x.Competition.Id,
                        },
                        Member = new Member
                        {
                            Id = x.Member.Id,
                            FamilyName = x.Member.FamilyName,
                            Name = x.Member.Name,
                            BornDate = x.Member.BornDate,
                            City = x.Member.City,
                            Club = new Club
                            {
                                Name = x.Member.Club.Name
                            },
                            Discharge = new Discharge
                            {
                                Name = x.Member.Discharge.Name
                            }
                        }
                    }).ToList();

                    var groups = dbContext.Set<Group>().Select(x => new Group
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Distance = new Distance
                        {
                            Id = x.Distance.Id
                        }
                    }).ToList();

                    var starts = dbContext.Set<Models.Start>().Select(x => new Models.Start
                    {
                        Id = x.Id,
                        Number = x.Number,
                        Status = x.Status,
                        Partisipation = new Partisipation
                        {
                            Id = x.Partisipation.Id
                        }
                    }).ToList();


                    var timings = dbContext.Set<Timing>().Select(x => new Timing
                    {
                        Id = x.Id,
                        TimeFromStart = x.TimeFromStart,
                        IsFinish = x.IsFinish,
                        Place = x.Place,
                        PlaceAbsolute = x.PlaceAbsolute,
                        Start = new Models.Start
                        {
                            Id = x.Start.Id,
                            Partisipation = new Partisipation
                            {
                                Id = x.Start.Partisipation.Id
                            },
                            Number = x.Start.Number
                        }
                    }).ToList();

                    foreach (Distance distance in distances)
                    {
                        foreach (Group group in groups)
                            if (distance.Id == group.Distance.Id)
                                foreach (Partisipation partisipation in partisipations)
                                    if (group.Id == partisipation.Group?.Id && partisipation.Competition?.Id == competition.Id)
                                        foreach (Models.Start start in starts)
                                        {
                                            if (start.Partisipation.Id == partisipation.Id)
                                            {
                                                if (start.Status == 0)
                                                    partisipationIds.Add(partisipation.Id);
                                                else if (start.Status == 1)
                                                    partisipationDNFIds.Add(partisipation.Id);
                                                else if (start.Status == 2)
                                                    partisipationDNSIds.Add(partisipation.Id);
                                            }
                                        }

                        if (partisipationIds.Count != 0)
                        {
                            wordRange = doc.GoTo(ref what, ref which, ref missing, ref missing);
                            if (fl)
                            {
                                fl = false;
                                table = doc.Tables.Add(wordRange, 2, 3, ref missingObj, ref missingObj);

                                table.Cell(2, 1).Range.Text = competition.Organizer;
                                table.Cell(1, 2).Range.Text = competition.Name;
                                table.Cell(1, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                table.Cell(2, 2).Range.Text = competition.Date?.ToShortDateString();
                                table.Cell(2, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                table.Cell(2, 3).Range.Text = competition.Place;
                                table.Cell(2, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight;
                                table.Range.Font.Size = 10;

                                wordRange = doc.GoTo(ref what, ref which, ref missing, ref missing);
                                wordRange.Text = "\n";
                                wordRange = doc.GoTo(ref what, ref which, ref missing, ref missing);
                            }

                            table = doc.Tables.Add(wordRange, 1, 1, ref missingObj, ref missingObj);
                            table.Cell(1, 1).Range.Text = distance.Name;
                            table.Cell(1, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

                            wordRange = doc.GoTo(ref what, ref which, ref missing, ref missing);
                            wordRange.Text = "\n";
                            wordRange = doc.GoTo(ref what, ref which, ref missing, ref missing);

                            constcol = 9 + Convert.ToInt32(distance.Circles);
                            col = 8;

                            table = doc.Tables.Add(wordRange, partisipationIds.Count + 1, constcol, ref missingObj, ref missingObj);


                            table.Cell(1, 1).Range.Text = "Место";
                            table.Cell(1, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                            table.Cell(1, 2).Range.Text = "Фамилия";
                            table.Cell(1, 3).Range.Text = "Имя";
                            table.Cell(1, 4).Range.Text = "Год Рождения";
                            table.Cell(1, 5).Range.Text = "Город";
                            table.Cell(1, 6).Range.Text = "Клуб";
                            table.Cell(1, 7).Range.Text = "Ст.№";
                            for (int i = 1; i < distance.Circles; i++)
                            {
                                table.Cell(1, col).Range.Text = "Круг " + i.ToString(); col++;
                            }
                            table.Cell(1, col).Range.Text = "Финиш"; col++;
                            table.Cell(1, col).Range.Text = "Категория"; col++;
                            table.Cell(1, col).Range.Text = "Место в кат.";
                            row = 1;
                            foreach (int partisipationId in partisipationIds)
                                foreach (Partisipation partisipation in partisipations)
                                {
                                    if (partisipation.Id == partisipationId)
                                    {
                                        col = 8;
                                        row++;
                                        table.Cell(row, 2).Range.Text = partisipation.Member?.FamilyName;
                                        table.Cell(row, 2).Range.Font.Bold = 1;
                                        table.Cell(row, 3).Range.Text = partisipation.Member?.Name;
                                        table.Cell(row, 3).Range.Font.Bold = 1;
                                        table.Cell(row, 4).Range.Text = partisipation.Member?.BornDate.ToShortDateString();
                                        table.Cell(row, 4).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        table.Cell(row, 5).Range.Text = partisipation.Member?.City;
                                        table.Cell(row, 5).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        table.Cell(row, 6).Range.Text = partisipation.Member?.Club?.Name;
                                        table.Cell(row, 6).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        foreach (Models.Start start in starts)
                                            if (start.Partisipation.Id == partisipationId)
                                            {
                                                table.Cell(row, 7).Range.Text = start.Number.ToString();
                                                table.Cell(row, 6).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                break;
                                            }
                                        foreach (Timing timing in timings)
                                        {
                                            if (timing.Start?.Partisipation.Id == partisipationId)
                                            {
                                                if (timing.TimeFromStart != null)
                                                    buf = (TimeOnly)timing.TimeFromStart;
                                                table.Cell(row, col).Range.Text = buf.ToLongTimeString();
                                                table.Cell(row, col).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                col++;
                                                if (timing.IsFinish == true)
                                                {
                                                    table.Cell(row, 1).Range.Text = timing.PlaceAbsolute.ToString();
                                                    table.Cell(row, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                    table.Cell(row, col - 1).Range.Font.Bold = 1;
                                                    table.Cell(row, col + 1).Range.Text = timing.Place.ToString();
                                                    table.Cell(row, col + 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                }
                                            }
                                        }
                                        table.Cell(row, constcol - 1).Range.Text = partisipation.Group?.Name;
                                    }
                                }
                            table.Sort(true, 1);
                            foreach (int partisipationId in partisipationDNFIds)
                                foreach (Partisipation partisipation in partisipations)
                                {
                                    if (partisipation.Id == partisipationId)
                                    {
                                        col = 8;
                                        row++;
                                        table.Rows.Add(ref missing);
                                        table.Cell(row, 2).Range.Text = partisipation.Member?.FamilyName;
                                        table.Cell(row, 2).Range.Font.Bold = 1;
                                        table.Cell(row, 3).Range.Text = partisipation.Member?.Name;
                                        table.Cell(row, 3).Range.Font.Bold = 1;
                                        table.Cell(row, 4).Range.Text = partisipation.Member?.BornDate.ToShortDateString();
                                        table.Cell(row, 4).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        table.Cell(row, 5).Range.Text = partisipation.Member?.City;
                                        table.Cell(row, 5).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        table.Cell(row, 6).Range.Text = partisipation.Member?.Club?.Name;
                                        table.Cell(row, 6).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        foreach (Models.Start start in starts)
                                            if (start.Partisipation.Id == partisipationId)
                                            {
                                                table.Cell(row, 7).Range.Text = start.Number.ToString();
                                                table.Cell(row, 6).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                break;
                                            }
                                        foreach (Timing timing in timings)
                                        {
                                            if (timing.Start?.Partisipation.Id == partisipationId)
                                            {
                                                if (timing.TimeFromStart != null)
                                                    buf = (TimeOnly)timing.TimeFromStart;
                                                table.Cell(row, col).Range.Text = buf.ToLongTimeString();
                                                table.Cell(row, col).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                col++;
                                            }
                                        }
                                        table.Cell(row, 1).Range.Text = "DNF";
                                        table.Cell(row, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        table.Cell(row, constcol).Range.Text = "DNF";
                                        table.Cell(row, constcol).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        table.Cell(row, constcol - 1).Range.Text = partisipation.Group?.Name;
                                    }
                                }
                            foreach (int partisipationId in partisipationDNSIds)
                                foreach (Partisipation partisipation in partisipations)
                                {
                                    if (partisipation.Id == partisipationId)
                                    {
                                        col = 8;
                                        row++;
                                        table.Rows.Add(ref missing);
                                        table.Cell(row, 2).Range.Text = partisipation.Member?.FamilyName;
                                        table.Cell(row, 2).Range.Font.Bold = 1;
                                        table.Cell(row, 3).Range.Text = partisipation.Member?.Name;
                                        table.Cell(row, 3).Range.Font.Bold = 1;
                                        table.Cell(row, 4).Range.Text = partisipation.Member?.BornDate.ToShortDateString();
                                        table.Cell(row, 4).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        table.Cell(row, 5).Range.Text = partisipation.Member?.City;
                                        table.Cell(row, 5).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        table.Cell(row, 6).Range.Text = partisipation.Member?.Club?.Name;
                                        table.Cell(row, 6).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        foreach (Models.Start start in starts)
                                            if (start.Partisipation.Id == partisipationId)
                                            {
                                                table.Cell(row, 7).Range.Text = start.Number.ToString();
                                                table.Cell(row, 6).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                break;
                                            }
                                        table.Cell(row, 1).Range.Text = "DNS";
                                        table.Cell(row, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        table.Cell(row, constcol).Range.Text = "DNS";
                                        table.Cell(row, constcol).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        table.Cell(row, constcol - 1).Range.Text = partisipation.Group?.Name;
                                    }
                                }
                            table.Range.Font.Size = 6;

                            table.Rows[1].Range.Font.Bold = 1;
                            table.Rows[1].Range.Borders[WdBorderType.wdBorderTop].LineStyle = Word.WdLineStyle.wdLineStyleSingle;
                            table.Rows[1].Range.Borders[WdBorderType.wdBorderBottom].LineStyle = Word.WdLineStyle.wdLineStyleSingle;
                            table.Rows[1].Range.Borders[WdBorderType.wdBorderBottom].LineWidth = Word.WdLineWidth.wdLineWidth150pt;
                            table.Rows[1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                            //maxwidth = table.Columns.Width;
                            table.Columns.AutoFit();
                            //width = table.Columns.Width;
                            //differencewidth = maxwidth - width;
                            //table.Columns[6].Width = table.Columns[6].Width + (differencewidth / 10 * 6);
                            //table.Columns[4].Width = table.Columns[6].Width + (differencewidth / 10 * 4);
                        }
                        else
                        {
                            doc.Close(ref falseObj, ref missingObj, ref missingObj);
                            word.Quit();
                            MessageBox.Show("Невозможно сформировать протокол по дистанции", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return false;
                        }
                    }
                    doc.SaveAs(System.IO.Path.Combine(Environment.CurrentDirectory, "temp") + "\\WordDP.docx");
                    doc.Close(ref falseObj, ref missingObj, ref missingObj);
                    word.Quit();
                    return true;
                }
            }
            catch (Exception ex)
            {
                doc.Close(ref falseObj, ref missingObj, ref missingObj);
                word.Quit();
                MessageBox.Show("Произошла ошибка:" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool GroupDistanceProtocol(Competition competition)
        {
            Word._Application word = new Word.Application();
            Word._Document doc = word.Documents.Add();
            try
            {

                Word.Range wordRange;
                Word.Table table;
                using (var dbContext = new RefereeHelperDbContextFactory().CreateDbContext())
                {
                    List<int> partisipationIds = new List<int>();
                    List<int> partisipationDNFIds = new List<int>();
                    List<int> partisipationDNSIds = new List<int>();
                    bool fl = true;

                    var partisipations = dbContext.Set<Partisipation>().Select(x => new Partisipation
                    {
                        Id = x.Id,
                        Group = new Group
                        {
                            Id = x.Group.Id,
                            Name = x.Group.Name,
                        },
                        Competition = new Competition
                        {
                            Id = x.Competition.Id,
                        },
                        Member = new Member
                        {
                            Id = x.Member.Id,
                            FamilyName = x.Member.FamilyName,
                            Name = x.Member.Name,
                            BornDate = x.Member.BornDate,
                            City = x.Member.City,
                            Club = new Club
                            {
                                Name = x.Member.Club.Name
                            },
                            Discharge = new Discharge
                            {
                                Name = x.Member.Discharge.Name
                            }
                        }
                    }).ToList();

                    var groups = dbContext.Set<Group>().Select(x => new Group
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Distance = new Distance
                        {
                            Id = x.Distance.Id,
                            Circles = x.Distance.Circles
                        }
                    }).ToList();

                    var starts = dbContext.Set<Models.Start>().Select(x => new Models.Start
                    {
                        Id = x.Id,
                        Number = x.Number,
                        Status = x.Status,
                        Partisipation = new Partisipation
                        {
                            Id = x.Partisipation.Id
                        }
                    }).ToList();


                    var timings = dbContext.Set<Timing>().Select(x => new Timing
                    {
                        Id = x.Id,
                        TimeFromStart = x.TimeFromStart,
                        IsFinish = x.IsFinish,
                        Place = x.Place,
                        Start = new Models.Start
                        {
                            Id = x.Start.Id,
                            Partisipation = new Partisipation
                            {
                                Id = x.Start.Partisipation.Id
                            },
                            Number = x.Start.Number
                        }
                    }).ToList();

                    foreach (Group group in groups)
                    {

                        foreach (Partisipation partisipation in partisipations)
                            if (group.Id == partisipation.Group?.Id && partisipation.Competition?.Id == competition.Id)
                                foreach (Models.Start start in starts)
                                {
                                    if (start.Partisipation.Id == partisipation.Id)
                                    {
                                        if (start.Status == 0)
                                            partisipationIds.Add(partisipation.Id);
                                        else if (start.Status == 1)
                                            partisipationDNFIds.Add(partisipation.Id);
                                        else if (start.Status == 2)
                                            partisipationDNSIds.Add(partisipation.Id);
                                    }
                                }

                        if (partisipationIds.Count != 0)
                        {
                            wordRange = doc.GoTo(ref what, ref which, ref missing, ref missing);
                            if (fl)
                            {
                                fl = false;
                                table = doc.Tables.Add(wordRange, 2, 3, ref missingObj, ref missingObj);

                                table.Cell(2, 1).Range.Text = competition.Organizer;
                                table.Cell(1, 2).Range.Text = competition.Name;
                                table.Cell(1, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                table.Cell(2, 2).Range.Text = competition.Date?.ToShortDateString();
                                table.Cell(2, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                table.Cell(2, 3).Range.Text = competition.Place;
                                table.Cell(2, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight;
                                table.Range.Font.Size = 10;

                                wordRange = doc.GoTo(ref what, ref which, ref missing, ref missing);
                                wordRange.Text = "\n";
                                wordRange = doc.GoTo(ref what, ref which, ref missing, ref missing);
                            }

                            table = doc.Tables.Add(wordRange, 1, 1, ref missingObj, ref missingObj);
                            table.Cell(1, 1).Range.Text = group.Name;
                            table.Cell(1, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

                            wordRange = doc.GoTo(ref what, ref which, ref missing, ref missing);
                            wordRange.Text = "\n";
                            wordRange = doc.GoTo(ref what, ref which, ref missing, ref missing);

                            constcol = 7 + Convert.ToInt32(group.Distance.Circles);
                            col = 8;

                            table = doc.Tables.Add(wordRange, partisipationIds.Count + 1, constcol, ref missingObj, ref missingObj);


                            table.Cell(1, 1).Range.Text = "Место";
                            table.Cell(1, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                            table.Cell(1, 2).Range.Text = "Фамилия";
                            table.Cell(1, 3).Range.Text = "Имя";
                            table.Cell(1, 4).Range.Text = "Год Рождения";
                            table.Cell(1, 5).Range.Text = "Город";
                            table.Cell(1, 6).Range.Text = "Клуб";
                            table.Cell(1, 7).Range.Text = "Ст.№";
                            for (int i = 1; i < group.Distance.Circles; i++)
                            {
                                table.Cell(1, col).Range.Text = "Круг " + i.ToString(); col++;
                            }
                            table.Cell(1, col).Range.Text = "Финиш"; col++;
                            row = 1;
                            foreach (int partisipationId in partisipationIds)
                                foreach (Partisipation partisipation in partisipations)
                                {
                                    if (partisipation.Id == partisipationId)
                                    {
                                        col = 8;
                                        row++;
                                        table.Cell(row, 2).Range.Text = partisipation.Member?.FamilyName;
                                        table.Cell(row, 2).Range.Font.Bold = 1;
                                        table.Cell(row, 3).Range.Text = partisipation.Member?.Name;
                                        table.Cell(row, 3).Range.Font.Bold = 1;
                                        table.Cell(row, 4).Range.Text = partisipation.Member?.BornDate.ToShortDateString();
                                        table.Cell(row, 4).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        table.Cell(row, 5).Range.Text = partisipation.Member?.City;
                                        table.Cell(row, 5).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        table.Cell(row, 6).Range.Text = partisipation.Member?.Club?.Name;
                                        table.Cell(row, 6).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        foreach (Models.Start start in starts)
                                            if (start.Partisipation.Id == partisipationId)
                                            {
                                                table.Cell(row, 7).Range.Text = start.Number.ToString();
                                                table.Cell(row, 6).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                break;
                                            }
                                        foreach (Timing timing in timings)
                                        {
                                            if (timing.Start?.Partisipation.Id == partisipationId)
                                            {
                                                if (timing.TimeFromStart != null)
                                                    buf = (TimeOnly)timing.TimeFromStart;
                                                table.Cell(row, col).Range.Text = buf.ToLongTimeString();
                                                table.Cell(row, col).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                col++;
                                                if (timing.IsFinish == true)
                                                {
                                                    table.Cell(row, 1).Range.Text = timing.Place.ToString();
                                                    table.Cell(row, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                    table.Cell(row, col - 1).Range.Font.Bold = 1;         
                                                }
                                            }
                                        }
                                    }
                                }
                            table.Sort(true, 1);
                            foreach (int partisipationId in partisipationDNFIds)
                                foreach (Partisipation partisipation in partisipations)
                                {
                                    if (partisipation.Id == partisipationId)
                                    {
                                        col = 8;
                                        row++;
                                        table.Rows.Add(ref missing);
                                        table.Cell(row, 2).Range.Text = partisipation.Member?.FamilyName;
                                        table.Cell(row, 2).Range.Font.Bold = 1;
                                        table.Cell(row, 3).Range.Text = partisipation.Member?.Name;
                                        table.Cell(row, 3).Range.Font.Bold = 1;
                                        table.Cell(row, 4).Range.Text = partisipation.Member?.BornDate.ToShortDateString();
                                        table.Cell(row, 4).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        table.Cell(row, 5).Range.Text = partisipation.Member?.City;
                                        table.Cell(row, 5).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        table.Cell(row, 6).Range.Text = partisipation.Member?.Club?.Name;
                                        table.Cell(row, 6).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        foreach (Models.Start start in starts)
                                            if (start.Partisipation.Id == partisipationId)
                                            {
                                                table.Cell(row, 7).Range.Text = start.Number.ToString();
                                                table.Cell(row, 6).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                break;
                                            }
                                        foreach (Timing timing in timings)
                                        {
                                            if (timing.Start?.Partisipation.Id == partisipationId)
                                            {
                                                if (timing.TimeFromStart != null)
                                                    buf = (TimeOnly)timing.TimeFromStart;
                                                table.Cell(row, col).Range.Text = buf.ToLongTimeString();
                                                table.Cell(row, col).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                col++;
                                            }
                                        }
                                        table.Cell(row, 1).Range.Text = "DNF";
                                        table.Cell(row, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                    }
                                }
                            foreach (int partisipationId in partisipationDNSIds)
                                foreach (Partisipation partisipation in partisipations)
                                {
                                    if (partisipation.Id == partisipationId)
                                    {
                                        col = 8;
                                        row++;
                                        table.Rows.Add(ref missing);
                                        table.Cell(row, 2).Range.Text = partisipation.Member?.FamilyName;
                                        table.Cell(row, 2).Range.Font.Bold = 1;
                                        table.Cell(row, 3).Range.Text = partisipation.Member?.Name;
                                        table.Cell(row, 3).Range.Font.Bold = 1;
                                        table.Cell(row, 4).Range.Text = partisipation.Member?.BornDate.ToShortDateString();
                                        table.Cell(row, 4).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        table.Cell(row, 5).Range.Text = partisipation.Member?.City;
                                        table.Cell(row, 5).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        table.Cell(row, 6).Range.Text = partisipation.Member?.Club?.Name;
                                        table.Cell(row, 6).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        foreach (Models.Start start in starts)
                                            if (start.Partisipation.Id == partisipationId)
                                            {
                                                table.Cell(row, 7).Range.Text = start.Number.ToString();
                                                table.Cell(row, 6).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                break;
                                            }
                                        table.Cell(row, 1).Range.Text = "DNS";
                                        table.Cell(row, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                    }
                                }
                            table.Range.Font.Size = 6;

                            table.Rows[1].Range.Font.Bold = 1;
                            table.Rows[1].Range.Borders[WdBorderType.wdBorderTop].LineStyle = Word.WdLineStyle.wdLineStyleSingle;
                            table.Rows[1].Range.Borders[WdBorderType.wdBorderBottom].LineStyle = Word.WdLineStyle.wdLineStyleSingle;
                            table.Rows[1].Range.Borders[WdBorderType.wdBorderBottom].LineWidth = Word.WdLineWidth.wdLineWidth150pt;
                            table.Rows[1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                            //maxwidth = table.Columns.Width;
                            table.Columns.AutoFit();
                            //width = table.Columns.Width;
                            //differencewidth = maxwidth - width;
                            //table.Columns[6].Width = table.Columns[6].Width + (differencewidth / 10 * 6);
                            //table.Columns[4].Width = table.Columns[6].Width + (differencewidth / 10 * 4);
                        }
                        else
                        {
                            doc.Close(ref falseObj, ref missingObj, ref missingObj);
                            word.Quit();
                            MessageBox.Show("Невозможно сформировать протокол по дистанции", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return false;
                        }
                    }
                    doc.SaveAs(System.IO.Path.Combine(Environment.CurrentDirectory, "temp") + "\\WordGDP.docx");
                    doc.Close(ref falseObj, ref missingObj, ref missingObj);
                    word.Quit();
                    return true;
                }
            }
            catch (Exception ex)
            {
                doc.Close(ref falseObj, ref missingObj, ref missingObj);
                word.Quit();
                MessageBox.Show("Произошла ошибка:" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool FinshProtocol(Competition competition)
        {
            Word._Application word = new Word.Application();
            Word._Document doc = word.Documents.Add();
            try 
            {
                Word.Range wordRange;
                Word.Table table;
                wordRange = doc.GoTo(ref what, ref which, ref missing, ref missing);
                wordRange.Text = "\n";
                wordRange = doc.GoTo(ref what, ref which, ref missing, ref missing);
                using (var dbContext = new RefereeHelperDbContextFactory().CreateDbContext())
                {
                    TimeSpan timeSpan = TimeSpan.Zero;
                    TimeOnly finishtime = TimeOnly.MinValue;
                    string s = "";
                    List<int> partisipationOKIds = new List<int>();
                    List<int> partisipationDNFIds = new List<int>();
                    List<int> partisipationDNSIds = new List<int>();
                    cur = 1;
                    float maxwidth = 0, width = 0, differencewidth = 0;
                    bool fl = true;

                    var distances = dbContext.Set<Distance>().Select(x => new Distance
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Circles = x.Circles
                    }).ToList();

                    var partisipations = dbContext.Set<Partisipation>().Select(x => new Partisipation
                    {
                        Id = x.Id,
                        Group = new Group
                        {
                            Id = x.Group.Id,
                            Name = x.Group.Name
                        },
                        Competition = new Competition
                        {
                            Id = x.Competition.Id,
                        },
                        Member = new Member
                        {
                            Id = x.Member.Id,
                            FamilyName = x.Member.FamilyName,
                            Name = x.Member.Name,
                            BornDate = x.Member.BornDate,
                            Club = new Club
                            {
                                Name = x.Member.Club.Name
                            },
                            Discharge = new Discharge
                            {
                                Name = x.Member.Discharge.Name
                            }
                        }
                    }).ToList();

                    var groups = dbContext.Set<Group>().Select(x => new Group
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Distance = new Distance
                        {
                            Id = x.Distance.Id
                        }
                    }).ToList();

                    var starts = dbContext.Set<Models.Start>().Select(x => new Models.Start
                    {
                        Id = x.Id,
                        Number = x.Number,
                        Status = x.Status,
                        Partisipation = new Partisipation
                        {
                            Id = x.Partisipation.Id
                        }
                    }).ToList();

                    var timings = dbContext.Set<Timing>().Select(x => new Timing
                    {
                        Id = x.Id,
                        TimeFromStart = x.TimeFromStart,
                        IsFinish = x.IsFinish,
                        Place = x.Place,
                        Start = new Models.Start
                        {
                            Id = x.Start.Id,
                            Partisipation = new Partisipation
                            {
                                Id = x.Start.Partisipation.Id
                            },
                            Number = x.Start.Number
                        }
                    }).ToList();

                    foreach (Distance distance in distances)
                        foreach (Group group in groups)
                            if (distance.Id == group.Distance.Id)
                            {
                                foreach (Partisipation partisipation in partisipations)
                                    if (group.Id == partisipation.Group?.Id && partisipation.Competition?.Id == competition.Id)
                                        foreach (Models.Start start in starts)
                                            if (start.Partisipation.Id == partisipation.Id)
                                            {
                                                if (start.Status == 0)
                                                    partisipationOKIds.Add(partisipation.Id);
                                                else if(start.Status == 1)
                                                    partisipationDNFIds.Add(partisipation.Id);
                                                else if(start.Status == 2)
                                                    partisipationDNSIds.Add(partisipation.Id);  
                                            }
                                if (partisipationOKIds.Count != 0 || partisipationDNFIds.Count != 0 || partisipationDNSIds.Count != 0)
                                {
                                    wordRange = doc.GoTo(ref what, ref which, ref missing, ref missing);
                                    if (fl)
                                    {
                                        fl = false;
                                        table = doc.Tables.Add(wordRange, 3, 2, ref missingObj, ref missingObj);

                                        table.Cell(1, 1).Range.Text = competition.Name;
                                        table.Cell(1, 1).Merge(table.Cell(1, 2));
                                        table.Cell(1, 1).Range.Paragraphs.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        table.Cell(2, 1).Range.Text = competition.Date?.ToShortDateString();
                                        table.Cell(2, 2).Range.Text = competition.Place;
                                        table.Cell(2, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight;
                                        table.Cell(3, 1).Range.Text = "ПРОТОКОЛ";
                                        table.Cell(3, 1).Merge(table.Cell(3, 2));
                                        table.Cell(3, 1).Range.Paragraphs.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        table.Range.Font.Size = 14;

                                        wordRange = doc.GoTo(ref what, ref which, ref missing, ref missing);
                                        wordRange.Text = "\n";
                                        wordRange = doc.GoTo(ref what, ref which, ref missing, ref missing);
                                    }

                                    table = doc.Tables.Add(wordRange, 1, 1, ref missingObj, ref missingObj);
                                    table.Cell(1, 1).Range.Text = group.Name + ", " + distance.Name;
                                    table.Cell(1, 1).Range.Bold = 1;

                                    wordRange = doc.GoTo(ref what, ref which, ref missing, ref missing);
                                    wordRange.Text = "\n";
                                    wordRange.Font.Size = 1;
                                    wordRange = doc.GoTo(ref what, ref which, ref missing, ref missing);

                                    constcol = 7 + Convert.ToInt32(distance.Circles);
                                    col = 6;

                                    table = doc.Tables.Add(wordRange, partisipationOKIds.Count + 1, constcol, ref missingObj, ref missingObj);

                                    table.Cell(1, 1).Range.Text = "№п/п";
                                    table.Cell(1, 2).Range.Text = "Фамилия, имя";
                                    table.Cell(1, 3).Range.Text = "Коллектив";
                                    table.Cell(1, 4).Range.Text = "Номер";
                                    table.Cell(1, 5).Range.Text = "ГР";
                                    for (int i = 1; i < distance.Circles; i++)
                                    {
                                        table.Cell(1, col).Range.Text = i.ToString() + "круг"; col++;
                                    }
                                    table.Cell(1, col).Range.Text = "Результат"; col++;
                                    table.Cell(1, col).Range.Text = "Отставание"; col++;
                                    table.Cell(1, col).Range.Text = "Место";
                                    row = 1;
                                    foreach (int partisipationId in partisipationOKIds)
                                        foreach (Partisipation partisipation in partisipations)
                                        {
                                            if (partisipation.Id == partisipationId)
                                            {
                                                col = 6;
                                                row++;
                                                
                                                table.Cell(row, 2).Range.Text = partisipation.Member?.FamilyName + ", " + partisipation.Member?.Name;
                                                table.Cell(row, 3).Range.Text = partisipation.Member?.Club?.Name;
                                                foreach (Models.Start start in starts)
                                                    if (start.Partisipation.Id == partisipationId)
                                                    {
                                                        table.Cell(row, 4).Range.Text = start.Number.ToString();
                                                        break;
                                                    }
                                                table.Cell(row, 5).Range.Text = partisipation.Member?.BornDate.ToShortDateString();
                                                foreach (Timing timing in timings)
                                                {
                                                    if (timing.Start?.Partisipation.Id == partisipationId)
                                                    {
                                                        if (timing.TimeFromStart != null)
                                                        {
                                                            buf = (TimeOnly)timing.TimeFromStart;
                                                            table.Cell(row, col).Range.Text = buf.ToLongTimeString(); col++;
                                                        }
                                                        if (timing.IsFinish == true)
                                                        {
                                                            table.Cell(row, col + 1).Range.Text = timing.Place.ToString();

                                                        }
                                                    }
                                                }
                                            }
                                        }

                                    table.Sort(true, col - 1);
                                    for (int i = 0; i < partisipationOKIds.Count; i++)
                                    {
                                        table.Cell(2 + i, 1).Range.Text = cur.ToString(); cur++;
                                        if (table.Cell(2 + i, constcol - 2).Range.Text != null)
                                        {
                                            s = table.Cell(2 + i, constcol - 2).Range.Text;
                                            s = s.Trim('\a');
                                            s = s.Trim('\r');
                                            if (i == 0)
                                                TimeOnly.TryParse(s, out finishtime);
                                            if (TimeOnly.TryParse(s, out buf))
                                            {
                                                timeSpan = buf - finishtime;
                                                buf = TimeOnly.FromTimeSpan(timeSpan);
                                                table.Cell(2 + i, constcol - 1).Range.Text = buf.ToLongTimeString();
                                            }
                                        }

                                    }
                                    foreach (int partisipationId in partisipationDNFIds)
                                        foreach (Partisipation partisipation in partisipations)
                                        {
                                            if (partisipation.Id == partisipationId)
                                            {
                                                table.Rows.Add(ref missing);
                                                col = 6;
                                                row++;
                                                table.Cell(row, 1).Range.Text = cur.ToString(); cur++;
                                                table.Cell(row, 2).Range.Text = partisipation.Member?.FamilyName + ", " + partisipation.Member?.Name;
                                                table.Cell(row, 3).Range.Text = partisipation.Member?.Club?.Name;
                                                foreach (Models.Start start in starts)
                                                    if (start.Partisipation.Id == partisipationId)
                                                    {
                                                        table.Cell(row, 4).Range.Text = start.Number.ToString();
                                                        break;
                                                    }
                                                table.Cell(row, 5).Range.Text = partisipation.Member?.BornDate.ToShortDateString();
                                                foreach (Timing timing in timings)
                                                {
                                                    if (timing.Start?.Partisipation.Id == partisipationId)
                                                    {
                                                        if (timing.TimeFromStart != null)
                                                        {
                                                            buf = (TimeOnly)timing.TimeFromStart;
                                                            table.Cell(row, col).Range.Text = buf.ToLongTimeString(); col++;
                                                        }
                                                    }
                                                }
                                                table.Cell(row, constcol).Range.Text = "DNF";
                                            }
                                        }
                                    foreach (int partisipationId in partisipationDNSIds)
                                        foreach (Partisipation partisipation in partisipations)
                                        {
                                            if (partisipation.Id == partisipationId)
                                            {
                                                table.Rows.Add(ref missing);
                                                col = 6;
                                                row++;
                                                table.Cell(row, 1).Range.Text = cur.ToString(); cur++;
                                                table.Cell(row, 2).Range.Text = partisipation.Member?.FamilyName + ", " + partisipation.Member?.Name;
                                                table.Cell(row, 3).Range.Text = partisipation.Member?.Club?.Name;
                                                foreach (Models.Start start in starts)
                                                    if (start.Partisipation.Id == partisipationId)
                                                    {
                                                        table.Cell(row, 4).Range.Text = start.Number.ToString();
                                                        break;
                                                    }
                                                table.Cell(row, 5).Range.Text = partisipation.Member?.BornDate.ToShortDateString();
                                                table.Cell(row, constcol).Range.Text = "DNS";
                                            }
                                        }
                                    table.Rows[1].Range.Font.Bold = 1;
                                    table.Rows[1].Borders[WdBorderType.wdBorderBottom].LineStyle = Word.WdLineStyle.wdLineStyleSingle;
                                    table.Range.Font.Size = 6;
                                    //table.Cell(1, 1).Range.Font.Size = 14;
                                    maxwidth = table.Columns.Width;
                                    table.Columns.AutoFit();
                                    width = table.Columns.Width;
                                    differencewidth = maxwidth - width;
                                    //table.Columns[3].Width += differencewidth;
                                }
                                wordRange = doc.GoTo(ref what, ref which, ref missing, ref missing);
                                wordRange.Text = "\n";
                                wordRange = doc.GoTo(ref what, ref which, ref missing, ref missing);
                            }
                            else
                            {
                                doc.Close(ref falseObj, ref missingObj, ref missingObj);
                                word.Quit();
                                MessageBox.Show("Невозможно сформировать финишний протокол", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                return false;
                            }

                    if (!fl)
                    {
                        table = doc.Tables.Add(wordRange, 2, 2, ref missingObj, ref missingObj);
                        table.Cell(1, 1).Range.Text = "Главный судья";
                        table.Cell(2, 1).Range.Text = "Главный Секретарь";
                        table.Cell(1, 2).Range.Text = competition.Judge;
                        table.Cell(2, 2).Range.Text = competition.Secretary;
                    }
                    doc.SaveAs(System.IO.Path.Combine(Environment.CurrentDirectory, "temp") + "\\WordFP.docx");
                    doc.Close(ref falseObj, ref missingObj, ref missingObj);
                    word.Quit();
                    return true;
                }
            }
            catch(Exception ex)
            {
                doc.Close(ref falseObj, ref missingObj, ref missingObj);
                word.Quit();
                MessageBox.Show("Произошла ошибка:" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool Print(String file)
        {
            
            Word._Application word = new Word.Application();
            Word._Document doc = word.Documents.Open(file);
            try
            {
                doc.PrintOut();
                doc.Close(ref falseObj, ref missingObj, ref missingObj);
                word.Quit();

                return true;
            }
            catch (Exception ex)
            {
                doc.Close(ref falseObj, ref missingObj, ref missingObj);
                word.Quit();
                MessageBox.Show("Произошла ошибка:" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        public bool PrintAs(String file)
        {
            Word._Application word = new Word.Application();
            Word._Document doc = word.Documents.Open(file);
            try
            {
                int dialogResult = word.Dialogs[Microsoft.Office.Interop.Word.WdWordDialog.wdDialogFilePrint].Show();
                word.Visible = false;
                doc.Close(ref falseObj, ref missingObj, ref missingObj);
                word.Quit();

                return true;
            }
            catch (Exception ex)
            {
                word.Visible = false;
                doc.Close(ref falseObj, ref missingObj, ref missingObj);
                word.Quit();
                MessageBox.Show("Произошла ошибка:" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}
