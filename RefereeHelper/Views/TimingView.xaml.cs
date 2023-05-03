using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RefereeHelper.Models;
using System;
using System.IO;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Data;
using RefereeHelper.EntityFramework;
using System.Linq;
using RefereeHelper.EntityFramework.Services;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RefereeHelper.OptionsWindows;
using System.Security.Policy;
using System.Windows.Data;

namespace RefereeHelper.Views
{
    /// <summary>
    /// Логика взаимодействия для TimingView.xaml
    /// </summary>
    public partial class TimingView : UserControl
    {
        /// <summary>
        /// Структура для заполнения таблицы участниками
        /// </summary>
        struct MemberDataItem
        {
            public string FamilyName { get; set; }
            public string MemberName { get; set; }
            public string SecondName { get; set; }
            public string BornDate { get; set; }
            public string City { get; set; }
            public string Phone { get; set; }
            public string ClubName { get; set; }
            public string DischargeName { get; set; }
        }

        struct DistanceDataItem
        {
            public string GroupName { get; set; }
            public string Gender { get; set; }
            public string Age { get; set; }
            public string DistanceName { get; set; }
            public string Length { get; set; }
            public string Height { get; set; }
            public string Circles { get; set; }
            public string StartTime { get; set; }  
        }

        struct TimingDataItem
        {
            public string Id { get; set; }
            public string FamilyName { get; set; }
            public string MemberName { get; set; }
            public string Team { get; set; }
            public string Startnumber { get; set; }
            public string Chip { get; set; }
            public string StartTime { get; set; }
            public string TimeNow { get; set; }
            public string TimeFromStart { get; set; }
            public string Circle { get; set; }
            public string CircleTime { get; set; }
            public string Place { get; set; }
            public string PlaceAbsolute { get; set; }
            public string IsFinish { get; set; }     
        }

        //показатель указывающий на то чем заполнена сейчас таблица: участники = 0, дистанции = 1, результаты = 2 
        byte position = 0;

        public TimingView()
        {
            InitializeComponent();
            dt.Tick += new EventHandler(dtTick);
            dt.Interval = new TimeSpan(0, 0, 0, 0, 1);
            if (FillCompList())
            {
                CompList.SelectedIndex = 0;
                LoadEvents();
                MembersFill();
                //LoadData();
            }
            else
            {
                MemberFillBut.IsEnabled = false;
                DistanceFillBut.IsEnabled = false;
                TimingFillBut.IsEnabled = false;
                ProtocolExcelBut.IsEnabled = false;
                ProtocolWordBut.IsEnabled = false;
                VisualGroupBut.IsEnabled = false;
                VisualDistanceBut.IsEnabled = false;
            }

        }
        int countOfStartingPeople = 0, countOfFinishingPeople = 0;

        /// <summary>
        /// Заполнение списка мероприятий
        /// </summary>
        /// <returns>true если мероприятия есть, false если нет</returns>
        bool FillCompList()
        {
            using (var dbContext = new RefereeHelperDbContextFactory().CreateDbContext())
            {
                var competitions = dbContext.Set<Competition>().Select(x => new Competition
                {
                    Id = x.Id,
                    Name = x.Name,
                    Date = x.Date,
                    Place = x.Place,
                    Organizer = x.Organizer,
                    Judge = x.Judge,
                    Secretary = x.Secretary,
                    TypeAge = x.TypeAge
                }).ToList();

                if (competitions.Count != 0)
                {
                    foreach (Competition competition in competitions)
                    {
                        CompList.Items.Add(competition);
                    }
                    return true;
                }
                else
                    return false;
                
            }
        }
        
