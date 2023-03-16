using RefereeHelper.OptionsWindows;
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

namespace RefereeHelper.Controls
{
    /// <summary>
    /// Логика взаимодействия для NavigationBar.xaml
    /// </summary>
    public partial class NavigationBar : UserControl
    {
        public NavigationBar()
        {
            InitializeComponent();
        }

        private void helpWindow_Checked(object sender, RoutedEventArgs e)
        {
            HelpWindow helpWindow = new HelpWindow();
            helpWindow.ShowDialog();
        }
    }
}
