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
    /// Interaction logic for ManualAddMembers.xaml
    /// </summary>
    public partial class ManualAddMembers : Window
    {
        public Member Member { get; private set; }
        public ManualAddMembers(Member member)
        {
            InitializeComponent();
            Member = member;
            DataContext= Member;
            using (var db=new RefereeHelperDbContextFactory().CreateDbContext())
            {
                List<Club> clubs = db.Clubs.ToList();
                clubsList.ItemsSource=clubs;
            }
        }


        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            //DateOnly born = new(bornDatePicker.SelectedDate.Value.Year, bornDatePicker.SelectedDate.Value.Month, bornDatePicker.SelectedDate.Value.Day);
            Member.FamilyName = secondNameTextBox.Text;
            Member.Name=nameTextBox.Text;
            if (girl.IsChecked==true)
            {
                Member.Gender=false;
            }
            else {
                Member.Gender=true;
            }
            Member.BornDate =bornDatePicker.SelectedDate.Value.Date;
            Member.FamilyName = familyNameTextBox.Text;
            Member.ClubId = clubsList.SelectedIndex;
            DialogResult=true;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
