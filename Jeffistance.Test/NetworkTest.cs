using NUnit.Framework;
using Networking;
using System;

namespace Jeffistance.Test
{
    [TestFixture]
    public class Tests
    {
        Host host;
        [SetUp]
        public void Setup()
        {
            host = new Host();
        }

        [TearDown]
        public void TearDown()
        {
            // This gives me a weird-ass null reference error for CancellationToken so it's epic
            // foreach (User user in host.UserList)
            // {
            //     user.Disconnect();
            // }
            host.Disconnect();
        }

        [Test]
        public void HostTest()
        {
            Assert.IsNotNull(host);
        }

        [Test, Timeout(1000)]
        public void ConnectionTest()
        {
            while(true)
            {
                if (host.UserList.Count > 0) break;
            }
        }
    }
}