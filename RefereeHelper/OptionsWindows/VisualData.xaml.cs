using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using RefereeHelper.Views;

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
                StartId = x.StartId
            }).ToList();

            var starts = dbContext.Set<Models.Start>().Select(x => new Models.Start
            {
                Id = x.Id,
                PartisipationId = x.PartisipationId,
                Number = x.Number
            }).ToList();

            var partisipations = dbContext.Set<Partisipation>().Select(x => new Partisipation
            {
                Id = x.Id,
                CompetitionId = x.CompetitionId,
                GroupId = x.GroupId,
                MemberId = x.MemberId
            }).ToList();

            var groups = dbContext.Set<Models.Group>().Select(x => new Models.Group
            {
                Id = x.Id,
                DistanceId = x.DistanceId
            }).ToList();

            var members = dbContext.Set<Member>().Select(x => new Member
            {
                Id = x.Id,
                Name = x.Name,
                FamilyName = x.FamilyName
            }).ToList();

            DataItems dataItem;

            Distance distance = (Distance)DistanceCMB.SelectedItem;
            if (distance != null)
            {
                foreach (Timing timing in timings)
                    if (timing.Circle != null)
                        if (timing.StartId != null)
                            foreach (Models.Start start in starts)
                                if (timing.StartId == start.Id)
                                    foreach (Partisipation partisipation in partisipations)
                                        if (start.PartisipationId == partisipation.Id)
                                            if (partisipation.GroupId != null)
                                                foreach (Models.Group group in groups)
                                                    if (group.Id == partisipation.GroupId)
                                                        if (group.DistanceId == distance.Id)
                                                            if (timing.Circle > cilcle)
                                                            {
                                                                cilcle = (int)timing.Circle;
                                                                goto exit;
                                                            }
                exit:
                foreach (Timing timing in timings)
                {
                    if (timing.StartId != null)
                        foreach (Models.Start start in starts)
                            if (timing.StartId == start.Id)
                                foreach (Partisipation partisipation in partisipations)
                                    if (start.PartisipationId == partisipation.Id)
                                        if (partisipation.GroupId != null)
                                            foreach (Models.Group group in groups)
                                                if (group.Id == partisipation.GroupId)
                                                    if (group.DistanceId == distance.Id)
                                                    {
                                                        if (timing.Circle == cilcle)
                                                        {
                                                            dataItem = new DataItems();
                                                            if (timing.TimeFromStart != null)
                                                            {
                                                                buf = (TimeOnly)timing.TimeFromStart;
                                                                dataItem.TimeFromStart = buf.ToShortTimeString();
                                                            }
                                                            if (partisipation.MemberId != null)
                                                                foreach (Member member in members)
                                                                    if (member.Id == partisipation.MemberId)
                                                                    {
                                                                        dataItem.Name = member.Name;
                                                                        dataItem.FamalyName = member.FamilyName;
                                                                    }
                                                            if (start.Number != null)
                                                                dataItem.Number = start.Number.ToString();
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
                StartId = x.StartId
            }).ToList();

            var starts = dbContext.Set<Models.Start>().Select(x => new Models.Start
            {
                Id = x.Id,
                PartisipationId = x.PartisipationId,
                Number = x.Number
            }).ToList();

            var partisipations = dbContext.Set<Partisipation>().Select(x => new Partisipation
            {
                Id = x.Id,
                CompetitionId = x.CompetitionId,
                GroupId = x.GroupId,
                MemberId = x.MemberId
            }).ToList();

            var groups = dbContext.Set<Models.Group>().Select(x => new Models.Group
            {
                Id = x.Id,
                DistanceId = x.DistanceId
            }).ToList();

            var members = dbContext.Set<Member>().Select(x => new Member
            {
                Id = x.Id,
                Name = x.Name,
                FamilyName = x.FamilyName
            }).ToList();

            DataItems dataItem;

            Distance distance = (Distance)Distance1CMB.SelectedItem;
            if (distance != null)
            {
                foreach (Timing timing in timings)
                    if (timing.Circle != null)
                        if (timing.StartId != null)
                            foreach (Models.Start start in starts)
                                if (timing.StartId == start.Id)
                                    foreach (Partisipation partisipation in partisipations)
                                        if (start.PartisipationId == partisipation.Id)
                                            if (partisipation.GroupId != null)
                                                foreach (Models.Group group in groups)
                                                    if (group.Id == partisipation.GroupId)
                                                        if (group.DistanceId == distance.Id)
                                                            if (timing.Circle > cilcle)
                                                            {
                                                                cilcle = (int)timing.Circle;
                                                                goto exit;
                                                            }
                exit:
                foreach (Timing timing in timings)
                {
                    if (timing.StartId != null)
                        foreach (Models.Start start in starts)
                            if (timing.StartId == start.Id)
                                foreach (Partisipation partisipation in partisipations)
                                    if (start.PartisipationId == partisipation.Id)
                                        if (partisipation.GroupId != null)
                                            foreach (Models.Group group in groups)
                                                if (group.Id == partisipation.GroupId)
                                                    if (group.DistanceId == distance.Id)
                                                    {
                                                        if (timing.Circle == cilcle)
                                                        {
                                                            dataItem = new DataItems();
                                                            if (timing.TimeFromStart != null)
                                                            {
                                                                buf = (TimeOnly)timing.TimeFromStart;
                                                                dataItem.TimeFromStart = buf.ToShortTimeString();
                                                            }
                                                            if (partisipation.MemberId != null)
                                                                foreach (Member member in members)
                                                                    if (member.Id == partisipation.MemberId)
                                                                    {
                                                                        dataItem.Name = member.Name;
                                                                        dataItem.FamalyName = member.FamilyName;
                                                                    }
                                                            if (start.Number != null)
                                                                dataItem.Number = start.Number.ToString();
                                                            Main1DB.Items.Add(dataItem);
                                                        }
                                                    }
                }
            }
        }
    }
}
