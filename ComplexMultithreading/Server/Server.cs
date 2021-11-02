using Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ComplexMultithreadingServer
{
    class Server
    {
        Dictionary<long, TcpClient> ActiveClientsById = new Dictionary<long, TcpClient>();
        long NextId = 0;
        List<string> ReceivedMessages = new List<string>();

        public void StartTpl()
        {
            TcpListener server = null;

            try
            {
                server = new TcpListener(IPAddress.Parse("127.0.0.1"), 13000);
                server.Start();

                while (true)
                {
                    Console.WriteLine("Waiting for a connection... ");
                    var client = server.AcceptTcpClient();
                    Task.Run(() => SendPreviousMessages(client));
                    Task.Run(() => EnterListeningLoop(client));
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                server.Stop();
            }

            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }

        public void StartThread()
        {
            TcpListener server = null;

            try
            {
                server = new TcpListener(IPAddress.Parse("127.0.0.1"), 13000);
                server.Start();

                while (true)
                {
                    Console.WriteLine("Waiting for a connection... ");
                    var client = server.AcceptTcpClient();
                    var thread = new Thread(() => StartClientThread(client));
                    thread.Start();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                server.Stop();
            }

            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }

        void StartClientThread(TcpClient client)
        {
            var stream = client.GetStream();

            SendPreviousMessages(client);
            SendMessage(stream, Consts.HistorySendSuccessfully);

            EnterListeningLoop(client);
        }

        private void SendPreviousMessages(TcpClient client) => SendPreviousMessages(client.GetStream());

        private void SendPreviousMessages(NetworkStream stream)
        {
            foreach (var message in ReceivedMessages)
            {
                SendMessage(stream, message);
            }
        }

        void SendMessage(NetworkStream stream, string message)
        {
            if (!message.Contains(Consts.EndOfMessage))
            {
                message = $"{message}{Consts.EndOfMessage}";
            }

            var data = message.ToByteArray();

            stream.Write(data, 0, data.Length);
        }

        void EnterListeningLoop(TcpClient client)
        {
            var clientId = AddNewClient(client);

            Console.WriteLine("Connected!");

            var stream = client.GetStream();

            var bytes = new byte[256];
            int i;

            try
            {
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    var receivedMessages = Utils.GetMessagesFromResponse(bytes, i);

                    foreach (var receivedMessage in receivedMessages)
                    {
                        Console.WriteLine($"Received: {receivedMessage.ToHumanReadable()}");
                    }

                    Broadcast(receivedMessages, clientId);
                }
            }
            catch (Exception) { }

            ActiveClientsById.Remove(clientId);
            client.Close();

            Broadcast($"Client #{clientId} left the chat...");
        }

        long AddNewClient(TcpClient client)
        {
            var clientId = NextId;
            ActiveClientsById.Add(NextId, client);
            NextId++;

            return clientId;
        }

        void Broadcast(string[] receivedMessages, long clientId)
        {
            foreach (var receivedMessage in receivedMessages)
            {
                foreach (var clientByKey in ActiveClientsById)
                {
                    var messageToSend = $"Client #{clientId}: {receivedMessage}";

                    var stream = clientByKey.Value.GetStream();
                    var data = messageToSend.ToByteArray();

                    try
                    {
                        if (clientByKey.Key != clientId)
                        {
                            stream.Write(data, 0, data.Length);
                        }

                        ReceivedMessages.Add(messageToSend);
                    }
                    catch(Exception) { }
                }

                Console.WriteLine($"Broadcast {receivedMessage.ToHumanReadable()}");
            }
        }

        void Broadcast(string receivedMessage)
        {
            if (!receivedMessage.Contains(Consts.EndOfMessage))
            {
                receivedMessage = $"{receivedMessage}{Consts.EndOfMessage}";
            }

            foreach (var clientByKey in ActiveClientsById)
            {
                var stream = clientByKey.Value.GetStream();
                var data = receivedMessage.ToByteArray();

                try
                {
                    stream.Write(data, 0, data.Length);
                    ReceivedMessages.Add(receivedMessage);
                }
                catch(Exception) { }
            }

            Console.WriteLine($"Broadcast {receivedMessage.ToHumanReadable()}");
        }
    }
}
