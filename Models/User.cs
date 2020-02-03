using System.Collections.ObjectModel;
using Jeffistance.Services;

namespace Jeffistance.Models
{
    public class User
    {
        public bool IsHost = false;
        public const int DEFAULT_PORT = 7700;

        public string Name { get; set; }

        public ClientConnection Connection;

        public User(string username="Guest")
        {
            Name = username;
        }

        public User(ClientConnection connection, string username)
        {
            Connection = connection;
            Name = username;
        }

        public void Connect(string ip, int port=DEFAULT_PORT)
        {
            Connection = new ClientConnection(ip, port);
            Connection.Send(Name);
        }

        public void Disconnect()
        {
            Connection.Stop();
        }
    }

    public class Host : User
    {
        public ServerConnection Server;
        public ObservableCollection<User> UserList;
        public Host(int port=DEFAULT_PORT, string username="Host", bool dedicated=false):base(username)
        {
            Server = new ServerConnection(port);
            Server.OnConnection += OnConnection;
            UserList = new ObservableCollection<User>();
            Server.Run();
            if(!dedicated)
            {
                Connect(NetworkUtilities.GetLocalIPAddress(), port);
            }
            IsHost = true;
        }

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

        private void OnConnection(object sender, ConnectionArgs args)
        {
            ClientConnection client = args.Client;
            string username = args.Greeting;
            UserList.Add(new User(client, username));
        }
    }
}
