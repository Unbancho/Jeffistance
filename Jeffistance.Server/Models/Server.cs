using ModusOperandi.Networking;
using ModusOperandi.Messaging;
using Jeffistance.Common.Services.MessageProcessing;
using Jeffistance.Common.Models;
using Jeffistance.JeffServer.Services.MessageProcessing;
using System.Collections.Generic;
using System;

namespace Jeffistance.JeffServer.Models
{
    public class Server
    {
        const string DEFAULT_HOST_NAME = "Admin";

        public LocalUser Host {get; set;}
        private ServerConnection Connection {get; set;}
        public List<User> UserList {get; set;}
        public MessageHandler MessageHandler {get; set;}

        public Server()
        {
            UserList = new List<User>();
            Host = new LocalUser("Server")
            {
                IsHost = true,
                Perms = new Permissions
                {
                    CanKick = true
                }
            };
        }

        public void ConnectHost(string username, JeffistanceMessageProcessor messageProcessor)
        {
            username = string.IsNullOrWhiteSpace(username) ? DEFAULT_HOST_NAME : username;
            Host.Name = username;
            Host.Connect(NetworkUtilities.GetLocalIPAddress(), Connection.PORT_NO);
            Host.AttachMessageHandler(new MessageHandler(messageProcessor, Host.Connection));
            Host.GreetServer();
        }

        public void Run(int port)
        {
            Connection = new ServerConnection(port);
            MessageHandler = new MessageHandler(new ServerMessageProcessor(this), Connection);
            Connection.OnMessageReceived += MessageHandler.OnMessageReceived;
            Connection.Run();
        }

        public void Stop()
        {
            Host.Disconnect();
            Connection.Stop();
        }

        public void Kick(User user)
        {
            UserList.Remove(user);
            Connection.Kick(user.Connection);
        }

        public void AddUser(User user)
        {
            user.ID =  Guid.NewGuid();
            UserList.Add(user);
            Message updateList = new Message($"{user.Name} has joined.", JeffistanceFlags.Update);
            updateList["UserList"] = UserList;
            MessageHandler.Broadcast(updateList);
        }

        public void Broadcast(Message message)
        {
            Connection.Broadcast(message);
        }
    }
}