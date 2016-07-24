using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;

namespace Whatsapp.AuthenticationManeger.MongoDb
{
    public class MongoDbIdentityManager : IIdentityManager
    {
        //more info here: http://mongodb.github.io/mongo-csharp-driver/2.2/getting_started/quick_tour/
        private readonly IMongoCollection<AxolotlIdentity> _collection;

        public MongoDbIdentityManager(string mongoDbConnectionString)
        {
            var mongoClient = new MongoClient(mongoDbConnectionString);
            var mongoDatabase = mongoClient.GetDatabase("whatsapp-authentication");
            _collection = mongoDatabase.GetCollection<AxolotlIdentity>("Identities");
        }

        private class AxolotlIdentity
        {
            public string RecipientId { get; set; }
            public uint RegistrationId { get; set; }
            public byte[] PublicKey { get; set; }
            public byte[] PrivateKey { get; set; }
        }

        public void OnstoreLocalData(uint registrationId, byte[] publickey, byte[] privatekey)
        {
            var axolotlIdentity = new AxolotlIdentity
            {
                RegistrationId = registrationId,
                PrivateKey = privatekey,
                PublicKey = publickey,
                RecipientId = "-1"
            };

            _collection.InsertOne(axolotlIdentity);
        }

        public bool OnisTrustedIdentity(string recipientId, byte[] identityKey)
        {
            //return AxolotlIdentities.ContainsKey(recipientId) && 
            //    AxolotlIdentities[recipientId].PublicKey.SequenceEqual(identityKey);
            return true;
        }

        public uint OngetLocalRegistrationId()
        {
            var identity = _collection.Find(x => x.RecipientId == "-1")
                .SingleOrDefault();

            if (identity != null)
            {
                return identity.RegistrationId;
            }
            return 0;
        }

        public  List<byte[]> OngetIdentityKeyPair()
        {
            List<byte[]> keyPair = new List<byte[]>();

            AxolotlIdentity identity = _collection.Find(x => x.RecipientId == "-1").SingleOrDefault();

            if (identity != null)
            {
                if (identity.PublicKey != null)
                {
                    keyPair.Add(identity.PublicKey);
                }
                if (identity.PrivateKey != null)
                {
                    keyPair.Add(identity.PrivateKey);
                }
            }

            if (keyPair.Any()) return keyPair;
            return null;
        }

        public bool OnsaveIdentity(string recipientId, byte[] identityKey)
        {
            var identity = new AxolotlIdentity
            {
                PublicKey = identityKey,
                RecipientId = recipientId
            };

            _collection.ReplaceOne(x => x.RecipientId == "-1", identity, new UpdateOptions {IsUpsert = true});

            return true;
        }
    }
}