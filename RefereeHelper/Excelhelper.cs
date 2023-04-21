using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using RefereeHelper.EntityFramework;
using RefereeHelper.EntityFramework.Services;
using RefereeHelper.Models;
using RefereeHelper.OptionsWindows;
using Epplus = OfficeOpenXml;
using EpplusSyle = OfficeOpenXml.Style;


namespace RefereeHelper
{
    public class Excelhelper
    {
        private Epplus.ExcelPackage package;
        TimeOnly buf;

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
                        Name = x.Name
                    }).ToList();

                    var starts = dbContext.Set<Start>().Select(x => new Start
                    {
                        Id = x.Id,
                        Number = x.Number,
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
                        Start = new Start
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
                                        sheet.Cells[row, 2].Value = partisipation.Member?.FamilyName + ", " + partisipation.Member?.Name;
                                        sheet.Cells[row, 2].Style.Font.Bold = true;
                                        sheet.Cells[row, 3].Value = partisipation.Member?.Club?.Name;
                                        sheet.Cells[row, 3].Style.Font.Bold = true;
                                        sheet.Cells[row, 4].Value = partisipation.Member?.Discharge?.Name;
                                        foreach (Start start in starts)
                                            if (start.Partisipation.Id == partisipationId)
                                            {
                                                sheet.Cells[row, 5].Value = start.Number;
                                                break;
                                            }
                                        sheet.Cells[row, 6].Value = partisipation.Member?.BornDate.ToShortDateString();
                                        foreach (Timing timing in timings)
                                        {
                                            if (timing.Start?.Partisipation.Id == partisipationId)
                                            {
                                                if (timing.IsFinish == true)
                                                {
                                                    buf = (TimeOnly)timing.TimeFromStart;
                                                    sheet.Cells[row, 7].Value = buf.ToLongTimeString();
                                                    break;
                                                }
                                            }
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
                MessageBox.Show("Произошла ошибка:\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    int row = 1, col = 8, costcol = 0, cur = 1;
                    List<int> partisipationIds = new List<int>();

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

                    var starts = dbContext.Set<Start>().Select(x => new Start
                    {
                        Id = x.Id,
                        Number = x.Number,
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
                        Start = new Start
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
                                        partisipationIds.Add(partisipation.Id);
                        if (partisipationIds.Count != 0)
                        {
                            sheet.Cells[row, 1].Value = distance.Name;
                            row++;
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
                                        sheet.Cells[row, 1].Value = cur; cur++;
                                        sheet.Cells[row, 2].Value = partisipation.Member?.FamilyName;
                                        sheet.Cells[row, 2].Style.Font.Bold = true;
                                        sheet.Cells[row, 3].Value = partisipation.Member?.Name;
                                        sheet.Cells[row, 3].Style.Font.Bold = true;
                                        sheet.Cells[row, 4].Value = partisipation.Member?.BornDate.ToShortDateString();
                                        sheet.Cells[row, 5].Value = partisipation.Member?.City;
                                        sheet.Cells[row, 6].Value = partisipation.Member?.Club?.Name;
                                        foreach (Start start in starts)
                                            if (start.Partisipation.Id == partisipationId)
                                            {
                                                sheet.Cells[row, 7].Value = start.Number;
                                                break;
                                            }
                                        foreach (Timing timing in timings)
                                        {
                                            if (timing.Start?.Partisipation.Id == partisipationId)
                                            {
                                                buf = (TimeOnly)timing.TimeFromStart;
                                                sheet.Cells[row, col].Value = buf.ToLongTimeString();
                                                col++;
                                                if (timing.IsFinish == true)
                                                {
                                                    sheet.Cells[row, col - 1].Style.Font.Bold = true;
                                                    sheet.Cells[row, col + 1].Value = timing.Place;
                                                }
                                            }
                                        }
                                        sheet.Cells[row, costcol - 1].Value = partisipation.Group?.Name;
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
                MessageBox.Show("Произошла ошибка:\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    int row = 1, col = 7, constcol = 0, rowcost = 0;
                    List<int> partisipationIds = new List<int>();
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

                    var starts = dbContext.Set<Start>().Select(x => new Start
                    {
                        Id = x.Id,
                        Number = x.Number,
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
                        Start = new Start
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
                            {
                                foreach (Partisipation partisipation in partisipations)
                                    if (group.Id == partisipation.Group?.Id && partisipation.Competition?.Id == competition.Id)
                                        partisipationIds.Add(partisipation.Id);
                                if (partisipationIds.Count != 0)
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
                                    sheet.Cells[row, 6].Value = "Отсечка";
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
                                    foreach (int partisipationId in partisipationIds)
                                        foreach (Partisipation partisipation in partisipations)
                                        {
                                            if (partisipation.Id == partisipationId)
                                            {
                                                col = 7;
                                                row++;
                                                sheet.Cells[row, 2].Value = partisipation.Member?.FamilyName + ", " + partisipation.Member?.Name;
                                                sheet.Cells[row, 3].Value = partisipation.Member?.Club?.Name;
                                                foreach (Start start in starts)
                                                    if (start.Partisipation.Id == partisipationId)
                                                    {
                                                        sheet.Cells[row, 4].Value = start.Number;
                                                        break;
                                                    }
                                                sheet.Cells[row, 5].Value = partisipation.Member?.BornDate.ToShortDateString();
                                                foreach (Timing timing in timings)
                                                {
                                                    if (timing.Start?.Partisipation.Id == partisipationId)
                                                    {
                                                        buf = (TimeOnly)timing.TimeFromStart;
                                                        sheet.Cells[row, col].Value = buf.ToLongTimeString(); col++;
                                                        if (timing.IsFinish == true)
                                                        {
                                                            sheet.Cells[row, col + 1].Value = timing.Place;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    sheet.Cells[rowcost, 1, row, constcol].Sort(constcol - 3, false);

                                    for (int i = 0; i < partisipationIds.Count; i++)
                                    {
                                        sheet.Cells[rowcost + i, 1].Value = i + 1;
                                        if (sheet.Cells[rowcost + i, constcol - 2].Value != null)
                                            if (TimeOnly.TryParse(sheet.Cells[rowcost + i, constcol - 2].Value.ToString(), out buf))
                                            {
                                                timeSpan = buf - TimeOnly.Parse(sheet.Cells[rowcost, constcol - 2].Value.ToString());
                                                buf = TimeOnly.FromTimeSpan(timeSpan);
                                                sheet.Cells[rowcost + i, constcol - 1].Value = buf.ToLongTimeString();
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
                MessageBox.Show("Произошла ошибка:\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show("Произошла ошибка:\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    MessageBox.Show("протокол был сформировон и сохранён по пути:\\n" + file, "Выполнено", MessageBoxButton.OK, MessageBoxImage.Information);
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
                MessageBox.Show("Произошла ошибка:\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}
