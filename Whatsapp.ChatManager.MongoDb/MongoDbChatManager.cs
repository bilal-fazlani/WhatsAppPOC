using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Whatsapp.ChatManager.MongoDb
{
    public class MongoDbChatManager :IChatManager<MongoChatMessage>
    {
        private readonly IMongoCollection<MongoChatMessage> _collection;

        public MongoDbChatManager(string mongoDbConnectionString)
        {
            var mongoClient = new MongoClient(mongoDbConnectionString);
            var mongoDatabase = mongoClient.GetDatabase("whatsapp-authentication");
            _collection = mongoDatabase.GetCollection<MongoChatMessage>("Chats");
        }

        public async Task SaveMessageAsync(MongoChatMessage chatMessage)
        {
            try
            {
                var filter = new FilterDefinitionBuilder<MongoChatMessage>().Eq(x => x._id, chatMessage._id);
                await _collection.ReplaceOneAsync(filter, chatMessage, new UpdateOptions {IsUpsert = true});
                //await _collection.InsertOneAsync(chatMessage);
            }
            catch (Exception e)
            {
                Console.WriteLine($"AddMessageAsync: {chatMessage._id}: "+ e?.GetBaseException()?.Message ?? e.Message);
            }
        }

        public async Task<IEnumerable<MongoChatMessage>> GetLastMessagesAsync(int count)
        {
            var filter = new BsonDocument();
            var sort = new SortDefinitionBuilder<MongoChatMessage>().Descending(x => x.TimeStamp);
            return await _collection
                .Find(filter)
                .Sort(sort)
                .Limit(count)
                .ToListAsync();
        }

        public async Task ChangeMessageStatus(string localMessageId, MessageStatus status)
        {
            try
            {
                var filter = Builders<MongoChatMessage>.Filter.Eq(x => x.LocalMessageId, 
                    localMessageId);

                var update = Builders<MongoChatMessage>.Update
                    .Set(x => x.MessageStatus, status)
                    .Set(x => x.TimeStamp, DateTime.Now);

                await _collection.UpdateOneAsync(filter, update);
            }
            catch (Exception e)
            {
                Console.WriteLine($"ChangeMessageStatus: {localMessageId}, {status}: " + e?.GetBaseException()?.Message ?? e.Message);
            }
        }
    }
}
