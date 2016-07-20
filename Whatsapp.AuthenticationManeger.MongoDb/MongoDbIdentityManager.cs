using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;

namespace Whatsapp.AuthenticationManeger.MongoDb
{
    public class MongoDbIdentityManager : IIdentityManager
    {
        //more info here: http://mongodb.github.io/mongo-csharp-driver/2.2/getting_started/quick_tour/
        private readonly MongoClient _mongoClient;
        private readonly IMongoDatabase _mongoDatabase;
        private readonly IMongoCollection<AxolotlIdentity> _collection;

        public MongoDbIdentityManager(string mongoDbConnectionString)
        {
            _mongoClient = new MongoClient(mongoDbConnectionString);
            _mongoDatabase = _mongoClient.GetDatabase("whatsapp-authentication");
            _collection = _mongoDatabase.GetCollection<AxolotlIdentity>("Identities");
        }

        private class AxolotlIdentity
        {
            public string RecipientId { get; set; }
            public uint RegistrationId { get; set; }
            public byte[] PublicKey { get; set; }
            public byte[] PrivateKey { get; set; }
        }

        private readonly Dictionary<string, AxolotlIdentity> AxolotlIdentities = new Dictionary<string, AxolotlIdentity>();

        public void OnstoreLocalData(uint registrationId, byte[] publickey, byte[] privatekey)
        {
            AxolotlIdentities["-1"]
                = new AxolotlIdentity
                {
                    RegistrationId = registrationId,
                    PrivateKey = privatekey,
                    PublicKey = publickey,
                    RecipientId = "-1"
                };
        }

        public bool OnisTrustedIdentity(string recipientId, byte[] identityKey)
        {
            //return AxolotlIdentities.ContainsKey(recipientId) && 
            //    AxolotlIdentities[recipientId].PublicKey.SequenceEqual(identityKey);
            return true;
        }

        public  uint OngetLocalRegistrationId()
        {
            if (AxolotlIdentities.ContainsKey("-1"))
            {
                return AxolotlIdentities["-1"].RegistrationId;
            }
            return 0;
        }

        public  List<byte[]> OngetIdentityKeyPair()
        {
            List<byte[]> keyPair = new List<byte[]>();

            AxolotlIdentity identity;

            if (AxolotlIdentities.TryGetValue("-1", out identity))

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

        public  bool OnsaveIdentity(string recipientId, byte[] identityKey)
        {
            AxolotlIdentities[recipientId] = new AxolotlIdentity
            {
                PublicKey = identityKey,
                RecipientId = recipientId
            };

            return true;
        }
    }
}