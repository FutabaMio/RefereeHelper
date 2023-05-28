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
    /// Логика взаимодействия для EditGroupInfo.xaml
    /// </summary>
    public partial class EditGroupInfo : Window
    {
        public Group Group { get; set; }
        Group grup = new Group();
        public EditGroupInfo(Group group)
        {
            InitializeComponent();
            grup=group;
            using (var db=new RefereeHelperDbContextFactory().CreateDbContext())
            {
                db.Database.EnsureCreated();
                CMBdistance.ItemsSource = db.Distances.ToList();
            }
        }

        private void BTNaccept_Click(object sender, RoutedEventArgs e)
        {
            using (var db=new RefereeHelperDbContextFactory().CreateDbContext())
            {
                Group dbgroup = db.Groups.Find(grup.Id);

                dbgroup.Name=TBXname.Text;
                int.TryParse(TBXminDate.Text, out int minDate);
                int.TryParse(TBXmaxDate.Text, out int maxDate);
                dbgroup.StartAge=minDate;
                dbgroup.EndAge=maxDate;
                var d = (Distance)CMBdistance.SelectedItem;
                dbgroup.Distance=d;

                db.SaveChanges();
                DialogResult=true;
            }
        }

        private void BTNclose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void ShowGroup(Group group)
        {
            Group = group;
            TBXname.Text = $"{Group.Name}";
            TBXminDate.Text = $"{Group.StartAge}";
            TBXmaxDate.Text = $"{Group.EndAge}";
            ShowDialog();
        }
    }
}
