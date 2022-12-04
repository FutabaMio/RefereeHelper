using Microsoft.Data.Sqlite;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using RefereeHelper.OptionsWindows;

namespace RefereeHelper.Views
{
    /// <summary>
    /// Логика взаимодействия для MembersView.xaml
    /// </summary>
    public partial class MembersView : UserControl
    {
        public MembersView()
        {
            InitializeComponent();
            RefreshData();
        }

        public void RefreshData()
        {
            SqliteConnection con = new SqliteConnection("Data Source=C:\\Users\\User\\Downloads\\SyclicSheck.db");
            con.Open();

            SqliteCommand command = new SqliteCommand(@"SELECT * FROM sportsman", con);
            SqliteDataReader dataReader = command.ExecuteReader();
            membersDataGrid.ItemsSource = dataReader;
        }

        private void AddMember_Click(object sender, RoutedEventArgs e)
        {
            ManualAddMembers manualAddWindow = new ManualAddMembers();
            manualAddWindow.ShowDialog();
        }
    }
}
