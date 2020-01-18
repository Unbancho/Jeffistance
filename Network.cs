using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace Networking
{
    public abstract class ConnectionTcp
    {
        public readonly int PORT_NO;
        public readonly string SERVER_IP;

        public bool IsLocalConnection;

        public CancellationTokenSource CancellationSource;

        public void Stop()
        {
            CancellationSource.Cancel();
        }

        protected ConnectionTcp(string ip, int port)
        {
            PORT_NO = port;
            SERVER_IP = ip;
        }
    }

    public class ServerConnection:ConnectionTcp
    {
        public static implicit operator TcpListener(ServerConnection s) => s.Listener;  // No reason for this, I just thought it was epic
        TcpListener Listener;
        public bool Listening;
        public delegate void ConnectionHandler(object obj, ConnectionArgs args);
        public event ConnectionHandler OnConnection;

        public ClientConnection LatestClient
        {
            set
            {
                ConnectionArgs args = new ConnectionArgs(value);
                OnConnection(this, args);
            }
        }
        List<ClientConnection> Clients = new List<ClientConnection>();

        public ServerConnection(int port):base(NetworkUtilities.GetLocalIPAddress(), port)
        {
            IsLocalConnection = true;
            Listener = new TcpListener(IPAddress.Parse(SERVER_IP), PORT_NO);
        }

        public void Run()
        {
            Listener.Start();
            ListenForConnections();
        }

        private void StopListening()
        {
            Listener.Stop();
            Listening = false;
        }

        public async void ListenForConnections()
        {
            ClientConnection newClient;
            CancellationSource = new CancellationTokenSource();
            Listening = true;
            Console.WriteLine("Listening...");
            using(CancellationSource.Token.Register(StopListening))
            {
                while(Listening)
                {
                    try
                    {
                        newClient = ProcessClient(await AcceptClient(CancellationSource.Token));
                        LatestClient = newClient;
                    }
                    catch(NullReferenceException)
                    {
                        continue;
                    }
                    ListenToClient(newClient);
                }
            }
        }

        private void ListenToClient(ClientConnection client)
        {
            Clients.Add(client);
            Task.Run(() => ListenForMessages(client));
        }

        public async Task<TcpClient> AcceptClient(CancellationToken token)
        {
            try
            {
                TcpClient client = await Listener.AcceptTcpClientAsync(); 
                return client;
            }
            catch(ObjectDisposedException e)
            {
                if(token.IsCancellationRequested)
                {
                    return null;
                }
                throw e;
            }
        }

        private ClientConnection ProcessClient(TcpClient client)
        {
            ClientConnection clientConnection = new ClientConnection(client, SERVER_IP, PORT_NO);
            Console.WriteLine(String.Format("New connection: {0}", client.Client.RemoteEndPoint));
            return clientConnection;
        }
        public void ListenForMessages(ClientConnection client)
        {
            string message = "";
            while (true)
            {
                try
                {
                    message = ReceiveMessage(client);
                }
                catch (Exception e) when (e is System.IO.IOException || e is InvalidOperationException)
                {   
                    message = ((TcpClient) client).Client.RemoteEndPoint + " has disconnected.";
                    Clients.Remove(client);
                    ((TcpClient) client).Close();
                    break;
                }
                finally
                {
                    Broadcast(message);
                }
            }
        }

        public string ReceiveMessage(ClientConnection client)
        {
            string dataReceived = NetworkUtilities.Receive(client).Result;
            if(dataReceived == "")
            {
                throw new InvalidOperationException();
            }
            return dataReceived;
        }

        public void Broadcast(string message)
        {
            foreach (ClientConnection client in this.Clients)    // This might not be thread safe /shrug
            {
                NetworkUtilities.Send(message, client);
            }
        }
    }

    public class ClientConnection:ConnectionTcp
    {
        public static implicit operator TcpClient(ClientConnection c) => c.Client; // No reason for this, I just thought it was epic

        private TcpClient Client;
        public bool Connected;

        public string IPAddress
        {
            get
            {
                return Client.Client.RemoteEndPoint.ToString();
            }
        }

        public ClientConnection(string ip, int port):base(ip, port)
        {
            IsLocalConnection = true;
            Client = new TcpClient(SERVER_IP, PORT_NO);
            Connected = true;
            Task.Run(() => ListenForMessages());
        }

        public ClientConnection(TcpClient client, string serverIP, int port):base(serverIP, port)
        {
            Client = client;
            IsLocalConnection = false;
        }

        private void Close()
        {
            Client.Close();
            Connected = false;
        }

        public void Send(string message)
        {
            NetworkUtilities.Send(message, Client);
        }

        public void ListenForMessages()
        {
            CancellationSource = new CancellationTokenSource();
            using(CancellationSource.Token.Register(Close))
            {
                while(Connected)
                {
                    Console.WriteLine(ReceiveMessage());
                }
            }
        }

        public string ReceiveMessage()
        {
            string message = NetworkUtilities.Receive(Client, CancellationSource.Token).Result;
            if(message == "")
            {
                Stop();
            }
            return message;
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
            byte[] bytesToSend = Encoding.ASCII.GetBytes(message);
            client.GetStream().Write(bytesToSend, 0, bytesToSend.Length);
        }

        public async static Task<string> Receive(TcpClient client, CancellationToken token = new CancellationToken())
        {
            try
            {
                byte[] buffer = new byte[client.Client.ReceiveBufferSize];
                int bytesRead = await client.GetStream().ReadAsync(buffer, 0, buffer.Length);
                string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                return dataReceived;
            }
            catch (Exception e) when (e is System.IO.IOException || e is SocketException || e is System.InvalidOperationException)
            {
                if(token.IsCancellationRequested || e is System.IO.IOException || e is SocketException || e is System.InvalidOperationException)
                {
                    return "";
                }
                throw e;
            }
        }
    }

    public class ConnectionArgs : EventArgs
    {
        private ClientConnection client;

        public ConnectionArgs(ClientConnection newClient)
        {
            client = newClient;
        }

        public ClientConnection Client
        {
            get
            {
                return client;
            }
        }
    }
}
