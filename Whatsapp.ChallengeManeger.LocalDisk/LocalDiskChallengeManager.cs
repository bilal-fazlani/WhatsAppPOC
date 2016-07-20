using System;
using System.IO;

namespace Whatsapp.ChallengeManeger.LocalDisk
{
    public class LocalDiskChallengeManager: IChallengeManager
    {
        private readonly string _phoneNumber;

        public LocalDiskChallengeManager(string phoneNumber)
        {
            _phoneNumber = phoneNumber;
        }

        public void SetNextChallege(byte[] bytes)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = Path.Combine(path, $"{_phoneNumber}_next_challenge.txt");
            File.WriteAllBytes(filePath, bytes);
        }

        public byte[] GetNextChallege(string password)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = Path.Combine(path, $"{_phoneNumber}_next_challenge.txt");

            try
            {
                return File.ReadAllBytes(filePath);
            }
            catch
            {
                byte[] challenge = Convert.FromBase64String(password); //initial default
                SetNextChallege(challenge);
                return File.ReadAllBytes(filePath);
            }
        }
    }
}