using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;

namespace Take_Away_Server
{
    class Server
    {
        private static TcpListener listener;
        private static List<ClientHandler> clients = new List<ClientHandler>();
        
        static void Main(string[] args)
        {
            Console.WriteLine("Server starting...");
            listener = new TcpListener(IPAddress.Any, 12345);
            listener.Start();
            listener.BeginAcceptTcpClient(new AsyncCallback(OnConnect), null);
        }

        private static void OnConnect(IAsyncResult asyncResult)
        {
            var tcpClient = listener.EndAcceptTcpClient(asyncResult);
            Console.WriteLine("Client connected");
            clients.Add(new )
        }

        internal static void Disconnect(ClientHandler clientHandler)
        {
            clients.Remove(clientHandler);
        }
    }
}
