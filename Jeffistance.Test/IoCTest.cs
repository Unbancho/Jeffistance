using NUnit.Framework;
using Jeffistance.Common.Services.IoC;
using Jeffistance.Test.Dummies;

namespace Jeffistance.Test
{
    [TestFixture]
    public class IocTest
    {
        [SetUp]
        public void Setup()
        {
            IoCManager.Clear();
        }

        [Test]
        public void TestRegister()
        {
            IoCManager.Register<IDummyMessageFactory, DummyMessageFactory>();
        }

        [Test]
        public void TestResolve()
        {
            IoCManager.Register<IDummyMessageFactory, DummyMessageFactory>();
            IoCManager.BuildGraph();
            var messageFactory = IoCManager.Resolve<IDummyMessageFactory>();

            Assert.That(messageFactory, Is.InstanceOf<IDummyMessageFactory>());
        }

        [Test]
        public void TestRegisterSameTwice()
        {
            IoCManager.Register<IDummyMessageFactory, DummyMessageFactory>();
            Assert.That(
                IoCManager.Register<IDummyMessageFactory, DummyMessageFactory>,
                Throws.InvalidOperationException
            );
        }

        [Test]
        public void TestResolveNotBuilt()
        {
            IoCManager.Register<IDummyMessageFactory, DummyMessageFactory>();
            Assert.That(
                IoCManager.Resolve<IDummyMessageFactory>,
                Throws.InvalidOperationException
            );
        }
    }
}
