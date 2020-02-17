using NUnit.Framework;
using Jeffistance.Services;
using System.Threading;

using System;

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
            server.ShutDown();
        }

        [Test, Timeout(20000)]
        public void TestConnection()
        {
            server.Run();
            var client = new ClientConnection(NetworkUtilities.GetLocalIPAddress(), DEFAULT_PORT);
            while(true)
            {
                 if (server.Clients.Count > 0) break;
            }
            Assert.IsTrue(server.Clients.Count > 0);
            // FIXME vvvvv THIS SHOULD WORK ^^^^^^^ THIS SHOULDN'T
            // Assert.That(server.Clients.Count, Is.GreaterThan(0).After(5).Seconds.PollEvery(500).MilliSeconds);
        }

        [Test, Timeout(2000)]
        public void TestDisconnectClient()
        {
            server.Run();
            var client = new ClientConnection(NetworkUtilities.GetLocalIPAddress(), DEFAULT_PORT);
            while(true)
            {
                if (server.Clients.Count > 0) break;
            }

            foreach(ClientConnection c in server.Clients.ToArray())
            {
                server.Kick(c);
            }

            Assert.IsTrue(server.Clients.Count == 0);
        }
    }
}