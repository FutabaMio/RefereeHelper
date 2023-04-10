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
            Partisipation.Member=(Member)membersList.SelectedItem;
            Partisipation.MemberId=Partisipation.Member.Id;
            Partisipation.Competition = (Competition)competitionsList.SelectedItem;
            Partisipation.CompetitionId = Partisipation.Competition.Id;
            Partisipation.Group = (Group)groupsList.SelectedItem;
            Partisipation.GroupId=Partisipation.Group.Id;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
