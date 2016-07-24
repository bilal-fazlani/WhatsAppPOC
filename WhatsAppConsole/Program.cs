using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MongoDB.Bson;
using Whatsapp.AuthenticationManeger.InMemory;
using Whatsapp.ChatManager;
using Whatsapp.ChatManager.MongoDb;
using WhatsAppApi;

namespace WhatsAppConsole
{
    class Program
    {
        private static WhatsApp _wa;
        private static bool _loggedIn;
        private static AppConfiguration _configuration;

        static void Main(string[] args)
        {
            _configuration = AppConfiguration.GetInstance(args);
            PrintLast10Messages();

            _wa = new WhatsApp(_configuration.PhoneNumber, _configuration.Password, _configuration.NickName);

            _wa.ConfigureInMemoryAuth(_configuration.PhoneNumber, _configuration.Password);
            _wa.ConfigureChatStorageWithMongoDb(_configuration.MongoConnectionString, _configuration.PhoneNumber);

            _wa.OnGetMessage += (node, from, id, name, message, sent) => Console.WriteLine($"[{DateTime.Now}] {from}: {message}");

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

            StartSending(_configuration.DestinationNumber);
            StartPolling();
        }

        private static void PrintLast10Messages()
        {
            MongoDbChatManager chatManager = new MongoDbChatManager(_configuration.MongoConnectionString);

            IEnumerable<MongoChatMessage> messages = chatManager.GetLastMessagesAsync(10).Result.Reverse();

            foreach (var message in messages)
            {
                Console.WriteLine(message.IsMine 
                    ? $"[{message.TimeStamp}] Me: {message.Message} ({message.MessageStatus})" 
                    : $"[{message.TimeStamp}] {message.From}: {message.Message}");
            }
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
                MongoDbChatManager chatManager = new MongoDbChatManager(_configuration.MongoConnectionString);

                while (_loggedIn)
                {
                    Console.Write("Me: ");
                    string input = Console.ReadLine();
                    if (string.IsNullOrEmpty(input)) continue;

                    string id = _wa.SendMessage(destinationNumber, input);

                    chatManager.SaveMessageAsync(new MongoChatMessage
                    {
                        Message = input,
                        MessageStatus = MessageStatus.Sending,
                        To = _configuration.DestinationNumber,
                        TimeStamp = DateTime.Now,
                        _id = ObjectId.GenerateNewId(),
                        IsMine = true,
                        From = _configuration.PhoneNumber,
                        LocalMessageId = id
                    }).Wait();
                }
            })
            { IsBackground = true };

            t.Start();
        }
    }
}
