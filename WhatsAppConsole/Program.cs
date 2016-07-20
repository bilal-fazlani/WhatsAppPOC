using System;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Whatsapp.AuthenticationManeger.InMemory;
using Whatsapp.ChallengeManeger.LocalDisk;
using WhatsAppApi;
using WhatsAppApi.Helper;
using Whatsapp.ChallengeManeger;

namespace WhatsAppConsole
{
    class Program
    {
        private static WhatsApp _wa;
        private static bool _loggedIn;

        static void Main(string[] args)
        {
            IConfiguration config =
                new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddJsonFile("settings.json", true)
                .Build();

            string phoneNumber = config["PhoneNumber"];
            string password = config["Password"];
            string destinationNumber = config["DestinationNumber"];
            string nickName = config["NickName"];
            string mongoConnectionString = config["MongoConnectionString"];

            _wa = new WhatsApp(phoneNumber, password, nickName);

            IChallengeManager challengeManager = new LocalDiskChallengeManager(phoneNumber);
            _wa.ConfigureInMemoryAuth(challengeManager, password);

            _wa.SendGetServerProperties();

            _wa.OnGetMessage += OnGetMessage;

            _wa.OnConnectSuccess += () => Console.WriteLine("Connected!");

            _wa.OnLoginSuccess += (number, data) =>
            {
                _loggedIn = true;
                Console.WriteLine("Logged in!");
            };

            _wa.OnConnectFailed += exception => Console.WriteLine("ERROR: " + exception?.GetBaseException()?.Message);

            _wa.OnLoginFailed += data => Console.WriteLine("ERROR: " + data);

            _wa.OnError += (id, from, code, text) => Console.WriteLine($"ERROR: {id}, {from}, {code}, {text}");

            _wa.Connect();

            Thread.Sleep(1000);

            StartSending(destinationNumber);
            StartPolling();
        }

        private static void StartPolling()
        {
            while (_loggedIn)
            {
                Thread.Sleep(500);
                _wa.PollMessages();
            }
        }

        private static void StartSending(string destinationNumber)
        {
            Thread t = new Thread(() =>
            {
                while (_loggedIn)
                {
                    Console.Write("Me: ");
                    string input = Console.ReadLine();
                    if (string.IsNullOrEmpty(input)) continue;

                    _wa.SendMessage(destinationNumber, input);
                }
            })
            { IsBackground = true };

            t.Start();
        }

        private static void OnGetMessage(ProtocolTreeNode messageNode, string from, string id, string name, string message, bool receiptSent)
        {
            Console.WriteLine($"{from}({name}): {message}");
        }
    }
}
