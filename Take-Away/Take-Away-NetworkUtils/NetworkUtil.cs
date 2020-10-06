using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Take_Away_NetworkUtil
{
    public class NetworkUtil
    {
        public static Encoding encoding = Encoding.UTF8;

        public static string ReadTextMessage(NetworkStream networkStream)
        {
            var stream = new StreamReader(networkStream, encoding);
            {
                return stream.ReadLine();
            }
        }

        public static void WriteTextMessage(NetworkStream networkStream, string message)
        {
            using (var stream = new StreamWriter(networkStream, encoding, -1, true))
            {
                stream.WriteLine(message);
                stream.Flush();
            }
        }

        public static string ReadMessage(NetworkStream networkStream)
        {
            byte[] payloadlength = new byte[4];
            networkStream.Read(payloadlength, 0, payloadlength.Length);
            Int32 size = BitConverter.ToInt32(payloadlength);
            byte[] buffer = new byte[size];
            int totalRead = 0;

            //read bytes until stream indicates there are no more
            do
            {
                int read = networkStream.Read(buffer, totalRead, buffer.Length - totalRead);
                totalRead += read;
                Console.WriteLine("ReadMessage: " + read);
            } while (networkStream.DataAvailable || totalRead < size);

            return encoding.GetString(buffer, 0, totalRead);
        }

        public static void SendMessage(NetworkStream networkStream, string message)
        {
            //make sure the other end decodes with the same format!
            byte[] bytes = WrapMessage(encoding.GetBytes(message));
            networkStream.Write(bytes, 0, bytes.Length);
        }

        public static byte[] WrapMessage(byte[] message)
        {
            byte[] lengthPrefix = BitConverter.GetBytes(message.Length);
            byte[] ret = new byte[lengthPrefix.Length + message.Length];
            lengthPrefix.CopyTo(ret, 0);
            message.CopyTo(ret, lengthPrefix.Length);

            return ret;
        }
    }
}
