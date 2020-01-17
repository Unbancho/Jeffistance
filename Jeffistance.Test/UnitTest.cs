using NUnit.Framework;

namespace Jeffistance.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            User user = new User();
            Assert.AreEqual("Guest", user.Name);
        }
    }
}