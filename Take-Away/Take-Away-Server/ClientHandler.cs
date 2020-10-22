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
        private SQLDatabaseManager sqlDatabaseManager;
        private User user;

        public ClientHandler(TcpClient tcpClient, SQLDatabaseManager databaseManager)
        {
            this.tcpClient = tcpClient;
            this.networkStream = this.tcpClient.GetStream();
            sqlDatabaseManager = databaseManager;
            restaurantList = databaseManager.GetAllRestaurantsIntoList();
            this.networkStream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
        }

        /* The clienthandler starts its reading for data. Whenever the data is received it is put in a buffer which will then be searched for commands.
         * The protocol is separated by a double "enter" provided by \r\n\r\n. This data will then be sent to the handleData method where the clienthandler sends out it's reaction. 
         */
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
                Server.Disconnect(this, tcpClient.Client.RemoteEndPoint.ToString());
                return;
            }

            while (totalBuffer.Contains("\r\n\r\n"))
            {
                string packet = this.totalBuffer.Substring(0, this.totalBuffer.IndexOf("\r\n\r\n"));
                this.totalBuffer = this.totalBuffer.Substring(this.totalBuffer.IndexOf("\r\n\r\n") + 4);
                string[] packetData = Regex.Split(packet, "\r\n");
                HandleData(packetData);
            }
            this.networkStream.BeginRead(this.buffer, 0, this.buffer.Length, new AsyncCallback(OnRead), null);
        }



        /* The clienthandler receives the data from the onRead() method and switches it actions based on the first data out of the packetData.
           Three possible actions:
            1. reqeustRestaurant - the client has requested the restaurant list and these will be sent directly from the restaurant list. (The clienthandler created this list at the constructor with the databasemanager.)
            2. requestProducts - the client has requested a list of products from a restaurant. The server will then send the list of products after getting them from the server.
            3. sendOrder - the client has sent its order and will create a receipt which will then be sent to the user.        
        */
        private void HandleData(string[] packetData)
        {
            Console.WriteLine("Packet recieved " + packetData[0] + " from : " + this.tcpClient.Client.RemoteEndPoint.ToString());
            //The first packet is the header (message type). the other packets are the data
            switch (packetData[0])
            {
                case "requestRestaurant":
                    string listRestaurant = JsonConvert.SerializeObject(restaurantList);
                    Write("requestRestaurant\r\n" + listRestaurant);
                    break;

                case "requestProducts":
                    string chosenRestaurant = packetData[1];
                    productList = sqlDatabaseManager.GetProductsFromRestaurantIntoList(chosenRestaurant);

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
                    List<Product> helpList = new List<Product>();
                    foreach(Product product in chosenProductList)
                    {
                        helpList.Add(product);
                    }
             
                    Console.WriteLine("HelpList: " + helpList.GetHashCode());
                    Console.WriteLine("ChosenList: " + chosenProductList.GetHashCode());
                    Console.WriteLine($"{user.firstName} {user.lastName} ordered the following products:");
                    while (helpList.Count > 0)
                    {
                        int amountInList = 1;
                        Product product = helpList[helpList.Count - 1];
                        for (int j = (helpList.Count - 1); j > 0; j--)
                        {
                            if (product.name.Equals(helpList[j - 1].name) && product.price.Equals(helpList[j - 1].price))
                            {
                                amountInList++;
                                helpList.Remove(helpList[j - 1]);
                            }
                        }
                        Console.WriteLine($"{amountInList} {product.name}");
                        helpList.Remove(product);
                    }
                    Console.WriteLine($"Total price: {price:##0.00}");
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

        private bool AssertPacketData(string[] packetData, int requiredLength)
        {
            if (packetData.Length < requiredLength)
            {
                return false;
            }
            return true;
        }

        //Creates a receipt for the user which will be sent after the user commits it's order.
        private void GenerateReceipt(string restaurant, double price)
        {
            Receipt receipt = new Receipt();
            receipt.restaurantName = restaurant;
            receipt.buyername = user.firstName + " " + user.lastName;
            receipt.buyerPostalCode = user.postalCode;
            receipt.buyerHouseNumber = user.houseNumber;
            receipt.boughtProducts = chosenProductList;
            receipt.totalPrice = price;
            Write($"getReceipt\r\n{receipt.ToString()}");
        }
    }
}