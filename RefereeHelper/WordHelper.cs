﻿using Microsoft.Office.Interop.Word;
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
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;
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
        static int circles;
        static int placecur;
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
                        GroupId = x.GroupId,
                        CompetitionId = x.CompetitionId,
                        MemberId = x.MemberId
                    }).ToList();

                    var members = dbContext.Set<Member>().Select(x => new Member
                    {
                        Id = x.Id,
                        FamilyName = x.FamilyName,
                        Name = x.Name,
                        BornDate = x.BornDate,
                        ClubId = x.ClubId,
                        DischargeId = x.DischargeId
                    }).ToList();

                    var clubs = dbContext.Set<Club>().Select(x => new Club
                    {
                        Id = x.Id,
                        Name = x.Name
                    }).ToList();

                    var discharges = dbContext.Set<Discharge>().Select(x => new Discharge
                    {
                        Id = x.Id,
                        Name = x.Name
                    }).ToList();

                    var groups = dbContext.Set<Group>().Select(x => new Group
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Distance = new Distance
                        {
                            Id = x.Distance.Id,
                            StartTime = x.Distance.StartTime
                        }
                    }).ToList();

                    var distances = dbContext.Set<Distance>().Select(x => new Distance
                    {
                        Id = x.Id,
                        StartTime = x.StartTime
                    }).ToList();

                    var starts = dbContext.Set<Models.Start>().Select(x => new Models.Start
                    {
                        Id = x.Id,
                        Number = x.Number,
                        StartTime = x.StartTime,
                        PartisipationId = x.PartisipationId
                    }).ToList();

                    foreach (Group group in groups)
                    {
                        foreach (Partisipation partisipation in partisipations)
                            if (partisipation.GroupId != null && partisipation.MemberId != null)
                                if (group.Id == partisipation.GroupId && partisipation.CompetitionId == competition.Id)
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
                                        foreach (Member member in members)
                                            if (member.Id == partisipation.MemberId)
                                            {
                                                table.Cell(row, 2).Range.Text = member.FamilyName + ", " + member.Name;
                                                table.Cell(row, 6).Range.Text = member.BornDate.ToShortDateString();
                                                table.Cell(row, 6).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                if (member.ClubId != null)
                                                    foreach (Club club in clubs)
                                                        if (club.Id == member.ClubId)
                                                            table.Cell(row, 3).Range.Text = club.Name;
                                                if (member.DischargeId != null)
                                                    foreach (Discharge discharge in discharges)
                                                        if (discharge.Id == member.DischargeId)
                                                            table.Cell(row, 4).Range.Text = discharge.Name;
                                                break;
                                            }
                                        foreach (Models.Start start in starts)
                                            if (start.PartisipationId == partisipationId)
                                            {
                                                table.Cell(row, 5).Range.Text = start.Number.ToString();
                                                table.Cell(row, 5).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight;
                                                if (start.StartTime != null)
                                                {
                                                    DateTimebuf = (DateTime)start.StartTime;
                                                    table.Cell(row, 7).Range.Text = DateTimebuf.ToShortTimeString();
                                                    table.Cell(row, 7).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                }
                                                break;
                                            }
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
                        Circles = x.Circles,
                        StartTime = x.StartTime
                    }).ToList();

                    var partisipations = dbContext.Set<Partisipation>().Select(x => new Partisipation
                    {
                        Id = x.Id,
                        GroupId = x.GroupId,
                        CompetitionId = competition.Id,
                        MemberId = x.MemberId
                    }).ToList();

                    var members = dbContext.Set<Member>().Select(x => new Member
                    {
                        Id = x.Id,
                        FamilyName = x.FamilyName,
                        Name = x.Name,
                        BornDate = x.BornDate,
                        City = x.City,
                        ClubId = x.ClubId,
                        DischargeId = x.DischargeId
                    }).ToList();

                    var clubs = dbContext.Set<Club>().Select(x => new Club
                    {
                        Id = x.Id,
                        Name = x.Name
                    }).ToList();

                    var groups = dbContext.Set<Group>().Select(x => new Group
                    {
                        Id = x.Id,
                        Name = x.Name,
                        DistanceId = x.DistanceId
                    }).ToList();

                    var starts = dbContext.Set<Models.Start>().Select(x => new Models.Start
                    {
                        Id = x.Id,
                        Number = x.Number,
                        StartTime = x.StartTime,
                        Status = x.Status,
                        Place = x.Place,
                        PlaceAbsolute = x.PlaceAbsolute,
                        PartisipationId = x.PartisipationId
                    }).ToList();


                    var timings = dbContext.Set<Timing>().Select(x => new Timing
                    {
                        Id = x.Id,
                        TimeFromStart = x.TimeFromStart,
                        StartId = x.StartId
                    }).ToList();

                    foreach (Distance distance in distances)
                    {
                        foreach (Group group in groups)
                            if (distance.Id == group.DistanceId)
                                foreach (Partisipation partisipation in partisipations)
                                    if (partisipation.GroupId != null)
                                        if (group.Id == partisipation.GroupId && partisipation.CompetitionId == competition.Id)
                                            foreach (Models.Start start in starts)
                                            {
                                                if (start.PartisipationId == partisipation.Id)
                                                {
                                                    if (start.Status == 0 || start.Status == 1)
                                                        partisipationIds.Add(partisipation.Id);
                                                    else if (start.Status == 2)
                                                        partisipationDNFIds.Add(partisipation.Id);
                                                    else if (start.Status == 3)
                                                        partisipationDNSIds.Add(partisipation.Id);
                                                }
                                            }

                        if (partisipationIds.Count != 0 || partisipationDNFIds.Count != 0 || partisipationDNSIds.Count != 0)
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
                                        circles = 0;
                                        if (partisipation.MemberId != null)
                                            foreach (Member member in members)
                                                if (member.Id == partisipation.MemberId)
                                                {
                                                    table.Cell(row, 2).Range.Text = member.FamilyName;
                                                    table.Cell(row, 2).Range.Font.Bold = 1;
                                                    table.Cell(row, 3).Range.Text = member.Name;
                                                    table.Cell(row, 3).Range.Font.Bold = 1;
                                                    table.Cell(row, 4).Range.Text = member.BornDate.ToShortDateString();
                                                    table.Cell(row, 4).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                    table.Cell(row, 5).Range.Text = member.City;
                                                    table.Cell(row, 5).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                    if (member.ClubId != null)
                                                        foreach (Club club in clubs)
                                                            if (member.ClubId == club.Id)
                                                            {
                                                                table.Cell(row, 6).Range.Text = club.Name;
                                                                table.Cell(row, 6).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                            }
                                                }
                                        foreach (Models.Start start in starts)
                                            if (start.PartisipationId == partisipationId)
                                            {
                                                table.Cell(row, 7).Range.Text = start.Number.ToString();
                                                table.Cell(row, 7).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                table.Cell(row, 1).Range.Text = start.PlaceAbsolute.ToString();
                                                table.Cell(row, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                table.Cell(row, constcol).Range.Text = start.Place.ToString();
                                                table.Cell(row, constcol).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                if (start.Status == 1)
                                                    table.Cell(row, constcol - 2).Range.Font.Bold = 1;
                                                foreach (Timing timing in timings)
                                                {
                                                    if (timing.StartId != null)
                                                        if (timing.StartId == start.Id)
                                                        {
                                                            if (timing.TimeFromStart != null)
                                                            {
                                                                buf = (TimeOnly)timing.TimeFromStart;
                                                                table.Cell(row, col).Range.Text = buf.ToLongTimeString();
                                                                table.Cell(row, col).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                                circles++;
                                                            }
                                                            col++;
                                                        }
                                                }
                                                break;
                                            }
                                        if (circles < distance.Circles)
                                        {
                                            table.Cell(row, constcol - 2).Range.Text = "+ " + (distance.Circles - circles) + " круг.";
                                            table.Cell(row, constcol - 2).Range.Font.Bold = 1;
                                        }
                                        if (partisipation.GroupId != null)
                                            foreach (Group group in groups)
                                                if (group.Id == partisipation.GroupId)
                                                    table.Cell(row, constcol - 1).Range.Text = group.Name;
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
                                        if (partisipation.MemberId != null)
                                            foreach (Member member in members)
                                                if (member.Id == partisipation.MemberId)
                                                {
                                                    table.Cell(row, 2).Range.Text = member.FamilyName;
                                                    table.Cell(row, 2).Range.Font.Bold = 1;
                                                    table.Cell(row, 3).Range.Text = member.Name;
                                                    table.Cell(row, 3).Range.Font.Bold = 1;
                                                    table.Cell(row, 4).Range.Text = member.BornDate.ToShortDateString();
                                                    table.Cell(row, 4).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                    table.Cell(row, 5).Range.Text = member.City;
                                                    table.Cell(row, 5).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                    if (member.ClubId != null)
                                                        foreach (Club club in clubs)
                                                            if (member.ClubId == club.Id)
                                                            {
                                                                table.Cell(row, 6).Range.Text = club.Name;
                                                                table.Cell(row, 6).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                            }
                                                }
                                        foreach (Models.Start start in starts)
                                            if (start.PartisipationId == partisipationId)
                                            {
                                                table.Cell(row, 7).Range.Text = start.Number.ToString();
                                                table.Cell(row, 7).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                foreach (Timing timing in timings)
                                                {
                                                    if (timing.StartId != null)
                                                        if (timing.StartId == start.Id)
                                                        {
                                                            if (timing.TimeFromStart != null)
                                                            {
                                                                buf = (TimeOnly)timing.TimeFromStart;
                                                                table.Cell(row, col).Range.Text = buf.ToLongTimeString();
                                                                table.Cell(row, col).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                            }
                                                            col++;
                                                        }
                                                }
                                                break;
                                            }
                                        if (partisipation.GroupId != null)
                                            foreach (Group group in groups)
                                                if (group.Id == partisipation.GroupId)
                                                    table.Cell(row, constcol - 1).Range.Text = group.Name;
                                        table.Cell(row, 1).Range.Text = "DNF";
                                        table.Cell(row, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        table.Cell(row, constcol).Range.Text = "DNF";
                                        table.Cell(row, constcol).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
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
                                        if (partisipation.MemberId != null)
                                            foreach (Member member in members)
                                                if (member.Id == partisipation.MemberId)
                                                {
                                                    table.Cell(row, 2).Range.Text = member.FamilyName;
                                                    table.Cell(row, 2).Range.Font.Bold = 1;
                                                    table.Cell(row, 3).Range.Text = member.Name;
                                                    table.Cell(row, 3).Range.Font.Bold = 1;
                                                    table.Cell(row, 4).Range.Text = member.BornDate.ToShortDateString();
                                                    table.Cell(row, 4).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                    table.Cell(row, 5).Range.Text = member.City;
                                                    table.Cell(row, 5).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                    if (member.ClubId != null)
                                                        foreach (Club club in clubs)
                                                            if (member.ClubId == club.Id)
                                                            {
                                                                table.Cell(row, 6).Range.Text = club.Name;
                                                                table.Cell(row, 6).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                            }
                                                }
                                        foreach (Models.Start start in starts)
                                            if (start.PartisipationId == partisipationId)
                                            {
                                                table.Cell(row, 7).Range.Text = start.Number.ToString();
                                                table.Cell(row, 7).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                foreach (Timing timing in timings)
                                                {
                                                    if (timing.StartId != null)
                                                        if (timing.StartId == start.Id)
                                                        {
                                                            if (timing.TimeFromStart != null)
                                                            {
                                                                buf = (TimeOnly)timing.TimeFromStart;
                                                                table.Cell(row, col).Range.Text = buf.ToLongTimeString();
                                                                table.Cell(row, col).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                            }
                                                            col++;
                                                        }
                                                }
                                                break;
                                            }
                                        if (partisipation.GroupId != null)
                                            foreach (Group group in groups)
                                                if (group.Id == partisipation.GroupId)
                                                    table.Cell(row, constcol - 1).Range.Text = group.Name;
                                        table.Cell(row, 1).Range.Text = "DNS";
                                        table.Cell(row, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        table.Cell(row, constcol).Range.Text = "DNS";
                                        table.Cell(row, constcol).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                    }
                                }
                            table.Range.Font.Size = 6;
                            table.Rows[1].Range.Font.Bold = 1;
                            table.Rows[1].Range.Borders[WdBorderType.wdBorderTop].LineStyle = Word.WdLineStyle.wdLineStyleSingle;
                            table.Rows[1].Range.Borders[WdBorderType.wdBorderBottom].LineStyle = Word.WdLineStyle.wdLineStyleSingle;
                            table.Rows[1].Range.Borders[WdBorderType.wdBorderBottom].LineWidth = Word.WdLineWidth.wdLineWidth150pt;
                            table.Rows[1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                            table.Columns.AutoFit();
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
                        GroupId = x.GroupId,
                        CompetitionId = x.CompetitionId,
                        MemberId = x.MemberId,
                    }).ToList();

                    var members = dbContext.Set<Member>().Select(x => new Member
                    {
                        Id = x.Id,
                        FamilyName = x.FamilyName,
                        Name = x.Name,
                        BornDate = x.BornDate,
                        City = x.City,
                        ClubId = x.ClubId
                    }).ToList();

                    var clubs = dbContext.Set<Club>().Select(x => new Club
                    {
                        Id = x.Id,
                        Name = x.Name
                    }).ToList();

                    var groups = dbContext.Set<Group>().Select(x => new Group
                    {
                        Id = x.Id,
                        Name = x.Name,
                        DistanceId = x.DistanceId,
                        Distance = new Distance
                        {
                            Circles = x.Distance.Circles   
                        }
                    }).ToList();

                    var starts = dbContext.Set<Models.Start>().Select(x => new Models.Start
                    {
                        Id = x.Id,
                        Number = x.Number,
                        Status = x.Status,
                        Place = x.Place,
                        PartisipationId = x.PartisipationId
                    }).ToList();

                    var timings = dbContext.Set<Timing>().Select(x => new Timing
                    {
                        Id = x.Id,
                        TimeFromStart = x.TimeFromStart,
                        StartId = x.StartId
                    }).ToList();

                    foreach (Group group in groups)
                    {

                        foreach (Partisipation partisipation in partisipations)
                            if (partisipation.GroupId != null)
                                if (group.Id == partisipation.GroupId && partisipation.CompetitionId == competition.Id)
                                    foreach (Models.Start start in starts)
                                    {
                                        if (start.PartisipationId == partisipation.Id)
                                        {
                                            if (start.Status == 0 || start.Status == 1)
                                                partisipationIds.Add(partisipation.Id);
                                            else if (start.Status == 2)
                                                partisipationDNFIds.Add(partisipation.Id);
                                            else if (start.Status == 3)
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
                                        circles = 0;
                                        row++;
                                        if (partisipation.MemberId != null)
                                            foreach (Member member in members)
                                                if (member.Id == partisipation.MemberId)
                                                {
                                                    table.Cell(row, 2).Range.Text = member.FamilyName;
                                                    table.Cell(row, 2).Range.Font.Bold = 1;
                                                    table.Cell(row, 3).Range.Text = member.Name;
                                                    table.Cell(row, 3).Range.Font.Bold = 1;
                                                    table.Cell(row, 4).Range.Text = member.BornDate.ToShortDateString();
                                                    table.Cell(row, 4).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                    table.Cell(row, 5).Range.Text = member.City;
                                                    table.Cell(row, 5).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                    if (member.ClubId != null)
                                                        foreach (Club club in clubs)
                                                            if (member.ClubId == club.Id)
                                                            {
                                                                table.Cell(row, 6).Range.Text = club.Name;
                                                                table.Cell(row, 6).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                            }
                                                }
                                        foreach (Models.Start start in starts)
                                            if (start.PartisipationId == partisipationId)
                                            {
                                                table.Cell(row, 7).Range.Text = start.Number.ToString();
                                                table.Cell(row, 7).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                table.Cell(row, 1).Range.Text = start.Place.ToString();
                                                table.Cell(row, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                if (start.Status == 1)
                                                    table.Cell(row, col - 1).Range.Font.Bold = 1;
                                                foreach (Timing timing in timings)
                                                {
                                                    if (timing.StartId != null)
                                                        if (timing.StartId == start.Id)
                                                        {
                                                            if (timing.TimeFromStart != null)
                                                            {
                                                                buf = (TimeOnly)timing.TimeFromStart;
                                                                table.Cell(row, col).Range.Text = buf.ToLongTimeString();
                                                                table.Cell(row, col).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                                circles++;
                                                            }
                                                            col++;
                                                            
                                                        }
                                                }
                                                break;
                                            }
                                        if (circles < group.Distance.Circles)
                                        {
                                            table.Cell(row, constcol).Range.Text = "+ " + (group.Distance.Circles - circles) + " круг.";
                                            table.Cell(row, constcol).Range.Font.Bold = 1;
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
                                        if (partisipation.MemberId != null)
                                            foreach (Member member in members)
                                                if (member.Id == partisipation.MemberId)
                                                {
                                                    table.Cell(row, 2).Range.Text = member.FamilyName;
                                                    table.Cell(row, 2).Range.Font.Bold = 1;
                                                    table.Cell(row, 3).Range.Text = member.Name;
                                                    table.Cell(row, 3).Range.Font.Bold = 1;
                                                    table.Cell(row, 4).Range.Text = member.BornDate.ToShortDateString();
                                                    table.Cell(row, 4).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                    table.Cell(row, 5).Range.Text = member.City;
                                                    table.Cell(row, 5).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                    if (member.ClubId != null)
                                                        foreach (Club club in clubs)
                                                            if (member.ClubId == club.Id)
                                                            {
                                                                table.Cell(row, 6).Range.Text = club.Name;
                                                                table.Cell(row, 6).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                            }
                                                }
                                        foreach (Models.Start start in starts)
                                            if (start.PartisipationId == partisipationId)
                                            {
                                                table.Cell(row, 7).Range.Text = start.Number.ToString();
                                                table.Cell(row, 7).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                foreach (Timing timing in timings)
                                                {
                                                    if (timing.StartId != null)
                                                        if (timing.StartId == start.Id)
                                                        {
                                                            if (timing.TimeFromStart != null)
                                                            {
                                                                buf = (TimeOnly)timing.TimeFromStart;
                                                                table.Cell(row, col).Range.Text = buf.ToLongTimeString();
                                                                table.Cell(row, col).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                            }
                                                            col++;
                                                        }
                                                }
                                                break;
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
                                        if (partisipation.MemberId != null)
                                            foreach (Member member in members)
                                                if (member.Id == partisipation.MemberId)
                                                {
                                                    table.Cell(row, 2).Range.Text = member.FamilyName;
                                                    table.Cell(row, 2).Range.Font.Bold = 1;
                                                    table.Cell(row, 3).Range.Text = member.Name;
                                                    table.Cell(row, 3).Range.Font.Bold = 1;
                                                    table.Cell(row, 4).Range.Text = member.BornDate.ToShortDateString();
                                                    table.Cell(row, 4).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                    table.Cell(row, 5).Range.Text = member.City;
                                                    table.Cell(row, 5).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                    if (member.ClubId != null)
                                                        foreach (Club club in clubs)
                                                            if (member.ClubId == club.Id)
                                                            {
                                                                table.Cell(row, 6).Range.Text = club.Name;
                                                                table.Cell(row, 6).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                            }
                                                }
                                        foreach (Models.Start start in starts)
                                            if (start.PartisipationId == partisipationId)
                                            {
                                                table.Cell(row, 7).Range.Text = start.Number.ToString();
                                                table.Cell(row, 7).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
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
                            table.Columns.AutoFit();
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
                    placecur = 0;
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
                        GroupId = x.GroupId,
                        CompetitionId = x.CompetitionId,
                        MemberId = x.MemberId,
                    }).ToList();

                    var members = dbContext.Set<Member>().Select(x => new Member
                    {
                        Id = x.Id,
                        FamilyName = x.FamilyName,
                        Name = x.Name,
                        BornDate = x.BornDate,
                        ClubId = x.ClubId
                    }).ToList();

                    var clubs = dbContext.Set<Club>().Select(x => new Club
                    {
                        Id = x.Id,
                        Name = x.Name
                    }).ToList();

                    var groups = dbContext.Set<Group>().Select(x => new Group
                    {
                        Id = x.Id,
                        Name = x.Name,
                        DistanceId = x.DistanceId
                    }).ToList();

                    var starts = dbContext.Set<Models.Start>().Select(x => new Models.Start
                    {
                        Id = x.Id,
                        Number = x.Number,
                        Status = x.Status,
                        Place = x.Place,
                        PartisipationId = x.PartisipationId
                    }).ToList();

                    var timings = dbContext.Set<Timing>().Select(x => new Timing
                    {
                        Id = x.Id,
                        TimeFromStart = x.TimeFromStart,
                        StartId = x.StartId
                    }).ToList();

                    foreach (Distance distance in distances)
                        foreach (Group group in groups)
                            if (distance.Id == group.DistanceId)
                            {
                                foreach (Partisipation partisipation in partisipations)
                                    if (partisipation.GroupId != null)
                                        if (group.Id == partisipation.GroupId && partisipation.CompetitionId == competition.Id)
                                            foreach (Models.Start start in starts)
                                                if (start.PartisipationId == partisipation.Id)
                                                {
                                                    if (start.Status == 0 || start.Status == 1)
                                                        partisipationOKIds.Add(partisipation.Id);
                                                    else if (start.Status == 2)
                                                        partisipationDNFIds.Add(partisipation.Id);
                                                    else if (start.Status == 3)
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
                                                circles = 0;
                                                if (partisipation.MemberId != null)
                                                    foreach (Member member in members)
                                                        if (member.Id == partisipation.MemberId)
                                                        {
                                                            table.Cell(row, 2).Range.Text = member.FamilyName + ", " + member.Name;
                                                            table.Cell(row, 5).Range.Text = member.BornDate.ToShortDateString();
                                                            if (member.ClubId != null)
                                                                foreach (Club club in clubs)
                                                                    if (member.ClubId == club.Id)
                                                                        table.Cell(row, 3).Range.Text = club.Name;
                                                        }
                                                foreach (Models.Start start in starts)
                                                    if (start.PartisipationId == partisipationId)
                                                    {
                                                        table.Cell(row, 4).Range.Text = start.Number.ToString();
                                                        table.Cell(row, constcol).Range.Text = start.Place.ToString();
                                                        foreach (Timing timing in timings)
                                                        {
                                                            if (timing.StartId != null)
                                                                if (start.Id == timing.StartId)
                                                                {
                                                                    if (timing.TimeFromStart != null)
                                                                    {
                                                                        buf = (TimeOnly)timing.TimeFromStart;
                                                                        table.Cell(row, col).Range.Text = buf.ToLongTimeString(); col++;
                                                                        circles++;
                                                                    }
                                                                }
                                                        }
                                                        break;
                                                    }
                                                if (circles < distance.Circles)
                                                {
                                                    table.Cell(row, constcol - 2).Range.Text = "+ " + (distance.Circles - circles) + " круг.";
                                                    placecur++;
                                                }
                                            }
                                        }

                                    table.Sort(true, constcol);
                                    for (int i = 0; i < partisipationOKIds.Count - placecur; i++)
                                    {
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
                                    for (int i = 0; i < partisipationOKIds.Count; i++)
                                    {
                                        table.Cell(2 + i, 1).Range.Text = cur.ToString(); cur++;
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
                                                if (partisipation.MemberId != null)
                                                    foreach (Member member in members)
                                                        if (member.Id == partisipation.MemberId)
                                                        {
                                                            table.Cell(row, 2).Range.Text = member.FamilyName + ", " + member.Name;
                                                            table.Cell(row, 5).Range.Text = member.BornDate.ToShortDateString();
                                                            if (member.ClubId != null)
                                                                foreach (Club club in clubs)
                                                                    if (member.ClubId == club.Id)
                                                                        table.Cell(row, 3).Range.Text = club.Name;
                                                        }
                                                foreach (Models.Start start in starts)
                                                    if (start.PartisipationId == partisipationId)
                                                    {
                                                        table.Cell(row, 4).Range.Text = start.Number.ToString();
                                                        foreach (Timing timing in timings)
                                                        {
                                                            if (timing.StartId != null)
                                                                if (start.Id == timing.StartId)
                                                                {
                                                                    if (timing.TimeFromStart != null)
                                                                    {
                                                                        buf = (TimeOnly)timing.TimeFromStart;
                                                                        table.Cell(row, col).Range.Text = buf.ToLongTimeString(); col++;
                                                                    }
                                                                }
                                                        }
                                                        break;
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
                                                if (partisipation.MemberId != null)
                                                    foreach (Member member in members)
                                                        if (member.Id == partisipation.MemberId)
                                                        {
                                                            table.Cell(row, 2).Range.Text = member.FamilyName + ", " + member.Name;
                                                            table.Cell(row, 5).Range.Text = member.BornDate.ToShortDateString();
                                                            if (member.ClubId != null)
                                                                foreach (Club club in clubs)
                                                                    if (member.ClubId == club.Id)
                                                                        table.Cell(row, 3).Range.Text = club.Name;
                                                        }
                                                foreach (Models.Start start in starts)
                                                    if (start.PartisipationId == partisipationId)
                                                    {
                                                        table.Cell(row, 4).Range.Text = start.Number.ToString();
                                                        break;
                                                    }
                                                table.Cell(row, constcol).Range.Text = "DNS";
                                            }
                                        }
                                    table.Rows[1].Range.Font.Bold = 1;
                                    table.Rows[1].Borders[WdBorderType.wdBorderBottom].LineStyle = Word.WdLineStyle.wdLineStyleSingle;
                                    table.Range.Font.Size = 6;
                                    maxwidth = table.Columns.Width;
                                    table.Columns.AutoFit();
                                    width = table.Columns.Width;
                                    differencewidth = maxwidth - width;
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
