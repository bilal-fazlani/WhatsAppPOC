using Whatsapp.ChallengeManeger;
using Whatsapp.ChallengeManeger.LocalDisk;
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
            wa.SendGetServerProperties();

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

        /// <summary>
        /// This will use a LocalDiskChallengeManager.
        /// </summary>
        /// <see cref="LocalDiskChallengeManager"/>
        /// <param name="wa"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="password"></param>
        public static void ConfigureInMemoryAuth(
            this WhatsApp wa,
            string phoneNumber,
            string password)
        {
            IChallengeManager challengeManager = new LocalDiskChallengeManager(phoneNumber);
            wa.ConfigureInMemoryAuth(challengeManager, password);
        }
    }
}