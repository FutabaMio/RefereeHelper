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
    /// Логика взаимодействия для MessageWindow.xaml
    /// </summary>
    public partial class MessageWindow : Window
    {
        /// <summary>
        /// Окно для сообщении информации пользователю
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <param name="title">Заголовок</param>
        public MessageWindow(String message, String title)
        {
            InitializeComponent();
            Message.Text = message;
            Title = title;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
