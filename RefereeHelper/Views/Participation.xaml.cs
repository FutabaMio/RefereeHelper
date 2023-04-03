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
                participationTable.DataContext=db.Starts.Local.ToBindingList();
                db.SaveChanges();
            }
        }

        private void manualAddStart_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
