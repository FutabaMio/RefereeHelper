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
    /// Interaction logic for ManualAddRegion.xaml
    /// </summary>
    public partial class ManualAddRegion : Window
    {
        public Region Region { get; private set; }
        public ManualAddRegion(Region region)
        {
            InitializeComponent();
            Region = region;
            DataContext = Region;
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            Region.Name = regionNameBox.Text;
            Int32.TryParse(regionNumberBox.Text, out int codeNumber);
            Region.codeNumber = codeNumber;
            DialogResult = true;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
