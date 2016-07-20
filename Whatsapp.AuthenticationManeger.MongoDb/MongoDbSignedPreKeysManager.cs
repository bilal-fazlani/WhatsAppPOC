using System.Collections.Generic;
using System.Linq;

namespace Whatsapp.AuthenticationManeger.MongoDb
{
    public class MongoDbSignedPreKeysManager : ISignedPreKeyManager
    {
        private class AxolotlSignedPrekey
        {
            public uint PrekeyId { get; set; }
            public byte[] Record { get; set; }
        }

        private  readonly Dictionary<uint, AxolotlSignedPrekey> AxolotlSignedPrekeys = new Dictionary<uint, AxolotlSignedPrekey>();

        public  void OnstoreSignedPreKey(uint signedPreKeyId, byte[] signedPreKeyRecord)
        {
            AxolotlSignedPrekeys[signedPreKeyId] = new AxolotlSignedPrekey
            {
                Record = signedPreKeyRecord,
                PrekeyId = signedPreKeyId
            };
        }

        public  void OnremoveSignedPreKey(uint preKeyId)
        {
            AxolotlSignedPrekeys.Remove(preKeyId);
        }

        public  List<byte[]> OnloadSignedPreKeys()
        {
            var keys = AxolotlSignedPrekeys.Values.Select(x => x.Record).ToList();
            return keys.Any() ? keys : null;
        }

        public  byte[] OnloadSignedPreKey(uint preKeyId)
        {
            if (AxolotlSignedPrekeys.ContainsKey(preKeyId))
                return AxolotlSignedPrekeys[preKeyId].Record;
            return new byte[] { };
        }

        public  bool OncontainsSignedPreKey(uint preKeyId)
        {
            return AxolotlSignedPrekeys.ContainsKey(preKeyId);
        }
    }
}