// Copyright (c) 2022 YuanRui
// GitHub: https://github.com/yuanrui
// License: Apache-2.0

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _Local.ConsoleApp
{
    public class TcpDemo2
    {
        public static void Run(string[] args)
        {
            if (args == null || (args.Length > 0 && args[0] == "s"))
            {
                Server();
            }
            else
            {
                Client();
            }
        }

        static void Server()
        {
            Console.Title = "Server";
            Console.WriteLine("Tcp Server");
            TcpListener server = null;
            try
            {
                // Set the TcpListener on port 13000.
                Int32 port = 13000;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[server.Server.ReceiveBufferSize];

                // Enter the listening loop.
                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    // You could also user server.AcceptSocket() here.
                    var client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    var sock = client.Client;
                    var msg = DoReceive(sock);
                    DoSendAsync(sock, msg + ".A.");
                    DoSendAsync(sock, msg + ".B.");
                    DoSendAsync(sock, msg + ".C.");
                    DoLoopReceiveAsync(sock);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
            }
        }

        private static void DoLoopReceiveAsync(Socket sock)
        {
            Task.Factory.StartNew(t =>
            {
                do
                {
                    try
                    {
                        var msg = DoReceive(t as Socket);
                        Console.WriteLine(msg);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        break;
                    }
                } while (true);
            }, sock);
        }

        private static string DoReceive(Socket sock)
        {
            if (sock == null || !sock.Connected)
            {
                Thread.Sleep(1);
                return string.Empty;
            }

            if (sock.Poll(-1, SelectMode.SelectRead) || sock.Available > 0)
            {
                var buffer = new Byte[4096];
                var receiveSize = sock.Receive(buffer, buffer.Length, SocketFlags.None);
                if (receiveSize > 0)
                {
                    var data = Encoding.UTF8.GetString(buffer, 0, receiveSize);
                    return data;
                }
            }

            return string.Empty;
        }


        private static void DoSendAsync(Socket sock, string text)
        {
            Task.Factory.StartNew(t =>
            {
                var s = t as Socket;
                for (int i = 0; i < 1000000; i++)
                {
                    var str = text + i.ToString().PadLeft(7, '0') + Guid.NewGuid() + Environment.NewLine;
                    try
                    {
                        DoSending(s, Encoding.UTF8.GetBytes(str));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        break;
                    }
                    Thread.Sleep(1);
                }
            }, sock);
        }

        private static void DoSending(Socket sock, Byte[] input)
        {
            if (sock == null || !sock.Connected)
            {
                return;
            }

            if (!sock.Poll(5000000, SelectMode.SelectWrite))
            {
                Console.WriteLine("time out unsend.");
                return;
            }

            try
            {
                var res = sock.Send(input, SocketFlags.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        static void Connect(String server, String message)
        {
            try
            {
                // Create a TcpClient.
                // Note, for this client to work you need to have a TcpServer 
                // connected to the same address as specified by the server, port
                // combination.
                Int32 port = 13000;
                Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sock.Connect(server, port);
                DoSending(sock, Encoding.UTF8.GetBytes(message));
                DoSendAsync(sock, message + ".");
                DoLoopReceiveAsync(sock);

            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }

            Console.WriteLine("Sent finish...\n");
        }

        static void Client()
        {
            Console.Title = "Client";
            Console.WriteLine("Tcp Client");

            while (true)
            {
                Console.Write("Input Message:");
                var input = Console.ReadLine();
                Console.WriteLine("Begin Send");
                Connect("127.0.0.1", input);
            }
        }
    }
}
