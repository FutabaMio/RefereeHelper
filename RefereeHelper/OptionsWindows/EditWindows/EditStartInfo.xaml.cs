using RefereeHelper.EntityFramework;
using RefereeHelper.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Логика взаимодействия для EditStartInfo.xaml
    /// </summary>
    public partial class EditStartInfo : Window
    {
        public Start Start { get; set; }
        Start start = new Start();
        public EditStartInfo(Start st)
        {
            InitializeComponent();

            using (var db = new RefereeHelperDbContextFactory().CreateDbContext())
            {
                db.Database.EnsureCreated();
                List<Partisipation> partisipations = db.Partisipations.ToList();
                List<Team> teams = db.Teams.ToList();

                //participationList.DataContext = partisipations;
                CMBpartisipation.ItemsSource = partisipations;
                //teamList.DataContext = teams;
                CMBteam.ItemsSource = teams;

                start=st;
            }
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            using(var db=new RefereeHelperDbContextFactory().CreateDbContext())
            {
                Start dbstart = db.Starts.Find(start.Id);
                int.TryParse(TBXnumber.Text, out int numb);
                dbstart.Number=numb;
                dbstart.Chip=TBXchip.Text;
                dbstart.StartTime=(DateTime)DPstartTime.Value;

                db.SaveChanges();
                DialogResult=true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void ShowStart(Start st)
        {
            Start=st;
            TBXnumber.Text=$"{Start.Number}";
            TBXchip.Text=$"{Start.Chip}";
            DPstartTime.Text = $"{Start.StartTime}";
            ShowDialog();
        }
    }
}
