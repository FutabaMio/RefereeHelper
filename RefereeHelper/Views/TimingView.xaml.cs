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

namespace RefereeHelper.Views
{
    /// <summary>
    /// Логика взаимодействия для TimingView.xaml
    /// </summary>
    public partial class TimingView : UserControl
    {
        

        //ApplicationContext db = new ApplicationContext();

        
        public TimingView()
        {
            InitializeComponent();

            Loaded+= TimingView_Loaded;
            dt.Tick+=new EventHandler(dtTick);
            dt.Interval = new TimeSpan(0, 0, 0, 0, 1);
            LoadEvents();
        }

        private void TimingView_Loaded(object sender, RoutedEventArgs e)
        {
            /* using (var dbContext = new RefereeHelperDbContextFactory().CreateDbContext())
             { 

             }
                 db.Database.EnsureCreated();
             db.Timings.Load();
             DataContext = db.Timings.Local.ToObservableCollection();
             TeamTimer.DataContext = db.Timings.Local.ToBindingList();*/
            LoadData();
        }

        //тут я напишу как обращаться к разному через линкью


        void AddData()
        {
            using (var dbContext = new RefereeHelperDbContextFactory().CreateDbContext())
            {

                dbContext.SaveChanges();
            }
        }




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

        void dtTick(object sender, EventArgs e)
        {
            if (sw.IsRunning)
            {
                TimeSpan ts = sw.Elapsed;
                currentTime=String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds/10);
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

        public void LoadEvents()            //надо привести возвращаемые объекты к тексту, придумать, как по названию (или айди выбранного объекта) искать в базе и подгружать участников
        {
            using (var dbContext = new RefereeHelperDbContextFactory().CreateDbContext())
            {
                dbContext.competition.Load();
                EventsListBox.DataContext = dbContext.competition.Local.ToBindingList();
            }
          //  db.competition.Load();
            //EventsListBox.DataContext = db.competition.Local.ToBindingList();
        }

        //це Миё
        class PeopleForTakeInfo
        {
            public string Tag { get; set; }
            public DateTime Time { get; set; }
        }
        int sportsmansCount;
        DateTime _time;
        UdpClient udpClient = new UdpClient(27069);             
        UdpReceiveResult result;                                
        byte[] datagram;
        string received;
        List<PeopleForTakeInfo> sportsmans = new();
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
        async void Reseive()
        {
            while (true)
            {
                secondOfDifference = Int32.Parse(textBox_TimeOfDifference.Text);
                timeOfDifference = new(0, 0, secondOfDifference);
                result = await udpClient.ReceiveAsync();
                datagram = result.Buffer;
                received = Encoding.UTF8.GetString(datagram);
                _time = DateTime.Now;
                received = received.Substring(received.IndexOf("Tag:") + 4);
                received = received.Substring(0, received.IndexOf(" "));
                if(!sportsmans.Exists(x => x.Tag == received))
                {
                    sportsmans.Add(new PeopleForTakeInfo()
                    {
                        Tag = received,
                        Time = _time
                    });
                    MessageBox.Show($"Tag:{received}");
                }
                else
                {
                    sportsmansCount = sportsmans.Count;
                    for (int i = sportsmans.Count - 1; i > -1; i--)
                    {

                        if (sportsmans[i].Tag == received)
                        {
                            if (_time - sportsmans[i].Time > timeOfDifference)
                            {
                                sportsmans.Add(new PeopleForTakeInfo()
                                {
                                    Tag = received,
                                    Time = _time
                                });
                                //MessageBox.Show($"Tag:{received}");
                            }
                            break;
                        }
                    }
                }
               // MessageBox.Show($"Tag:{received}");// дальше 111 строка в моём проекте
            }
            

        }
        //ne maё
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            timeOfDifference = new(0, 0, secondOfDifference);
            if (automode.IsChecked == true) 
            {
                Reseive();
            }
            else
            {
                udpClient.Close();
            }
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
