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
    /// Логика взаимодействия для DistancesView.xaml
    /// </summary>
    public partial class DistancesView : UserControl
    {
        //ApplicationContext db = new ApplicationContext();
        public DistancesView()
        {
            InitializeComponent();
            RefreshData();
           
        }

        public void RefreshData()
        {
            using (var db=new RefereeHelperDbContextFactory().CreateDbContext())
            {
                db.Database.EnsureCreated();
                db.Distances.Load();
                DataContext = db.Distances.Local.ToObservableCollection();
                distanceTable.DataContext = db.Distances.Local.ToBindingList();
            }
        }

        private void AddDistanceButton_Click(object sender, RoutedEventArgs e)
        {
            ManualAddDistances manualAddDistances = new ManualAddDistances(new Distance());
            using (var db=new RefereeHelperDbContextFactory().CreateDbContext())
            {
                if (manualAddDistances.ShowDialog()==true)
                {
                    Distance Distance = manualAddDistances.Distance;
                    db.Distances.Add(Distance);
                    db.SaveChanges();
                }
            }
            RefreshData();
        }

        private void distanceTable_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ManualAddDistances manualAddDistances = new ManualAddDistances(new Distance());
            using (var db=new RefereeHelperDbContextFactory().CreateDbContext())
            {
                if (manualAddDistances.ShowDialog()==true)
                {
                    Distance Distance = manualAddDistances.Distance;
                    db.Distances.Add(Distance);
                    db.SaveChanges();
                }
            }
            RefreshData();
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            var dist = row.DataContext as Distance;
            EditDistanceInfo window = new EditDistanceInfo(dist);
            window.ShowDistance(dist);
            if (window.DialogResult==true)
            {
                RefreshData();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DelDistance();
            RefreshData();
        }

        public void DelDistance()
        {
            Distance SelectedDistance = (Distance)distanceTable.SelectedItem;
            if(SelectedDistance != null )
            {
                using (var db=new RefereeHelperDbContextFactory().CreateDbContext())
                {
                    Distance dbdistance = db.Distances.Find(SelectedDistance.Id);
                    db.Remove(dbdistance);
                    db.SaveChanges();
                }
            }
        }

        private void FilterBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filterStr = FilterBox.Text;
            if (filterStr!=null)
            {
                using (var db=new RefereeHelperDbContextFactory().CreateDbContext())
                {
                    var filteredDistances = db.Distances.Where(f => f.Name.StartsWith(filterStr)).ToList();
                    distanceTable.ItemsSource= filteredDistances;
                }
            }
        }
    }
}
