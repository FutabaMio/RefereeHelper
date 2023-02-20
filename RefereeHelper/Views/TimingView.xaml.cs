using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RefereeHelper.Models;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace RefereeHelper.Views
{
    /// <summary>
    /// Логика взаимодействия для TimingView.xaml
    /// </summary>
    public partial class TimingView : UserControl
    {
        

        ApplicationContext db = new ApplicationContext();
        public TimingView()
        {
            InitializeComponent();

            Loaded+= TimingView_Loaded;
        }

        private void TimingView_Loaded(object sender, RoutedEventArgs e)
        {
            db.Database.EnsureCreated();
            db.Timings.Load();
            DataContext = db.Timings.Local.ToObservableCollection();
            TeamTimer.DataContext = db.Timings.Local.ToBindingList();
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

        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        DateTime currentTime;
        DateTime startTime;
        int hours;
        int minutes;
        int seconds;
        int milliseconds;
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            // Изменение отображаемого времени на форме

            hours = (currentTime-startTime).Hours;
            minutes = (currentTime-startTime).Minutes;
            seconds = (currentTime-startTime).Seconds;
            milliseconds = (currentTime-startTime).Milliseconds;
            currentTime = DateTime.Now;

            secundomer.Content = $"{hours}:{minutes}:{seconds}.{milliseconds}";

            // Forcing the CommandManager to raise the RequerySuggested event
            CommandManager.InvalidateRequerySuggested();
        }

        private void TimerStart(object sender, RoutedEventArgs e)
        {
                dispatcherTimer.Start(); 
                dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
                dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0,1); //обновление информации каждую 0.1 мс

        } 


        private void TimerStop(object sender, RoutedEventArgs e)
        {
            
            startTime = DateTime.Now;
            dispatcherTimer.Stop();
        }

        private void StartTimeAccepter_Click(object sender, RoutedEventArgs e) //Потенциальный фикс - асинхронный метод сравнения
        {                                                                      //А лучше подумать, как можно постоянно (или через промежутки времени)
            DateTime.TryParse(StartTimeBox.Text, out startTime);               //Сравнивать текущую дату с заданной, если они равны
            
        }

        private void TimerDataGrid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Timing timing;
            db.Timings.Add(timing); //надо решить косяк с записью
            //надо придумать, как перед записью подсосать данные из таблиц по номеру спортсмена
            //db.SaveChanges;
        }
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
                                MessageBox.Show($"Tag:{received}");
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
            Timing timing;
            db.Timings.Add(timing); //пофиксить запись (почитать об ошибке)
            //почитать, как при записи в бд подсосать данные из таблиц по номеру участнику
            db.SaveChanges();
        }
    }
}
