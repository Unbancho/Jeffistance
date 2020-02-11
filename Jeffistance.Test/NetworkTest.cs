using NUnit.Framework;
using Jeffistance.Services;
using System.Threading;

namespace Jeffistance.Test
{
    [TestFixture]
    public class Tests
    {
        ServerConnection server;
        public const int DEFAULT_PORT = 7700;

        [SetUp]
        public void Setup()
        {
            server = new ServerConnection(DEFAULT_PORT);
        }

        [TearDown]
        public void TearDown()
        {
            server.Stop();
        }

        [Test, Timeout(2000)]
        public void TestConnection()
        {
            server.Run();
            var client = new ClientConnection(NetworkUtilities.GetLocalIPAddress(), DEFAULT_PORT);
            while(true)
            {
                 if (server.Clients.Count > 0) break;
            }
            Assert.IsTrue(server.Clients.Count > 0);
        }
    }
}