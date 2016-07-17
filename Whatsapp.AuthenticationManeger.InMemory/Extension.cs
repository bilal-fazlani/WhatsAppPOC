using WhatsAppApi;

namespace Whatsapp.AuthenticationManeger.InMemory
{
    public static class Extension
    {
        public static void ConfigureInMemoryAuth(this WhatsApp wa)
        {
            new AuthenticationConfigurator(wa)
                .ConfigureAuthentication(new InmemoryAuthenticationConfigurationManager());
        }
    }
}