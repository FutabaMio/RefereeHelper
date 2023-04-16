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
using RefereeHelper.EntityFramework;
using Microsoft.EntityFrameworkCore;
using RefereeHelper.Models;
using OfficeOpenXml.ConditionalFormatting;

namespace RefereeHelper.Views
{
    /// <summary>
    /// Логика взаимодействия для ComandsView.xaml
    /// </summary>
    public partial class ComandsView : UserControl
    {
        public ComandsView()
        {
            InitializeComponent();
            Loaded+=ComandsView_Loaded;
        }

        private void ComandsView_Loaded(object sender, RoutedEventArgs e)
        {
            using (var db = new RefereeHelperDbContextFactory().CreateDbContext())
            {
                List<Team> teams = new List<Team>();
                db.Database.EnsureCreated();
                db.Groups.Load();
                DataContext = db.Teams.Local.ToObservableCollection();
                teams.AddRange(db.Teams);
                teamsDataGrid.ItemsSource=teams;    //db.Teams.Local.ToBindingList();
            }
        }

        /*public void RefreshData()
        {
         SqliteConnection con = new SqliteConnection("Data Source=C:\\Users\\User\\Downloads\\SyclicSheck.db");
        con.Open();

            SqliteCommand command = new SqliteCommand(@"SELECT * FROM [group]", con);
        SqliteDataReader dataReader = command.ExecuteReader();
        commandsDataGrid.ItemsSource = dataReader;
        }*/

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddCompetitionCommands addCompetitionCommands = new AddCompetitionCommands();
            addCompetitionCommands.ShowDialog();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ManualAddMemberInComand manualAddMemberInComand = new ManualAddMemberInComand();
            manualAddMemberInComand.ShowDialog();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            AddMemberInCommandFromTable addMemberInCommandFromTable = new AddMemberInCommandFromTable();
            addMemberInCommandFromTable.ShowDialog();
        }
    }
}
