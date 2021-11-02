namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new ComplexMultithreadingClient.Client();
            // Start client that uses tasks
            //client.StartTpl();

            // Start client that uses threads
            client.StartThread();
        }
    }
}
