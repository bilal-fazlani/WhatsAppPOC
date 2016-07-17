using System;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Whatsapp.AuthenticationManeger.InMemory;
using WhatsAppApi;
using WhatsAppApi.Helper;

namespace WhatsAppConsole
{
    class Program
    {
        private static string _phoneNumber;
        private static string _password;
        private static string _destinationNumber;

        private static WhatsApp _wa;

        static void Main(string[] args)
        {
            IConfiguration config =
                new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddJsonFile("settings.json", true)
                .Build();

            _phoneNumber = config["PhoneNumber"];
            _password = config["Password"];
            _destinationNumber = config["DestinationNumber"];
            string nickName = config["NickName"];

            _wa = new WhatsApp(_phoneNumber, _password, nickName);

            _wa.SendGetServerProperties();

            _wa.ConfigureInMemoryAuth();

            _wa.OnGetMessage += OnGetMessage;

            _wa.OnConnectSuccess += () => Console.WriteLine("Connected!");

            _wa.OnLoginSuccess += LoginSuccess;

            _wa.Connect();

            Thread.Sleep(1000);

            _wa.Login(GetNextChallege());

            if (_wa.LoadPreKeys() == null)
                _wa.sendSetPreKeys(true);

            Thread t = new Thread(() =>
            {
                while (true)
                {
                    Console.Write("Me: ");
                    string input = Console.ReadLine();
                    if (string.IsNullOrEmpty(input)) continue;

                    _wa.SendMessage(_destinationNumber, input);
                }
            })
            { IsBackground = true };

            t.Start();

            while (true)
            {
                Thread.Sleep(200);
                _wa.PollMessages();
            }
        }

        private static void LoginSuccess(string phoneNumber, byte[] data)
        {
            Console.WriteLine("Logged in!");
            SetNextChallege(data);
        }

        static void SetNextChallege(byte[] bytes)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = Path.Combine(path, $"{_phoneNumber}_next_challenge.txt");
            File.WriteAllBytes(filePath, bytes);
        }

        static byte[] GetNextChallege()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = Path.Combine(path, $"{_phoneNumber}_next_challenge.txt");

            try
            {
                return File.ReadAllBytes(filePath);
            }
            catch
            {
                byte[] challenge = Convert.FromBase64String(_password); //initial default
                SetNextChallege(challenge);
                return File.ReadAllBytes(filePath);
            }
        }

        private static void OnGetMessage(ProtocolTreeNode messageNode, string from, string id, string name, string message, bool receiptSent)
        {
            Console.WriteLine($"{from}({name}): {message}");
        }
    }
}
