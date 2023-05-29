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
    /// Логика взаимодействия для EditDistanceInfo.xaml
    /// </summary>
    public partial class EditDistanceInfo : Window
    {
        public Distance Distance { get; set; }
        Distance distance= new Distance();
        public EditDistanceInfo(Distance dist)
        {
            InitializeComponent();
            distance = dist;
        }

        private void BTNaccept_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new RefereeHelperDbContextFactory().CreateDbContext())
            {
                Distance dbdistance = db.Distances.Find(distance.Id);

                dbdistance.Name = TBXname.Text;
                int.TryParse(TBXcircles.Text, out int circles);
                dbdistance.Circles = circles;
                int.TryParse(TBXlength.Text, out int length);
                dbdistance.Length = length;
                int.TryParse(TBXheight.Text, out int height);
                dbdistance.Height = height;
                dbdistance.StartTime = (DateTime)DPdate.Value;

                db.SaveChanges();
                DialogResult=true;
            }
        }

        private void BTNcancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void ShowDistance(Distance distance)
        {
            Distance=distance;
            TBXname.Text = $"{Distance.Name}";
            TBXcircles.Text = $"{Distance.Circles}";
            TBXlength.Text = $"{Distance.Length}";
            TBXheight.Text = $"{Distance.Height}";
            DPdate.Text = $"{Distance.StartTime}";
            ShowDialog();
        }
    }
}
