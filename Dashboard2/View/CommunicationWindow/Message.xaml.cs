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

namespace Dashboard2.View.CommunicationWindow
{
    /// <summary>
    /// Interaction logic for Message.xaml
    /// </summary>
    public partial class Message : Window
    {
        public Message(string message)
        {
            InitializeComponent();
            this.TextBoxMessage.Text = message;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.Show();
        }

        public static void ShowCommunicationWindow(string message)
        {
            Message message1 = new Message(message);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
