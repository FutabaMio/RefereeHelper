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

namespace RefereeHelper.OptionsWindows
{
    /// <summary>
    /// Interaction logic for ManualAddCompetition.xaml
    /// </summary>
    public partial class ManualAddCompetition : Window
    {
        public competition Competition { get; private set; }
        public ManualAddCompetition(competition competition)
        {
            InitializeComponent();
            Competition = competition;
            DataContext= Competition;
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            Competition.Name = competitionNameBox.Text;
            Competition.Place = competitionPlaceBox.Text;
            Competition.Organizer = competitionOrganizerBox.Text;
            Competition.Judge = competitionJudgeBox.Text;
            Competition.Secretary = competitionSecretaryBox.Text;
            Competition.Date = competitionDatePicker.SelectedDate.Value.Date;
            DialogResult=true;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
