using System;
using Common;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ComplexMultithreadingClient
{
    class Client
    {
        static string[] messages =
        {
            "bonjour",
            "hola",
            "bonjour",
            "guten tag",
            "salve",
            "nin hao",
            "ola",
            "asalaam alaikum",
            "konnichiwa",
            "anyoung haseyo",
            "zdravstvuyte",
            "labas",
            "hello",
        };

        public void Start()
        {
            try
            {
                var client = new TcpClient("127.0.0.1", 13000);
                var stream = client.GetStream();

                var messageCount = Utils.GetNumberOfMessagesToSend();

                Console.WriteLine($"Messages to send = {messageCount}");

                Task.Run(() =>
                {
                    while (client.Connected)
                    {
                        try
                        {
                            var data = new byte[256];
                            var bytes = stream.Read(data, 0, data.Length);
                            var responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                            if (!string.IsNullOrEmpty(responseData))
                            {
                                Console.WriteLine("Received: {0}", responseData);
                            }
                        }
                        catch(Exception) { }
                    }
                });

                for (var i = 0; i < messageCount; i++)
                {
                    SendMessage(client);

                    Thread.Sleep(Utils.GetRandomDelay());
                }

                client.GetStream().Close();
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

            Console.WriteLine("\n Press Enter to continue...");
            Console.Read();
        }

        void SendMessage(TcpClient client)
        {
            string message = GetMessage();
            var data = message.ToByteArray();

            NetworkStream stream = client.GetStream();

            stream.Write(data, 0, data.Length);

            Console.WriteLine("Sent: {0}", message);
        }

        string GetMessage() => $"{messages[Utils.GetRandom().Next(0, messages.Length)]}";
    }
}
