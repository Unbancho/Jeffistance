using System;
using Jeffistance.Common.Services.MessageProcessing;
using ModusOperandi.Messaging;
using Jeffistance.Common.Models;
using System.Collections.Generic;

namespace Jeffistance.Common.Services
{
    public interface IClientMessageFactory
    {
        Message MakeChatMessage(string text, string userID = null);
        Message MakeChatMessage(string text, Guid userID);

        Message MakeLobbyReadyMessage(string userID);
        Message MakeLobbyReadyMessage(Guid userID);

        Message MakeGreetingMessage(User user);

        Message MakeEditChatMessage(string content, string messageID);

        Message MakeDeleteChatMessage(string messageID);

        Message MakeJoinGameMessage();

        Message MakeGetPlayerInfoMessage(List<Player> players);

        Message MakeGamePhaseReadyMessage(string UserID);

        Message MakeAdvanceGamePhaseMessage();

        Message MakePickTeamMessageMessage(List<string> playersInTeamIDs);

        Message MakeDeclareLeaderMessage(int teamSize, Player leader);

        Message MakeVoteMessage(string userID, bool vote);
        Message MakeStartMissionVotingMessage(List<string> playersInTeamIDs);

        Message MakeShowTeamVoteResultMessage(Dictionary<string, bool> voters, bool successfulTeamFormation, int fails);

        Message MakeMissionVoteMessage(string userID, bool vote);

        Message MakeShowMissionResultMessage(bool result);
        Message MakeEndGameMessage(string name, List<string> spyPlayersIDs);
    }

    public class ClientMessageFactory : IClientMessageFactory
    {
        public Message MakeChatMessage(string text, string userID = null)
        {
            var message = new Message(text, JeffistanceFlags.Chat);
            message["MessageID"] = Guid.NewGuid().ToString();
            if (userID != null)
            {
                message["UserID"] = userID;
            }

            return message;
        }

        public Message MakeChatMessage(string text, Guid userID)
        {
            return this.MakeChatMessage(text, userID.ToString());
        }

        public Message MakeLobbyReadyMessage(string userID)
        {
            var message = new Message(flags: JeffistanceFlags.LobbyReady);
            message["UserID"] = userID;
            return message;
        }

        public Message MakeLobbyReadyMessage(Guid userID)
        {
            return this.MakeLobbyReadyMessage(userID.ToString());
        }

        public Message MakeGreetingMessage(User user)
        {
            var message = new Message(flags: JeffistanceFlags.Greeting);
            message["User"] = user;
            return message;
        }

        public Message MakeEditChatMessage(string content, string messageID)
        {
            var message = new Message(flags: JeffistanceFlags.EditChatMessage);
            message["MessageID"] = messageID;
            message["NewText"] = content;
            return message;
        }

        public Message MakeDeleteChatMessage(string messageID)
        {
            var message = new Message(flags: JeffistanceFlags.DeleteChatMessage);
            message["MessageID"] = messageID;
            return message;
        }
        
        public Message MakeJoinGameMessage()
        {
            return new Message(flags: JeffistanceFlags.JoinGameMessage);
        }

        public Message MakeGetPlayerInfoMessage(List<Player> players)
        {
            var message = new Message(flags: JeffistanceFlags.GetPlayerInfoMessage);
            message["Players"] = players;
            return message;
        }

        public Message MakeGamePhaseReadyMessage(string userID)
        {
            var message = new Message(flags: JeffistanceFlags.GamePhaseReadyMessage);
            message["UserID"] = userID;
            return message;
        }

        public Message MakeAdvanceGamePhaseMessage()
        {
            return new Message(flags: JeffistanceFlags.AdvanceGamePhaseMessage);
        }

        public Message MakePickTeamMessageMessage(List<string> playersInTeamIDs)
        {
            var message =  new Message(flags: JeffistanceFlags.PickTeamMessage);
            message["PlayersInTeamIDs"] = playersInTeamIDs;
            return message;
        }

        public Message MakeDeclareLeaderMessage(int teamSize, Player leader)
        {
            var message =  new Message(flags: JeffistanceFlags.DeclareLeaderMessage);
            message["TeamSize"] = teamSize;
            message["Leader"] = leader;
            return message;
        }

        public Message MakeVoteMessage(string userID, bool vote)
        {
            var message =  new Message(flags: JeffistanceFlags.VoteMessage);
            message["Vote"] = vote;
            message["UserID"] = userID;
            return message;
        }

        public Message MakeStartMissionVotingMessage(List<string> playersInTeamIDs)
        {
            var message =  new Message(flags: JeffistanceFlags.StartMissionVotingMessage);
            message["PlayersInTeamIDs"] = playersInTeamIDs;
            return message;
        }
        
        public Message MakeShowTeamVoteResultMessage(Dictionary<string, bool> voters, bool successfulTeamFormation, int fails)
        {
            var message =  new Message(flags: JeffistanceFlags.ShowTeamVoteResultMessage);
            message["Voters"] = voters;
            message["SuccessfulTeamFormation"] = successfulTeamFormation;
            message["Fails"] = fails;
            return message;
        }

        public Message MakeMissionVoteMessage(string userID, bool vote)
        {
            var message =  new Message(flags: JeffistanceFlags.MissionVoteMessage);
            message["Vote"] = vote;
            message["UserID"] = userID;
            return message;
        }

        public Message MakeShowMissionResultMessage(bool result)
        {
            var message =  new Message(flags: JeffistanceFlags.ShowMissionResultMessage);
            message["Result"] = result;
            return message;
        }

        public Message MakeEndGameMessage(string name, List<string> spyPlayersIDs)
        {
            var message =  new Message(flags: JeffistanceFlags.EndGameMessage);
            message["Name"] = name;
            message["SpyPlayersIDs"] = spyPlayersIDs;
            return message;
        }

    }
}
