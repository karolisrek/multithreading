using System;
using Common;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new ComplexMultithreadingServer.Server();
            // start server that uses threads
            server.StartThread();

            // start server that uses tasks
            //server.StartTpl();
        }
    }
}
