using Microsoft.Data.Sqlite;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using RefereeHelper.OptionsWindows;
using Microsoft.EntityFrameworkCore;
using RefereeHelper.Models;
using RefereeHelper.EntityFramework;

namespace RefereeHelper.Views
{
    /// <summary>
    /// Логика взаимодействия для GroupsView.xaml
    /// </summary>
    public partial class GroupsView : UserControl
    {
        //ApplicationContext db = new ApplicationContext();
        public GroupsView()
        {
            InitializeComponent();
            Loaded+=GroupsView_Loaded;
        }

        private void GroupsView_Loaded(object sender, RoutedEventArgs e)
        {
            using (var db=new RefereeHelperDbContextFactory().CreateDbContext())
            {
                db.Database.EnsureCreated();
                db.Groups.Load();
                DataContext = db.Groups.Local.ToObservableCollection();
                groupsTable.DataContext = db.Groups.Local.ToBindingList();
            }
           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ManualAddGroup manualAddGroup = new ManualAddGroup(new Group());
            using (var db=new RefereeHelperDbContextFactory().CreateDbContext())
            {
                if (manualAddGroup.ShowDialog()==true)
                {
                    Group Group = manualAddGroup.Group;
                    db.Groups.Add(Group);
                    db.SaveChanges();
                }
            }
                
        }

        private void groupsTable_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ManualAddGroup manualAddGroup = new ManualAddGroup(new Group());
            using (var db=new RefereeHelperDbContextFactory().CreateDbContext())
            {
                if (manualAddGroup.ShowDialog()==true)
                {
                    Group Group = manualAddGroup.Group;
                    db.Groups.Add(Group);
                    db.SaveChanges();
                }
            }
                
        }
    }
}
