using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using Take_Away_Data;
using Take_Away_NetworkUtil;

namespace Take_Away_Server
{
    class ClientHandler
    {
        private TcpClient tcpClient;
        private NetworkStream networkStream;
        private byte[] buffer = new byte[1024];
        private List<Product> productList = new List<Product>();
        private List<Product> chosenProductList;
        private string totalBuffer = "";
        private string username;
        private string password;

        public ClientHandler(TcpClient tcpClient, List<Product> products)
        {
            this.tcpClient = tcpClient;
            this.networkStream = this.tcpClient.GetStream();
            productList = products;
            this.networkStream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
        }

        public void OnRead(IAsyncResult asyncResult)
        {
            try
            {
                int recievedBytes = this.networkStream.EndRead(asyncResult);
                string recievedText = Encoding.ASCII.GetString(buffer, 0, recievedBytes);
                this.totalBuffer += recievedText;
            } 
            catch (IOException)
            {
                Server.Disconnect(this);
                return;
            }

            while (totalBuffer.Contains("\r\n\r\n"))
            {
                string packet = this.totalBuffer.Substring(0, this.totalBuffer.IndexOf("\r\n\r\n"));
                this.totalBuffer = this.totalBuffer.Substring(this.totalBuffer.IndexOf("\r\n\r\n") + 4);
                string[] packetData = Regex.Split(packet, "\r\n");
                handleData(packetData);
            }
            this.networkStream.BeginRead(this.buffer, 0, this.buffer.Length, new AsyncCallback(OnRead), null);
        }

        private void handleData(string[] packetData)
        {
            Console.WriteLine("Packet recieved " + packetData[0]);
            //The first packet is the header (message type). the other packets are the data
            switch (packetData[0])
            {
                case "login": //message type 'login'
                    Console.WriteLine("Login received");
                    Console.WriteLine(packetData[1] + "   " + packetData[2]);
                    if (!assertPacketData(packetData, 3))
                        return;
                    Console.WriteLine("correct packetData");
                    this.username = packetData[1];
                    this.password = packetData[2];
                    Console.WriteLine($"User {this.username} is connected!");
                    Write("login\r\nok");
                    // Code to receive the login
                    break;
                case "requestRestaurant":

                    Console.WriteLine(productList.Count);
                    
                    string list = JsonConvert.SerializeObject(productList);

                    Write("requestRestaurant\r\n" + list);

                    break;
                case "sendOrder": //message type 'sendOrder'
                    dynamic json = packetData[1];
                    chosenProductList = JsonConvert.DeserializeObject<List<Product>>(json);

                    Console.WriteLine($"{username} has chosen the following products: ");
                    foreach(Product p in chosenProductList)
                    {
                        Console.WriteLine(p);
                    }
                    // Code to recieve the order of the customer
                    break;
            }
        }

        public void Write(string data)
        {
            var dataAsBytes = Encoding.ASCII.GetBytes(data + "\r\n\r\n");
            this.networkStream.Write(dataAsBytes, 0, dataAsBytes.Length);
            this.networkStream.Flush();
        }

        private bool assertPacketData(string[] packetData, int requiredLength)
        {
            if (packetData.Length < requiredLength)
            {
                //Write("error");
                return false;
            }
            return true;
        }
    }
}
