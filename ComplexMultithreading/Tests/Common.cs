using NUnit.Framework;
using Common;

namespace Tests
{
    public class Tests
    {
        [Test]
        public void GetMessagesFromResponse_TwoMessages()
        {
            var responseAsString = "foo<<EOM>>bar<<EOM>>";
            var response = responseAsString.ToByteArray();
            var result = Utils.GetMessagesFromResponse(response, responseAsString.Length);

            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("foo<<EOM>>", result[0]);
            Assert.AreEqual("bar<<EOM>>", result[1]);
        }

        [Test]
        public void GetMessagesFromResponse_OneAndAHalfMessage()
        {
            var responseAsString = "foo<<EOM>>bar<<E";
            var response = responseAsString.ToByteArray();
            var result = Utils.GetMessagesFromResponse(response, responseAsString.Length);

            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("foo<<EOM>>", result[0]);
            Assert.AreEqual("bar<<E", result[1]);
        }


    }
}