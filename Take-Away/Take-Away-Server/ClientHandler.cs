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
using Take_Away_SQLConnection;

namespace Take_Away_Server
{
    class ClientHandler
    {
        private TcpClient tcpClient;
        private NetworkStream networkStream;
        private byte[] buffer = new byte[1024];
        private List<Product> productList = new List<Product>();
        private List<Restaurant> restaurantList = new List<Restaurant>();
        private List<Product> chosenProductList;
        private string totalBuffer = "";
        private string username;
        private string password;
        private SQLDatabaseManager SQLDatabaseManager;
        private User user;

        public ClientHandler(TcpClient tcpClient, SQLDatabaseManager databaseManager)
        {
            this.tcpClient = tcpClient;
            this.networkStream = this.tcpClient.GetStream();
            SQLDatabaseManager = databaseManager;
            restaurantList = databaseManager.getAllRestaurantsIntoList();
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
                    if (!assertPacketData(packetData, 1))
                        return;

                    Console.WriteLine($"Client connected!");
                    Write("login\r\nok");
                    break;

                case "requestRestaurant":
                    string listRestaurant = JsonConvert.SerializeObject(restaurantList);
                    Write("requestRestaurant\r\n" + listRestaurant);
                    break;

                case "requestProducts":
                    string chosenRestaurant = packetData[1];
                    productList = SQLDatabaseManager.getProductsFromRestaurantIntoList(chosenRestaurant);

                    string listProducts = JsonConvert.SerializeObject(productList);
                    Write("requestProducts\r\n" + listProducts);
                    break;

                case "sendOrder": //message type 'sendOrder'
                    dynamic jsonProducts = packetData[1];
                    chosenProductList = JsonConvert.DeserializeObject<List<Product>>(jsonProducts);

                    dynamic jsonUser = packetData[2];
                    user = JsonConvert.DeserializeObject<User>(jsonUser);

                    string restaurant = packetData[3];
                    double price = double.Parse(packetData[4]);
                    GenerateReceipt(restaurant, price);
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
                return false;
            }
            return true;
        }

        private void GenerateReceipt(string restaurant, double price)
        {
            Receipt receipt = new Receipt();
            Debug.WriteLine("Restaurant: " + restaurant);
            receipt.restaurantName = restaurant;
            receipt.buyername = user.FirstName + " " + user.LastName;
            receipt.buyerPostalCode = user.PostalCode;
            receipt.buyerHouseNumber = user.HouseNumber;
            receipt.boughtProducts = chosenProductList;
            receipt.totalPrice = price;
            Write($"getReceipt\r\n{receipt.ToString()}");
        }
    }
}
