using System.Collections.Generic;
using System.Threading.Tasks;

namespace Whatsapp.ChatManager
{
    public interface IChatManager
    {
        Task SaveChat(ChatMessage chatMessage);
        Task<IEnumerable<ChatMessage>> GetLastMessages(int count);
        Task<IEnumerable<ChatMessage>> GetAllMessages();
    }
}
