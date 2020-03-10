using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Jeffistance.Services;
using Jeffistance.Services.Messaging;
using Jeffistance.Services.MessageProcessing;
using Jeffistance.ViewModels;

namespace Jeffistance.Models
{
    [Serializable]
    public class User : ISerializable, INotifyPropertyChanged
    {
        public override bool Equals(object u2){return ID == ((User)u2).ID;}
        public override int GetHashCode(){ return ID.GetHashCode();}

        public event PropertyChangedEventHandler PropertyChanged;

        private List<User> _userList;
        public List<User> UserList
        {
            get{ return _userList;}
            set{ _userList = value; OnPropertyChanged();}
        }

        public bool IsHost = false;
        public const int DEFAULT_PORT = 7700;

        public const string DEFAULT_HOST_USERNAME = "Host";

        public const string DEFAULT_USER_USERNAME = "Guest";

        public int ID;
        public string Name {get; set;}

        public ClientConnection Connection { get; set; }

        public UserMessageProcessor Processor { get; set;}

        public Permissions Perms { get; set; }

        public ViewModelBase CurrentWindow {get; set; }

        public User(string username)
        {
            if(username == null){
                Name = DEFAULT_USER_USERNAME;
            }
            Name = username;
            UserList = new List<User>();
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

        private void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    [Serializable]
    public class Host : User
    {
        public ServerConnection Server;

        public new HostMessageProcessor Processor;

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
            Server.OnMessageReceived += OnMessageReceived;
            Server.OnConnection += OnConnection;
            Server.Run();
            Processor = new HostMessageProcessor();
            if(!dedicated)
            {
                Connect(NetworkUtilities.GetLocalIPAddress(), port);
                Processor.ProcessingMethods = Processor + new UserMessageProcessor();
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
            List<User> tempList = new List<User>(UserList);
            tempList.Add(user);
            Message updateList = new Message($"{user.Name} has joined.", JeffistanceFlags.Update);
            updateList["UserList"] = tempList;
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

        public void OnConnection(object sender, ConnectionArgs args)
        {

        }
    }
}
