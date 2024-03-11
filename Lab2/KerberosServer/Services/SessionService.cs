namespace KerberosServer.Services;

public class SessionService
{
    public Dictionary<string, byte[]> Keys { get; } = new Dictionary<string, byte[]>();
}
