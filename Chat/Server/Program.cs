using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Server
{
    class Program
    {
        public static void StartListening(string port)
        {
            IPAddress ipAddress = IPAddress.Any; 
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, Convert.ToInt32(port));
            List<string> messageHistory = new List<string>();

            // CREATE
            Socket listener = new Socket(
                ipAddress.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                while (true)
                {
                    Console.WriteLine("Waiting for connection...");
                    Socket handler = listener.Accept();

                    Console.WriteLine("Data receiving...");
                    byte[] buf = new byte[1024];
                    string data = null;
                    int bytesRec = handler.Receive(buf);

                    data = Encoding.UTF8.GetString(buf, 0, bytesRec);

                    messageHistory.Add(data);

                    Console.WriteLine("Received message: {0}", data);

                    string historyJson = JsonSerializer.Serialize(messageHistory);
                    byte[] history = Encoding.UTF8.GetBytes(historyJson);

                    handler.Send(history);

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                StartListening(args[0]);

                Console.WriteLine("\nPress enter to stop...");
                Console.Read();    
            }
            else
            {
                Console.WriteLine("Invalid arguments count");
            }
        }
    }
}
