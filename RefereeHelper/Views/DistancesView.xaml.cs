using Microsoft.Data.Sqlite;
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
using RefereeHelper.OptionsWindows;

namespace RefereeHelper.Views
{
    /// <summary>
    /// Логика взаимодействия для DistancesView.xaml
    /// </summary>
    public partial class DistancesView : UserControl
    {
        public DistancesView()
        {
            InitializeComponent();
            RefreshData();
        }

        public void RefreshData()
        {
            SqliteConnection con = new SqliteConnection("Data Source=C:\\Users\\User\\Downloads\\SyclicSheck.db");
            con.Open();

            SqliteCommand command = new SqliteCommand(@"SELECT * FROM distance", con);
            SqliteDataReader dataReader = command.ExecuteReader();
            distanceTable.ItemsSource = dataReader;
        }

        private void AddDistanceButton_Click(object sender, RoutedEventArgs e)
        {
            ManualAddDistances manualAddDistances = new ManualAddDistances();
            manualAddDistances.ShowDialog();
        }

        private void EditDistance_Click(object sender, RoutedEventArgs e)
        {
            ManualAddDistances manualAddDistances = new ManualAddDistances();
            manualAddDistances.ShowDialog();
        }
    }
}
