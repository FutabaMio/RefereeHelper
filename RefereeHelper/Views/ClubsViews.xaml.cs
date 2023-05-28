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
using Microsoft.EntityFrameworkCore;
using RefereeHelper.Models;
using RefereeHelper.EntityFramework;
using RefereeHelper.OptionsWindows.EditWindows;

namespace RefereeHelper.Views
{
    /// <summary>
    /// Логика взаимодействия для ClubsViews.xaml
    /// </summary>
    public partial class ClubsViews : UserControl
    {
        //ApplicationContext db = new ApplicationContext();
        public ClubsViews()
        {
            InitializeComponent();
            RefreshData();
        }

        public void RefreshData()
        {
            using (var db=new RefereeHelperDbContextFactory().CreateDbContext())
            {
                db.Database.EnsureCreated();
                db.Clubs.Load();
                DataContext = db.Clubs.Local.ToObservableCollection();
                clubDataGrid.DataContext = db.Clubs.Local.ToBindingList();
            }
             
        }

        private void AddClubButton_Click(object sender, RoutedEventArgs e)
        {
            ManualAddClub manualAddClub = new ManualAddClub(new Club());
            using (var db=new RefereeHelperDbContextFactory().CreateDbContext())
            {
                if (manualAddClub.ShowDialog()==true)
                {
                    Club Club = manualAddClub.Club;
                    db.Clubs.Add(Club);
                    db.SaveChanges();
                }
            }
            RefreshData();
               
        }

        private void clubDataGrid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
           
            using (var db=new RefereeHelperDbContextFactory().CreateDbContext())
            { ManualAddClub manualAddClub = new ManualAddClub(new Club());
                if (manualAddClub.ShowDialog() == true)
                {
                    Club Club = manualAddClub.Club;
                    db.Add(Club);
                    db.SaveChanges();
                }
                
            }
               RefreshData();
        }

        private void clubDataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
           // db.SaveChanges();   <- не так, не сохраняет
        }

        private void FilterBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filterStr = FilterBox.Text;
            if (filterStr != null)
            {
                using (var db = new RefereeHelperDbContextFactory().CreateDbContext())
                {
                    var filteredClubs = db.Clubs.Where(f => f.Name.StartsWith(filterStr)).ToList();

                    clubDataGrid.ItemsSource = filteredClubs;
                }

                //RefreshData();
            }
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            var club = row.DataContext as Club;
            EditClubInfo window = new EditClubInfo(club);
            window.ShowClub(club);
            if (window.DialogResult==true)
            {
                RefreshData();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DelClub();
            RefreshData();
        }

        public void DelClub()
        {
            Club selectedClub = (Club)clubDataGrid.SelectedItem;
            if (selectedClub!=null)
            {
                using(var db=new RefereeHelperDbContextFactory().CreateDbContext())
                {
                    Club dbclub = db.Clubs.Find(selectedClub.Id);
                    db.Remove(dbclub);
                    db.SaveChanges();
                }
            }
        }
    }
}
