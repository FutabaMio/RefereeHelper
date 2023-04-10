using Microsoft.EntityFrameworkCore;
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

namespace RefereeHelper.OptionsWindows
{
    /// <summary>
    /// Interaction logic for ManualAddGroup.xaml
    /// </summary>
    public partial class ManualAddGroup : Window
    {
        public Group Group { get;private set; }
        public ManualAddGroup(Group group)
        {
            InitializeComponent();
            Group = group;
            DataContext= Group;
            using(var db=new RefereeHelperDbContextFactory().CreateDbContext())
            {
                db.Database.EnsureCreated();
                List<Distance> distances = db.Distances.ToList();
                distancesList.DataContext = distances;
                distancesList.ItemsSource = distances;
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            Group.Name = nameTextBox.Text;
            int.TryParse(minAgeBox.Text, out int minAge);
            int.TryParse(maxAgeBox.Text, out int maxAge);
            Group.StartAge = minAge;
            Group.EndAge = maxAge;
            var d = (Distance)distancesList.SelectedItem;
            Group.DistanceId = d.Id;
            DialogResult=true;
        }
    }
}
