using System.Collections.Generic;

namespace Whatsapp.AuthenticationManeger.InMemory
{
    public class InmemoryAuthenticationConfigurationManager : IAuthenticationConfigurationManager
    {
        readonly InmemoryIdentityManager _inmemoryIdentityManager = new InmemoryIdentityManager();
        readonly InmemoryPreKeysManager _inmemoryPreKeysManager = new InmemoryPreKeysManager();
        readonly InMemorySessionsManager _inMemorySessionsManager = new InMemorySessionsManager();
        readonly InmemorySignedPreKeysManager _inmemorySignedPreKeysManager = new InmemorySignedPreKeysManager();

        //identity
        public void OnstoreLocalData(uint registrationId, byte[] publickey, byte[] privatekey)
        {
            _inmemoryIdentityManager.OnstoreLocalData(registrationId, publickey, privatekey);
        }

        public bool OnisTrustedIdentity(string recipientId, byte[] identityKey)
        {
            return _inmemoryIdentityManager.OnisTrustedIdentity(recipientId, identityKey);
        }

        public uint OngetLocalRegistrationId()
        {
            return _inmemoryIdentityManager.OngetLocalRegistrationId();
        }

        public List<byte[]> OngetIdentityKeyPair()
        {
            return _inmemoryIdentityManager.OngetIdentityKeyPair();
        }

        public bool OnsaveIdentity(string recipientId, byte[] identityKey)
        {
            return _inmemoryIdentityManager.OnsaveIdentity(recipientId, identityKey);
        }


        //prekey
        public void OnstorePreKey(uint prekeyId, byte[] preKeyRecord)
        {
            _inmemoryPreKeysManager.OnstorePreKey(prekeyId, preKeyRecord);
        }

        public void OnremovePreKey(uint preKeyId)
        {
            _inmemoryPreKeysManager.OnremovePreKey(preKeyId);
        }

        public List<byte[]> OnloadPreKeys()
        {
            return _inmemoryPreKeysManager.OnloadPreKeys();
        }

        public byte[] OnloadPreKey(uint preKeyId)
        {
            return _inmemoryPreKeysManager.OnloadPreKey(preKeyId);
        }

        public bool OncontainsPreKey(uint preKeyId)
        {
            return _inmemoryPreKeysManager.OncontainsPreKey(preKeyId);
        }


        //sessions
        public List<uint> OngetSubDeviceSessions(string recipientId)
        {
            return _inMemorySessionsManager.OngetSubDeviceSessions(recipientId);
        }

        public void OndeleteAllSessions(string recipientId)
        {
            _inMemorySessionsManager.OndeleteAllSessions(recipientId);
        }

        public void OnstoreSession(string recipientId, uint deviceId, byte[] sessionRecord)
        {
            _inMemorySessionsManager.OnstoreSession(recipientId, deviceId, sessionRecord);
        }

        public byte[] OnloadSession(string recipientId, uint deviceId)
        {
            return _inMemorySessionsManager.OnloadSession(recipientId, deviceId);
        }

        public void OndeleteSession(string recipientId, uint deviceId)
        {
            _inMemorySessionsManager.OndeleteSession(recipientId, deviceId);
        }

        public bool OncontainsSession(string recipientId, uint deviceId)
        {
            return _inMemorySessionsManager.OncontainsSession(recipientId, deviceId);
        }


        // Signed Prekeys
        public void OnstoreSignedPreKey(uint signedPreKeyId, byte[] signedPreKeyRecord)
        {
            _inmemorySignedPreKeysManager.OnstoreSignedPreKey(signedPreKeyId, signedPreKeyRecord);
        }

        public void OnremoveSignedPreKey(uint preKeyId)
        {
            _inmemorySignedPreKeysManager.OnremoveSignedPreKey(preKeyId);
        }

        public List<byte[]> OnloadSignedPreKeys()
        {
            return _inmemorySignedPreKeysManager.OnloadSignedPreKeys();
        }

        public byte[] OnloadSignedPreKey(uint preKeyId)
        {
            return _inmemorySignedPreKeysManager.OnloadSignedPreKey(preKeyId);
        }

        public bool OncontainsSignedPreKey(uint preKeyId)
        {
            return _inmemorySignedPreKeysManager.OncontainsSignedPreKey(preKeyId);
        }
    }
}