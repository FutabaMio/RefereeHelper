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
using System.Windows.Input.Manipulations;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RefereeHelper.Views
{
    /// <summary>
    /// Interaction logic for Start.xaml
    /// </summary>
    public partial class Start : UserControl
    {
        public Start()
        {
            InitializeComponent();
            RefreshData();
        }

        public void RefreshData()
        {
            using (var db= new RefereeHelperDbContextFactory().CreateDbContext())
            {
                db.Database.EnsureCreated();
                db.Starts.Load();
                DataContext = db.Starts.Local.ToObservableCollection();
                startTable.DataContext=db.Starts.Local.ToBindingList();
                db.SaveChanges();
            }
        }

        private void manualAddStart_Click(object sender, RoutedEventArgs e)
        {
            using (var db=new RefereeHelperDbContextFactory().CreateDbContext())
            {
                ManualAddStart manualAddWindow = new ManualAddStart(new Models.Start());
                if(manualAddWindow.ShowDialog() == true)
                {
                    Models.Start Start = manualAddWindow.Start;
                    db.Starts.Add(Start);
                    db.SaveChanges();
                }
            }
            RefreshData();
        }

        private void deleteStart_Click(object sender, RoutedEventArgs e)
        {
            DelStart();
            RefreshData();
        }

        public void DelStart()
        {
            Models.Start SelectedStart = (Models.Start)startTable.SelectedItem;
            if(SelectedStart!= null)
            {
                using(var db=new RefereeHelperDbContextFactory().CreateDbContext())
                {
                    Models.Start dbstart = db.Starts.Find(SelectedStart.Id);
                    db.Remove(dbstart);
                    db.SaveChanges();
                }
            }
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            var st = row.DataContext as Models.Start;
            EditStartInfo window = new EditStartInfo(st);
            window.ShowStart(st);
            if (window.DialogResult==true)
            {
                RefreshData();
            }
        }
    }
}
