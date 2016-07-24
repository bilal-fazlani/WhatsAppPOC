using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using WhatsAppApi;
using WhatsAppApi.Helper;

namespace Whatsapp.ChatManager.MongoDb
{
    public static class Extension
    {
        private static MongoDbChatManager _chatManager;
        private static string _phoneNumber;

        public static void ConfigureChatStorageWithMongoDb(
            this WhatsApp wa,
            string mongoDbConnectionString,
            string phoneNumber)
        {
            var conventionPack = new ConventionPack
            {
                new EnumRepresentationConvention(BsonType.String),
            };

            ConventionRegistry
                .Register("EnumStringConvention",
                conventionPack, 
                t=> true);

            _phoneNumber = phoneNumber;
            _chatManager = new MongoDbChatManager(mongoDbConnectionString);
            wa.OnGetMessage += OnGetMessage;
            wa.OnGetMessageReceivedServer += OnMessageReceivedByServer;
            wa.OnGetMessageReceivedClient += OnMessageReceivedByClient;
            wa.OnGetMessageReadedClient += OnGetMessageReadByClient;
        }

        private static async void OnGetMessageReadByClient(string from, string id)
        {
            await _chatManager.ChangeMessageStatus(id, MessageStatus.Read);
        }

        private static async void OnMessageReceivedByClient(string from, string id)
        {
            await _chatManager.ChangeMessageStatus(id, MessageStatus.Delivered);
        }

        private static async void OnMessageReceivedByServer(string from, string id)
        {
            await _chatManager.ChangeMessageStatus(id, MessageStatus.Sent);
        }

        private static async void OnGetMessage(ProtocolTreeNode messageNode, string from, string id, string name, string message, bool receiptSent)
        {
            await _chatManager.SaveMessageAsync(new MongoChatMessage
            {
                _id = new ObjectId(id),
                Message = message,
                TimeStamp = DateTime.Now,
                From = from,
                To = _phoneNumber,
                MessageStatus = MessageStatus.Unknown,
                IsMine = false
            });
        }
    }
}