        /// <summary>
        /// заполнение таблицы участниками
        /// </summary>
        void MembersFill()
        {
            using (var dbContext = new RefereeHelperDbContextFactory().CreateDbContext())
            {
                TeamTimer.Items.Clear();
                TeamTimer.Columns.Clear();

                var partisipations = dbContext.Set<Partisipation>().Select(x => new Partisipation
                {
                    Id = x.Id,
                    Competition = new Competition
                    {
                        Id = x.Competition.Id,
                    },
                    Member = new Member
                    {
                        Id = x.Member.Id,
                        FamilyName = x.Member.FamilyName,
                        Name = x.Member.Name,
                        SecondName= x.Member.SecondName,
                        BornDate = x.Member.BornDate,
                        City= x.Member.City,
                        Phone= x.Member.Phone,
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

                Competition competition = (Competition)CompList.SelectedItem;
                MemberDataItem buf = new MemberDataItem();
                var column = new DataGridTextColumn();

                column.Header = "Фамилия";
                column.Binding = new Binding("FamilyName");
                TeamTimer.Columns.Add(column);
                column = new DataGridTextColumn();
                column.Header = "Имя";
                column.Binding = new Binding("MemberName");
                TeamTimer.Columns.Add(column);
                column = new DataGridTextColumn();
                column.Header = "Отчество";
                column.Binding = new Binding("SecondName");
                TeamTimer.Columns.Add(column);
                column = new DataGridTextColumn();
                column.Header = "Дата Рождения";
                column.Binding = new Binding("BornDate");
                TeamTimer.Columns.Add(column);
                column = new DataGridTextColumn();
                column.Header = "Город";
                column.Binding = new Binding("City");
                TeamTimer.Columns.Add(column);
                column = new DataGridTextColumn();
                column.Header = "Клуб";
                column.Binding = new Binding("ClubName");
                TeamTimer.Columns.Add(column);
                column = new DataGridTextColumn();
                column.Header = "Разряд";
                column.Binding = new Binding("DischargeName");
                TeamTimer.Columns.Add(column);
                foreach (Partisipation partisipation in partisipations)
                {
                    if (partisipation.Competition.Id == competition.Id)
                    {
                        buf.FamilyName = partisipation.Member?.FamilyName;
                        buf.MemberName = partisipation.Member?.Name;
                        buf.SecondName = partisipation.Member?.SecondName;
                        buf.BornDate = partisipation.Member?.BornDate.ToShortDateString();
                        buf.City = partisipation.Member?.Phone;
                        buf.ClubName = partisipation.Member?.Club?.Name;
                        buf.DischargeName= partisipation.Member?.Discharge?.Name;
                        TeamTimer.Items.Add(buf);
                    }
                }
            }
        }

        void DistanceFill()
        {
            using (var dbContext = new RefereeHelperDbContextFactory().CreateDbContext())
            {
                TeamTimer.Items.Clear();
                TeamTimer.Columns.Clear();
                var groups = dbContext.Set<Models.Group>().Select(x => new Models.Group
                {
                    Id = x.Id,
                    Name = x.Name,
                    Gender = x.Gender,
                    StartAge = x.StartAge,
                    EndAge = x.EndAge,
                    Distance = new Distance
                    {
                        Id = x.Distance.Id,
                        Name = x.Distance.Name,
                        Length = x.Distance.Length,
                        Height = x.Distance.Height,
                        Circles = x.Distance.Circles,
                        StartTime = x.Distance.StartTime
                    }
                }).ToList();
                var partisipations = dbContext.Set<Partisipation>().Select(x => new Partisipation
                {
                    Id = x.Id,
                    Competition = new Competition
                    {
                        Id = x.Competition.Id,
                    },
                    Group = new Models.Group
                    {
                        Id = x.Group.Id
                    }
                }).ToList();

                Competition competition = (Competition)CompList.SelectedItem;
                DistanceDataItem buf = new DistanceDataItem();
                var column = new DataGridTextColumn();

                column.Header = "Группа";
                column.Binding = new Binding("GroupName");
                TeamTimer.Columns.Add(column);
                column = new DataGridTextColumn();
                column.Header = "Пол";
                column.Binding = new Binding("Gender");
                TeamTimer.Columns.Add(column);
                column = new DataGridTextColumn();
                column.Header = "Возрост";
                column.Binding = new Binding("Age");
                TeamTimer.Columns.Add(column);
                column = new DataGridTextColumn();
                column.Header = "Дистанция";
                column.Binding = new Binding("DistanceName");
                TeamTimer.Columns.Add(column);
                column = new DataGridTextColumn();
                column.Header = "Длина";
                column.Binding = new Binding("Length");
                TeamTimer.Columns.Add(column);
                column = new DataGridTextColumn();
                column.Header = "Высота";
                column.Binding = new Binding("Height");
                TeamTimer.Columns.Add(column);
                column = new DataGridTextColumn();
                column.Header = "Круги";
                column.Binding = new Binding("Circles");
                TeamTimer.Columns.Add(column);
                column = new DataGridTextColumn();
                column.Header = "Время старта";
                column.Binding = new Binding("StartTime");
                TeamTimer.Columns.Add(column);

                foreach (Models.Group group in groups)
                {
                    foreach(Partisipation partisipation in partisipations)
                    {
                        if (group.Id == partisipation.Group?.Id)
                        {
                            if (partisipation.Competition.Id == competition.Id)
                            {
                                buf.GroupName = group.Name;
                                if (group.Gender)
                                    buf.Gender = "М";
                                else
                                    buf.Gender = "Ж";
                                buf.Age = group.StartAge.ToString() + "-" + group.EndAge.ToString();
                                buf.DistanceName = group.Distance.Name;
                                buf.Length = group.Distance.Length.ToString() + " м.";
                                buf.Height = group.Distance.Height.ToString() + " м.";
                                buf.Circles = group.Distance.Circles.ToString();
                                buf.StartTime = group.Distance.StartTime.ToShortTimeString();
                                TeamTimer.Items.Add(buf);
                                break;
                            }
                        }
                    }
                }
            }
        }

        void TimingFill()
        {
            using (var dbContext = new RefereeHelperDbContextFactory().CreateDbContext())
            {
                TeamTimer.Items.Clear();
                TeamTimer.Columns.Clear();

                var timings = dbContext.Set<Timing>().Select(x => new Timing
                {
                    Id = x.Id,
                    TimeNow = x.TimeNow,
                    TimeFromStart = x.TimeFromStart,
                    Circle = x.Circle,
                    CircleTime = x.CircleTime,
                    Place = x.Place,
                    PlaceAbsolute = x.PlaceAbsolute,
                    IsFinish = x.IsFinish,
                    Start = new Models.Start
                    {
                        Id = x.Start.Id,
                        Number = x.Start.Number,
                        Chip = x.Start.Chip,
                        StartTime = x.Start.StartTime,
                        Team = new Team
                        {
                            Id = x.Start.Team.Id,
                            Name = x.Start.Team.Name
                        },
                        Partisipation = new Partisipation
                        {
                            Id = x.Start.Partisipation.Id,
                            Competition = new Competition
                            {
                                Id = x.Start.Partisipation.Competition.Id
                            },
                            Member = new Member
                            {
                                Id = x.Start.Partisipation.Member.Id,
                                Name = x.Start.Partisipation.Member.Name,
                                FamilyName = x.Start.Partisipation.Member.FamilyName
                            }
                        }
                    }
                }).ToList();

                Competition competition = (Competition)CompList.SelectedItem;
                DateTime timebuf = DateTime.MinValue;
                TimeOnly timenowbuf = new TimeOnly();
                TimingDataItem buf = new TimingDataItem();
                var column = new DataGridTextColumn();


                column.Header = "Id";
                column.Binding = new Binding("Id");
                TeamTimer.Columns.Add(column);
                column = new DataGridTextColumn();
                column.Header = "Фамилия";
                column.Binding = new Binding("FamilyName");
                TeamTimer.Columns.Add(column);
                column = new DataGridTextColumn();
                column.Header = "Имя";
                column.Binding = new Binding("MemberName");
                TeamTimer.Columns.Add(column);
                column = new DataGridTextColumn();
                column.Header = "Команда";
                column.Binding = new Binding("Team");
                TeamTimer.Columns.Add(column);
                column = new DataGridTextColumn();
                column.Header = "Старт.№";
                column.Binding = new Binding("Startnumber");
                TeamTimer.Columns.Add(column);
                column = new DataGridTextColumn();
                column.Header = "Чип";
                column.Binding = new Binding("Chip");
                TeamTimer.Columns.Add(column);
                column = new DataGridTextColumn();
                column.Header = "Врямя старта";
                column.Binding = new Binding("StartTime");
                TeamTimer.Columns.Add(column);
                column = new DataGridTextColumn();
                column.Header = "Время";
                column.Binding = new Binding("TimeNow");
                TeamTimer.Columns.Add(column);
                column = new DataGridTextColumn();
                column.Header = "Время со страта";
                column.Binding = new Binding("TimeFromStart");
                TeamTimer.Columns.Add(column);
                column = new DataGridTextColumn();
                column.Header = "Круг";
                column.Binding = new Binding("Circle");
                TeamTimer.Columns.Add(column);
                column = new DataGridTextColumn();
                column.Header = "Время круга";
                column.Binding = new Binding("CircleTime");
                TeamTimer.Columns.Add(column);
                column = new DataGridTextColumn();
                column.Header = "Место";
                column.Binding = new Binding("Place");
                TeamTimer.Columns.Add(column);
                column = new DataGridTextColumn();
                column.Header = "Абсолютное место";
                column.Binding = new Binding("PlaceAbsolute");
                TeamTimer.Columns.Add(column);
                column = new DataGridTextColumn();
                column.Header = "Финиш";
                column.Binding = new Binding("IsFinish");
                TeamTimer.Columns.Add(column);

                foreach (Timing timing in timings)
                {
                    if (competition.Id == timing.Start.Partisipation.Competition.Id)
                    {
                        timebuf = (DateTime)timing.Start?.StartTime;
                        buf.Id = timing.Id.ToString();
                        buf.FamilyName = timing.Start?.Partisipation.Member?.FamilyName;
                        buf.MemberName = timing.Start?.Partisipation.Member?.Name;
                        buf.Team = timing.Start?.Team?.Name;
                        buf.Startnumber = timing.Start?.Number.ToString();
                        buf.Chip = timing.Start?.Chip;
                        buf.StartTime = timebuf.ToShortTimeString();
                        if (timing.TimeNow != null)
                        {
                            timenowbuf = (TimeOnly)timing.TimeNow;
                            buf.TimeNow = timenowbuf.ToLongTimeString();
                        }
                        if (timing.TimeFromStart != null)
                        {
                            timenowbuf = (TimeOnly)timing.TimeFromStart;
                            buf.TimeFromStart = timenowbuf.ToLongTimeString();
                        }
                        buf.Circle = timing.Circle.ToString();
                        if (timing.CircleTime != null)
                        {
                            timenowbuf = (TimeOnly)timing.CircleTime;
                            buf.CircleTime = timenowbuf.ToLongTimeString();
                        }
                        buf.Place = timing.Place.ToString();
                        buf.PlaceAbsolute = timing.PlaceAbsolute.ToString();
                        if (timing.IsFinish != null)
                        {
                            if (timing.IsFinish == true)
                                buf.IsFinish = "да";
                            else
                                buf.IsFinish = "нет";
                        }
                        TeamTimer.Items.Add(buf);
                    }
                }
            }
        }

        /// <summary>
        /// Функция ручного добавления добавления данных в тайминг.
        /// </summary>
        /// <returns>Возвращает объект типа Timing, который был добавлен в базу данных.</returns>
        Timing AddData()
        {
            return DataService.Create(new Timing { TimeNow = TimeOnly.FromDateTime(DateTime.Now) }).Result;
        }



        List<string> idsOfFinishingPeople = new List<string>();

        int i = 0;
        //DateTable для записи тайминга в бд
        void LoadData()//я создал эту фунцкию для обновления таблицы, вывода данных
        {
            i = 0;
            using (var dbContext = new RefereeHelperDbContextFactory().CreateDbContext())
            {
                var timings = dbContext.Timings.Include(x => x.Start).ThenInclude(y => y.Partisipation).ThenInclude(z => z.Member).ToList();
                var teams = dbContext.Teams.ToList();
                TeamTimer.DataContext = timings;
                TeamTimer.ItemsSource = timings;
                /*foreach (var t in timings)
                {
                    i++;
                    Console.WriteLine($"{i}\n\tStart Number:{t.Start?.Number}" +
                                         $"\n\tChip number:{t.Start?.Chip}" +
                                         $"\n\tTime now:{t.TimeNow}" +//возможно его надо не отсюда брать, но это мелочи
                                         $"\n\tTime from start:{t.TimeFromStart}" +
                                         $"\n\tName:{t.Start?.Partisipation?.Member?.FamilyName} {t.Start?.Partisipation?.Member?.Name} {t.Start?.Partisipation?.Member?.SecondName}" +
                                         $"\n\tCity:{t.Start?.Partisipation?.Member?.City}" +
                                         //$"\n\tPlace:{t.Place}{}" +
                                         $"\n\tTime of circle: {t.CircleTime}" +
                                         $"\n\tTeam:{t.Start?.Team?.Name}"); //+
                                       //$"\n\tisFinish?");
                }*/

            }

        }
        private static void TimingDateTable()
        {





            DataColumn[] columns =
            {
                new DataColumn("Id", typeof(int)),
                new DataColumn("IdStart",typeof(int)),
                new DataColumn("TimeNow",typeof(string)), //не уверен насчёт типа, но в БД у нас стоит просто Text
                new DataColumn("TimeFromStart",typeof(string)),
                new DataColumn("CircleTime",typeof(string)),
                new DataColumn("Circle", typeof(int)),
                new DataColumn("Place", typeof(int)),
                new DataColumn("PlaceAbsolute", typeof(int)),
                new DataColumn("currentTime", typeof(string))
            };

        }
        
        //конец

        //
        //блок секундомер (не асинхронный)
        //
        DispatcherTimer dt = new DispatcherTimer();
        Stopwatch sw = new Stopwatch();
        string currentTime = string.Empty;
        int CountOfFinishingPeople = 0;

        void dtTick(object sender, EventArgs e)
        {
            if (sw.IsRunning)
            {
                TimeSpan ts = sw.Elapsed;
                currentTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                secundomer.Text = currentTime;
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            sw.Start();
            dt.Start();
        }

        private void TimerStop(object sender, RoutedEventArgs e)
        {
            if (sw.IsRunning)
            {
                sw.Stop();
            }
        }
        //
        //блок секундомера конец
        //

        public void LoadEvents()//надо привести возвращаемые объекты к тексту, придумать, как по названию (или айди выбранного объекта) искать в базе и подгружать участников
        {                       //придумать, как можно сохранить выбранное мероприятие (зафиксировать его при переключении вкладок)
            //db.Competitions.Load();
            //EventsListBox.DataContext = db.Competitions.Local.ToBindingList();
        }// я это убрал потому что не пойму чё тут происходит, плюс теперь надо переписать под "сервисы"

        //це Миё
        TimeOnly _time;
        //UdpClient udpClient = new UdpClient(27069);
        UdpReceiveResult result;
        byte[] datagram;
        string received;

        /// <summary>
        /// Время, в момент которого не будет записываться.
        /// </summary>
        int secondOfDifference;
        TimeSpan timeOfDifference = new(0, 0, 5);
        //


        /* private void StartTimeAccepter_Click(object sender, RoutedEventArgs e) //Потенциальный фикс - асинхронный метод сравнения
         {                                                                      //А лучше подумать, как можно постоянно (или через промежутки времени)
             DateTime.TryParse(StartTimeBox.Text, out startTime);               //Сравнивать текущую дату с заданной, если они равны
             //стартовое время хранится в distance
         }

         private void TimerDataGrid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
         {
             //Timing timing;
             //db.Timings.Add(timing); //надо решить косяк с записью
             //надо придумать, как перед записью подсосать данные из таблиц по номеру спортсмена
             //db.SaveChanges;
         }*/
        //full maё
        GenericDataService<Timing> DataService = new(new RefereeHelperDbContextFactory());
        GenericDataService<RefereeHelper.Models.Start> startDataService = new(new RefereeHelperDbContextFactory());
        decimal CountOfLapsForHim;//кол-во кругов для данного спортсмена, чтобы считать, финишировал чи як

        UDPReceive u = UDPReceive.GetUdpClient();
        Processing p = new();
        void Received()
        {
            
            u.secondOfDifference = Int32.Parse(textBox_TimeOfDifference.Text);
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        string received = u.Receive().Result.ToString();

                        Process(received, p.dbContext, u.secondOfDifference);
                    }
                    catch
                    {

                    }
                }
            });
            
            
        }
        void Process(string received, DbContext dbContext, int SecondOfDifference)
        {
            timeOfDifference = new(0, 0, SecondOfDifference);
            if (!idsOfFinishingPeople.Any(x => x == received))
            {
                TimeOnly to = TimeOnly.FromDateTime(DateTime.Now);
                GenericDataService<Timing> DataService = new(new RefereeHelperDbContextFactory());
                if (!dbContext.Set<Timing>().Any(x => x.Start.Chip == received))
                {
                    to = TimeOnly.FromDateTime(DateTime.Now);
                    countOfStartingPeople++; 
                    //var t = dbContext.Set<Timing>().Add(new Timing
                    //{
                    //    TimeNow = to
                    //});
                    if (dbContext.Set<Models.Start>().ToList().Any(x => x.Chip == received)) 
                    {
                        var t = dbContext.Set<Timing>().Add(new Timing
                        {
                            TimeNow = to,
                            Start = dbContext.Set<Models.Start>().ToList().First(x => x.Chip == received)
                        });
                        dbContext.SaveChanges();
                        var ll = TimeOnly.FromTimeSpan((TimeOnly.FromDateTime(t.Entity.Start.StartTime.Value) - t.Entity.TimeNow).Value);
                        t.Entity.TimeFromStart = ll;
                        t.Entity.Circle = p.GetOfLapsForHim(dbContext, t.Entity);

                        t.Entity.CircleTime = t.Entity.TimeFromStart;
                        t.Entity.IsFinish = p.GetIsFinish(t.Entity.Id);
                        if (t.Entity.IsFinish.Value)
                        {
                            CountOfFinishingPeople++;
                            idsOfFinishingPeople.Add(received);
                        }
                        dbContext.SaveChanges();
                        p.RefrechPlace(dbContext, t.Entity);
                        p.RefrechAbsolutePlace(dbContext, t.Entity);
                    }
                    

                    dbContext.SaveChanges();
                    //var t = p.dbContext.Set<Timing>().First(x => x.TimeNow == to);

                    //CountOfLapsForHim = p.dbContext.Set<Timing>().First(x => x.Id == t.Entity.Id).Start.Partisipation.Group.Distance.Circles;
                    
                    //p.dbContext.Update(t.Entity);
                    //p.dbContext.SaveChanges();
                }
                else
                {

                    for (int i = p.dbContext.Set<Timing>().ToList().Last().Id; i > -1; i--)
                    {
                        if (p.dbContext.Set<Timing>().Include(z => z.Start).Any(x => x.Start.Chip == received))
                        {
                            if ((TimeOnly.FromDateTime(DateTime.Now)) - p.dbContext.Set<Timing>().ToList().First(x => x.Id == i).TimeNow > timeOfDifference)
                            {
                                var t = p.dbContext.Add(new Timing
                                {
                                    TimeNow = to,

                                    Start = p.dbContext.Set<Models.Start>().ToList().First(x => x.Chip == received)
                                });
                                p.dbContext.SaveChanges();
                                CountOfLapsForHim = p.dbContext.Set<Timing>().Include(x => x.Start).ThenInclude(z => z.Partisipation).ThenInclude(c => c.Group).ThenInclude(v => v.Distance).ToList().First(x => x.Id == t.Entity.Id).Start.Partisipation.Group.Distance.Circles;

                                t.Entity.Circle = p.GetOfLapsForHim(p.dbContext, t.Entity);
                                t.Entity.TimeFromStart = TimeOnly.FromTimeSpan((TimeOnly.FromDateTime((t.Entity.Start?.StartTime).Value) - t.Entity.TimeNow).Value);

                                p.dbContext.SaveChanges();
                                t.Entity.CircleTime = p.GetTimeOfLap(p.dbContext, t.Entity);
                                t.Entity.IsFinish = p.GetIsFinish(t.Entity.Id);
                                if (t.Entity.IsFinish.Value)
                                {
                                    CountOfFinishingPeople++;
                                    idsOfFinishingPeople.Add(received);
                                }
                                p.RefrechPlace(p.dbContext, t.Entity);
                                p.RefrechAbsolutePlace(p.dbContext, t.Entity);
                                //p.dbContext.Update(t.Entity);
                            }
                            break;
                        }
                    }

                }

            }
        }
        /// <summary>
        /// Обновляет все позиции
        /// </summary>
        /// <param name="idOfTiming"> - идентификационный номер Timing, относительно которого будут обновляться позиции</param>
        private void RefrechPlace(int idOfTiming)                                                    
        {
            var t = DataService.Get(idOfTiming).Result;
            var ts = DataService.GetAll(x => x.Start.Partisipation.Group.Distance == t.Start.Partisipation.Group.Distance
                                          && x.Start.Partisipation.Group          == t.Start.Partisipation.Group).Result.
                                             OrderBy(x => x.TimeFromStart);
            int i = 0;
            foreach(var k in ts)
            {
                k.Place = i + 1; i++;
                DataService.Update(k.Id, k);
            }
        }

