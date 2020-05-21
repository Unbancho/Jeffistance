using ModusOperandi.Networking;
using ModusOperandi.Messaging;
using Jeffistance.Common.Services.MessageProcessing;
using Jeffistance.Common.Models;
using Jeffistance.Common.Services.IoC;
using Jeffistance.JeffServer.Services;
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
        private ObservableCollection<User> ObservableUserList {get; set;} = new ObservableCollection<User>();
        public List<User> UserList {get {return new List<User> (ObservableUserList);}}
        private Dictionary<ClientConnection, User> UserConnectionDictionary = new Dictionary<ClientConnection, User>();
        private Dictionary<string, User> UserNameDictionary = new Dictionary<string, User>();
        private Dictionary<Guid, User> UserGuidDictionary = new Dictionary<Guid, User>();
        private MessageHandler MessageHandler {get; set;}


        public Server()
        {
            RegisterServerDependencies();
            Host = new LocalUser("Server")
            {
                IsHost = true,
                Perms = new Permissions
                {
                    CanKick = true
                }
            };
        }

        private void RegisterServerDependencies()
        {
            IoCManager.Register<IServerMessageFactory, ServerMessageFactory>();

            IoCManager.BuildGraph();
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
            ObservableUserList.CollectionChanged += OnUserListChanged;
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
            ObservableUserList.Remove(user);
            Connection.Kick(user.Connection);
        }

        public void AddUser(User user)
        {
            UserConnectionDictionary[user.Connection] = user;
            UserNameDictionary[user.Name.ToLower()] = user;
            UserGuidDictionary[user.ID] = user;
            ObservableUserList.Add(user);
        }

        public User GetUser(string username)
        {
            UserNameDictionary.TryGetValue(username.ToLower(), out User user);
            return user;
        }

        public User GetUser(Guid guid)
        {
            UserGuidDictionary.TryGetValue(guid, out User user);
            return user;            
        }

        public User GetUser(ClientConnection connection)
        {
            UserConnectionDictionary.TryGetValue(connection, out User user);
            return user;        
        }

        public void OnUserDisconnect(object obj, DisconnectionArgs args)
        {
            ObservableUserList.Remove(GetUser(args.Client));
        }

        public void Broadcast(Message message)
        {
            Connection.Broadcast(message);
        }

        private void OnUserListChanged(object obj, NotifyCollectionChangedEventArgs args)
        {
            // Greeting message also handles the greeting chat message
            // TODO Figure out where to put the "user left" mesasge

            // string messageText;
            // switch (args.Action)
            // {
            //     case NotifyCollectionChangedAction.Add:
            //         messageText = $"{((User)args.NewItems[0]).Name} has joined.";
            //         break;
            //     case NotifyCollectionChangedAction.Remove:
            //         messageText = $"{((User)args.OldItems[0]).Name} has left.";
            //         break;
            //     default:
            //         messageText = "";
            //         break;
            // }
            
            var messageFactory = IoCManager.Resolve<IServerMessageFactory>();

            var updateList = messageFactory.MakeUpdateMessage();
            updateList["UserList"] = UserList;
            MessageHandler.Broadcast(updateList);
        }
    }
}