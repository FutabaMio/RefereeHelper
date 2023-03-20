using Microsoft.Data.Sqlite;
using RefereeHelper.Models;
using ExcelDataReader;
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
using Microsoft.Win32;
using System.Data;
using System.IO;
using Microsoft.EntityFrameworkCore;
using RefereeHelper.EntityFramework;

namespace RefereeHelper.Views
{
    /// <summary>
    /// Логика взаимодействия для MembersView.xaml
    /// </summary>
    public partial class MembersView : UserControl
    {
        //ApplicationContext db = new ApplicationContext();
        public MembersView()
        {
            InitializeComponent();

            Loaded+=MembersView_Loaded;
        }

        private void MembersView_Loaded(object sender, RoutedEventArgs e)
        {
            using(var db = new RefereeHelperDbContextFactory().CreateDbContext())
            {
                //var Members=db.Members.Include();
                //MembersList.DataContext= Members;
            }
        }

        private void AddMember_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new RefereeHelperDbContextFactory().CreateDbContext())
            {
                ManualAddMembers manualAddWindow = new ManualAddMembers(new Member());
                if (manualAddWindow.ShowDialog() == true)
                {
                    Member Member = manualAddWindow.Member;
                    db.Members.Add(Member);
                    db.SaveChanges();
                }
            }
               
        }

        private void AddFromExcel_Click(object sender, RoutedEventArgs e)
        {
            /*OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Файлы Excel 2003(.xls)|*.xls|Файлы Excel 2007+(.xlsx)|*.xlsx|All files (*.*)|*.*";
            if(fileDialog.ShowDialog() == true)
            {
               fileName = fileDialog.FileName;
                OpenExcelFile(fileName);
            }*/
        }


        private void OpenExcelFile(string path)
        {
            /*FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read);
            IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream);
            DataSet db = reader.AsDataSet(new ExcelDataSetConfiguration()
            {
                ConfigureDataTable =(x)=>new ExcelDataTableConfiguration()
                {
                    UseHeaderRow = true
                }
            });

            dataTableCollection = db.Tables;

            foreach (DataTable table in dataTableCollection)
            {
                membersDataGrid.Items.Add(table.TableName);
            }*/
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
           /* string key = string.Empty;
            if (FilterBox.TextChanged)
            {
                key = FilterBox.Text;
                SqliteCommand command = new SqliteCommand(@"SELECT * FROM sportsman where", con);
                SqliteDataReader dataReader = command.ExecuteReader();
                membersDataGrid.ItemsSource = dataReader;

            }*/
            
        }

        private void MembersList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //здесь нужно вызвать форму редактирования по текущему элементу (на котором дабл клик был)
        }

        private void MembersList_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            using(var db = new RefereeHelperDbContextFactory().CreateDbContext())
            {
                ManualAddMembers manualAddWindow = new ManualAddMembers(new Member());
                if (manualAddWindow.ShowDialog() == true)
                {
                    Member Member = manualAddWindow.Member;
                    db.Members.Add(Member);
                    db.SaveChanges();
                }
            }
           
        }
    }
}
