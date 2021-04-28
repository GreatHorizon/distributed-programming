﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Text.Json;

namespace Client
{
    class Program
    {
        private const string EndOfMessage = "<EOF>";
        public static void StartClient(string host, string port, string message)
        {
            try
            {
                IPAddress ipAddress;

                if (host == "localhost")
                {
                    ipAddress = IPAddress.Loopback;
                }
                else
                {
                    ipAddress = IPAddress.Parse(host);
                }

                IPEndPoint remoteEP = new IPEndPoint(ipAddress, Convert.ToInt32(port));

                Socket sender = new Socket(
                    ipAddress.AddressFamily,
                    SocketType.Stream, 
                    ProtocolType.Tcp);

                try
                {
                    sender.Connect(remoteEP);
                    
                    byte[] msg = Encoding.UTF8.GetBytes(message + EndOfMessage);
                    int bytesSent = sender.Send(msg);

                    byte[] buf = new byte[1024];
                    string stringifiedHistory = null;
                    List<string> history = new List<string>();
                   
                    while (true)
                    {
                        int bytesRec = sender.Receive(buf);
                        stringifiedHistory += Encoding.UTF8.GetString(buf, 0, bytesRec);

                        try
                        {
                            history = JsonSerializer.Deserialize<List<string>>(stringifiedHistory);
                            break;
                        }
                        catch (JsonException)
                        {
                            continue;
                        }
                    }

                    foreach (string item in history)
                    {
                        Console.WriteLine(item);
                    }

                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        

        static void Main(string[] args)
        {
            StartClient(args[0], args[1], args[2]);
        }
    }
}
