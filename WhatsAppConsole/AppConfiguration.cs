using Microsoft.Extensions.Configuration;

namespace WhatsAppConsole
{
    public class AppConfiguration
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string DestinationNumber { get; set; }
        public string NickName { get; set; }
        public string MongoConnectionString { get; set; }

        public static AppConfiguration GetInstance(string[] args)
        {
            IConfiguration config =
                new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddJsonFile("settings.json", true)
                .AddCommandLine(args)
                .Build();

            AppConfiguration instance = new AppConfiguration
            {
                PhoneNumber = config["PhoneNumber"],
                Password = config["Password"],
                DestinationNumber = config["DestinationNumber"],
                NickName = config["NickName"],
                MongoConnectionString = config["MongoConnectionString"]
            };

            return instance;
        }

        private AppConfiguration() { }
    }
}