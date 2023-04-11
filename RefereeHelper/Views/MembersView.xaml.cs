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
using Epplus = OfficeOpenXml;
using EpplusSyle = OfficeOpenXml.Style;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using System.Diagnostics;

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
            RefreshData();            
           
        }
        int ID = 0;
        public void RefreshData()
        {
            using(var db = new RefereeHelperDbContextFactory().CreateDbContext())
            {
                db.Database.EnsureCreated(); //гарантия создания или подключения к бд
                db.Members.Load(); //Загружаем участников из бд
                DataContext = db.Members.Local.ToObservableCollection(); //устанавливаем данные в качестве контекста
                MembersList.DataContext = db.Members.Local.ToBindingList();
                ID=db.Members.Count()+1;
                db.SaveChanges();
                
            }
            
        }

        private void AddMember_Click(object sender, RoutedEventArgs e)
        { 
            using (var db = new RefereeHelperDbContextFactory().CreateDbContext())
            {
                ManualAddMembers manualAddWindow = new ManualAddMembers(new Member(), ID);
                if (manualAddWindow.ShowDialog() == true)
                {
                    Member Member = manualAddWindow.Member;
                    db.Members.Add(Member);
                    db.SaveChanges();
                }
            }
            RefreshData();
        }

        private void AddFromExcel_Click(object sender, RoutedEventArgs e)
        {
            string fileName = string.Empty;
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Файлы Excel 2003(.xls)|*.xls|Файлы Excel 2007+(.xlsx)|*.xlsx|All files (*.*)|*.*";
            if(fileDialog.ShowDialog()==true)
            {
                //System.IO.Path.GetDirectoryName(fileDialog.FileName);
                //OpenExcelFile(FilePath);
                fileName = fileDialog.FileName;
                OpenExcelFile(fileName);
            }
        }


        private void OpenExcelFile(string fileName)
        {
            using (var db=new RefereeHelperDbContextFactory().CreateDbContext())
            {
            Epplus.ExcelPackage package = new Epplus.ExcelPackage(new FileInfo(fileName));
            var sheet = package.Workbook.Worksheets.Add("лист1");
            Member member = new()
            {
                FamilyName = sheet.Cells[2, 1].Value.ToString(),
                Name = sheet.Cells[2, 2].Value.ToString(),
                SecondName=sheet.Cells[2, 3].Value.ToString(),
                ClubId=(int)sheet.Cells[2, 4].Value,
                DischargeId=(int)sheet.Cells[2, 5].Value,
                BornDate=(DateTime)sheet.Cells[2, 6].Value,
                City=sheet.Cells[2, 7].Value.ToString(),
                Gender=(bool)sheet.Cells[2, 8].Value
            };
                db.Set<Member>().Add(member);
                db.SaveChanges();
            }
           
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
            ManualAddMembers manualAddWindow = new ManualAddMembers(new Member(), ID);
            using (var db=new RefereeHelperDbContextFactory().CreateDbContext())
            {
                if (manualAddWindow.ShowDialog() == true)
                {
                    Member Member = manualAddWindow.Member;
                    db.Members.Add(Member);
                    db.SaveChanges();
                }
            }
            RefreshData();
        }
    }
}
