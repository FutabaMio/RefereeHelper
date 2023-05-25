using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using RefereeHelper.EntityFramework;
using RefereeHelper.EntityFramework.Services;
using RefereeHelper.Models;
using RefereeHelper.OptionsWindows;
using RefereeHelper.Views;
using Epplus = OfficeOpenXml;
using EpplusSyle = OfficeOpenXml.Style;


namespace RefereeHelper
{
    public class Excelhelper
    {
        private Epplus.ExcelPackage package;
        TimeOnly buf;
        DateTime DateTimebuf;

        public Excelhelper()
        {
            package = new Epplus.ExcelPackage();
        }

        public bool StarProtocol(Competition competition)
        {
            try
            {
                using (var dbContext = new RefereeHelperDbContextFactory().CreateDbContext()) 
                {
                    var sheet = package.Workbook.Worksheets.Add("лист 1");
                    int row = 1, cur = 1;
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
                            sheet.Cells[row, 1].Value = group.Name;
                            sheet.Cells[row, 1].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            sheet.Cells[row, 1].Style.Font.Size = 20;
                            sheet.Cells[row, 1, row, 2].Merge = true;
                            sheet.Cells[row, 3].Value = competition.Place;
                            sheet.Cells[row, 3].Style.Font.Size = 20;
                            row = row + 2;
                            sheet.Cells[row, 1].Value = "№";
                            sheet.Cells[row, 2].Value = "Фамилия, Имя";
                            sheet.Cells[row, 3].Value = "Коллектив";
                            sheet.Cells[row, 4].Value = "Квал";
                            sheet.Cells[row, 5].Value = "Номер";
                            sheet.Cells[row, 6].Value = "ГР";
                            sheet.Cells[row, 7].Value = "Старт";
                            sheet.Cells[row, 8].Value = "Примечание";
                            sheet.Cells[row, 1, row, 8].Style.Font.Bold = true;
                            sheet.Cells[row, 1, row, 8].Style.Border.Bottom.Style = EpplusSyle.ExcelBorderStyle.Medium;
                            foreach (int partisipationId in partisipationIds)
                                foreach (Partisipation partisipation in partisipations)
                                {
                                    if (partisipation.Id == partisipationId)
                                    {
                                        row++;
                                        sheet.Cells[row, 1].Value = cur; cur++;
                                        foreach (Member member in members)
                                            if (member.Id == partisipation.MemberId)
                                            {
                                                sheet.Cells[row, 2].Value = member.FamilyName + ", " + member.Name;
                                                sheet.Cells[row, 2].Style.Font.Bold = true;
                                                sheet.Cells[row, 6].Value = member.BornDate.ToShortDateString();
                                                if (member.ClubId != null)
                                                    foreach (Club club in clubs)
                                                        if (club.Id == member.ClubId)
                                                        {
                                                            sheet.Cells[row, 3].Value = club.Name;
                                                            sheet.Cells[row, 3].Style.Font.Bold = true;
                                                        }
                                                if (member.DischargeId != null)
                                                    foreach (Discharge discharge in discharges)
                                                        if (discharge.Id == member.DischargeId)
                                                            sheet.Cells[row, 4].Value = discharge.Name;
                                                break;
                                            }
                                        foreach (Models.Start start in starts)
                                            if (start.PartisipationId == partisipationId)
                                            {
                                                sheet.Cells[row, 5].Value = start.Number;
                                                if (start.StartTime != null)
                                                {
                                                    DateTimebuf = (DateTime)start.StartTime;
                                                    sheet.Cells[row, 7].Value = DateTimebuf.ToShortTimeString();
                                                }
                                                break;
                                            }
                                    }
                                }
                            row++;
                            cur = 1;
                        }
                        else 
                        {
                            MessageBox.Show("Невозможно сформировать стратовый протокол","Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
                            return false; 
                        }
                    }
                    sheet.Cells[1, 1, row, 8].AutoFitColumns(1, 150);
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка:" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false; 
            }
        }

