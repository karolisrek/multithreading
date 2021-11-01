using Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ComplexMultithreadingServer
{
    class Server
    {
        Dictionary<long, TcpClient> ActiveClientsById = new Dictionary<long, TcpClient>();
        long NextId = 0;

        public void Start()
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

        void EnterListeningLoop(TcpClient client)
        {
            var clientId = AddNewClient(client);

            Console.WriteLine("Connected!");

            var stream = client.GetStream();

            var bytes = new byte[256];
            int i;

            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                var receivedMessage = Encoding.ASCII.GetString(bytes, 0, i);
                Console.WriteLine($"Received: {receivedMessage}");

                Broadcast(receivedMessage, clientId);
            }

            ActiveClientsById.Remove(clientId);
            client.Close();
        }

        long AddNewClient(TcpClient client)
        {
            var clientId = NextId;
            ActiveClientsById.Add(NextId, client);
            NextId++;

            return clientId;
        }

        void Broadcast(string receivedMessage, long clientId)
        {
            foreach (var clientByKey in ActiveClientsById)
            {
                var messageToSend = clientByKey.Key != clientId ? $"Client #{clientId}: {receivedMessage}" :  $"You: {receivedMessage}";

                var stream = clientByKey.Value.GetStream();
                var data = messageToSend.ToByteArray();

                try
                {
                    stream.Write(data, 0, data.Length);
                }
                catch(Exception) { }
            }

            Console.WriteLine($"Broadcast {receivedMessage}");
        }
    }
}
