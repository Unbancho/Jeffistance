using NUnit.Framework;
using Jeffistance.Models;

namespace Jeffistance.Test
{
    [TestFixture]
    public class StandardGameTests
    {
        Game game;

        [SetUp]
        public void Setup()
        {
            game = new Game(new StandardGamemode());
        }

        [Test]
        public void TestStart()
        {
            game.Start(new Player[] {new Player(), new Player()});
            
            Assert.IsTrue(game.InProgress);
            Assert.That(game.Players, Has.All.Property("Faction").Not.Null);
        }
    }
}
