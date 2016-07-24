using MongoDB.Bson;

namespace Whatsapp.ChatManager.MongoDb
{
    public class MongoChatMessage : ChatMessage
    {
        // ReSharper disable once InconsistentNaming - MongoDb primary key
        public ObjectId _id { get; set; }
    }
}