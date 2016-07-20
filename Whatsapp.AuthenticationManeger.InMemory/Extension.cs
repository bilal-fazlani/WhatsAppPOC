using Whatsapp.ChallengeManeger;
using WhatsAppApi;

namespace Whatsapp.AuthenticationManeger.InMemory
{
    public static class Extension
    {
        public static void ConfigureInMemoryAuth(
            this WhatsApp wa,
            IChallengeManager challengeManager,
            string password)
        {
            new AuthenticationConfigurator(wa)
                .ConfigureAuthentication(new InmemoryAuthenticationConfigurationManager());

            wa.OnConnectSuccess += () =>
            {
                wa.Login(challengeManager.GetNextChallege(password));
            };

            wa.OnLoginSuccess += (number, data) =>
            {
                if (wa.LoadPreKeys() == null)
                    wa.sendSetPreKeys(true);

                challengeManager.SetNextChallege(data);
            };
        }
    }
}