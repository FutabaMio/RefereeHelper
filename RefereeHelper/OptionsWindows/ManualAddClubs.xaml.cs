using RefereeHelper.EntityFramework;
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
    /// Interaction logic for ManualAddClub.xaml
    /// </summary>
    public partial class ManualAddClub : Window
    {
        //ApplicationContext db = new ApplicationContext();
        public Club Club { get; private set; }
        public ManualAddClub(Club club)
        {
            InitializeComponent();
            Club = club;
            DataContext = Club;
            RefreshData();
                //regionsList.ItemsSource = db.Regions.Local.ToBindingList();
        }

        public void RefreshData()
        {
            using (var db=new RefereeHelperDbContextFactory().CreateDbContext())
            {
                db.Database.EnsureCreated();
                List<Region> regions = db.Regions.ToList();
                regionsList.DataContext = regions;
                regionsList.ItemsSource = regions;
            }
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            Club.Name = clubNameTextBox.Text;
            Club.Couch = couchTextBox.Text;
            Club.Region = (Region)regionsList.SelectedItem;
            DialogResult=true;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RegionAdd_Click(object sender, RoutedEventArgs e)
        {
            using (var db=new RefereeHelperDbContextFactory().CreateDbContext())
            {
                ManualAddRegion manualAddRegion = new ManualAddRegion(new Region());
                if (manualAddRegion.ShowDialog() == true)
                {
                    Region Region = manualAddRegion.Region;
                    db.Regions.Add(Region);
                    db.SaveChanges();
                }
            }
            RefreshData();
              
        }
    }
}
