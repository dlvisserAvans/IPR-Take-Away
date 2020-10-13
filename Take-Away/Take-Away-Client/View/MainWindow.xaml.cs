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

            client = new TcpClient();
            client.BeginConnect("localhost", 12345, new AsyncCallback(OnConnect), null);
        }

        public static void OnConnect(IAsyncResult ar)
        {
            client.EndConnect(ar);
            networkStream = client.GetStream();
            networkStream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
            Write($"login\r\n{username}\r\n{password}");
        }

        public static void OnRead(IAsyncResult ar)
        {
            int recievedBytes = networkStream.EndRead(ar);
            string recievedText = Encoding.ASCII.GetString(buffer, 0, recievedBytes);
            totalBuffer += recievedText;

            while (totalBuffer.Contains("\r\n\r\n"))
            {
                string packet = totalBuffer.Substring(0, totalBuffer.IndexOf("\r\n\r\n"));
                totalBuffer = totalBuffer.Substring(totalBuffer.IndexOf("\r\n\r\n") + 4);
                string[] packetData = Regex.Split(packet, "\r\n");
                handleData(packetData);
            }
            networkStream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
        }

        public static void handleData(string[] packetData)
        {
            switch (packetData[0])
            {
                case "login": //message type 'login'
                    if (packetData[1] == "ok")
                    {
                        Console.WriteLine("Logged in");
                    }
                    else
                    {
                        Console.WriteLine("Error");
                    }
                    break;
                case "sendOrder": //message type 'sendOrder'

                    // Code to recieve the order of the customer
                    break;
                case "sendList":
                    int size = int.Parse(packetData[1]);
                    for (int i = 0; i < size; i++)
                    {

                    }
                    break;
            }
        }

        public static void Write(string data)
        {
            var dataAsBytes = Encoding.ASCII.GetBytes(data + "\r\n\r\n");
            networkStream.Write(dataAsBytes, 0, dataAsBytes.Length);
            networkStream.Flush();
        }
    }
}
