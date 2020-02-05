using System.Collections.ObjectModel;
using Jeffistance.Services;
using System;
using System.Runtime.Serialization;
using Avalonia.Threading;

namespace Jeffistance.Models
{
    [Serializable]
    public class User : ISerializable
    {
        public bool IsHost = false;
        public const int DEFAULT_PORT = 7700;

        public string Name {get; set;}

        public ClientConnection Connection;

        public User(string username="Guest")
        {
            Name = username;
        }

        protected User(SerializationInfo info, StreamingContext context)
        {
            Name = info.GetString("Name");
            IsHost = info.GetBoolean("IsHost");
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            info.AddValue("IsHost", IsHost);
        }

        public void Connect(string ip, int port=DEFAULT_PORT)
        {
            Connection = new ClientConnection(ip, port);
            Connection.Send(this);
        }

        public void Disconnect()
        {
            Connection.Stop();
        }
    }

    [Serializable]
    public class Host : User
    {
        public ServerConnection Server;
        public ObservableCollection<User> UserList;
        public Host(int port=DEFAULT_PORT, string username="Host", bool dedicated=false):base(username)
        {
            IsHost = true;
            Server = new ServerConnection(port);
            Server.OnMessageReceived += OnMessageReceived;
            Server.OnConnection += OnConnection;
            UserList = new ObservableCollection<User>();
            Server.Run();
            if(!dedicated)
            {
                Connect(NetworkUtilities.GetLocalIPAddress(), port);
            }
        }

        protected Host(SerializationInfo info, StreamingContext context) : base(info, context){}

        public new void Disconnect()
        {
            base.Disconnect();
            Server.Stop();
        }

        public void Kick(User user)
        {
            UserList.Remove(user);
            Server.Kick(user.Connection);
        }

        private ClientConnection client;
        private ClientConnection ConnectingClient
        {
            set
            {
                if(value == null || ConnectingClient == null)
                {
                    client = value;
                }
            }
            get
            {
                return client;
            }
        }
        public void OnMessageReceived(object sender, MessageReceivedArgs args)
        {
            object receivedObject = args.Message;
            if(receivedObject is User user)
            {   
                user.Connection = ConnectingClient;
                ConnectingClient = null;
                Dispatcher.UIThread.Post(()=> UserList.Add(user));
                Server.Broadcast(String.Format("{0} has joined from {1}.", user.Name, user.Connection.IPAddress));
            }
            else if(receivedObject is string str)
            {
                Server.Broadcast(str);
            }
        }

        public void OnConnection(object sender, ConnectionArgs args)
        {
            ClientConnection client = args.Client;
            ConnectingClient = client;
        }
    }
}
