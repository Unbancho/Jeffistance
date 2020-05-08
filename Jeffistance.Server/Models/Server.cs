using ModusOperandi.Networking;
using ModusOperandi.Messaging;
using Jeffistance.Common.Services.MessageProcessing;
using Jeffistance.Common.Models;
using Jeffistance.JeffServer.Services.MessageProcessing;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System;

namespace Jeffistance.JeffServer.Models
{
    public class Server
    {
        const string DEFAULT_HOST_NAME = "Admin";

        public LocalUser Host {get; set;}
        private ServerConnection Connection {get; set;}
        public ObservableCollection<User> UserList {get; set;} = new ObservableCollection<User>();
        private Dictionary<ClientConnection, User> UserConnectionDictionary = new Dictionary<ClientConnection, User>();
        public MessageHandler MessageHandler {get; set;}

        public Server()
        {
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
            UserList.CollectionChanged += OnUserListChanged;
            Connection.OnDisconnection += OnUserDisconnect;
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
            if(user.Connection != null)
                UserConnectionDictionary[user.Connection] = user;
            UserList.Add(user);
        }

        public void OnUserDisconnect(object obj, DisconnectionArgs args)
        {
            UserList.Remove(UserConnectionDictionary[args.Client]);
        }

        public void Broadcast(Message message)
        {
            Connection.Broadcast(message);
        }

        private void OnUserListChanged(object obj, NotifyCollectionChangedEventArgs args)
        {
            string messageText;
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    messageText = $"{((User)args.NewItems[0]).Name} has joined.";
                    break;
                case NotifyCollectionChangedAction.Remove:
                    messageText = $"{((User)args.OldItems[0]).Name} has left.";
                    break;
                default:
                    messageText = "";
                    break;
            }
            Message updateList = new Message(messageText, JeffistanceFlags.Update);
            updateList["UserList"] = new List<User>(UserList);
            MessageHandler.Broadcast(updateList);
        }
    }
}