using Microsoft.Data.Sqlite;
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

namespace RefereeHelper.Views
{
    /// <summary>
    /// Логика взаимодействия для CompetitionView.xaml
    /// </summary>
    public partial class CompetitionView : UserControl
    {
        public CompetitionView()
        {
            InitializeComponent();
            RefreshData();
        }

        public void RefreshData()
        {
            SqliteConnection con = new SqliteConnection("Data Source=C:\\Users\\User\\Downloads\\SyclicSheck.db");
            con.Open();

            SqliteCommand command = new SqliteCommand(@"SELECT * FROM competition", con);
            SqliteDataReader dataReader = command.ExecuteReader();
            competitionsTable.ItemsSource = dataReader;

            //создать копию, где вызываются только те команды, у которых в таблице group_competition айдишниики принадлежат соревнованию
        }
    }
}
