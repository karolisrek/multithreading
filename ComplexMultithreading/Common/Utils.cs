using System;

namespace Common
{
    public static class Utils
    {
        public static int GetRandomDelay() => GetRandom().Next(1500, 3000);
        public static int GetNumberOfMessagesToSend() => GetRandom().Next(5, 10);
        public static Random GetRandom() => new Random(Guid.NewGuid().GetHashCode());
        public static byte[] ToByteArray(this string message) => System.Text.Encoding.ASCII.GetBytes(message);
    }
}
