using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using ModusOperandi.Networking;
using ModusOperandi.Messaging;
using Jeffistance.Services.MessageProcessing;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace Jeffistance.Models
{
    [Serializable]
    public class User : ISerializable
    {
        public override bool Equals(object u2){return ID == ((User)u2).ID;}
        public override int GetHashCode(){ return ID.GetHashCode();}

        public event PropertyChangedEventHandler PropertyChanged;

        public int ID;
        public string Name {get; set;}
        public bool IsHost {get; set;}

        public Permissions Perms { get; set; }

        private ClientConnection _connection;
        public ClientConnection Connection { get{ return _connection;} set{ _connection = value; /*OnPropertyChanged();*/} }

        public User(){}

        public User(LocalUser localUser)
        {
            Name = localUser.Name;
            IsHost = localUser.IsHost;
        }

        protected User(SerializationInfo info, StreamingContext context)
        {
            Name = info.GetString("Name");
            IsHost = info.GetBoolean("IsHost");
            ID = info.GetInt32("id");
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            info.AddValue("IsHost", IsHost);
            info.AddValue("id", ID);
        }
    }

    public class LocalUser : User
    {
        public LocalUser(string username)
        {
            Name = username;
            Perms = new Permissions();
        }

        public void Connect(string ip, int port)
        {
            MessageHandler messageHandler = AppState.GetAppState().MessageHandler;
            Connection = new ClientConnection(ip, port);
            Connection.OnMessageReceived += messageHandler.OnMessageReceived;
            Message greetingMessage = new Message($"{Name} has joined from {Connection.IPAddress}.", JeffistanceFlags.Greeting);
            greetingMessage["User"] = new User(this);
            messageHandler.Send(greetingMessage);
        }

        public void Disconnect()
        {
            Connection.Stop();
        }
    }

    public class Server
    {
        public LocalUser Host {get; set;}
        public ServerConnection Connection {get; set;}
        public List<User> UserList {get; set;}

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

        public void ConnectHost(string username)
        {
            username = string.IsNullOrWhiteSpace(username) ? "Admin" : username;
            Host.Name = username;
            Host.Connect(NetworkUtilities.GetLocalIPAddress(), Connection.PORT_NO);
        }

        public void Run(int port)
        {
            Connection = new ServerConnection(port);
            Connection.Run();
            Connection.OnMessageReceived += AppState.GetAppState().MessageHandler.OnMessageReceived;
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
            user.ID =  UserList.Count;
            UserList.Add(user);
            Message updateList = new Message($"{user.Name} has joined.", JeffistanceFlags.Update);
            updateList["UserList"] = UserList;
            AppState.GetAppState().MessageHandler.Broadcast(updateList);
        }
    }
}
