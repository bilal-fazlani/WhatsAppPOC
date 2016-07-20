using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whatsapp.ChallengeManeger;
using WhatsAppApi;

namespace Whatsapp.AuthenticationManeger.MongoDb
{
    public static class Extension
    {
        public static void ConfigureMongoDbAuth(
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
