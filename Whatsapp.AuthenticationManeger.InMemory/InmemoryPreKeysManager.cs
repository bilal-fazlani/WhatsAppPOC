using System;
using System.Collections.Generic;
using System.Linq;

namespace Whatsapp.AuthenticationManeger.InMemory
{
    public class InmemoryPreKeysManager : IPreKeyManager
    {
        private class AxolotlPrekey
        {
            public uint PrekeyId { get; set; }
            public byte[] Record { get; set; }

        }

        private  readonly Dictionary<uint, AxolotlPrekey> AxolotlPrekeys = new Dictionary<uint, AxolotlPrekey>();

        public  void OnstorePreKey(uint prekeyId, byte[] preKeyRecord)
        {
            AxolotlPrekeys[prekeyId] = new AxolotlPrekey
            {
                PrekeyId = prekeyId,
                Record = preKeyRecord
            };
        }

        public  void OnremovePreKey(uint preKeyId)
        {
            if (AxolotlPrekeys.ContainsKey(preKeyId))
                AxolotlPrekeys.Remove(preKeyId);
        }

        public  List<byte[]> OnloadPreKeys()
        {
            var keys = AxolotlPrekeys.Select(x => x.Value.Record).ToList();
            return keys.Any() ? keys : null;
        }

        public  byte[] OnloadPreKey(uint preKeyId)
        {
            if (AxolotlPrekeys.ContainsKey(preKeyId))
                return AxolotlPrekeys[preKeyId].Record;
            return new byte[] { };
        }

        public  bool OncontainsPreKey(uint preKeyId)
        {
            return AxolotlPrekeys.ContainsKey(preKeyId);
        }
    }
}