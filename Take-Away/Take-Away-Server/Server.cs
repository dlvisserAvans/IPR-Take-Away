﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using Take_Away_Data;
using Take_Away_SQLConnection;

namespace Take_Away_Server
{
    class Server
    {
        private static TcpListener listener;
        private static List<ClientHandler> clients = new List<ClientHandler>();
        private static List<Restaurant> restaurantList = new List<Restaurant>();
        private static SQLDatabaseManager databaseManager = new SQLDatabaseManager("takeaway");

        static void Main(string[] args)
        {
            Console.WriteLine("Server starting...");
            listener = new TcpListener(IPAddress.Any, 12345);
            listener.Start();
            listener.BeginAcceptTcpClient(new AsyncCallback(OnConnect), null);
            Console.ReadLine();
        }

        private static void OnConnect(IAsyncResult asyncResult)
        {
            var tcpClient = listener.EndAcceptTcpClient(asyncResult);
            Console.WriteLine("Client connected");
            clients.Add(new ClientHandler(tcpClient, databaseManager));
            listener.BeginAcceptTcpClient(new AsyncCallback(OnConnect), null);
        }

        internal static void Disconnect(ClientHandler clientHandler)
        {
            clients.Remove(clientHandler);
            Console.WriteLine("Client disconnected");
        }

        public static void fillProductList()
        {
            
        }
    }
}
