using System.Collections.Generic;

namespace Whatsapp.AuthenticationManeger.MongoDb
{
    public class MongoDbAdapter : IAuthenticationConfigurationManager
    {
        public void OnstoreLocalData(uint registrationId, byte[] publickey, byte[] privatekey)
        {
            throw new System.NotImplementedException();
        }

        public bool OnisTrustedIdentity(string recipientId, byte[] identityKey)
        {
            throw new System.NotImplementedException();
        }

        public uint OngetLocalRegistrationId()
        {
            throw new System.NotImplementedException();
        }

        public List<byte[]> OngetIdentityKeyPair()
        {
            throw new System.NotImplementedException();
        }

        public bool OnsaveIdentity(string recipientId, byte[] identityKey)
        {
            throw new System.NotImplementedException();
        }

        public void OnstorePreKey(uint prekeyId, byte[] preKeyRecord)
        {
            throw new System.NotImplementedException();
        }

        public void OnremovePreKey(uint preKeyId)
        {
            throw new System.NotImplementedException();
        }

        public List<byte[]> OnloadPreKeys()
        {
            throw new System.NotImplementedException();
        }

        public byte[] OnloadPreKey(uint preKeyId)
        {
            throw new System.NotImplementedException();
        }

        public bool OncontainsPreKey(uint preKeyId)
        {
            throw new System.NotImplementedException();
        }

        public List<uint> OngetSubDeviceSessions(string recipientId)
        {
            throw new System.NotImplementedException();
        }

        public void OndeleteAllSessions(string recipientId)
        {
            throw new System.NotImplementedException();
        }

        public void OnstoreSession(string recipientId, uint deviceId, byte[] sessionRecord)
        {
            throw new System.NotImplementedException();
        }

        public byte[] OnloadSession(string recipientId, uint deviceId)
        {
            throw new System.NotImplementedException();
        }

        public void OndeleteSession(string recipientId, uint deviceId)
        {
            throw new System.NotImplementedException();
        }

        public bool OncontainsSession(string recipientId, uint deviceId)
        {
            throw new System.NotImplementedException();
        }

        public void OnstoreSignedPreKey(uint signedPreKeyId, byte[] signedPreKeyRecord)
        {
            throw new System.NotImplementedException();
        }

        public void OnremoveSignedPreKey(uint preKeyId)
        {
            throw new System.NotImplementedException();
        }

        public List<byte[]> OnloadSignedPreKeys()
        {
            throw new System.NotImplementedException();
        }

        public byte[] OnloadSignedPreKey(uint preKeyId)
        {
            throw new System.NotImplementedException();
        }

        public bool OncontainsSignedPreKey(uint preKeyId)
        {
            throw new System.NotImplementedException();
        }
    }
}