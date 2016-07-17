using WhatsAppApi;

namespace Whatsapp.AuthenticationManeger
{
    public class AuthenticationConfigurator
    {
        private readonly WhatsApp _wa;

        public AuthenticationConfigurator(WhatsApp wa)
        {
            _wa = wa;
        }

        public void ConfigureAuthentication(IAuthenticationConfigurationManager adapter)
        {
            ConfigureIdentity(adapter);
            ConfigurePreKeys(adapter);
            ConfigureSessions(adapter);
            ConfigureSignedPreKeys(adapter);
        }

        private void ConfigureSignedPreKeys(IAuthenticationConfigurationManager adapter)
        {
            //SIGNED PREKEYS
            _wa.OncontainsSignedPreKey += adapter.OncontainsSignedPreKey;
            _wa.OnloadSignedPreKey += adapter.OnloadSignedPreKey;
            _wa.OnloadSignedPreKeys += adapter.OnloadSignedPreKeys;
            _wa.OnremoveSignedPreKey += adapter.OnremoveSignedPreKey;
            _wa.OnstoreSignedPreKey += adapter.OnstoreSignedPreKey;
        }

        private void ConfigureSessions(IAuthenticationConfigurationManager adapter)
        {
            //SESSIONS
            _wa.OncontainsSession += adapter.OncontainsSession;
            _wa.OndeleteSession += adapter.OndeleteSession;
            _wa.OnloadSession += adapter.OnloadSession;
            _wa.OnstoreSession += adapter.OnstoreSession;
            _wa.OndeleteAllSessions += adapter.OndeleteAllSessions;
            _wa.OngetSubDeviceSessions += adapter.OngetSubDeviceSessions;
        }

        private void ConfigurePreKeys(IAuthenticationConfigurationManager adapter)
        {
            //PRE KEYS
            _wa.OncontainsPreKey += adapter.OncontainsPreKey;
            _wa.OnloadPreKey += adapter.OnloadPreKey;
            _wa.OnloadPreKeys += adapter.OnloadPreKeys;
            _wa.OnremovePreKey += adapter.OnremovePreKey;
            _wa.OnstorePreKey += adapter.OnstorePreKey;
        }

        private void ConfigureIdentity(IAuthenticationConfigurationManager adapter)
        {
            //IDENTIY
            _wa.OnsaveIdentity += adapter.OnsaveIdentity;
            _wa.OngetIdentityKeyPair += adapter.OngetIdentityKeyPair;
            _wa.OngetLocalRegistrationId += adapter.OngetLocalRegistrationId;
            _wa.OnisTrustedIdentity += adapter.OnisTrustedIdentity;
            _wa.OnstoreLocalData += adapter.OnstoreLocalData;
            _wa.OngetLocalRegistrationId += adapter.OngetLocalRegistrationId;
        }
    }
}