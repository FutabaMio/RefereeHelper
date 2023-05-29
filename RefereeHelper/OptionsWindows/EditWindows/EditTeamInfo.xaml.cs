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
    /// Логика взаимодействия для EditTeamInfo.xaml
    /// </summary>
    public partial class EditTeamInfo : Window
    {
        Team team = new Team();
        public Team Team { get; set; }
        public EditTeamInfo(Team tm)
        {
            InitializeComponent();
            team=tm;
        }

        private void BTNaccept_Click(object sender, RoutedEventArgs e)
        {
            using(var db=new RefereeHelperDbContextFactory().CreateDbContext())
            {
                Team dbteam = db.Teams.Find(team.Id);
                dbteam.Name=TBXname.Text;

                db.SaveChanges();
                DialogResult=true;
            }
        }

        private void BTNcancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void ShowTeam(Team tm)
        {
            Team=tm;
            TBXname.Text=$"{Team.Name}";
            ShowDialog();
        }
    }
}
