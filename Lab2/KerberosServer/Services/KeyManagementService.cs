using System.Text;

namespace KerberosServer.Services;

public class KeyManagementService
{
    public Dictionary<string, byte[]> Keys { get; } = new Dictionary<string, byte[]>();

    public static string GenerateKey(int length)
    {
        string symbols = "1234567890qwertyuiopasdfghjklzxcvbnm";

        Random random = new Random();

        StringBuilder result = new StringBuilder();
        for (int i = 0; i < length; i++)
        {
            result.Append(symbols[random.Next(symbols.Length)]);
        }

        return result.ToString();
    }
}
