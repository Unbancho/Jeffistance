using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using ModusOperandi.Networking;
using ModusOperandi.Messaging;
using Jeffistance.Services.MessageProcessing;

namespace Jeffistance.Models
{
    [Serializable]
    public class User : ISerializable
    {
        public override bool Equals(object u2){return ID == ((User)u2).ID;}
        public override int GetHashCode(){ return ID.GetHashCode();}

        public bool IsHost = false;
        public const int DEFAULT_PORT = 7700;

        public const string DEFAULT_HOST_USERNAME = "Host";

        public const string DEFAULT_USER_USERNAME = "Guest";

        public int ID;
        public string Name {get; set;}

        public ClientConnection Connection { get; set; }

        public UserMessageProcessor Processor { get; set;}

        public Permissions Perms { get; set; }

        public User(string username)
        {
            if(username == null){
                Name = DEFAULT_USER_USERNAME;
            }
            Name = username;
            Processor = new UserMessageProcessor();
            Perms = new Permissions();
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
            Message greetingMessage = new Message($"{Name} has joined from {Connection.IPAddress}.", JeffistanceFlags.Greeting);
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
            Processor.ProcessMessage(message);
        }
    }

    [Serializable]
    public class Host : User
    {
        public ServerConnection Server {get; set;}

        public new HostMessageProcessor Processor {get; set;}

        public List<User> UserList {get; set;}

        public Host(string username, int port=DEFAULT_PORT, bool dedicated=false):base(username)
        {
            if(username == null){
                Name = DEFAULT_HOST_USERNAME;
            }
            IsHost = true;
            Server = new ServerConnection(port);
            Perms = new Permissions
            {
                CanKick = true
            };
            UserList = new List<User>();
            Server.OnMessageReceived += OnMessageReceived;
            Server.Run();
            Processor = new HostMessageProcessor();
            if(!dedicated)
            {
                Connect(NetworkUtilities.GetLocalIPAddress(), port);
                Processor.ProcessingMethods = (Processor + new UserMessageProcessor()).ProcessingMethods;
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
            UserList.Add(user);
            Message updateList = new Message($"{user.Name} has joined.", JeffistanceFlags.Update);
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

        public new void OnMessageReceived(object sender, MessageReceivedArgs args)
        {
            base.OnMessageReceived(sender, args);
            Processor.ProcessMessage((Message)args.Message);
        }
    }
}
