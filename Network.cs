using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

public abstract class ConnectionTCP
{
    public readonly int PORT_NO;
    public readonly string SERVER_IP;
    public bool IsLocalConnection;

    protected ConnectionTCP(string ip, int port=7700)
    {
        PORT_NO = port;
        SERVER_IP = ip;
    }
}

public class ServerConnection:ConnectionTCP
{
    public static implicit operator TcpListener(ServerConnection s) => s.Listener;

    TcpListener Listener;
    List<User> UserList = new List<User>();

    public ServerConnection():base(NetworkUtilities.GetLocalIPAddress())
    {
        IsLocalConnection = true;
        Listener = new TcpListener(IPAddress.Parse(SERVER_IP), PORT_NO);
        ListenForConnections();
    }

    public void ListenForConnections()
    {
        User newUser;
        ClientConnection newClientConnection;
        Listener.Start();
        Console.WriteLine("Listening...");
        Task.Factory.StartNew(async () =>
        {
            while (true)
            {
                newClientConnection = new ClientConnection(await Listener.AcceptTcpClientAsync(), SERVER_IP);
                newUser = new User(newClientConnection);
                UserList.Add(newUser);
                Console.WriteLine(String.Format("New connection: {0}", newClientConnection.Client.Client.RemoteEndPoint));
                Thread t = new Thread(() => ListenForMessages(newUser));
                t.IsBackground = true;
                t.Start();
            }
        });
    }
    public void ListenForMessages(User user)
    {
        string message;
        bool stopListening = false;
        while (true)
        {
            try
            {
                message = ReceiveMessage(user);
            }
            catch (Exception e) when (e is System.IO.IOException || e is InvalidOperationException)
            {UserList.Remove(user);
                stopListening = true;
                message = user.Name + " has disconnected.";
                Console.WriteLine(message);
            }
            Broadcast(message);
            if (stopListening)
                break;
        }
    }

    public string ReceiveMessage(User user)
    {
        string dataReceived = ": ";
        dataReceived += NetworkUtilities.Receive((ClientConnection) user.Connection);
        dataReceived = user.Name+dataReceived;
        return dataReceived;
    }

    public void Broadcast(string message)
    {
        foreach (User user in this.UserList)
        {
            NetworkUtilities.Send(message, (ClientConnection) user.Connection);
        }
    }
}

public class ClientConnection:ConnectionTCP
{
    public static implicit operator TcpClient(ClientConnection c) => c.Client;

    public TcpClient Client;

    public ClientConnection(string ip):base(ip)
    {
        IsLocalConnection = true;
        Client = new TcpClient(SERVER_IP, PORT_NO);
        Thread listenThread = new Thread(ListenForMessages);
        listenThread.Start();
    }

    public ClientConnection(TcpClient client, string serverIP):base(serverIP)
    {
        Client = client;
        IsLocalConnection = false;
    }

    public void Send(string message)
    {
        NetworkUtilities.Send(message, Client);
    }

    public void ListenForMessages()
    {
        while(true)
        {
            Console.WriteLine(ReceiveMessage());
        }
    }

    public string ReceiveMessage()
    {
        return NetworkUtilities.Receive(Client);
    }
}


public static class NetworkUtilities
{
    public static string GetLocalIPAddress()
    {
        string strHostName = Dns.GetHostName();
        IPHostEntry ipHostEntry = Dns.GetHostEntry(strHostName);
        IPAddress[] address = ipHostEntry.AddressList;
        return address[address.Length-1].ToString();
    }

    public static void Send(string message, TcpClient client)
    {
        NetworkStream nwStream = client.GetStream();
        byte[] bytesToSend = Encoding.ASCII.GetBytes(message);
        nwStream.Write(bytesToSend, 0, bytesToSend.Length);
    }

    public static string Receive(TcpClient client)
    {
        NetworkStream nwStream = client.GetStream();
        byte[] buffer = new byte[client.ReceiveBufferSize];
        int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);
        string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
        return dataReceived;
    }
}