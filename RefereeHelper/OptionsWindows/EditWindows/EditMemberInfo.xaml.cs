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

namespace RefereeHelper.OptionsWindows.EditWindows
{
    /// <summary>
    /// Логика взаимодействия для EditMemberInfo.xaml
    /// </summary>
    public partial class EditMemberInfo : Window
    {
        public EditMemberInfo(Member memb)
        {
            InitializeComponent();
            using (var db=new RefereeHelperDbContextFactory().CreateDbContext())
            {
                db.Database.EnsureCreated();
                //db.Clubs.Load();
               //db.Discharges.Load();
                CMBclub.ItemsSource = db.Clubs.ToList();
                CMBdischarge.ItemsSource = db.Discharges.ToList();
            }
                member= memb;
        }
        Member member = new Member();
        public Member Member { get; set; }

        private void BTNaccept_Click(object sender, RoutedEventArgs e)
        {
            
            using (var db=new RefereeHelperDbContextFactory().CreateDbContext())
            {
                Member dbmember = db.Members.Find(member.Id);

                dbmember.Name = TBXname.Text;
                dbmember.FamilyName = TBXfamilyName.Text;
                dbmember.SecondName = TBXsecondName.Text;
                dbmember.BornDate = DPbirthDate.DisplayDate;
                if (TBXgender.Text == "муж" || TBXgender.Text == "м" || TBXgender.Name == "мужской")
                {
                    dbmember.Gender = true;
                }
                else if (TBXgender.Text == "жен" || TBXgender.Text == "ж" || TBXgender.Name == "женский")
                {
                    dbmember.Gender = false;
                }
                else
                {
                    MessageBox.Show("Похоже, вы ввели неправильный пол. Повторите попытку.");
                }
                var c = (Club)CMBclub.SelectedItem;
                dbmember.Club = c;
                var d = (Discharge)CMBdischarge.SelectedItem;
                dbmember.Discharge = d;
                dbmember.City = TBXcity.Text;
                dbmember.Phone = TBXphone.Text;

                db.SaveChanges();
                DialogResult=true;
            }
            
        }

        private void BTNcancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void ShowMember(Member member)
        {
            Member=member;
            TBXfamilyName.Text = $"{Member.FamilyName}";
            TBXname.Text = $"{Member.Name}";
            TBXsecondName.Text = $"{Member.SecondName}";
            DPbirthDate.Text = $"{Member.BornDate}";
            TBXcity.Text = $"{Member.City}";
            TBXphone.Text = $"{Member.Phone}";
            ShowDialog();
        }

    }
}
