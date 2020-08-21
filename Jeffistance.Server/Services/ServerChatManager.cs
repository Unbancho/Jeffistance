using Jeffistance.Common.Services.IoC;
using Jeffistance.Common.Services;
using Jeffistance.Common.Models;
using Jeffistance.JeffServer.Models;

namespace Jeffistance.JeffServer.Services
{
    public interface IServerChatManager
    {
        Server Server { get; set; }

        void Notify(string text);
    }

    public class ServerChatManager : IServerChatManager
    {
        public Server Server { get; set; }

        private IServerMessageFactory _messageFactory;
        private IServerMessageFactory MessageFactory
        {
            get
            {
                return _messageFactory ??= IoCManager.Resolve<IServerMessageFactory>();
            }
        }

        public void Notify(string text)
        {
            var message = MessageFactory.MakeChatMessage(text);
            Server.Broadcast(message);
        }
    }
}
