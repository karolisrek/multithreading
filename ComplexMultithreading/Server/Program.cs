using System;
using Common;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new ComplexMultithreadingServer.Server();
            server.Start();
        }
    }
}
