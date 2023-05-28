﻿using Microsoft.Data.Sqlite;
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
using RefereeHelper.OptionsWindows.EditWindows;

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
            RefreshData();
        }

        public void RefreshData()
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
            RefreshData();
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
            RefreshData();
        }

        private void FilterBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filterStr = FilterBox.Text;
            if (filterStr != null)
            {
                using (var db = new RefereeHelperDbContextFactory().CreateDbContext())
                {
                    var filteredGroups = db.Groups.Where(f => f.Name.StartsWith(filterStr)).ToList();

                    groupsTable.ItemsSource = filteredGroups;
                }

                //RefreshData();
            }
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            var group = row.DataContext as Group;
            EditGroupInfo window = new EditGroupInfo(group);
            window.ShowGroup(group);
            if (window.DialogResult==true)
            {
                RefreshData();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DelGroup();
            RefreshData();
        }

        public void DelGroup()
        {
            Group SelectedGroup = (Group)groupsTable.SelectedItem;
            if (SelectedGroup!=null)
            {
                using (var db=new RefereeHelperDbContextFactory().CreateDbContext())
                {
                    Group dbgroup = db.Groups.Find(SelectedGroup.Id);
                    db.Remove(dbgroup);
                    db.SaveChanges();
                }
            }
        }
    }
}
