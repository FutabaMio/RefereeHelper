using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RefereeHelper.Models;
using System;
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

namespace RefereeHelper.Views
{
    /// <summary>
    /// Логика взаимодействия для TimingView.xaml
    /// </summary>
    public partial class TimingView : UserControl
    {
        public TimingView()
        {
            InitializeComponent();
            dt.Tick += new EventHandler(dtTick);
            dt.Interval = new TimeSpan(0, 0, 0, 0, 1);
            LoadEvents();
            LoadData();


        }
        int countOfStartingPeople = 0, countOfFinishingPeople = 0;
       
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
                    string received = u.Receive().Result.ToString();

                    Process(received, p.dbContext, u.secondOfDifference);
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
            var dbContext = new RefereeHelperDbContextFactory().CreateDbContext();

            var competition = dbContext.Set<Competition>().Select(x => new Competition
            {
                Id = x.Id,
                Organizer = x.Organizer,
                Place = x.Place,
                Date = x.Date,
                Judge = x.Judge,
                Secretary = x.Secretary,
                TypeAge = x.TypeAge
            }).ToList();

            VisualData f = new VisualData(true, competition[0]);
            f.Show();
        }

        private void VisualDistanceBut_Click(object sender, RoutedEventArgs e)
        {
            var dbContext = new RefereeHelperDbContextFactory().CreateDbContext();

            var competition = dbContext.Set<Competition>().Select(x => new Competition
            {
                Id = x.Id,
                Organizer = x.Organizer,
                Place = x.Place,
                Date = x.Date,
                Judge = x.Judge,
                Secretary = x.Secretary,
                TypeAge = x.TypeAge
            }).ToList();

            VisualData f = new VisualData(false, competition[0]);
            f.Show();
        }

        private void TeamTimer_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Timing timing;
            //db.Timings.Add(timing); //пофиксить запись (почитать об ошибке)
            //почитать, как при записи в бд подсосать данные из таблиц по номеру участнику
            //db.SaveChanges();
        }

       
    }
}