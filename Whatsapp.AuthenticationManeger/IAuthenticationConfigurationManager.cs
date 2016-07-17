using System.Collections.Generic;

namespace Whatsapp.AuthenticationManeger
{
    public interface IAuthenticationConfigurationManager 
        : IIdentityManager, IPreKeyManager, ISessionManager, ISignedPreKeyManager
    {
        
    }

    public interface IIdentityManager
    {
        //identity
        void OnstoreLocalData(uint registrationId, byte[] publickey, byte[] privatekey);
        bool OnisTrustedIdentity(string recipientId, byte[] identityKey);
        uint OngetLocalRegistrationId();
        List<byte[]> OngetIdentityKeyPair();
        bool OnsaveIdentity(string recipientId, byte[] identityKey);
    }

    public interface IPreKeyManager
    {
        //prekeys
        void OnstorePreKey(uint prekeyId, byte[] preKeyRecord);
        void OnremovePreKey(uint preKeyId);
        List<byte[]> OnloadPreKeys();
        byte[] OnloadPreKey(uint preKeyId);
        bool OncontainsPreKey(uint preKeyId);
    }

    public interface ISessionManager
    {
        //sessions
        List<uint> OngetSubDeviceSessions(string recipientId);
        void OndeleteAllSessions(string recipientId);
        void OnstoreSession(string recipientId, uint deviceId, byte[] sessionRecord);
        byte[] OnloadSession(string recipientId, uint deviceId);
        void OndeleteSession(string recipientId, uint deviceId);
        bool OncontainsSession(string recipientId, uint deviceId);
    }

    public interface ISignedPreKeyManager
    {
        //signed pre keys
        void OnstoreSignedPreKey(uint signedPreKeyId, byte[] signedPreKeyRecord);
        void OnremoveSignedPreKey(uint preKeyId);
        List<byte[]> OnloadSignedPreKeys();
        byte[] OnloadSignedPreKey(uint preKeyId);
        bool OncontainsSignedPreKey(uint preKeyId);
    }
}