using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Whatsapp.AuthenticationManeger.InMemory;
using WhatsAppApi;
using WhatsAppApi.Helper;

namespace WhatsAppConsole
{
    public class Program
    {
        private static string _phoneNumber;
        private static string _password;
        private static string _destinationNumber;

        private static WhatsApp _wa;

        private static bool _connected = false;
        private static bool _poll = false;

        public static void Main(string[] args)
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

            _wa = new WhatsApp(_phoneNumber, _password, nickName, debug: true);

            try
            {
                Connect();
            }
            catch (Exception e)
            {
                WriteError(e);
            }

#if DEBUG
            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
#endif
        }

        private static void StartSendingMessages()
        {
            while (true)
            {
                try
                {
                    string input = GetInputfromUser("text message");
                    if(string.IsNullOrWhiteSpace(input)) continue;
                    if (input == "quit();") break;

                    string sendResposne = _wa.SendMessage(_destinationNumber, input);
                    Console.WriteLine($"SendResponse: {sendResposne}");
                }
                catch (Exception ex)
                {
                    WriteError(ex);
                }
            }
        }

        private static void Connect()
        {
            var challenge = GetNextChallege();

            _wa.OnConnectFailed += WriteError;
            _wa.OnConnectSuccess += () =>
            {
                Console.WriteLine("Successfully connected!");
                try
                {
                    _wa.Login(challenge);

                    _wa.SendGetPrivacyList();
                    _wa.SendGetClientConfig();

                    var prekeys = _wa.LoadPreKeys();
                    if (prekeys == null || !prekeys.Any())
                        _wa.sendSetPreKeys(true);
                }
                catch (Exception e)
                {
                    WriteError(e);
                }
            };

            _wa.OnDisconnect += WriteError;
            _wa.OnError += (id, from, code, text) => Console.WriteLine("ERROR: " +
                                                                      $"Id: {id}," +
                                                                      $"From: {from}," +
                                                                      $"Code: {code}," +
                                                                      $"Text: {text}");
            _wa.OnGetMessage += OnGetMessage;

            _wa.OnGetSyncResult += (int index, string sid, Dictionary<string, string> users, string[] numbers) =>
            {
                Console.WriteLine($"Synced. index: {index}");
            };

            _wa.OnGetTyping += from =>
            {
                Console.WriteLine($"{from} is typing...");
            };

            _wa.OnGetPhoto += OnGetPhoto;

            _wa.OnLoginFailed += WriteError;

            _wa.OnLoginSuccess += (number, data) =>
            {
                Console.WriteLine($"Logged in successfully! {number}");
                SetNextChallege(data);

                _connected = true;

                StartPolling();
                StartSendingMessages();
            };

            _wa.OnErrorAxolotl += WriteError;

            _wa.OnGetMessageReceivedServer += (from, id) => Console.WriteLine($"Message received by server. {from}, {id}");
            _wa.OnGetMessageReceivedClient += (from, id) => Console.WriteLine($"Message received by user. {from}, {id}");
            _wa.OnGetMessageReadedClient += (from, id) => Console.WriteLine($"Message read by client. {from}, {id}");

            DebugAdapter.Instance.OnPrintDebug += Instance_OnPrintDebug;

            _wa.ConfigureInMemoryAuth();

            _wa.Connect();

            _wa.SendGetServerProperties();
        }

        
        private static void Instance_OnPrintDebug(object value)
        {
            Console.WriteLine("\nDebug:" + value+"\n");
        }

        static void StartPolling()
        {
            _poll = true;

            Thread t = new Thread(() =>
            {
                while (_poll && _connected)
                {
                    Thread.Sleep(200);
                    try
                    {
                        _wa.PollMessages();
                    }
                    catch (Exception ex)
                    {
                        WriteError(ex);
                    }
                }
            })
            {
                IsBackground = true
            };

            t.Start();
        }

        static void SetNextChallege(byte[] bytes)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = Path.Combine(path, "next_challenge.txt");
            File.WriteAllBytes(filePath, bytes);
        }

        static byte[] GetNextChallege()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = Path.Combine(path, "next_challenge.txt");

            try
            {
                return File.ReadAllBytes(filePath);
            }
            catch (Exception e)
            {
                WriteError(e);
                byte[] challenge = Convert.FromBase64String(_password); //initial default
                SetNextChallege(challenge);
                return File.ReadAllBytes(filePath);
            }
        }

        private static string GetInputfromUser(string inputName)
        {
            Console.Write($"Enter {inputName}: ");
            return Console.ReadLine();
        }

        public static void WriteError(Exception exception)
        {
            Console.WriteLine("ERROR: " + exception?.GetBaseException()?.Message);
        }

        public static void WriteError(string error)
        {
            Console.WriteLine("ERROR: " + error);
        }

        public static void OnGetPhoto(string @from, string id, byte[] data)
        {
            Console.WriteLine($"New image received from {from}");
        }

        public static void OnGetMessage(ProtocolTreeNode messageNode, string @from, string id, string name, string message, bool receiptSent)
        {
            Console.WriteLine($"New message arrived from {from}. Message: {message}");
        }
    }
}
