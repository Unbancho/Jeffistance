using System.Collections.ObjectModel;
using Jeffistance.Services;
using System;
using System.Runtime.Serialization;
using Avalonia.Threading;

using Jeffistance.Services.Messaging;

namespace Jeffistance.Models
{
    [Flags]
    enum CustomFlagsHere{}   // Have to be powers of 2 for bitwise operations! Cast them to MessageFlags when passing to a Message.

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
            Message greetingMessage = new Message(String.Format("{0} has joined from {1}.", Name, Connection.IPAddress), MessageFlags.Greeting | MessageFlags.Broadcast);
            greetingMessage["User"] = this;
            Send(greetingMessage);
        }

        public void Disconnect()
        {
            Connection.Stop();
        }

        public void Send(Message message)
        {
            Connection.Send(message);
        }

        public void Send(string message)
        {
            Connection.Send(new Message(message, MessageFlags.Broadcast));
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

        private void AddUser(User user, ClientConnection connection)
        {
            user.Connection = connection;
            Dispatcher.UIThread.Post(()=> UserList.Add(user));
        }
        public void OnMessageReceived(object sender, MessageReceivedArgs args)
        {
            Message message = (Message) args.Message;
            if(message.HasFlag(MessageFlags.Greeting))
            {
                User user = (User)message["User"];
                ClientConnection connection = (ClientConnection) args.Sender;
                AddUser(user, connection);
            }
            if(message.HasFlag(MessageFlags.Broadcast))
            {
                Server.Broadcast(message.Text);
            }
        }

        public void Broadcast(Message message)
        {
            Server.Broadcast(message);
        }

        public void Broadcast(string message)
        {
            Server.Broadcast(new Message(message));
        }

        public void OnConnection(object sender, ConnectionArgs args)
        {

        }
    }
}