        public bool DistanceProtocol(Competition competition)
        {
            try
            {
                using (var dbContext = new RefereeHelperDbContextFactory().CreateDbContext())
                {
                    var sheet = package.Workbook.Worksheets.Add("лист 1");
                    int row = 1, col = 8, costcol = 0, cur = 1, rowcost, circle = 0;
                    List<int> partisipationIds = new List<int>();
                    List<int> partisipationDNFIds = new List<int>();
                    List<int> partisipationDNSIds = new List<int>();

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
                            sheet.Cells[row, 1].Value = distance.Name;
                            row++;
                            rowcost = row + 1;
                            sheet.Cells[row, 1].Value = "Место";
                            sheet.Cells[row, 2].Value = "Фамилия";
                            sheet.Cells[row, 3].Value = "Имя";
                            sheet.Cells[row, 4].Value = "Год Рождения";
                            sheet.Cells[row, 5].Value = "Город";
                            sheet.Cells[row, 6].Value = "Клуб";
                            sheet.Cells[row, 7].Value = "Ст.№";
                            for (int i = 1; i < distance.Circles; i++)
                            {
                                sheet.Cells[row, col].Value = "Круг " + i.ToString(); col++;
                            }
                            sheet.Cells[row, col].Value = "Финиш"; col++;
                            sheet.Cells[row, col].Value = "Категория"; col++;
                            sheet.Cells[row, col].Value = "Место в кат.";
                            costcol = col;
                            sheet.Cells[row - 1, 1, row - 1, col].Merge = true;
                            sheet.Cells[row - 1, 1, row - 1, col].Style.HorizontalAlignment = EpplusSyle.ExcelHorizontalAlignment.Center;
                            sheet.Cells[row, 1, row, col].Style.Font.Bold = true;
                            sheet.Cells[row, 1, row, col].Style.Border.Bottom.Style = EpplusSyle.ExcelBorderStyle.Medium;
                            sheet.Cells[row, 1, row, col].Style.Border.Top.Style = EpplusSyle.ExcelBorderStyle.Thick;
                            sheet.Cells[row, 1, row, col].Style.HorizontalAlignment = EpplusSyle.ExcelHorizontalAlignment.Center;
                            sheet.Column(1).Style.HorizontalAlignment = EpplusSyle.ExcelHorizontalAlignment.Center;
                            for (int i = 4; i <= col; i++)
                                sheet.Column(i).Style.HorizontalAlignment = EpplusSyle.ExcelHorizontalAlignment.Center;
                            col = 8;
                            foreach (int partisipationId in partisipationIds)
                                foreach (Partisipation partisipation in partisipations)
                                {
                                    if (partisipation.Id == partisipationId)
                                    {
                                        row++;
                                        if (partisipation.MemberId != null)
                                            foreach (Member member in members)
                                                if (member.Id == partisipation.MemberId)
                                                {
                                                    sheet.Cells[row, 2].Value = member.FamilyName;
                                                    sheet.Cells[row, 2].Style.Font.Bold = true;
                                                    sheet.Cells[row, 3].Value = member.Name;
                                                    sheet.Cells[row, 3].Style.Font.Bold = true;
                                                    sheet.Cells[row, 4].Value = member.BornDate.ToShortDateString();
                                                    sheet.Cells[row, 5].Value = member.City;
                                                    if (member.ClubId != null)
                                                        foreach (Club club in clubs)
                                                            if (member.ClubId == club.Id)
                                                                sheet.Cells[row, 6].Value = club.Name;
                                                } 
                                        foreach (Models.Start start in starts)
                                            if (start.PartisipationId == partisipationId)
                                            {
                                                sheet.Cells[row, 7].Value = start.Number;
                                                sheet.Cells[row, 1].Value = start.PlaceAbsolute;
                                                sheet.Cells[row, costcol].Value = start.Place;
                                                if (start.Status == 1)
                                                    sheet.Cells[row, costcol - 2].Style.Font.Bold = true;
                                                foreach (Timing timing in timings)
                                                {
                                                    if (timing.StartId != null)
                                                        if (timing.StartId == start.Id)
                                                        {
                                                            if (timing.TimeFromStart != null)
                                                            {
                                                                buf = (TimeOnly)timing.TimeFromStart;
                                                                sheet.Cells[row, col].Value = buf.ToLongTimeString();
                                                                circle++;
                                                            }
                                                            col++;
                                                        }
                                                }
                                                break;
                                            }
                                        if (circle < distance.Circles)
                                        {
                                            sheet.Cells[row, costcol - 2].Value = "+ " + (distance.Circles - circle) + " круг.";
                                            sheet.Cells[row, costcol - 2].Style.Font.Bold = true;
                                        }
                                        if (partisipation.GroupId != null)
                                            foreach (Group group in groups)
                                                if (group.Id == partisipation.GroupId)
                                                    sheet.Cells[row, costcol - 1].Value = group.Name;
                                        sheet.Cells[1, 1, row, costcol].AutoFitColumns(1, 150);
                                        sheet.Cells[row, 1, row, costcol].Style.Border.Bottom.Style = EpplusSyle.ExcelBorderStyle.Medium;
                                        col = 8;
                                        circle = 0;
                                    }
                                }
                            sheet.Cells[rowcost, 1, row, costcol].Sort(0, false);
                            foreach (int partisipationId in partisipationDNFIds)
                                foreach (Partisipation partisipation in partisipations)
                                {
                                    if (partisipation.Id == partisipationId)
                                    {
                                        row++;
                                        if (partisipation.MemberId != null)
                                            foreach (Member member in members)
                                                if (member.Id == partisipation.MemberId)
                                                {
                                                    sheet.Cells[row, 2].Value = member.FamilyName;
                                                    sheet.Cells[row, 2].Style.Font.Bold = true;
                                                    sheet.Cells[row, 3].Value = member.Name;
                                                    sheet.Cells[row, 3].Style.Font.Bold = true;
                                                    sheet.Cells[row, 4].Value = member.BornDate.ToShortDateString();
                                                    sheet.Cells[row, 5].Value = member.City;
                                                    if (member.ClubId != null)
                                                        foreach (Club club in clubs)
                                                            if (member.ClubId == club.Id)
                                                                sheet.Cells[row, 6].Value = club.Name;
                                                }
                                        foreach (Models.Start start in starts)
                                            if (start.PartisipationId == partisipationId)
                                            {
                                                sheet.Cells[row, 7].Value = start.Number;
                                                foreach (Timing timing in timings)
                                                {
                                                    if (timing.StartId != null)
                                                        if (timing.StartId == start.Id)
                                                        {
                                                            if (timing.TimeFromStart != null)
                                                            {
                                                                buf = (TimeOnly)timing.TimeFromStart;
                                                                sheet.Cells[row, col].Value = buf.ToLongTimeString();
                                                            }
                                                            col++;
                                                        }
                                                }
                                                break;
                                            }
                                        if (partisipation.GroupId != null)
                                            foreach (Group group in groups)
                                                if (group.Id == partisipation.GroupId)
                                                    sheet.Cells[row, costcol - 1].Value = group.Name;
                                        sheet.Cells[row, 1].Value = "DNF";
                                        sheet.Cells[row, costcol].Value = "DNF";                                    
                                        sheet.Cells[1, 1, row, costcol].AutoFitColumns(1, 150);
                                        sheet.Cells[row, 1, row, costcol].Style.Border.Bottom.Style = EpplusSyle.ExcelBorderStyle.Medium;
                                        col = 8;
                                    }
                                }
                            foreach (int partisipationId in partisipationDNSIds)
                                foreach (Partisipation partisipation in partisipations)
                                {
                                    if (partisipation.Id == partisipationId)
                                    {
                                        row++;
                                        if (partisipation.MemberId != null)
                                            foreach (Member member in members)
                                                if (member.Id == partisipation.MemberId)
                                                {
                                                    sheet.Cells[row, 2].Value = member.FamilyName;
                                                    sheet.Cells[row, 2].Style.Font.Bold = true;
                                                    sheet.Cells[row, 3].Value = member.Name;
                                                    sheet.Cells[row, 3].Style.Font.Bold = true;
                                                    sheet.Cells[row, 4].Value = member.BornDate.ToShortDateString();
                                                    sheet.Cells[row, 5].Value = member.City;
                                                    if (member.ClubId != null)
                                                        foreach (Club club in clubs)
                                                            if (member.ClubId == club.Id)
                                                                sheet.Cells[row, 6].Value = club.Name;
                                                }
                                        foreach (Models.Start start in starts)
                                            if (start.PartisipationId == partisipationId)
                                            {
                                                sheet.Cells[row, 7].Value = start.Number;
                                                break;
                                            }
                                        if (partisipation.GroupId != null)
                                            foreach (Group group in groups)
                                                if (group.Id == partisipation.GroupId)
                                                    sheet.Cells[row, costcol - 1].Value = group.Name;
                                        sheet.Cells[row, 1].Value = "DNS";
                                        sheet.Cells[row, costcol].Value = "DNS";                           
                                        sheet.Cells[1, 1, row, costcol].AutoFitColumns(1, 150);
                                        sheet.Cells[row, 1, row, costcol].Style.Border.Bottom.Style = EpplusSyle.ExcelBorderStyle.Medium;
                                        col = 8;
                                    }
                                }
                            row = row + 2;
                            cur = 1;
                        }
                        else 
                        {
                            MessageBox.Show("Невозможно сформировать протокол по дистанции", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return false; 
                        }
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка:" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool GroupDistanceProtocol(Competition competition)
        {
            try
            {
                using (var dbContext = new RefereeHelperDbContextFactory().CreateDbContext())
                {
                    var sheet = package.Workbook.Worksheets.Add("лист 1");
                    int row = 1, col = 8, costcol = 0, cur = 1, rowcost, circles = 0;
                    List<int> partisipationIds = new List<int>();
                    List<int> partisipationDNFIds = new List<int>();
                    List<int> partisipationDNSIds = new List<int>();

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

                    var distances = dbContext.Set<Distance>().Select(x => new Distance
                    {
                        Id = x.Id,
                        Circles = x.Circles
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
                        if(partisipationIds.Count != 0 || partisipationDNFIds.Count != 0 || partisipationDNSIds.Count != 0)
                        {
                            sheet.Cells[row, 1].Value = group.Name;
                            row++;
                            rowcost = row + 1;
                            sheet.Cells[row, 1].Value = "Место";
                            sheet.Cells[row, 2].Value = "Фамилия";
                            sheet.Cells[row, 3].Value = "Имя";
                            sheet.Cells[row, 4].Value = "Год Рождения";
                            sheet.Cells[row, 5].Value = "Город";
                            sheet.Cells[row, 6].Value = "Клуб";
                            sheet.Cells[row, 7].Value = "Ст.№";
                            foreach (Distance distance in distances)
                                if (group.DistanceId == distance.Id)
                                {
                                    for (int i = 1; i < distance.Circles; i++)
                                    {
                                        sheet.Cells[row, col].Value = "Круг " + i.ToString(); col++;
                                    }
                                    break;
                                }
                            sheet.Cells[row, col].Value = "Финиш";
                            costcol = col;
                            sheet.Cells[row - 1, 1, row - 1, col].Merge = true;
                            sheet.Cells[row - 1, 1, row - 1, col].Style.HorizontalAlignment = EpplusSyle.ExcelHorizontalAlignment.Center;
                            sheet.Cells[row, 1, row, col].Style.Font.Bold = true;
                            sheet.Cells[row, 1, row, col].Style.Border.Bottom.Style = EpplusSyle.ExcelBorderStyle.Medium;
                            sheet.Cells[row, 1, row, col].Style.Border.Top.Style = EpplusSyle.ExcelBorderStyle.Thick;
                            sheet.Cells[row, 1, row, col].Style.HorizontalAlignment = EpplusSyle.ExcelHorizontalAlignment.Center;
                            sheet.Column(1).Style.HorizontalAlignment = EpplusSyle.ExcelHorizontalAlignment.Center;
                            for (int i = 4; i <= col; i++)
                                sheet.Column(i).Style.HorizontalAlignment = EpplusSyle.ExcelHorizontalAlignment.Center;
                            col = 8;
                            foreach (int partisipationId in partisipationIds)
                                foreach (Partisipation partisipation in partisipations)
                                {
                                    if (partisipation.Id == partisipationId)
                                    {
                                        row++;
                                        if (partisipation.MemberId != null)
                                            foreach (Member member in members)
                                                if (member.Id == partisipation.MemberId)
                                                {
                                                    sheet.Cells[row, 2].Value = member.FamilyName;
                                                    sheet.Cells[row, 2].Style.Font.Bold = true;
                                                    sheet.Cells[row, 3].Value = member.Name;
                                                    sheet.Cells[row, 3].Style.Font.Bold = true;
                                                    sheet.Cells[row, 4].Value = member.BornDate.ToShortDateString();
                                                    sheet.Cells[row, 5].Value = member.City;
                                                    if (member.ClubId != null)
                                                        foreach (Club club in clubs)
                                                            if (member.ClubId == club.Id)
                                                                sheet.Cells[row, 6].Value = club.Name;
                                                }
                                        foreach (Models.Start start in starts)
                                            if (start.PartisipationId == partisipationId)
                                            {
                                                sheet.Cells[row, 7].Value = start.Number;
                                                sheet.Cells[row, 1].Value = start.Place;
                                                if (start.Status == 1)
                                                    sheet.Cells[row, costcol].Style.Font.Bold = true;
                                                foreach (Timing timing in timings)
                                                {
                                                    if (timing.StartId != null)
                                                        if (timing.StartId == start.Id)
                                                        {
                                                            if (timing.TimeFromStart != null)
                                                            {
                                                                buf = (TimeOnly)timing.TimeFromStart;
                                                                sheet.Cells[row, col].Value = buf.ToLongTimeString();
                                                                circles++;
                                                            }
                                                            col++;
                                                        }
                                                }
                                                break;
                                            }
                                        if (circles < group.Distance.Circles)
                                        {
                                            sheet.Cells[row, costcol].Value = "+ " + (group.Distance.Circles - circles) + " круг.";
                                            sheet.Cells[row, costcol].Style.Font.Bold = true;
                                        }
                                        sheet.Cells[1, 1, row, costcol].AutoFitColumns(1, 150);
                                        sheet.Cells[row, 1, row, costcol].Style.Border.Bottom.Style = EpplusSyle.ExcelBorderStyle.Medium;
                                        circles = 0;
                                        col = 8;
                                    }
                                }
                            sheet.Cells[rowcost, 1, row, costcol].Sort(0, false);
                            foreach (int partisipationId in partisipationDNFIds)
                                foreach (Partisipation partisipation in partisipations)
                                {
                                    if (partisipation.Id == partisipationId)
                                    {
                                        row++;
                                        if (partisipation.MemberId != null)
                                            foreach (Member member in members)
                                                if (member.Id == partisipation.MemberId)
                                                {
                                                    sheet.Cells[row, 2].Value = member.FamilyName;
                                                    sheet.Cells[row, 2].Style.Font.Bold = true;
                                                    sheet.Cells[row, 3].Value = member.Name;
                                                    sheet.Cells[row, 3].Style.Font.Bold = true;
                                                    sheet.Cells[row, 4].Value = member.BornDate.ToShortDateString();
                                                    sheet.Cells[row, 5].Value = member.City;
                                                    if (member.ClubId != null)
                                                        foreach (Club club in clubs)
                                                            if (member.ClubId == club.Id)
                                                                sheet.Cells[row, 6].Value = club.Name;
                                                }
                                        foreach (Models.Start start in starts)
                                            if (start.PartisipationId == partisipationId)
                                            {
                                                sheet.Cells[row, 7].Value = start.Number;
                                                foreach (Timing timing in timings)
                                                {
                                                    if (timing.StartId != null)
                                                        if (timing.StartId == start.Id)
                                                        {
                                                            if (timing.TimeFromStart != null)
                                                            {
                                                                buf = (TimeOnly)timing.TimeFromStart;
                                                                sheet.Cells[row, col].Value = buf.ToLongTimeString();
                                                            }
                                                            col++;
                                                        }
                                                }
                                                break;
                                            }
                                        sheet.Cells[row, 1].Value = "DNF";
                                        sheet.Cells[1, 1, row, costcol].AutoFitColumns(1, 150);
                                        sheet.Cells[row, 1, row, costcol].Style.Border.Bottom.Style = EpplusSyle.ExcelBorderStyle.Medium;
                                        col = 8;
                                    }
                                }
                            foreach (int partisipationId in partisipationDNSIds)
                                foreach (Partisipation partisipation in partisipations)
                                {
                                    if (partisipation.Id == partisipationId)
                                    {
                                        row++;
                                        if (partisipation.MemberId != null)
                                            foreach (Member member in members)
                                                if (member.Id == partisipation.MemberId)
                                                {
                                                    sheet.Cells[row, 2].Value = member.FamilyName;
                                                    sheet.Cells[row, 2].Style.Font.Bold = true;
                                                    sheet.Cells[row, 3].Value = member.Name;
                                                    sheet.Cells[row, 3].Style.Font.Bold = true;
                                                    sheet.Cells[row, 4].Value = member.BornDate.ToShortDateString();
                                                    sheet.Cells[row, 5].Value = member.City;
                                                    if (member.ClubId != null)
                                                        foreach (Club club in clubs)
                                                            if (member.ClubId == club.Id)
                                                                sheet.Cells[row, 6].Value = club.Name;
                                                }
                                        foreach (Models.Start start in starts)
                                            if (start.PartisipationId == partisipationId)
                                            {
                                                sheet.Cells[row, 7].Value = start.Number;
                                                break;
                                            }
                                        sheet.Cells[row, 1].Value = "DNS";
                                        sheet.Cells[1, 1, row, costcol].AutoFitColumns(1, 150);
                                        sheet.Cells[row, 1, row, costcol].Style.Border.Bottom.Style = EpplusSyle.ExcelBorderStyle.Medium;
                                        col = 8;
                                    }
                                }
                            row = row + 2;
                            cur = 1;
                        }
                        else
                        {
                            MessageBox.Show("Невозможно сформировать протокол по дистанции", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return false;
                        }
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка:" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool FinshProtocol(Competition competition)
        {
            try
            {
                using (var dbContext = new RefereeHelperDbContextFactory().CreateDbContext())
                {
                    var sheet = package.Workbook.Worksheets.Add("лист 1");
                    int row = 1, col = 6, constcol = 0, rowcost = 0,cur = 1, circles = 0, placecur = 0;
                    List<int> partisipationOKIds = new List<int>();
                    List<int> partisipationDNFIds = new List<int>();
                    List<int> partisipationDNSIds = new List<int>();
                    TimeOnly buf;
                    TimeSpan timeSpan = TimeSpan.Zero;

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
                    {
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
                                    sheet.Cells[row, 1].Value = group.Name + ", " + distance.Name;
                                    sheet.Cells[row, 1, row, 2].AutoFitColumns(1, 450);
                                    sheet.Cells[row, 1, row, 2].Style.Font.Size = 20;
                                    sheet.Cells[row, 1, row, 2].Style.Font.Bold = true;
                                    row++;
                                    rowcost = row + 1;
                                    sheet.Cells[row, 1].Value = "№п/п";
                                    sheet.Cells[row, 2].Value = "Фамилия, имя";
                                    sheet.Cells[row, 3].Value = "Коллектив";
                                    sheet.Cells[row, 4].Value = "Номер";
                                    sheet.Cells[row, 5].Value = "ГР";
                                    for (int i = 1; i < distance.Circles; i++)
                                    {
                                        sheet.Cells[row, col].Value = i.ToString() + "круг"; col++;
                                    }
                                    sheet.Cells[row, col].Value = "Результат"; col++;
                                    sheet.Cells[row, col].Value = "Отставание"; col++;
                                    sheet.Cells[row, col].Value = "Место";
                                    constcol = col;
                                    sheet.Cells[row - 1, 1, row - 1, constcol].Merge = true;
                                    sheet.Cells[row, 1, row, constcol].Style.Font.Bold = true;
                                    sheet.Cells[row, 1, row, constcol].Style.Border.Bottom.Style = EpplusSyle.ExcelBorderStyle.Medium;
                                    foreach (int partisipationId in partisipationOKIds)
                                        foreach (Partisipation partisipation in partisipations)
                                        {
                                            if (partisipation.Id == partisipationId)
                                            {
                                                col = 6;
                                                circles = 0;
                                                row++;
                                                if (partisipation.MemberId != null)
                                                    foreach (Member member in members)
                                                        if (member.Id == partisipation.MemberId)
                                                        {
                                                            sheet.Cells[row, 2].Value = member.FamilyName + ", " + member.Name;
                                                            sheet.Cells[row, 5].Value = member.BornDate.ToShortDateString();
                                                            if (member.ClubId != null)
                                                                foreach (Club club in clubs)
                                                                    if (member.ClubId == club.Id)
                                                                        sheet.Cells[row, 3].Value = club.Name;
                                                        }
                                                foreach (Models.Start start in starts)
                                                    if (start.PartisipationId == partisipationId)
                                                    {
                                                        sheet.Cells[row, 4].Value = start.Number;
                                                        sheet.Cells[row, constcol].Value = start.Place;
                                                        foreach (Timing timing in timings)
                                                        {
                                                            if (timing.StartId != null)
                                                                if (start.Id == timing.StartId)
                                                                {
                                                                    if (timing.TimeFromStart != null)
                                                                    {
                                                                        buf = (TimeOnly)timing.TimeFromStart;
                                                                        sheet.Cells[row, col].Value = buf.ToLongTimeString(); col++;
                                                                        circles++;
                                                                    }
                                                                }
                                                        }
                                                        
                                                        break;
                                                    }
                                                if (circles < distance.Circles)
                                                {
                                                    sheet.Cells[row, constcol - 2].Value = "+ " + (distance.Circles - circles) + " круг.";
                                                    placecur++;
                                                }
                                            }
                                        }
                                    sheet.Cells[rowcost, 1, row, constcol].Sort(constcol - 1, false);

                                    for (int i = 0; i < partisipationOKIds.Count - placecur; i++)
                                    {
                                        if (sheet.Cells[rowcost + i, constcol - 2].Value != null)
                                            if (TimeOnly.TryParse(sheet.Cells[rowcost + i, constcol - 2].Value.ToString(), out buf))
                                            {
                                                timeSpan = buf - TimeOnly.Parse(sheet.Cells[rowcost, constcol - 2].Value.ToString());
                                                buf = TimeOnly.FromTimeSpan(timeSpan);
                                                sheet.Cells[rowcost + i, constcol - 1].Value = buf.ToLongTimeString();
                                            }
                                    }
                                    for (int i = 0; i < partisipationOKIds.Count; i++)
                                    {
                                        sheet.Cells[rowcost + i, 1].Value = cur; cur++;
                                    }

                                    foreach (int partisipationId in partisipationDNFIds)
                                        foreach (Partisipation partisipation in partisipations)
                                        {
                                            if (partisipation.Id == partisipationId)
                                            {
                                                col = 6;
                                                row++;
                                                sheet.Cells[row, 1].Value = cur; cur++;
                                                if (partisipation.MemberId != null)
                                                    foreach (Member member in members)
                                                        if (member.Id == partisipation.MemberId)
                                                        {
                                                            sheet.Cells[row, 2].Value = member.FamilyName + ", " + member.Name;
                                                            sheet.Cells[row, 5].Value = member.BornDate.ToShortDateString();
                                                            if (member.ClubId != null)
                                                                foreach (Club club in clubs)
                                                                    if (member.ClubId == club.Id)
                                                                        sheet.Cells[row, 3].Value = club.Name;
                                                        }
                                                foreach (Models.Start start in starts)
                                                    if (start.PartisipationId == partisipationId)
                                                    {
                                                        sheet.Cells[row, 4].Value = start.Number;
                                                        foreach (Timing timing in timings)
                                                        {
                                                            if (timing.StartId != null)
                                                                if (start.Id == timing.StartId)
                                                                {
                                                                    if (timing.TimeFromStart != null)
                                                                    {
                                                                        buf = (TimeOnly)timing.TimeFromStart;
                                                                        sheet.Cells[row, col].Value = buf.ToLongTimeString(); col++;
                                                                    }
                                                                }
                                                        }
                                                        break;
                                                    }
                                                sheet.Cells[row, constcol].Value = "DNF";
                                            }
                                        }
                                    foreach (int partisipationId in partisipationDNSIds)
                                        foreach (Partisipation partisipation in partisipations)
                                        {
                                            if (partisipation.Id == partisipationId)
                                            {
                                                col = 6;
                                                row++;
                                                sheet.Cells[row, 1].Value = cur; cur++;
                                                if (partisipation.MemberId != null)
                                                    foreach (Member member in members)
                                                        if (member.Id == partisipation.MemberId)
                                                        {
                                                            sheet.Cells[row, 2].Value = member.FamilyName + ", " + member.Name;
                                                            sheet.Cells[row, 5].Value = member.BornDate.ToShortDateString();
                                                            if (member.ClubId != null)
                                                                foreach (Club club in clubs)
                                                                    if (member.ClubId == club.Id)
                                                                        sheet.Cells[row, 3].Value = club.Name;
                                                        }
                                                foreach (Models.Start start in starts)
                                                    if (start.PartisipationId == partisipationId)
                                                    {
                                                        sheet.Cells[row, 4].Value = start.Number;
                                                        break;
                                                    }
                                                sheet.Cells[row, constcol].Value = "DNS";
                                            }
                                        }
                                    row = row + 2;
                                }
                                else
                                {
                                    MessageBox.Show("Невозможно сформировать финишний протокол", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                    return false;
                                }
                                sheet.Cells[1, 1, row, constcol].AutoFitColumns(1, 150);
                            }
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка:" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool saveAs(String file)
        {
            try 
            {
                file = file.Replace("\\\\", "\\");
                if (!File.Exists(file))
                {
                    package.SaveAs(new FileInfo(file));
                    MessageBox.Show("протокол был сформировон и сохранён по пути:\\n" + file, "Выполнено", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    file = file.Insert(file.Length - 5, "(1)");
                    this.saveAs(file, 1);
                }
                    
                return true; 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка:" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        private bool saveAs(String file,int i)
        {
            try
            {
                if (!File.Exists(file))
                {
                    package.SaveAs(new FileInfo(file));
                    MessageBox.Show("протокол был сформировон и сохранён по пути:" + file, "Выполнено", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    file = file.Replace("(" + i + ")", "(" + (i + 1) + ")");
                    i++;
                    this.saveAs(file, i);
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка:" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}
