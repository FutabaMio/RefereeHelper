using RefereeHelper.Domain.Models;
using RefereeHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RefereeHelper.OptionsWindows
{
    /// <summary>
    /// Логика взаимодействия для TimingEdit.xaml
    /// </summary>
    public partial class TimingEdit : Window
    {
        public TimingDataItem Timing { get; set; }

        public TimingEdit()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        public void ShowTiming(TimingDataItem timing)
        {
            Timing = timing;
            //FamilyNameTbx.Text=$"{timing.FamilyName}";
            //NameTbx.Text=$"{timing.MemberName}";
            startNumberTbx.Text=$"{timing.Startnumber}";
            this.Show();
        }
    }
}
