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

namespace RefereeHelper.OptionsWindows.EditWindows
{
    /// <summary>
    /// Логика взаимодействия для EditClubInfo.xaml
    /// </summary>
    public partial class EditClubInfo : Window
    {
        Club club = new Club();
        public Club Club { get; set; }
        public EditClubInfo(Club clubB)
        {
            InitializeComponent();

            using (var db=new RefereeHelperDbContextFactory().CreateDbContext())
            {
                db.Database.EnsureCreated();
                CBXregions.ItemsSource = db.Regions.ToList();
            }
            club=clubB;
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            using (var db=new RefereeHelperDbContextFactory().CreateDbContext())
            {
                Club dbclub = db.Clubs.Find(club.Id);
                dbclub.Name=TBXname.Text;
                dbclub.Couch=TBXcouch.Text;
                var r = (Region)CBXregions.SelectedItem;
                dbclub.Region=r;
                db.SaveChanges();
                DialogResult=true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void ShowClub(Club club)
        {
            Club=club;
            TBXname.Text = $"{Club.Name}";
            TBXcouch.Text=$"{Club.Couch}";
            ShowDialog();
        }
    }
}
