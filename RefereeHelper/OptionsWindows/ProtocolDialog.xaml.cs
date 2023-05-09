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
using Microsoft.WindowsAPICodePack.Dialogs;
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
        Competition competition;
        int mod;
        public ProtocolDialog(Competition c, int m)
        {
            InitializeComponent();
            competition = c;
            mod = m;
        }

        private void AbsolBut_Click(object sender, RoutedEventArgs e)
        {
            if (mod == 0)
            {
                Excelhelper ex = new Excelhelper();
                var dbContext = new RefereeHelperDbContextFactory().CreateDbContext();
                INIManager manager = new INIManager(System.IO.Path.Combine(Environment.CurrentDirectory, "Option.ini"));
                string namefile = "Distance_Protocol_Excel.xlsx";

                if (ex.DistanceProtocol(competition))
                {
                    string file = manager.GetPrivateString("Option", "SaveExcelPath") + "\\" + namefile;
                    file = file.Replace("\\\\", "\\");

                    ex.saveAs(file);
                }
            }
            else if (mod == 1)
            {
                Excelhelper ex = new Excelhelper();
                var dbContext = new RefereeHelperDbContextFactory().CreateDbContext();
                string namefile = "Distance_Protocol_Excel.xlsx";
                CommonOpenFileDialog dialog = new CommonOpenFileDialog();

                dialog.IsFolderPicker = true;
                if (CommonFileDialogResult.Ok == dialog.ShowDialog())
                {
                    if (ex.DistanceProtocol(competition))
                    {
                        string file = dialog.FileName + "\\" + namefile;
                        file = file.Replace("\\\\", "\\");

                        ex.saveAs(file);
                    }
                }
            }
            else if (mod == 2)
            {
                WordHelper wd = new WordHelper();
                var dbContext = new RefereeHelperDbContextFactory().CreateDbContext();

                if (wd.DistanceProtocol(competition))
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
            else if (mod == 3)
            {
                WordHelper wd = new WordHelper();
                var dbContext = new RefereeHelperDbContextFactory().CreateDbContext();

                if (wd.DistanceProtocol(competition))
                {
                    string file = System.IO.Path.Combine(Environment.CurrentDirectory, "temp") + "\\WordTestDP.docx";
                    if (File.Exists(file))
                        wd.Print(file);
                }
            }
            else if (mod == 4)
            {
                WordHelper wd = new WordHelper();
                var dbContext = new RefereeHelperDbContextFactory().CreateDbContext();

                if (wd.DistanceProtocol(competition))
                {
                    string file = System.IO.Path.Combine(Environment.CurrentDirectory, "temp") + "\\WordTestDP.docx";
                    if (File.Exists(file))
                        wd.PrintAs(file);
                }
            }
            this.Close();
        }

        private void GroupBut_Click(object sender, RoutedEventArgs e)
        {
            if (mod == 0)
            {
                Excelhelper ex = new Excelhelper();
                var dbContext = new RefereeHelperDbContextFactory().CreateDbContext();
                INIManager manager = new INIManager(System.IO.Path.Combine(Environment.CurrentDirectory, "Option.ini"));
                string namefile = "Distance_Protocol_Excel.xlsx";

                if (ex.GroupDistanceProtocol(competition))
                {
                    string file = manager.GetPrivateString("Option", "SaveExcelPath") + "\\" + namefile;
                    file = file.Replace("\\\\", "\\");

                    ex.saveAs(file);
                }
            }
            else if (mod == 1)
            {
                Excelhelper ex = new Excelhelper();
                var dbContext = new RefereeHelperDbContextFactory().CreateDbContext();
                string namefile = "Distance_Protocol_Excel.xlsx";
                CommonOpenFileDialog dialog = new CommonOpenFileDialog();

                dialog.IsFolderPicker = true;
                if (CommonFileDialogResult.Ok == dialog.ShowDialog())
                {
                    if (ex.GroupDistanceProtocol(competition))
                    {
                        string file = dialog.FileName + "\\" + namefile;
                        file = file.Replace("\\\\", "\\");

                        ex.saveAs(file);
                    }
                }
            }
            else if (mod == 2)
            {
                WordHelper wd = new WordHelper();
                var dbContext = new RefereeHelperDbContextFactory().CreateDbContext();

                if (wd.GroupDistanceProtocol(competition))
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
            else if (mod == 3)
            {
                WordHelper wd = new WordHelper();
                var dbContext = new RefereeHelperDbContextFactory().CreateDbContext();

                if (wd.GroupDistanceProtocol(competition))
                {
                    string file = System.IO.Path.Combine(Environment.CurrentDirectory, "temp") + "\\WordTestDP.docx";
                    if (File.Exists(file))
                        wd.Print(file);
                }
            }
            else if (mod == 4)
            {
                WordHelper wd = new WordHelper();
                var dbContext = new RefereeHelperDbContextFactory().CreateDbContext();

                if (wd.GroupDistanceProtocol(competition))
                {
                    string file = System.IO.Path.Combine(Environment.CurrentDirectory, "temp") + "\\WordTestDP.docx";
                    if (File.Exists(file))
                        wd.PrintAs(file);
                }
            }
            this.Close();
        }
    }
}
