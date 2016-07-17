using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Whatsapp.AuthenticationManeger.InMemory;
using WhatsAppApi;
using WhatsAppApi.Helper;
using WhatsAppConsole.Properties;

namespace WhatsAppConsole
{
    public class Program
    {
        private static string PhoneNumber;
        private static string Password;
        private static string DestinationNumber;

        static WhatsApp wa = new WhatsApp(PhoneNumber, Password, "Bilal", debug: true);

        private static bool Connected = false;
        private static bool Poll = false;

        public static void Main(string[] args)
        {
            IConfiguration config = 
                new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddJsonFile("settings.json", true)
                .Build();

            PhoneNumber = config["PhoneNumber"];
            Password = config["Password"];
            DestinationNumber = config["DestinationNumber"];

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

                    string sendResposne = wa.SendMessage(DestinationNumber, input);
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

            wa.OnConnectFailed += WriteError;
            wa.OnConnectSuccess += () =>
            {
                Console.WriteLine("Successfully connected!");
                try
                {
                    wa.Login(challenge);

                    wa.SendGetPrivacyList();
                    wa.SendGetClientConfig();

                    var prekeys = wa.LoadPreKeys();
                    if (prekeys == null || !prekeys.Any())
                        wa.sendSetPreKeys(true);
                }
                catch (Exception e)
                {
                    WriteError(e);
                }
            };

            wa.OnDisconnect += WriteError;
            wa.OnError += (id, from, code, text) => Console.WriteLine("ERROR: " +
                                                                      $"Id: {id}," +
                                                                      $"From: {from}," +
                                                                      $"Code: {code}," +
                                                                      $"Text: {text}");
            wa.OnGetMessage += OnGetMessage;

            wa.OnGetSyncResult += (int index, string sid, Dictionary<string, string> users, string[] numbers) =>
            {
                Console.WriteLine($"Synced. index: {index}");
            };

            wa.OnGetTyping += from =>
            {
                Console.WriteLine($"{from} is typing...");
            };

            wa.OnGetPhoto += OnGetPhoto;

            wa.OnLoginFailed += WriteError;

            wa.OnLoginSuccess += (number, data) =>
            {
                Console.WriteLine($"Logged in successfully! {number}");
                SetNextChallege(data);

                Connected = true;

                StartPolling();
                StartSendingMessages();
            };

            wa.OnErrorAxolotl += WriteError;

            wa.OnGetMessageReceivedServer += (from, id) => Console.WriteLine($"Message received by server. {from}, {id}");
            wa.OnGetMessageReceivedClient += (from, id) => Console.WriteLine($"Message received by user. {from}, {id}");
            wa.OnGetMessageReadedClient += (from, id) => Console.WriteLine($"Message read by client. {from}, {id}");

            DebugAdapter.Instance.OnPrintDebug += Instance_OnPrintDebug;

            wa.ConfigureInMemoryAuth();

            wa.Connect();

            wa.SendGetServerProperties();
        }

        
        private static void Instance_OnPrintDebug(object value)
        {
            Console.WriteLine("\nDebug:" + value+"\n");
        }

        static void StartPolling()
        {
            Poll = true;

            Thread t = new Thread(() =>
            {
                while (Poll && Connected)
                {
                    Thread.Sleep(200);
                    try
                    {
                        wa.PollMessages();
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
                byte[] challenge = Convert.FromBase64String(Password); //initial default
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
