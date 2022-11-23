using System;
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
        public TimingView()
        {
            InitializeComponent();
        }

        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        DateTime currentTime;
        DateTime startTime = DateTime.MinValue;
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
            DateTime bufTime = DateTime.Now;
            if (StartButton.Content.ToString() == "Старт")
            {
                if (startTime == DateTime.MinValue)
                {
                    startTime = DateTime.Now;
                    currentTime = DateTime.Now;
                }
                else{
                    startTime += DateTime.Now - bufTime;
                }
                dispatcherTimer.Start(); 
                dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
                dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0,1);//если старт -> старт, меняем текст на паузу
                StartButton.Content = "Пауза";  //если написано пауза -> пауза секундомер и меняем текст на старт
            }
            else
            {
                dispatcherTimer.Stop();
                bufTime = DateTime.Now;
                StartButton.Content="Старт";
            }


            /*
            dispatcherTimer.Start(); */

        }


        private void TimerStop(object sender, RoutedEventArgs e)
        {
            currentTime = DateTime.Now;
            startTime = DateTime.MinValue;
            hours = (currentTime-startTime).Hours;
            minutes = (currentTime-startTime).Minutes;
            seconds = (currentTime-startTime).Seconds;
            milliseconds = (currentTime-startTime).Milliseconds;
            dispatcherTimer.Stop();
        }
    }
}