        /// <summary>
        /// Обновляет все абсолютные позиции
        /// </summary>
        /// <param name="idOfTiming"> - идентификационный номер Timing, относительно которого будут обновляться абсолютные позиции</param>
        private void RefrechAbsolutePlace(int idOfTiming)                                                    
        {
            var t = DataService.Get(idOfTiming).Result;
            var ts = DataService.GetAll(x => x.Start.Partisipation.Group.Distance == t.Start.Partisipation.Group.Distance)
                                                   .Result.OrderBy(x => x.TimeFromStart);
            int i = 0;
            foreach (var k in ts)
            {
                k.Place = i + 1; i++;
                DataService.Update(k.Id, k);
            }
        }

        /// <summary>
        /// Определяет, финишировал ли или нет.
        /// </summary>
        /// <param name="idOfTiming"> - идентификационный номер Timing, для которого будет определяться финишировал ли спортсмен</param>
        /// <returns>True в случае, если финишировал, False в ином случае</returns>
        private bool GetIsFinish(int idOfTiming)                                                    
        {
            var t = DataService.Get(idOfTiming).Result;
            if(t.Start?.Partisipation.Group?.Distance.Circles < t.Circle)
                return false;
            else
                return true;
        }
        /// <summary>
        /// Определяет время круга для определённого спортсмена.
        /// </summary>
        /// <param name="idOfTiming"> - идентификационный номер Timing, по которому будет определяться время круга.</param>
        /// <returns>Время круга.</returns>
        private TimeOnly GetTimeOfLap(int idOfTiming)
        {
            var t = DataService.Get(idOfTiming).Result;
            if (DataService.GetAll(x => x.Start == t.Start).Result.Count() == 0)
                return t.TimeFromStart.Value;
            return TimeOnly.FromTimeSpan(t.TimeFromStart.Value - DataService.GetAll(x => x.Start == t.Start).Result.Last(x => x.Id != t.Id).TimeFromStart.Value);
        }
        /// <summary>
        /// Определяет количество кругов для определённого человека.
        /// </summary>
        /// <param name="idOfTiming"> - идентификационный номер Timing, по которому будет определяться количество кругов для определённого спортсмена.</param>
        /// <returns>количество кругов.</returns>
        private int GetOfLapsForHim(int idOfTiming)
        {
            var t = DataService.Get(idOfTiming).Result;
            return DataService.GetAll(x => x.Start.Number == t.Start.Number).Result.Count() + 1;
        }
        /// <summary>
        /// Определяет количество людей на определённом круге
        /// </summary>
        /// <param name="idOfTiming"> - ификационный номер Timing, по которому будет определяться количество людей на определённом кругу.</param>
        /// <returns></returns>
        private int GetCountPeopleOnThisLap(int idOfTiming)
        {
            var t = DataService.Get(idOfTiming).Result;
            return DataService.GetAll(x => x.Start.Partisipation.Competition == t.Start.Partisipation.Competition
                                        && x.Start.Partisipation.Group       == t.Start.Partisipation.Group
                                        && x.Circle                          == t.Circle).Result.Count();
        }

