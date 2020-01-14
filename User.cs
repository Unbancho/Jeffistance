public class User
{
    public string Name { get; set; }

    public ConnectionTCP Connection;

    public bool IsCurrentUser;

    public User(ConnectionTCP connection)
    {
        Connection = connection;
        IsCurrentUser = Connection.IsLocalConnection;
    }
}