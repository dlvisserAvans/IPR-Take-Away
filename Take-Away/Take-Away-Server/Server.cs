using System;
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
        private static SQLDatabaseManager databaseManager = new SQLDatabaseManager("takeaway");

        static void Main(string[] args)
        {
            Console.WriteLine("Server starting...");
            listener = new TcpListener(IPAddress.Any, 12345);
            listener.Start();
            listener.BeginAcceptTcpClient(new AsyncCallback(OnConnect), null);
            Console.ReadLine();
        }

        //This method starts a new thread for the established connection made with the client.
        //After creating a new thread the listener starts listening on that thread.
        private static void OnConnect(IAsyncResult asyncResult)
        {
            var tcpClient = listener.EndAcceptTcpClient(asyncResult);
            Console.WriteLine("Client connected");
            clients.Add(new ClientHandler(tcpClient, databaseManager));
            listener.BeginAcceptTcpClient(new AsyncCallback(OnConnect), null);
        }

        //This method disconnects and removes the client from the list, this method can be called whenever a client fails to connect.
        internal static void Disconnect(ClientHandler clientHandler)
        {
            clients.Remove(clientHandler);
            Console.WriteLine("Client disconnected");
        }
    }
}