        //ne maё
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            timeOfDifference = new(0, 0, secondOfDifference);
            if (automode.IsChecked == true) 
            {
                Received();
            }
            else
            {
                u.client.Close();
            }
        }

        private void ProtocolDialogBut_Click(object sender, RoutedEventArgs e)
        {
            ProtocolDialog f = new ProtocolDialog();
            f.ShowDialog();
        }

        private void VisualGroupBut_Click(object sender, RoutedEventArgs e)
        {
            Competition competition = (Competition)CompList.SelectedItem;

            VisualData f = new VisualData(true, competition);
            f.Show();
        }

        private void VisualDistanceBut_Click(object sender, RoutedEventArgs e)
        {
            Competition competition = (Competition)CompList.SelectedItem;

            VisualData f = new VisualData(false, competition);
            f.Show();
        }

        private void MemberFillBut_Click(object sender, RoutedEventArgs e)
        {
            position = 0;
            MembersFill();
            ProtocolExcelBut.Content = "Выгрузить стартовый протокол";
            ProtocolWordBut.Content = "Распечатать стартовый протокол";
        }

        private void DistanceFillBut_Click(object sender, RoutedEventArgs e)
        {
            position = 1;
            DistanceFill();
            ProtocolExcelBut.Content = "Выгрузить протокол по дистанции";
            ProtocolWordBut.Content = "Распечатать протокол по дистанции";
        }

