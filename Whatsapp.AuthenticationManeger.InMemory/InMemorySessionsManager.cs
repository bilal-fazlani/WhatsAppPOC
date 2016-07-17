using System.Collections.Generic;

namespace Whatsapp.AuthenticationManeger.InMemory
{
    public  class InMemorySessionsManager : ISessionManager
    {
        private class AxolotlSession
        {
            public string RecipientId { get; set; }
            public uint DeviceId { get; set; }
            public byte[] Record { get; set; }
        }

        private  readonly Dictionary<string, AxolotlSession> AxolotlSessions = new Dictionary<string, AxolotlSession>();

        public  List<uint> OngetSubDeviceSessions(string recipientId)
        {
            return new List<uint>()
            {
                AxolotlSessions[recipientId].DeviceId
            };
        }

        public  void OndeleteAllSessions(string recipientId)
        {
            AxolotlSessions.Clear();
        }

        public  void OnstoreSession(string recipientId, uint deviceId, byte[] sessionRecord)
        {
            AxolotlSessions[recipientId] = new AxolotlSession
            {
                Record = sessionRecord,
                DeviceId = deviceId,
                RecipientId = recipientId
            };
        }

        public  byte[] OnloadSession(string recipientId, uint deviceId)
        {
            if (AxolotlSessions.ContainsKey(recipientId))
            {
                return AxolotlSessions[recipientId].Record;
            }
            return new byte[] { };
        }

        public  void OndeleteSession(string recipientId, uint deviceId)
        {
            AxolotlSessions.Remove(recipientId);
        }

        public  bool OncontainsSession(string recipientId, uint deviceId)
        {
            return AxolotlSessions.ContainsKey(recipientId);
        }
    }
}