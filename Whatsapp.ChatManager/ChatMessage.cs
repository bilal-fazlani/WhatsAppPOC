using System;

namespace Whatsapp.ChatManager
{
    public class ChatMessage
    {
        public string From { get; set; }

        public string To { get; set; }

        public string Message { get; set; }

        public DateTime TimeStamp { get; set; }

        public MessageStatus MessageStatus { get; set; }

        public bool IsMine { get; set; }

        public string LocalMessageId { get; set; }
    }

    public enum MessageStatus
    {
        Unknown = 0,
        Sending = 1,
        Sent = 2,
        Delivered = 3,
        Read = 4
    }
}