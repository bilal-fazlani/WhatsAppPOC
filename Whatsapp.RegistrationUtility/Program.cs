using System;
using System.Linq;
using System.Windows.Forms;
using WhatsAppApi.Register;

namespace Whatsapp.RegistrationUtility
{
    
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                string phoneNumber;

                if (!args.Any())
                {
                    phoneNumber = GetPhoneNumberFromUser();
                }
                else
                {
                    phoneNumber = GetPhoneNumberFromArgs(args);
                }

                Process(phoneNumber);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

#if DEBUG
            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
#endif
        }

        private static string GetPhoneNumberFromArgs(string[] args)
        {
            string phoneNumber = args[0];

            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                throw new Exception("Empty phone number");
            }

            //todo:rege verify phone number here.

            return phoneNumber;
        }

        static void Process(string phoneNumber)
        {
            SendTextMessage(phoneNumber);

            string code = GetCodeFromUser();

            string password = RegisterCode(code, phoneNumber);

            Clipboard.SetText(password);
            Console.WriteLine($"Password (copied to clipboard): {password}");
        }

        private static string GetPhoneNumberFromUser()
        {
            GetPhoneNumber:
            Console.Write("Please enter phone number with country code: ");
            string phoneNumber = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                Console.WriteLine("Phone number can not be empty.");
                goto GetPhoneNumber;
            }

            //todo:rege verify phone number here.

            return phoneNumber;
        }

        private static string RegisterCode(string code, string phoneNumber)
        {
            string response;
            string password = WhatsRegisterV2.RegisterCode(phoneNumber, code, out response);

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new Exception("There was an error while registering verification code");
            }
            return password;
        }

        private static string GetCodeFromUser()
        {
            GetCode:
            Console.Write("Please enter the verification code: ");
            string code = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(code))
            {
                Console.WriteLine("Code can not be empty.");
                goto GetCode;
            }

            //todo: add regex validation for code here

            return code.Replace("-", "");
        }

        private static void SendTextMessage(string phoneNumber)
        {
            string password;
            string request;
            string response;

            bool success = WhatsRegisterV2.RequestCode(
                phoneNumber, out password,
                out request, out response);

            if (success)
            {
                Console.WriteLine("You will now receive a text message with a code...");
            }
            else
            {
                throw new Exception("There was an error sending request for verification code sms");
            }
        }
    }
}
