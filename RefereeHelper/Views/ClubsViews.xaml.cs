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
    /// Логика взаимодействия для ClubsViews.xaml
    /// </summary>
    public partial class ClubsViews : UserControl
    {
        //ApplicationContext db = new ApplicationContext();
        public ClubsViews()
        {
            InitializeComponent();

            Loaded+=ClubsViews_Loaded;
        }

        private void ClubsViews_Loaded(object sender, RoutedEventArgs e)
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
               
        }

        private void clubDataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
           // db.SaveChanges();   <- не так, не сохраняет
        }
    }
}
