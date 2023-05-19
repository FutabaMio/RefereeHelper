using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RefereeHelper.EntityFramework;
using RefereeHelper.Models;
using RefereeHelper.OptionsWindows;
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
    }
}
