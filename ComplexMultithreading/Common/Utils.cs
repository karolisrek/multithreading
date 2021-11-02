using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Common
{
    public static class Utils
    {
        public static int GetRandomDelay() => GetRandom().Next(1500, 3000);
        public static int GetNumberOfMessagesToSend() => GetRandom().Next(5, 10);
        public static Random GetRandom() => new Random(Guid.NewGuid().GetHashCode());
        public static byte[] ToByteArray(this string message) => Encoding.ASCII.GetBytes(message);
        public static byte[] ToSendable(this string message) => $"{message}{Consts.EndOfMessage}".ToByteArray();
        public static string[] GetMessagesFromResponse(byte[] bytes, int count) =>
            Regex.Split(Encoding.ASCII.GetString(bytes, 0, count), $"(?<={Consts.EndOfMessage})").Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        public static string ToHumanReadable(this string message) => message.Replace(Consts.EndOfMessage, "");
    }
}
