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
using RefereeHelper.Models;

namespace RefereeHelper.OptionsWindows
{
    /// <summary>
    /// Логика взаимодействия для ManualAddParticipation.xaml
    /// </summary>
    public partial class ManualAddParticipation : Window
    {
        public Partisipation Partisipation { get; private set; }
        public ManualAddParticipation(Partisipation partisipation)
        {
            InitializeComponent();
            Partisipation = partisipation;
            DataContext = Partisipation;
            using (var db=new RefereeHelperDbContextFactory().CreateDbContext())
            {
                db.Database.EnsureCreated();
                List<Member> members = db.Members.ToList();
                List<Competition> competitions = db.Competitions.ToList();
                List<Group> groups = db.Groups.ToList();

                membersList.DataContext= members;
                membersList.ItemsSource=members;
                competitionsList.DataContext= competitions;
                competitionsList.ItemsSource=competitions;
                groupsList.DataContext = groups;
                groupsList.ItemsSource=groups;
            }
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            var m=(Member)membersList.SelectedItem;
            Partisipation.MemberId=m.Id;
            var c = (Competition)competitionsList.SelectedItem;
            Partisipation.CompetitionId = c.Id;
            var g = (Group)groupsList.SelectedItem;
            Partisipation.GroupId=g.Id;
            DialogResult=true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
