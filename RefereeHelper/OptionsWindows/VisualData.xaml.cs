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

        Competition competition;
        int cilcle = 1, cilcle1  = 1;

        public VisualData(bool Type, Competition c)
        {
            InitializeComponent();
            competition = c;
            var dbContext = new RefereeHelperDbContextFactory().CreateDbContext();
            var distances = dbContext.Set<Distance>().Select(x => new Distance
            {
                Id = x.Id,
                Name = x.Name,
                Circles = x.Circles,
                Length= x.Length,
                Height= x.Height,
                StartTime = x.StartTime
            }).ToList();
            if (distances.Count != 0)
            {
                foreach (var distance in distances)
                {
                    DistanceCMB.Items.Add(distance);
                    Distance1CMB.Items.Add(distance);
                }
                DistanceCMB.SelectedIndex = 0;
                Distance1CMB.SelectedIndex = 0;
                System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();

                timer.Tick += new EventHandler(FillDistance);
                timer.Interval = new TimeSpan(0, 0, 1);
                timer.Start();

                System.Windows.Threading.DispatcherTimer timer1 = new System.Windows.Threading.DispatcherTimer();

                timer1.Tick += new EventHandler(FillDistance1);
                timer1.Interval = new TimeSpan(0, 0, 1);
                timer1.Start();
            }
        }

        private void FillDistance(object sender, EventArgs e)
        {
            var dbContext = new RefereeHelperDbContextFactory().CreateDbContext();

            TimeOnly buf;

            MainDB.Items.Clear();

            var timings = dbContext.Set<Timing>().Select(x => new Timing
            {
                Id = x.Id,
                TimeFromStart = x.TimeFromStart,
                IsFinish = x.IsFinish,
                Circle = x.Circle,
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

            DataItems dataItem;

            Distance distance = (Distance)DistanceCMB.SelectedItem;
            if (distance != null)
            {
                foreach (Timing timing in timings)
                    if (timing.Start?.Partisipation.Group?.DistanceId == distance.Id)
                        if (timing.Circle > cilcle && timing.Circle != null)
                        {
                            cilcle = (int)timing.Circle;
                            break;
                        }
                foreach (Timing timing in timings)
                {
                    if (timing.Start?.Partisipation.Group?.DistanceId == distance.Id)
                    {
                        if (timing.Circle == cilcle)
                        {
                            dataItem = new DataItems();
                            if (timing.TimeFromStart != null)
                            {
                                buf = (TimeOnly)timing.TimeFromStart;
                                dataItem.TimeFromStart = buf.ToShortTimeString();
                            }
                            if (timing.Start?.Partisipation?.Member?.Name != null)
                                dataItem.Name = timing.Start?.Partisipation?.Member?.Name;
                            if (timing.Start?.Partisipation?.Member?.FamilyName != null)
                                dataItem.FamalyName = timing.Start?.Partisipation?.Member?.FamilyName;
                            if (timing.Start?.Number != null)
                                dataItem.Number = timing.Start?.Number.ToString();
                            MainDB.Items.Add(dataItem);
                        }
                    }
                }
            }
        }
        private void FillDistance1(object sender, EventArgs e)
        {
            var dbContext = new RefereeHelperDbContextFactory().CreateDbContext();

            TimeOnly buf;

            Main1DB.Items.Clear();

            var timings = dbContext.Set<Timing>().Select(x => new Timing
            {
                Id = x.Id,
                TimeFromStart = x.TimeFromStart,
                IsFinish = x.IsFinish,
                Circle = x.Circle,
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

            DataItems dataItem;

            Distance distance = (Distance)Distance1CMB.SelectedItem;
            if (distance != null)
            {
                foreach (Timing timing in timings)
                    if (timing.Start?.Partisipation.Group?.DistanceId == distance.Id)
                        if (timing.Circle > cilcle1 && timing.Circle != null)
                        {
                            cilcle1 = (int)timing.Circle;
                            break;
                        }
                foreach (Timing timing in timings)
                {
                    if (timing.Start?.Partisipation.Group?.DistanceId == distance.Id)
                    {
                        if (timing.Circle == cilcle1)
                        {
                            dataItem = new DataItems();
                            if (timing.TimeFromStart != null)
                            {
                                buf = (TimeOnly)timing.TimeFromStart;
                                dataItem.TimeFromStart = buf.ToShortTimeString();
                            }
                            if (timing.Start?.Partisipation?.Member?.Name != null)
                                dataItem.Name = timing.Start?.Partisipation?.Member?.Name;
                            if (timing.Start?.Partisipation?.Member?.FamilyName != null)
                                dataItem.FamalyName = timing.Start?.Partisipation?.Member?.FamilyName;
                            if (timing.Start?.Number != null)
                                dataItem.Number = timing.Start?.Number.ToString();
                            Main1DB.Items.Add(dataItem);
                        }
                    }
                }
            }
        }
    }
}
