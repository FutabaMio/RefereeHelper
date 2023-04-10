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
        int currentID=0;
        public ManualAddMembers(Member member, int ID)
        {
            InitializeComponent();
            currentID=ID;
            Member = member;
            DataContext= Member;
            using (var db=new RefereeHelperDbContextFactory().CreateDbContext())
            {
                List<Club> clubs = db.Clubs.ToList();
                clubsList.DataContext = clubs;
                clubsList.ItemsSource=clubs;

                List<Discharge> discharges= db.Discharges.ToList();
                dischargeList.DataContext = discharges;
                dischargeList.ItemsSource=discharges;
            }
        }


        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            Member.Id= currentID;
            Member.SecondName = familyNameTextBox.Text;
            Member.Name=nameTextBox.Text;
            if (girl.IsChecked==true)
            {
                Member.Gender=false;
            }
            else {
                Member.Gender=true;
            }
            Member.BornDate = bornDatePicker.SelectedDate.Value.Date;
            Member.FamilyName = secondNameTextBox.Text;
            //Member.ClubId = clubsList.SelectedIndex;
            var c = (Club)clubsList.SelectedItem;
            Member.ClubId = c.Id;
            //Member.DischargeId = dischargeList.SelectedIndex;
            var d = (Discharge)dischargeList.SelectedItem;
            Member.DischargeId = d.Id;
            DialogResult=true;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
