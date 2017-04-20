using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Permissions;
using System.Text;
using System.Web;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace _Local.ConsoleApp
{
    class Program
    {

        static void Main(string[] args)
        {
            var input = @"2#9#爱情篇#1205@3#10#恐怖篇#818@4#11#家庭篇#644@5#12#校园篇#1464@6#13#儿童篇#1759@7#14#医疗篇#734@8#15#愚人篇#958@9#16#交往篇#289@10#17#动物篇#168@11#18#军事篇#483@12#19#民间篇#937@13#21#经营篇#994@14#22#名人篇#1285@15#24#搞笑歌词#54@16#25#体育篇#417@18#27#宗教篇#497@19#28#文艺篇#216@20#29#电脑篇#611@22#31#司法篇#339@23#32#交通篇#644@24#33#顺口溜篇#53@25#34#名著爆笑#48@26#35#古代篇#1250@27#36#幽默篇#739@28#37#恋爱必读#241@29#38#哈哈趣闻#198@30#39#综合篇#494@31#40#国外笑话#47";
            var cates = input.Split('@');

            var dic = new Dictionary<string, Tuple<string, string>>();

            foreach (var item in cates)
            {
                Console.WriteLine(item);
                var ids = item.Split('#');
                
            }

            foreach (var item in dic)
            {
                Console.WriteLine("{0}:{1}", item.Key, item.Value);
            }
            
            Console.WriteLine("\nPress Any Key To Exit...");
            Console.ReadLine();
        }

        static void Run(string[] args)
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
                String data = null;

                // Enter the listening loop.
                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    // You could also user server.AcceptSocket() here.
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    data = null;

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        data = System.Text.Encoding.UTF8.GetString(bytes, 0, i);
                        Console.WriteLine("Received: {0}", data);

                        // Process the data sent by the client.
                        data = data.ToUpper();

                        byte[] msg = System.Text.Encoding.UTF8.GetBytes(data);

                        // Send back a response.
                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Sent: {0}", data);
                    }

                    // Shutdown and end connection
                    client.Close();
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

        static void Connect(String server, String message)
        {
            try
            {
                // Create a TcpClient.
                // Note, for this client to work you need to have a TcpServer 
                // connected to the same address as specified by the server, port
                // combination.
                Int32 port = 13000;
                TcpClient client = new TcpClient(server, port);

                // Translate the passed message into ASCII and store it as a Byte array.
                Byte[] data = System.Text.Encoding.UTF8.GetBytes(message);

                // Get a client stream for reading and writing.
                //  Stream stream = client.GetStream();

                NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer. 
                stream.Write(data, 0, data.Length);

                Console.WriteLine("Sent: {0}", message);

                // Receive the TcpServer.response.

                // Buffer to store the response bytes.
                data = new Byte[client.SendBufferSize];

                // String to store the response ASCII representation.
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.UTF8.GetString(data, 0, bytes);
                Console.WriteLine("Received: {0}", responseData);

                // Close everything.
                stream.Close();
                client.Close();
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
