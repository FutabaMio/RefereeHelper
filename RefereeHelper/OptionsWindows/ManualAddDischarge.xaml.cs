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
    /// Логика взаимодействия для ManualAddDischarge.xaml
    /// </summary>
    public partial class ManualAddDischarge : Window
    {
        public Discharge Discharge { get; private set; }
        public ManualAddDischarge(Discharge discharge)
        {
            InitializeComponent();
            Discharge = discharge;
            DataContext = Discharge;
        }

        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            Discharge.Name=dischargeName.Text;
            DialogResult=true;
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
