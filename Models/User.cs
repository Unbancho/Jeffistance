using System.Collections.ObjectModel;
using Jeffistance.Services;
using System;
using System.Runtime.Serialization;
using Avalonia.Threading;

using Jeffistance.Services.Messaging;
using Jeffistance.Services.MessageProcessing;
using System.Linq;

namespace Jeffistance.Models
{
    [Serializable]
    public class User : ISerializable
    {
        public override bool Equals(object u2){return ID == ((User)u2).ID;}
        public override int GetHashCode(){ return ID.GetHashCode();}
        private ObservableCollection<User> _userList;
        public ObservableCollection<User> UserList
        {
            get{ return _userList;}
            set{foreach (var item in value.Except(UserList)){ Dispatcher.UIThread.Post(()=> UserList.Add(item));}}
        }

        public bool IsHost = false;
        public const int DEFAULT_PORT = 7700;

        public int ID;
        public string Name {get; set;}

        public ClientConnection Connection;

        public MessageProcessor MessageProcessor;

        public User(string username="Guest")
        {
            Name = username;
            _userList = new ObservableCollection<User>();
            MessageProcessor = new MessageProcessor();
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

        public void Connect(string ip, int port=DEFAULT_PORT)
        {
            Connection = new ClientConnection(ip, port);
            Connection.OnMessageReceived += OnMessageReceived;
            Message greetingMessage = new Message(String.Format("{0} has joined from {1}.", Name, Connection.IPAddress), JeffistanceFlags.Greeting, JeffistanceFlags.Broadcast);
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
            Send(new Message(message, JeffistanceFlags.Broadcast));
        }

        public void OnMessageReceived(object sender, MessageReceivedArgs args)
        {
            Message message = (Message) args.Message;
            message.Sender = args.Sender;
            MessageProcessor.ProcessMessage(message);
        }
    }

    [Serializable]
    public class Host : User
    {
        public ServerConnection Server;

        public Host(int port=DEFAULT_PORT, string username="Host", bool dedicated=false):base(username)
        {
            IsHost = true;
            Server = new ServerConnection(port);
            Server.OnMessageReceived += OnMessageReceived;
            Server.OnConnection += OnConnection;
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

        public void AddUser(User user)
        {
            user.ID =  UserList.Count;
            Dispatcher.UIThread.Post(()=> UserList.Add(user));
            Message updateList = new Message("Update your lists, scrubs", (MessageFlags) JeffistanceFlags.Update);
            updateList["UserList"] = UserList;
            Broadcast(updateList);
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
