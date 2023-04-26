using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
            INIManager manager = new INIManager(System.IO.Path.Combine(Environment.CurrentDirectory, "Option.ini"));
            string namefile = "Start_Protocol_Excel.xlsx";

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
                string file = manager.GetPrivateString("Option", "SaveExcelPath") + "\\" + namefile;
                file = file.Replace("\\\\","\\");

                ex.saveAs(file);
            }
        }

        private void ExcelDPBut_Click(object sender, RoutedEventArgs e)
        {
            Excelhelper ex = new Excelhelper();
            var dbContext = new RefereeHelperDbContextFactory().CreateDbContext();
            INIManager manager = new INIManager(System.IO.Path.Combine(Environment.CurrentDirectory, "Option.ini"));
            string namefile = "Distance_Protocol_Excel.xlsx";

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
                string file = manager.GetPrivateString("Option", "SaveExcelPath") + "\\" + namefile;
                file = file.Replace("\\\\", "\\");

                ex.saveAs(file);
            }
        }

        private void ExcelFPBut_Click(object sender, RoutedEventArgs e)
        {
            Excelhelper ex = new Excelhelper();
            var dbContext = new RefereeHelperDbContextFactory().CreateDbContext();
            INIManager manager = new INIManager(System.IO.Path.Combine(Environment.CurrentDirectory, "Option.ini"));
            string namefile = "Finish_Protocol_Excel.xlsx";

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
                string file = manager.GetPrivateString("Option", "SaveExcelPath") + "\\" + namefile;
                file = file.Replace("\\\\", "\\");

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
                try
                {
                    var p = new Process();
                    p.StartInfo = new ProcessStartInfo(System.IO.Path.Combine(Environment.CurrentDirectory, "temp") + "\\WordTestSP.docx")
                    {
                        UseShellExecute = true
                    };
                    p.Start();
                }
                catch
                { }
            }
        }

        private void PrintSPBut_Click(object sender, RoutedEventArgs e)
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
                string file = System.IO.Path.Combine(Environment.CurrentDirectory, "temp") + "\\WordTestSP.docx";
                if (File.Exists(file))
                    wd.Print(file);
            }
        }

        private void PrintAsSPBut_Click(object sender, RoutedEventArgs e)
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
                string file = System.IO.Path.Combine(Environment.CurrentDirectory, "temp") + "\\WordTestSP.docx";
                if (File.Exists(file))
                    wd.PrintAs(file);
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
                try
                {
                    var p = new Process();
                    p.StartInfo = new ProcessStartInfo(System.IO.Path.Combine(Environment.CurrentDirectory, "temp") + "\\WordTestDP.docx")
                    {
                        UseShellExecute = true
                    };
                    p.Start();
                }
                catch
                { }
            }
        }

        private void PrintDPBut_Click(object sender, RoutedEventArgs e)
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
                string file = System.IO.Path.Combine(Environment.CurrentDirectory, "temp") + "\\WordTestDP.docx";
                if (File.Exists(file))
                    wd.Print(file);
            }
        }

        private void PrintAsDPBut_Click(object sender, RoutedEventArgs e)
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
                string file = System.IO.Path.Combine(Environment.CurrentDirectory, "temp") + "\\WordTestDP.docx";
                if (File.Exists(file))
                    wd.PrintAs(file);
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
                try
                {
                    var p = new Process();
                    p.StartInfo = new ProcessStartInfo(System.IO.Path.Combine(Environment.CurrentDirectory, "temp") + "\\WordTestFP.docx")
                    {
                        UseShellExecute = true
                    };
                    p.Start();
                }
                catch
                { }
            }
        }

        private void PrintFPBut_Click(object sender, RoutedEventArgs e)
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
                string file = System.IO.Path.Combine(Environment.CurrentDirectory, "temp") + "\\WordTestFP.docx";
                if (File.Exists(file))
                    wd.Print(file);
            }
        }

        private void PrintAsFPBut_Click(object sender, RoutedEventArgs e)
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
                string file = System.IO.Path.Combine(Environment.CurrentDirectory, "temp") + "\\WordTestFP.docx";
                if (File.Exists(file))
                    wd.PrintAs(file);
            }
        }
    }
}
