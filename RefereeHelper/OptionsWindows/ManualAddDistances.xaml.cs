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
    /// Interaction logic for ManualAddDistances.xaml
    /// </summary>
    public partial class ManualAddDistances : Window
    {
        public Distance Distance { get; private set; }
        public ManualAddDistances(Distance distance)
        {
            InitializeComponent();
            Distance = distance;
            DataContext = Distance;

        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            Distance.Name = distanceNameTextBox.Text;
            Decimal.TryParse(lengthNameTextBox.Text, out decimal lenght);
            Distance.Length= lenght;
            Decimal.TryParse(heightNameTextBox.Text, out decimal height);
            Distance.Height= height;
            DialogResult=true;
        }
    }
}
