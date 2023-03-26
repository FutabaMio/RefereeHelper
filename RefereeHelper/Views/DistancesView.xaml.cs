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
            Loaded+=DistancesView_Loaded;
        }

        private void DistancesView_Loaded(object sender, RoutedEventArgs e)
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
               
        }
    }
}
