using RefereeHelper.EntityFramework;
using RefereeHelper.Models;
using RefereeHelper.Views;
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
    /// Логика взаимодействия для EditParticipationInfo.xaml
    /// </summary>
    public partial class EditParticipationInfo : Window
    {
        public Partisipation Partisipation { get; set; }

        Partisipation partisipation = new Partisipation();
        public EditParticipationInfo(Partisipation part)
        {
            InitializeComponent();
            using (var db = new RefereeHelperDbContextFactory().CreateDbContext())
            {
                db.Database.EnsureCreated();
                List<Member> members = db.Members.ToList();
                List<Competition> competitions = db.Competitions.ToList();
                List<Group> groups = db.Groups.ToList();

                CMBmembers.ItemsSource=members;
                CMBcompetitions.ItemsSource=competitions;
                CMBgroups.ItemsSource=groups;
            }

            partisipation=part;
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new RefereeHelperDbContextFactory().CreateDbContext())
            {
                Partisipation dbpartisipation = db.Partisipations.Find(partisipation.Id);
                var m = (Member)CMBmembers.SelectedItem;
                var c = (Competition)CMBcompetitions.SelectedItem;
                var g=(Group)CMBgroups.SelectedItem;

                dbpartisipation.Member=m;
                dbpartisipation.Competition=c;
                dbpartisipation.Group=g;

                db.SaveChanges();
                DialogResult=true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void ShowPartisipation(Partisipation part)
        {
            Partisipation=part;
            CMBmembers.Text=$"{Partisipation.Member}";
            CMBcompetitions.Text=$"{Partisipation.Competition}";
            CMBgroups.Text=$"{Partisipation}";
            ShowDialog();
        }
    }
}
