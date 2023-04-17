using System;
using System.Collections.Generic;
using System.IO;
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
using RefereeHelper;
using RefereeHelper.EntityFramework;
using RefereeHelper.EntityFramework.Services;
using RefereeHelper.Models;

namespace RefereeHelper.OptionsWindows
{
    /// <summary>
    /// Логика взаимодействия для ProtocolDialog.xaml
    /// </summary>
    public partial class ProtocolDialog : Window
    {
        public ProtocolDialog()
        {
            InitializeComponent();
        }

        private void ExcelSPBut_Click(object sender, RoutedEventArgs e)
        {
            Excelhelper ex = new Excelhelper();
            var dbContext = new RefereeHelperDbContextFactory().CreateDbContext();

            var competition = dbContext.Set<Competition>().Select(x => new Competition
            {
                Id = x.Id,
                Organizer = x.Organizer,
                Place = x.Place,
                Date = x.Date,
                Judge = x.Judge,
                Secretary = x.Secretary,
                TypeAge = x.TypeAge
            }).ToList();
            if(ex.StarProtocol(competition[0]))
            {
                string file = Environment.GetFolderPath(Environment.SpecialFolder.Desktop).ToString() + "\\ExcelTesSP.xlsx";

                ex.saveAs(file);
            }

        }

        private void ExcelDPBut_Click(object sender, RoutedEventArgs e)
        {
            Excelhelper ex = new Excelhelper();
            var dbContext = new RefereeHelperDbContextFactory().CreateDbContext();

            var competition = dbContext.Set<Competition>().Select(x => new Competition
            {
                Id = x.Id,
                Organizer = x.Organizer,
                Place = x.Place,
                Date = x.Date,
                Judge = x.Judge,
                Secretary = x.Secretary,
                TypeAge = x.TypeAge
            }).ToList();
            if (ex.DistanceProtocol(competition[0]))
            {
                string file = Environment.GetFolderPath(Environment.SpecialFolder.Desktop).ToString() + "\\ExcelTestDP.xlsx";

                ex.saveAs(file);
            }
        }

        private void ExcelFPBut_Click(object sender, RoutedEventArgs e)
        {
            Excelhelper ex = new Excelhelper();
            var dbContext = new RefereeHelperDbContextFactory().CreateDbContext();

            var competition = dbContext.Set<Competition>().Select(x => new Competition
            {
                Id = x.Id,
                Organizer = x.Organizer,
                Place = x.Place,
                Date = x.Date,
                Judge = x.Judge,
                Secretary = x.Secretary,
                TypeAge = x.TypeAge
            }).ToList();
            if (ex.FinshProtocol(competition[0]))
            {
                string file = Environment.GetFolderPath(Environment.SpecialFolder.Desktop).ToString() + "\\ExcelTestFP.xlsx";

                ex.saveAs(file);
            }
        }

        private void WordSpBut_Click(object sender, RoutedEventArgs e)
        {
            WordHelper wd = new WordHelper();
            var dbContext = new RefereeHelperDbContextFactory().CreateDbContext();

            var competition = dbContext.Set<Competition>().Select(x => new Competition
            {
                Id = x.Id,
                Organizer = x.Organizer,
                Place = x.Place,
                Date = x.Date,
                Judge = x.Judge,
                Secretary = x.Secretary,
                TypeAge = x.TypeAge
            }).ToList();

            if (wd.StarProtocol(competition[0]))
            {
                string file = Environment.GetFolderPath(Environment.SpecialFolder.Desktop).ToString() + "\\WordTestSP.docx";

                wd.saveAs(file);
            }
        }

        private void WordDpBut_Click(object sender, RoutedEventArgs e)
        {
            WordHelper wd = new WordHelper();
            var dbContext = new RefereeHelperDbContextFactory().CreateDbContext();

            var competition = dbContext.Set<Competition>().Select(x => new Competition
            {
                Id = x.Id,
                Organizer = x.Organizer,
                Place = x.Place,
                Date = x.Date,
                Judge = x.Judge,
                Secretary = x.Secretary,
                TypeAge = x.TypeAge
            }).ToList();

            if (wd.DistanceProtocol(competition[0]))
            {
                string file = Environment.GetFolderPath(Environment.SpecialFolder.Desktop).ToString() + "\\WordTestDP.docx";

                wd.saveAs(file);
            }
        }

        private void WordFpBut_Click(object sender, RoutedEventArgs e)
        {
            WordHelper wd = new WordHelper();
            var dbContext = new RefereeHelperDbContextFactory().CreateDbContext();

            var competition = dbContext.Set<Competition>().Select(x => new Competition
            {
                Id = x.Id,
                Organizer = x.Organizer,
                Place = x.Place,
                Date = x.Date,
                Judge = x.Judge,
                Secretary = x.Secretary,
                TypeAge = x.TypeAge
            }).ToList();

            if (wd.FinshProtocol(competition[0]))
            {
                string file = Environment.GetFolderPath(Environment.SpecialFolder.Desktop).ToString() + "\\WordTestFP.docx";

                wd.saveAs(file);
            }
        }
    }
}
