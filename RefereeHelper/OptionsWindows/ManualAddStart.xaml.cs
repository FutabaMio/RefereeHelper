using RefereeHelper.Models;
using RefereeHelper.Views;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace RefereeHelper.OptionsWindows
{
    /// <summary>
    /// Логика взаимодействия для ManualAddStart.xaml
    /// </summary>
    public partial class ManualAddStart : Window
    {
        public Models.Start Start { get; private set; }
        public ManualAddStart(Models.Start start)
        {
            InitializeComponent();
            Start=start;
            DataContext=Start;
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            var p = (Participation)participationList.SelectedItem;
            Start.PartisipationId = p.Id;
            var t = (Team)teamList.SelectedItem;
            Start.TeamId = t.Id;
            Int32.TryParse(startNumberBox.Text, out int numb);
            Start.Number=numb;
            Start.Chip=chipBox.Text;

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
