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

        public void StartTpl()
        {
            try
            {
                var client = new TcpClient("127.0.0.1", 13000);
                var stream = client.GetStream();

                StartReceivingMessages(client);

                SendMessages(client, Utils.GetNumberOfMessagesToSend());

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

            Console.WriteLine("\n Press Enter to continue...");
            Console.Read();
        }

        public void StartThread()
        {
            try
            {
                var client = new TcpClient("127.0.0.1", 13000);
                var stream = client.GetStream();
                var unfinishedMessage = "";

                ReceiveMessagesFromHistory(client);

                for (var i = 0; i < Utils.GetNumberOfMessagesToSend(); i++)
                {
                    SendMessage(client);

                    unfinishedMessage = PrintReceivedMessages(stream, unfinishedMessage);
                    Thread.Sleep(Utils.GetRandomDelay());
                }

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

            Console.WriteLine("\n Press Enter to continue...");
            Console.Read();
        }

        private void ReceiveMessagesFromHistory(TcpClient client)
        {
            var unfinishedMessage = "";
            var stream = client.GetStream();

            while (client.Connected)
            {
                try
                {
                    var bytes = new byte[256];
                    var count = stream.Read(bytes, 0, bytes.Length);
                    var responseData = Utils.GetMessagesFromResponse(bytes, count);

                    foreach (var receiveivedMessage in responseData)
                    {
                        if (string.IsNullOrWhiteSpace(receiveivedMessage))
                        {
                            continue;
                        }

                        if (receiveivedMessage.Contains(Consts.EndOfMessage))
                        {
                            if (receiveivedMessage.Contains(Consts.HistorySendSuccessfully))
                            {
                                return;
                            }

                            var messageToPrint = string.IsNullOrWhiteSpace(unfinishedMessage)
                                ? receiveivedMessage
                                : $"{unfinishedMessage}{receiveivedMessage}";

                            Console.WriteLine($"Received: {messageToPrint.ToHumanReadable()}");
                            unfinishedMessage = "";
                        }
                        else
                        {
                            unfinishedMessage = receiveivedMessage;
                        }
                    }
                }
                catch (Exception) { }
            }
        }

        private void StartReceivingMessages(TcpClient client)
        {
            var stream = client.GetStream();

            Task.Run(() =>
            {
                var unfinishedMessage = "";

                while (client.Connected)
                {
                    unfinishedMessage = PrintReceivedMessages(stream, unfinishedMessage);
                }
            });
        }

        string PrintReceivedMessages(NetworkStream stream, string unfinishedMessage)
        {
            try
            {
                var bytes = new byte[256];
                stream.ReadTimeout = 500;
                var count = stream.Read(bytes, 0, bytes.Length);
                var responseData = Utils.GetMessagesFromResponse(bytes, count);

                foreach(var receiveivedMessage in responseData)
                {
                    if (string.IsNullOrWhiteSpace(receiveivedMessage))
                    {
                        continue;
                    }

                    if (receiveivedMessage.Contains(Consts.EndOfMessage))
                    {
                        var messageToPrint = string.IsNullOrWhiteSpace(unfinishedMessage)
                            ? receiveivedMessage
                            : $"{unfinishedMessage}{receiveivedMessage}";

                        Console.WriteLine($"Received: {messageToPrint.ToHumanReadable()}");
                        unfinishedMessage = "";
                    }
                    else
                    {
                        unfinishedMessage = receiveivedMessage;
                    }
                }
            }
            catch(Exception) { }

            return unfinishedMessage;
        }

        void SendMessages(TcpClient client, long messageCount)
        {
            for (var i = 0; i < messageCount; i++)
            {
                SendMessage(client);

                Thread.Sleep(Utils.GetRandomDelay());
            }
        }

        void SendMessage(TcpClient client)
        {
            string message = GetMessage();
            var data = message.ToSendable();

            NetworkStream stream = client.GetStream();

            stream.Write(data, 0, data.Length);

            Console.WriteLine("Sent: {0}", message);
        }

        string GetMessage() => $"{messages[Utils.GetRandom().Next(0, messages.Length)]}";
    }
}
