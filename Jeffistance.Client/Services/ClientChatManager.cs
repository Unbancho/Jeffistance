using Jeffistance.Common.Services.IoC;
using Jeffistance.Common.Services;
using Jeffistance.Common.Models;
using Jeffistance.Client.Models;

namespace Jeffistance.Client.Services
{
    public interface IClientChatManager
    {
        void Send(string text);
    }

    public class ClientChatManager : IClientChatManager
    {
        private IClientMessageFactory _messageFactory;
        private IClientMessageFactory MessageFactory
        {
            get
            {
                return _messageFactory ??= IoCManager.Resolve<IClientMessageFactory>();
            }
        }

        private LocalUser _currentUser;
        private LocalUser CurrentUser
        {
            get
            {
                return _currentUser ??= AppState.GetAppState().CurrentUser;
            }
        }

        public void Send(string text)
        {
            var message = MessageFactory.MakeChatMessage(text, CurrentUser.ID);
            CurrentUser.Send(message);
        }
    }
}
