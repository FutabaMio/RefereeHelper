﻿using System;
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
    /// Interaction logic for AddMemberInCommandFromTable.xaml
    /// </summary>
    public partial class AddMemberInCommandFromTable : Window
    {
        public AddMemberInCommandFromTable()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void readChip_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
