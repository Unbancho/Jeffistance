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
            
            Assert.Multiple(() =>
            {
                Assert.That(game.InProgress, Is.True);
                Assert.That(game.Players, Has.All.Property("Faction").Not.Null);
                Assert.That(game.Players, Has.All.Property("ID").GreaterThanOrEqualTo(0));
                Assert.That(game.Players, Has.All.Property("Role").Not.Null);
            });
        }

        [Test]
        public void TestPhases()
        {
            Assert.That(game.CurrentPhase, Is.EqualTo(Phase.Standby));
            game.Start(new Player[] {new Player()});
            Assert.That(game.CurrentPhase, Is.EqualTo(Phase.TeamPicking));
        }
    }

    [TestFixture]
    public class BasicGamemodeTests
    {
        BasicGamemode gm;

        [SetUp]
        public void Setup()
        {
            gm = new BasicGamemode();
        }

        [Test]
        public void PickLeaderTest([Range(1, 10)] int n)
        {
            Player[] players = new Player[n];
            for (int i = 0; i < n; i++)
            {
                players[i] = new Player
                {
                    ID = i
                };
            }

            for (int i = 0; i < n; i++)
            {
                gm.PickLeader(players);
            }
            Assert.That(players, Has.Exactly(1).Property("IsLeader").True);
            Assert.That(players[n-1].IsLeader, Is.True);
            gm.PickLeader(players);
            Assert.That(players[0].IsLeader, Is.True);
        }
    }
}
