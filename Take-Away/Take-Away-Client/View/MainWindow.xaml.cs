using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
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
using Take_Away_Client.ViewModel;

namespace Take_Away_Client.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static TcpClient client;
        private static NetworkStream networkStream;
        private static byte[] buffer = new byte[1024];
        private static string totalBuffer;

        private static string username = "JKB";
        private static string password = "1234";

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new TakeAwayViewModel();
        }
    }
}
