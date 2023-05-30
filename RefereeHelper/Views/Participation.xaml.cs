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
    /// Interaction logic for Participation.xaml
    /// </summary>
    public partial class Participation : UserControl
    {
        public Participation()
        {
            InitializeComponent();
            Loaded+=Participation_Loaded;
        }

        private void Participation_Loaded(object sender, RoutedEventArgs e)
        {
            using (var db = new RefereeHelperDbContextFactory().CreateDbContext())
            {
                db.Database.EnsureCreated();
                db.Partisipations.Load();
                DataContext = db.Starts.Local.ToObservableCollection();
                partisipationTable.DataContext=db.Starts.Local.ToBindingList();
                partisipationTable.ItemsSource=db.Partisipations.Local.ToBindingList();
                db.SaveChanges();
            }
        }

        private void manualAddStart_Click(object sender, RoutedEventArgs e)
        {
            using (var db=new RefereeHelperDbContextFactory().CreateDbContext())
            {
                ManualAddParticipation manualAddWindow = new ManualAddParticipation(new Models.Partisipation());
                if(manualAddWindow.ShowDialog() == true)
                {
                    Models.Partisipation Partisipation = manualAddWindow.Partisipation;
                    db.Partisipations.Add(Partisipation);
                    db.SaveChanges();
                }
            }
            RefreshData();
        }



        public void RefreshData()
        {
            using (var db = new RefereeHelperDbContextFactory().CreateDbContext())
            {
                db.Database.EnsureCreated();
                List<Models.Partisipation> partisipations = db.Partisipations.ToList();
                partisipationTable.DataContext=partisipations;
                partisipationTable.ItemsSource=partisipations;
            }
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            var part = row.DataContext as Partisipation;
            EditParticipationInfo window = new EditParticipationInfo(part);
            window.ShowPartisipation(part);
            if (window.DialogResult==true)
            {
                RefreshData();
            }
        }

        private void BTNdelete_Click(object sender, RoutedEventArgs e)
        {
            DelPartisipation();
            RefreshData();
        }

        public void DelPartisipation()
        {
            Partisipation SelectedPartisipation = (Partisipation)partisipationTable.SelectedItem;
            if (SelectedPartisipation!=null)
            {
                using (var db = new RefereeHelperDbContextFactory().CreateDbContext())
                {
                    Partisipation dbpartisipation = db.Partisipations.Find(SelectedPartisipation.Id);
                    db.Remove(dbpartisipation);
                    db.SaveChanges();
                }
            }
        }
    }
}
