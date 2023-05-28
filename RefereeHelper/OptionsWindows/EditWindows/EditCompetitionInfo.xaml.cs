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
    /// Логика взаимодействия для EditCompetitionInfo.xaml
    /// </summary>
    public partial class EditCompetitionInfo : Window
    {
        Competition competition = new Competition();
        public Competition Competition { get; set; }
        public EditCompetitionInfo(Competition comp)
        {
            InitializeComponent();
            competition=comp;
        }

        private void BTNaccept_Click(object sender, RoutedEventArgs e)
        {
            using (var db=new RefereeHelperDbContextFactory().CreateDbContext())
            {
                Competition dbcompetition = db.Competitions.Find(competition.Id);

                dbcompetition.Name = TBXname.Text;
                dbcompetition.Organizer = TBXorganizer.Text;
                dbcompetition.Place = TBXplace.Text;
                dbcompetition.Judge = TBXjudge.Text;
                dbcompetition.Secretary = TBXsecretary.Text;
                dbcompetition.Date = DPdate.SelectedDate;

                db.SaveChanges();
                DialogResult=true;
            }
        }

        private void BTNcancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void ShowCompetition(Competition comp)
        {
            Competition=competition;
            TBXname.Text=$"{Competition.Name}";
            TBXorganizer.Text=$"{Competition.Organizer}";
            TBXplace.Text = $"{Competition.Place}";
            TBXsecretary.Text = $"{Competition.Secretary}";
            TBXjudge.Text = $"{Competition.Judge}";
            DPdate.Text = $"{Competition.Date}";
            ShowDialog();
        }
    }
}
