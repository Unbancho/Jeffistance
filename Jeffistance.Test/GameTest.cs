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
            for (int i = 0; i < 5; i++)
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
        public void TestPickTeam([Range(0, 2)] int x, [Range(3, 4)] int y)
        {
            game.Start(players);
            Assert.That(game.NextTeamSize, Is.EqualTo(2));
            int[] team = {x, y};
            playerEventManager.PickTeam(team);
            Assert.That(game.CurrentPhase, Is.EqualTo(Phase.TeamVoting));
            var currentTeamIDs = game.CurrentTeam.Select((p) => p.ID);
            Assert.That(currentTeamIDs, Is.EqualTo(team));
        }

        [Test]
        public void TestVoteTeamAllAccept()
        {
            game.Start(players);
            playerEventManager.PickTeam(new int[] {0, 1});
            foreach (var player in players)
            {
                playerEventManager.VoteTeam(player.ID, true);
            }
            Assert.That(game.CurrentPhase, Is.EqualTo(Phase.MissionVoting));
        }

        [Test]
        public void TestVoteTeamAllReject()
        {
            game.Start(players);
            var firstLeader = game.CurrentLeader;
            playerEventManager.PickTeam(new int[] {0, 1});
            foreach (var player in players)
            {
                playerEventManager.VoteTeam(player.ID, false);
            }
            Assert.That(game.CurrentPhase, Is.EqualTo(Phase.TeamPicking));
            Assert.That(game.CurrentLeader.ID, Is.Not.EqualTo(firstLeader.ID));
        }

        [Test]
        public void TestVoteTeamMajorityAccept()
        {
            game.Start(players);
            playerEventManager.PickTeam(new int[] {0, 1});
            playerEventManager.VoteTeam(0, true);
            playerEventManager.VoteTeam(1, false);
            playerEventManager.VoteTeam(2, true);
            playerEventManager.VoteTeam(3, false);
            playerEventManager.VoteTeam(4, true);

            Assert.That(game.CurrentPhase, Is.EqualTo(Phase.MissionVoting));
        }

        [Test]
        public void TestVoteTeamMajorityReject()
        {
            game.Start(players);
            playerEventManager.PickTeam(new int[] {0, 1});
            playerEventManager.VoteTeam(0, false);
            playerEventManager.VoteTeam(1, false);
            playerEventManager.VoteTeam(2, true);
            playerEventManager.VoteTeam(3, false);
            playerEventManager.VoteTeam(4, true);

            Assert.That(game.CurrentPhase, Is.EqualTo(Phase.TeamPicking));
        }

        [Test]
        public void TestVoteTeamTie()
        {
            players.Add(new Player());
            game.Start(players);
            playerEventManager.PickTeam(new int[] {0, 1});
            playerEventManager.VoteTeam(0, true);
            playerEventManager.VoteTeam(1, false);
            playerEventManager.VoteTeam(2, true);
            playerEventManager.VoteTeam(3, false);
            playerEventManager.VoteTeam(4, true);
            playerEventManager.VoteTeam(5, false);

            Assert.That(game.CurrentPhase, Is.EqualTo(Phase.TeamPicking));
        }

        [Test]
        public void TestVoteMissionSuccess()
        {
            game.Start(players);
            playerEventManager.PickTeam(new int[] {0, 1, 2});
            playerEventManager.VoteMission(0, true);
            playerEventManager.VoteMission(1, true);
            playerEventManager.VoteMission(2, true);

            Assert.That(game.CurrentPhase, Is.EqualTo(Phase.TeamPicking));
            Assert.That(game.ResistanceWinCount, Is.EqualTo(1));
            Assert.That(game.CurrentRound, Is.EqualTo(1));
        }

        [Test]
        public void TestVoteMissionFailure()
        {
            game.Start(players);
            playerEventManager.PickTeam(new int[] {0, 1, 2});
            playerEventManager.VoteMission(0, true);
            playerEventManager.VoteMission(1, false);
            playerEventManager.VoteMission(2, true);

            Assert.That(game.CurrentPhase, Is.EqualTo(Phase.TeamPicking));
            Assert.That(game.SpiesWinCount, Is.EqualTo(1));
            Assert.That(game.CurrentRound, Is.EqualTo(1));
        }

        [Test]
        public void TestResistanceVictory()
        {
            game.Start(players);

            for (int i = 0; i < 3; i++)
            {
                playerEventManager.PickTeam(new int[] {0, 1, 2});
                playerEventManager.VoteMission(0, true);
                playerEventManager.VoteMission(1, true);
                playerEventManager.VoteMission(2, true);
            }
            Assert.That(game.CurrentPhase, Is.EqualTo(Phase.GameEnd));
            FactionFactory ff = new FactionFactory();
            Assert.That(game.Winner, Is.InstanceOf<ResistanceFaction>());
        }

        [Test]
        public void TestSpiesVictory()
        {
            game.Start(players);

            for (int i = 0; i < 3; i++)
            {
                playerEventManager.PickTeam(new int[] {0, 1, 2});
                playerEventManager.VoteMission(0, false);
                playerEventManager.VoteMission(1, false);
                playerEventManager.VoteMission(2, false);
            }
            Assert.That(game.CurrentPhase, Is.EqualTo(Phase.GameEnd));
            FactionFactory ff = new FactionFactory();
            Assert.That(game.Winner, Is.InstanceOf<SpiesFaction>());
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
