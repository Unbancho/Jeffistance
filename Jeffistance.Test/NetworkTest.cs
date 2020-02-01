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
            host.Disconnect();
        }

        [Test]
        public void TestHost()
        {
            Assert.IsNotNull(host);
        }

        [Test, Timeout(1000)]
        public void TestConnection()
        {
            while(true)
            {
                if (host.UserList.Count > 0) break;
            }
        }

        [Test, Timeout(2000)]
        public void TestDisconnectClient()
        {
            while(true)
            {
                if (host.UserList.Count > 0) break;
            }

            foreach(User user in host.UserList.ToArray())
            {
                host.Kick(user);
            }

            Assert.IsTrue(host.UserList.Count == 0);
        }
    }
}