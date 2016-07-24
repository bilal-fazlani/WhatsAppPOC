using System.Collections.Generic;
using System.Threading.Tasks;

namespace Whatsapp.ChatManager
{
    public interface IChatManager<T> where T:ChatMessage
    {
        Task SaveMessageAsync(T chatMessage);
        Task <IEnumerable<T>> GetLastMessagesAsync(int count);
        Task ChangeMessageStatus(string localMessageId, MessageStatus status);
    }
}
