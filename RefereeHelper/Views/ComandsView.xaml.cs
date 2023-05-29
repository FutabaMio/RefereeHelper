﻿using Microsoft.Data.Sqlite;
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
using RefereeHelper.OptionsWindows.EditWindows;

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
            AddCompetitionCommands manualAddWindow = new AddCompetitionCommands(new Team());
            using (var db = new RefereeHelperDbContextFactory().CreateDbContext())
            {
                if (manualAddWindow.ShowDialog()==true)
                {
                    Team Team = manualAddWindow.Team;
                    db.Teams.Add(Team);
                    db.SaveChanges();
                }
            }
            RefreshData();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ManualAddMemberInComand manualAddMemberInComand = new ManualAddMemberInComand();
            manualAddMemberInComand.ShowDialog();
        }

        
        public void RefreshData()
        {
            using (var db=new RefereeHelperDbContextFactory().CreateDbContext())
            {
                db.Database.EnsureCreated();
                List<Team> teams = db.Teams.ToList();
                teamsDataGrid.DataContext = teams;
                teamsDataGrid.ItemsSource= teams;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filterStr = FilterBox.Text;
            if (filterStr != null)
            {
                using (var db = new RefereeHelperDbContextFactory().CreateDbContext())
                {
                    var filteredTeams = db.Teams.Where(f => f.Name.StartsWith(filterStr)).ToList();

                    teamsDataGrid.ItemsSource = filteredTeams;
                }
            }
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            var tm = row.DataContext as Team;
            EditTeamInfo window = new EditTeamInfo(tm);
            window.ShowTeam(tm);
            if (window.DialogResult==true)
            {
                RefreshData();
            }
        }

        private void BTNdelete_Click(object sender, RoutedEventArgs e)
        {
            DelTeam();
            RefreshData();
        }

        public void DelTeam()
        {
            Team SelectedTeam = (Team)teamsDataGrid.SelectedItem;
            if (SelectedTeam != null)
            {
                using(var db=new RefereeHelperDbContextFactory().CreateDbContext())
                {
                    Team dbteam = db.Teams.Find(SelectedTeam.Id);
                    db.Remove(dbteam);
                    db.SaveChanges();
                }
            }
        }
    }
}
