using System.Collections.Generic;
using Networking;

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

    public User(ClientConnection connection)
    {
        Connection = connection;
    }

    public void Connect(string ip, int port=DEFAULT_PORT)
    {
        Connection = new ClientConnection(ip, port);
    }

    public void Disconnect()
    {
        Connection.Stop();
    }
}

public class Host : User
{
    public ServerConnection Server;
    public List<User> UserList;

    public Host(string username="Host", bool dedicated=false):base(username)
    {
        Server = new ServerConnection(DEFAULT_PORT);
        Server.OnConnection += OnConnection;
        UserList = new List<User>();
        Server.Run();
        if(!dedicated)
        {
            Connect(Networking.NetworkUtilities.GetLocalIPAddress());
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
        UserList.Add(new User(client));
    }
}