        private void TimingFillBut_Click(object sender, RoutedEventArgs e)
        {
            position = 2;
            TimingFill();
            TeamTimer.IsReadOnly = true;
            ProtocolExcelBut.Content = "Выгрузить финишный протокол";
            ProtocolWordBut.Content = "Распечатать финишный протокол";
        }

        private void TeamTimer_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            
        }

        private void ProtocolExcelBut_Click(object sender, RoutedEventArgs e)
        {
            if (position == 0)
            {
                Excelhelper ex = new Excelhelper();
                var dbContext = new RefereeHelperDbContextFactory().CreateDbContext();
                INIManager manager = new INIManager(System.IO.Path.Combine(Environment.CurrentDirectory, "Option.ini"));
                string namefile = "Start_Protocol_Excel.xlsx";

                Competition competition = (Competition)CompList.SelectedItem;
                if (ex.StarProtocol(competition))
                {
                    string file = manager.GetPrivateString("Option", "SaveExcelPath") + "\\" + namefile;
                    file = file.Replace("\\\\", "\\");

                    ex.saveAs(file);
                }
            }
            else if (position == 1)
            {
                Excelhelper ex = new Excelhelper();
                var dbContext = new RefereeHelperDbContextFactory().CreateDbContext();
                INIManager manager = new INIManager(System.IO.Path.Combine(Environment.CurrentDirectory, "Option.ini"));
                string namefile = "Distance_Protocol_Excel.xlsx";

                Competition competition = (Competition)CompList.SelectedItem;
                if (ex.DistanceProtocol(competition))
                {
                    string file = manager.GetPrivateString("Option", "SaveExcelPath") + "\\" + namefile;
                    file = file.Replace("\\\\", "\\");

                    ex.saveAs(file);
                }
            }
            else if (position == 2)
            {
                Excelhelper ex = new Excelhelper();
                var dbContext = new RefereeHelperDbContextFactory().CreateDbContext();
                INIManager manager = new INIManager(System.IO.Path.Combine(Environment.CurrentDirectory, "Option.ini"));
                string namefile = "Finish_Protocol_Excel.xlsx";

                Competition competition = (Competition)CompList.SelectedItem;
                if (ex.FinshProtocol(competition))
                {
                    string file = manager.GetPrivateString("Option", "SaveExcelPath") + "\\" + namefile;
                    file = file.Replace("\\\\", "\\");

                    ex.saveAs(file);
                }
            }
        }

        private void ProtocolWordBut_Click(object sender, RoutedEventArgs e)
        {
            if (position == 0)
            {
                WordHelper wd = new WordHelper();
                var dbContext = new RefereeHelperDbContextFactory().CreateDbContext();

                Competition competition = (Competition)CompList.SelectedItem;

                if (wd.StarProtocol(competition))
                {
                    try
                    {
                        var p = new Process();
                        p.StartInfo = new ProcessStartInfo(System.IO.Path.Combine(Environment.CurrentDirectory, "temp") + "\\WordTestSP.docx")
                        {
                            UseShellExecute = true
                        };
                        p.Start();
                    }
                    catch
                    { }
                }
            }
            else if (position == 1)
            {
                WordHelper wd = new WordHelper();
                var dbContext = new RefereeHelperDbContextFactory().CreateDbContext();

                Competition competition = (Competition)CompList.SelectedItem;

                if (wd.DistanceProtocol(competition))
                {
                    try
                    {
                        var p = new Process();
                        p.StartInfo = new ProcessStartInfo(System.IO.Path.Combine(Environment.CurrentDirectory, "temp") + "\\WordTestDP.docx")
                        {
                            UseShellExecute = true
                        };
                        p.Start();
                    }
                    catch
                    { }
                }
            }
            else if (position == 2)
            {
                WordHelper wd = new WordHelper();
                var dbContext = new RefereeHelperDbContextFactory().CreateDbContext();

                Competition competition = (Competition)CompList.SelectedItem;

                if (wd.FinshProtocol(competition))
                {
                    try
                    {
                        var p = new Process();
                        p.StartInfo = new ProcessStartInfo(System.IO.Path.Combine(Environment.CurrentDirectory, "temp") + "\\WordTestFP.docx")
                        {
                            UseShellExecute = true
                        };
                        p.Start();
                    }
                    catch
                    { }
                }
            }
        }

        private void PrintBut_Click(object sender, RoutedEventArgs e)
        {
            if (position == 0)
            {
                WordHelper wd = new WordHelper();
                var dbContext = new RefereeHelperDbContextFactory().CreateDbContext();

                Competition competition = (Competition)CompList.SelectedItem;

                if (wd.StarProtocol(competition))
                {
                    string file = System.IO.Path.Combine(Environment.CurrentDirectory, "temp") + "\\WordTestSP.docx";
                    if (File.Exists(file))
                        wd.Print(file);
                }
            }
            else if (position == 1)
            {
                WordHelper wd = new WordHelper();
                var dbContext = new RefereeHelperDbContextFactory().CreateDbContext();

                Competition competition = (Competition)CompList.SelectedItem;

                if (wd.StarProtocol(competition))
                {
                    string file = System.IO.Path.Combine(Environment.CurrentDirectory, "temp") + "\\WordTestDP.docx";
                    if (File.Exists(file))
                        wd.Print(file);
                }
            }
            else if (position == 2)
            {
                WordHelper wd = new WordHelper();
                var dbContext = new RefereeHelperDbContextFactory().CreateDbContext();

                Competition competition = (Competition)CompList.SelectedItem;

                if (wd.StarProtocol(competition))
                {
                    string file = System.IO.Path.Combine(Environment.CurrentDirectory, "temp") + "\\WordTestFP.docx";
                    if (File.Exists(file))
                        wd.Print(file);
                }
            }     
        }

        private void PrintAsBut_Click(object sender, RoutedEventArgs e)
        {
            if (position == 0)
            {
                WordHelper wd = new WordHelper();
                var dbContext = new RefereeHelperDbContextFactory().CreateDbContext();

                Competition competition = (Competition)CompList.SelectedItem;

                if (wd.StarProtocol(competition))
                {
                    string file = System.IO.Path.Combine(Environment.CurrentDirectory, "temp") + "\\WordTestSP.docx";
                    if (File.Exists(file))
                        wd.PrintAs(file);
                }
            }
            else if (position == 1)
            {
                WordHelper wd = new WordHelper();
                var dbContext = new RefereeHelperDbContextFactory().CreateDbContext();

                Competition competition = (Competition)CompList.SelectedItem;

                if (wd.StarProtocol(competition))
                {
                    string file = System.IO.Path.Combine(Environment.CurrentDirectory, "temp") + "\\WordTestDP.docx";
                    if (File.Exists(file))
                        wd.PrintAs(file);
                }
            }
            else if (position == 2)
            {
                WordHelper wd = new WordHelper();
                var dbContext = new RefereeHelperDbContextFactory().CreateDbContext();

                Competition competition = (Competition)CompList.SelectedItem;

                if (wd.StarProtocol(competition))
                {
                    string file = System.IO.Path.Combine(Environment.CurrentDirectory, "temp") + "\\WordTestFP.docx";
                    if (File.Exists(file))
                        wd.PrintAs(file);
                }
            }
        }

        private void TeamTimer_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (position == 2)
            {
                //Timing timing = new()
                //{
                //    TimeNow = TimeOnly.Parse(DateTime.Now.ToShortTimeString())
                //};
                //using (var dbContext = new RefereeHelperDbContextFactory().CreateDbContext())
                //{
                //    dbContext.Add(timing);
                //    dbContext.SaveChanges();
                //    TimingDataItem item = new TimingDataItem();

                //}
                //TimingFill();
                TimingDataItem item = new TimingDataItem();
                item.TimeNow = DateTime.Now.ToLongTimeString();
                TeamTimer.Items.Add(item);
            }
            //Timing timing;
            //db.Timings.Add(timing); //пофиксить запись (почитать об ошибке)
            //почитать, как при записи в бд подсосать данные из таблиц по номеру участнику
            //db.SaveChanges();
        }

       
    }
}