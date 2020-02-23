using NUnit.Framework;
using Jeffistance.Models;
using Jeffistance.Services;
using System.Linq;
using System.Collections.Generic;

namespace Jeffistance.Test
{
    [TestFixture]
    public class BasicGameTests
    {
        List<Player> players;
        Game game;
        PlayerEventManager playerEventManager;

        [SetUp]
        public void Setup()
        {
            playerEventManager = new PlayerEventManager();
            game = new Game(new BasicGamemode(), playerEventManager);
            players = new List<Player>();
            for (int i = 0; i < 10; i++)
            {
                players.Add(new Player());
            }
        }

        [Test]
        public void TestStart()
        {
            game.Start(players);
            
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
            game.Start(players);
            Assert.That(game.CurrentPhase, Is.EqualTo(Phase.TeamPicking));
        }

        [Test]
        public void TestPickTeam([Range(0, 2)] int x, [Range(3, 5)] int y, [Range(6, 8)] int z)
        {
            game.Start(players);
            int[] team = {x, y, z};
            playerEventManager.PickTeam(team);
            Assert.That(game.CurrentPhase, Is.EqualTo(Phase.TeamVoting));
            var currentTeamIDs = game.CurrentTeam.Select((p) => p.ID);
            Assert.That(currentTeamIDs, Is.EqualTo(team));
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
