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
        DateTime startTime = DateTime.Now;
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
    }
}
