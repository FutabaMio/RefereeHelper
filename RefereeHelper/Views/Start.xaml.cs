using Microsoft.EntityFrameworkCore;
using RefereeHelper.EntityFramework;
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
    /// Interaction logic for Start.xaml
    /// </summary>
    public partial class Start : Page
    {
        public Start()
        {
            InitializeComponent();
            Loaded+=Start_Loaded;
        }

        private void Start_Loaded(object sender, RoutedEventArgs e)
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

        }

        private void editStart_Click(object sender, RoutedEventArgs e)
        {

        }

        private void deleteStart_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
