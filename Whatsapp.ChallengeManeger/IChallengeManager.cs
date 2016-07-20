namespace Whatsapp.ChallengeManeger
{
    public interface IChallengeManager
    {
        void SetNextChallege(byte[] bytes);
        byte[] GetNextChallege(string password);
    }
}
