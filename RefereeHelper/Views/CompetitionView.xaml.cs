using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RefereeHelper.EntityFramework;
using RefereeHelper.Models;
using RefereeHelper.OptionsWindows;
using RefereeHelper.OptionsWindows.EditWindows;
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

namespace RefereeHelper.Views
{
    /// <summary>
    /// Логика взаимодействия для CompetitionView.xaml
    /// </summary>
    public partial class CompetitionView : UserControl
    {
        //ApplicationContext db = new ApplicationContext();
        public CompetitionView()
        {
            InitializeComponent();
            RefreshData();
        }

        public void RefreshData()
        {
            using (var db=new RefereeHelperDbContextFactory().CreateDbContext())
            {
                db.Database.EnsureCreated();
                db.Competitions.Load();
                DataContext = db.Competitions.Local.ToObservableCollection();
                competitionsTable.DataContext = db.Competitions.Local.ToBindingList();
            }
           
        }

        private void AddCompetition_Click(object sender, RoutedEventArgs e)
        {
            ManualAddCompetition manualAddCompetition = new ManualAddCompetition(new Competition());
            using (var db=new RefereeHelperDbContextFactory().CreateDbContext())
            {
                if (manualAddCompetition.ShowDialog() == true)
                {
                    Competition Competition = manualAddCompetition.Competition;
                    db.Competitions.Add(Competition);
                    db.SaveChanges();
                }
            }
            RefreshData();
               
        }

        private void competitionsTable_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ManualAddCompetition manualAddCompetition = new ManualAddCompetition(new Competition());
            using (var db=new RefereeHelperDbContextFactory().CreateDbContext())
            {
                if (manualAddCompetition.ShowDialog() == true)
                {
                    Competition Competition = manualAddCompetition.Competition;
                    db.Competitions.Add(Competition);
                    db.SaveChanges();
                }
            }
            RefreshData();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filterStr = FilterBox.Text;
            if (filterStr != null)
            {
                using (var db = new RefereeHelperDbContextFactory().CreateDbContext())
                {
                    var filteredComps = db.Competitions.Where(f => f.Name.StartsWith(filterStr)).ToList();

                    competitionsTable.ItemsSource = filteredComps;
                }

                //RefreshData();
            }
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            var comp = row.DataContext as Competition;
            EditCompetitionInfo window = new EditCompetitionInfo(comp);
            window.ShowCompetition(comp);
            if (window.DialogResult==true)
            {
                RefreshData();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DelCompetition();
            RefreshData();
        }

        public void DelCompetition()
        {
            Competition SelectedCompetition = (Competition)competitionsTable.SelectedItem;
            if (SelectedCompetition!=null)
            {
                using (var db=new RefereeHelperDbContextFactory().CreateDbContext())
                {
                    Competition dbcompetition = db.Competitions.Find(SelectedCompetition.Id);
                    db.Remove(dbcompetition);
                    db.SaveChanges();
                }
            }
        }
    }
}
