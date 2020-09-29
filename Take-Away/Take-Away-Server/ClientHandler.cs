using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace Take_Away_Server
{
    class ClientHandler
    {
        private TcpClient tcpClient;
        private NetworkStream networkStream;
        private byte[] buffer = new byte[1024];
        private string totalBuffer = "";

        public ClientHandler(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
            this.networkStream = this.tcpClient.GetStream();
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
            Console.WriteLine("Packet recieved!");
            //The first packet is the header (message type). the other packets are the data
            switch (packetData[0])
            {
                case "sendOrder": //message type 'sendOrder'
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
    }
}
