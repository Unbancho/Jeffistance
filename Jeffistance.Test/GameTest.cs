using NUnit.Framework;
using Jeffistance.Models;

namespace Jeffistance.Test
{
    [TestFixture]
    public class BasicGameTests
    {
        Game game;

        [SetUp]
        public void Setup()
        {
            game = new Game(new BasicGamemode());
        }

        [Test]
        public void TestStart()
        {
            game.Start(new Player[] {new Player(), new Player()});
            
            Assert.IsTrue(game.InProgress);
            Assert.That(game.Players, Has.All.Property("Faction").Not.Null);
            Assert.That(game.Players, Has.All.Property("ID").GreaterThanOrEqualTo(0));
            Assert.That(game.Players, Has.All.Property("Role").Not.Null);
        }

        [Test]
        public void TestPhases()
        {
            Assert.That(game.CurrentPhase, Is.EqualTo(Phase.Standby));
            game.Start(new Player[] {new Player()});
            Assert.That(game.CurrentPhase, Is.EqualTo(Phase.TeamPicking));
        }
    }
}
