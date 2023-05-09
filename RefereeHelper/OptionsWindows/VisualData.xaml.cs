using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using RefereeHelper;
using RefereeHelper.EntityFramework;
using RefereeHelper.EntityFramework.Services;
using RefereeHelper.Models;

namespace RefereeHelper.OptionsWindows
{
    /// <summary>
    /// Логика взаимодействия для VisualData.xaml
    /// </summary>
    public partial class VisualData : Window
    {
        class DataItems
        {
            public string Name { get; set; }
            public string FamalyName { get; set; }
            public string Number { get; set; }
            public string TimeFromStart { get; set; }
        }

        public VisualData(bool Type, Competition competition)
        {
            InitializeComponent();

            if (Type)
            {
                TimerCallback tm = new TimerCallback(FillGroup);

                Timer timer = new Timer(tm, competition, 0, 1000);
            }
            else
            {
                TimerCallback tm = new TimerCallback(FillDistance);

                Timer timer = new Timer(tm, competition, 0, 1000);
            }
        }

        void FillDistance(object obj)
        {
            var dbContext = new RefereeHelperDbContextFactory().CreateDbContext();

            Competition competition = (Competition)obj;
            TimeOnly buf;

            MainDB.Dispatcher.Invoke(() => { MainDB.Items.Clear(); });

            var distances = dbContext.Set<Distance>().Select(x => new Distance
            {
                Id = x.Id,
                Name = x.Name
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
                        Id = x.Start.Partisipation.Id,
                        CompetitionId = x.Start.Partisipation.CompetitionId,
                        Group = new Group
                        {
                            DistanceId = x.Start.Partisipation.Group.DistanceId
                        },
                        Member = new Member
                        {
                            Name = x.Start.Partisipation.Member.Name,
                            FamilyName = x.Start.Partisipation.Member.FamilyName
                        }
                    },
                    Number = x.Start.Number
                }
            }).ToList();

            foreach (Distance distance in distances)
                if (timings.Last().Start?.Partisipation.Group?.DistanceId == distance.Id &&
                    timings.Last().Start?.Partisipation.CompetitionId == competition.Id)
                {
                    Libal.Dispatcher.Invoke(() => { Libal.Text = distance.Name; });
                    foreach (Timing timing in timings)
                        if (timing.Start?.Partisipation.Group?.DistanceId == distance.Id)
                        {
                            MainDB.Dispatcher.Invoke(() =>
                            {
                                if (timing.TimeFromStart != null)
                                    buf = (TimeOnly)timing.TimeFromStart;
                                MainDB.Items.Add(new DataItems
                                {
                                    Name = timing.Start?.Partisipation.Member.Name,
                                    FamalyName = timing.Start?.Partisipation.Member?.FamilyName,
                                    Number = timing.Start?.Number.ToString(),
                                    TimeFromStart = buf.ToLongTimeString()
                                });
                            });
                        }
                }
        }

        void FillGroup(object obj)
        {
            var dbContext = new RefereeHelperDbContextFactory().CreateDbContext();

            Competition competition = (Competition)obj;
            TimeOnly buf;

            MainDB.Dispatcher.Invoke(() => { MainDB.Items.Clear(); });

            var groups = dbContext.Set<Group>().Select(x => new Group
            {
                Id = x.Id,
                Name = x.Name
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
                        Id = x.Start.Partisipation.Id,
                        GroupId = x.Start.Partisipation.GroupId,
                        CompetitionId = x.Start.Partisipation.CompetitionId,
                        Member = new Member
                        {
                            Name = x.Start.Partisipation.Member.Name,
                            FamilyName = x.Start.Partisipation.Member.FamilyName
                        }
                    },
                    Number = x.Start.Number
                }
            }).ToList();

            foreach (Group group in groups)
            {
                if (timings.Last().Start?.Partisipation.GroupId == group.Id && 
                    timings.Last().Start?.Partisipation.CompetitionId == competition.Id)
                {
                    Libal.Dispatcher.Invoke(() => { Libal.Text = group.Name; });
                    foreach (Timing timing in timings)
                        if (timing.Start?.Partisipation.GroupId == group.Id)
                        {
                            if (timing.TimeFromStart != null)
                                buf = (TimeOnly)timing.TimeFromStart;
                            MainDB.Dispatcher.Invoke(() =>
                            {
                                MainDB.Items.Add(new DataItems
                                {
                                    Name = timing.Start?.Partisipation.Member.Name,
                                    FamalyName = timing.Start?.Partisipation.Member?.FamilyName,
                                    Number = timing.Start?.Number.ToString(),
                                    TimeFromStart = buf.ToLongTimeString()
                                });
                            });
                        }
                }
            }
        }
    }
}
