using System;
using System.Runtime.Serialization;
using ModusOperandi.Networking;
using ModusOperandi.Messaging;
using Jeffistance.Common.Services.MessageProcessing;
using Jeffistance.Common.Services;
using Jeffistance.Common.Services.IoC;
using System.ComponentModel;

namespace Jeffistance.Common.Models
{
    [Serializable]
    public class User : ISerializable
    {
        public override bool Equals(object u2){return ID == ((User)u2).ID;}
        public override int GetHashCode(){ return ID.GetHashCode();}

        public Guid ID;
        public string Name {get; set;}
        public bool IsHost {get; set;}

        public Permissions Perms { get; set; }

        public ClientConnection Connection { get; set;}

        public User(){}

        public User(LocalUser localUser)
        {
            Name = localUser.Name;
            IsHost = localUser.IsHost;
            ID = localUser.ID;
        }

        protected User(SerializationInfo info, StreamingContext context)
        {
            Name = info.GetString("Name");
            IsHost = info.GetBoolean("IsHost");
            ID = Guid.Parse(info.GetString("id"));
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            info.AddValue("IsHost", IsHost);
            info.AddValue("id", ID.ToString());
        }
    }

    public class LocalUser : User
    {
        MessageHandler MessageHandler {get; set;}

        public LocalUser(string username)
        {
            Name = username;
            Perms = new Permissions();
            ID = Guid.NewGuid();
        }

        public void Connect(string ip, int port)
        {
            Connection = new ClientConnection(ip, port);
        }

        public void AttachMessageHandler(MessageHandler messageHandler)
        {
            MessageHandler = messageHandler;
            Connection.OnMessageReceived += MessageHandler.OnMessageReceived;
        }

        public void Send(Message message)
        {
            MessageHandler.Send(message);
        }

        public void Send(string message)
        {
            MessageHandler.Send(message);
        }

        public void GreetServer()
        {
            var messageFactory = IoCManager.Resolve<IClientMessageFactory>();

            var greetingMessage = messageFactory.MakeGreetingMessage(new User(this));
            Send(greetingMessage);

            var greetingChatMessage = messageFactory.MakeChatMessage(
                $"{Name} has joined from {Connection.IPAddress}.");
            Send(greetingChatMessage);
        }

        public void Disconnect()
        {
            Connection.Stop();
        }
    }
}
