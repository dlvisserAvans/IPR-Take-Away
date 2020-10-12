using System;
using System.Dynamic;
using System.Globalization;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace Client
{
    class Program
    {
        private static TcpClient client;
        private static NetworkStream networkStream;
        private static byte[] buffer = new byte[1024];
        private static string totalBuffer;


        private static string username;
        private static string password;
        static void Main(string[] args)
        {
            Console.WriteLine("What is your username?");
            username = Console.ReadLine();
            Console.WriteLine("What is your password?");
            password = Console.ReadLine();

            client = new TcpClient();
            client.BeginConnect("localhost", 12345, new AsyncCallback(OnConnect), null);

            while (true)
            {
                //handle this
            }
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
                    if(packetData[1] == "ok")
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
                    for(int i = 0; i < size; i++)
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
