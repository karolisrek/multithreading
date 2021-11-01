namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new ComplexMultithreadingClient.Client();
            client.Start();
        }
    }
}
