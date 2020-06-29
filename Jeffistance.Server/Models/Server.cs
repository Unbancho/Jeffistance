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
using Jeffistance.Common.Services.PlayerEventManager;
using Jeffistance.Common.ExtensionMethods;
using Microsoft.Extensions.Logging;
using System.Configuration;

namespace Jeffistance.JeffServer.Models
{
    public class Server
    {
        public ServerLobby Lobby { get; private set; }

        public IServerChatManager ChatManager { get; private set; }

        private ILogger _logger;

        const string DEFAULT_HOST_NAME = "Admin";

        public LocalUser Host {get; set;}
        private ServerConnection Connection {get; set;}
        private ObservableCollection<User> ObservableUserList {get; set;} = new ObservableCollection<User>();
        public List<User> UserList {get {return new List<User> (ObservableUserList);}}
        private Dictionary<ClientConnection, User> UserConnectionDictionary = new Dictionary<ClientConnection, User>();
        private Dictionary<string, User> UserNameDictionary = new Dictionary<string, User>();
        private Dictionary<Guid, User> UserGuidDictionary = new Dictionary<Guid, User>();
        private MessageHandler MessageHandler {get; set;}
        public Game Game {get; set;}

        private bool _inGame = false;

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
            ChatManager = IoCManager.Resolve<IServerChatManager>();
            ChatManager.Server = this;
            Lobby = new ServerLobby(this);
        }

        private void RegisterServerDependencies()
        {
            IoCManager.Register<IServerMessageFactory, ServerMessageFactory>();
            IoCManager.Register<IServerChatManager, ServerChatManager>();

            var logLevel = ConfigurationManager.AppSettings["LogLevel"].ToLogLevel();
            IoCManager.AddServerLogging(builder => builder
                .AddFile("Logs/Jeffistance-Server-{Date}.txt", logLevel)
                .AddConsole()
                .SetMinimumLevel(logLevel));

            IoCManager.BuildGraph();

            _logger = IoCManager.GetServerLogger();
            _logger.LogInformation("Registered server dependencies.");
        }

        public void ConnectHost(string username, JeffistanceMessageProcessor messageProcessor)
        {
            username = string.IsNullOrWhiteSpace(username) ? DEFAULT_HOST_NAME : username;
            Host.Name = username;
            Host.Connect(NetworkUtilities.GetLocalIPAddress(), Connection.PORT_NO);
            Host.AttachMessageHandler(new MessageHandler(messageProcessor, Host.Connection));
            Host.GreetServer();
            _logger.LogInformation($"Connected host: {username}");
        }

        public void StartGame(List<User> Users)
        {
            PlayerEventManager playerEventManager = new PlayerEventManager();
            Game = new Game(new BasicGamemode(), playerEventManager);
            Game.Start(Users);
            _inGame = true;
        }

        public void Run(int port)
        {
            Connection = new ServerConnection(port);
            MessageHandler = new MessageHandler(new ServerMessageProcessor(this), Connection);
            ObservableUserList.CollectionChanged += OnUserListChanged;
            Connection.OnDisconnection += OnUserDisconnect;
            Connection.OnMessageReceived += MessageHandler.OnMessageReceived;
            Connection.Run();
            var ip = NetworkUtilities.GetLocalIPAddress();
            _logger.LogInformation($"Started server on {ip}:{port}");
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
            _logger.LogInformation($"Kicked {user.Name}");
        }

        public void AddUser(User user)
        {
            UserConnectionDictionary[user.Connection] = user;
            UserNameDictionary[user.Name.ToLower()] = user;
            UserGuidDictionary[user.ID] = user;
            ObservableUserList.Add(user);
            _logger.LogInformation($"Added new user: {user.Name}");
            _logger.LogDebug($"ID: {user.ID}");
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
            var user = GetUser(args.Client);
            ObservableUserList.Remove(user);
            _logger.LogInformation($"{user.Name} has disconnected.");
        }

        public void Broadcast(Message message)
        {
            MessageHandler.Broadcast(message);
        }

        private void OnUserListChanged(object obj, NotifyCollectionChangedEventArgs args)
        {
            var messageFactory = IoCManager.Resolve<IServerMessageFactory>();

            var updateList = messageFactory.MakeUpdateMessage();
            updateList["UserList"] = UserList;
            MessageHandler.Broadcast(updateList);

            if (!_inGame) Lobby.CheckIfAllReady();
        }
    }
}