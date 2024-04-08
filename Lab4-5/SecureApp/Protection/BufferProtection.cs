namespace SecureApp.Protection;

public static class BufferProtection
{
    public static int MaxLength = 100;
    public static bool ValidateInput(string text)
    {
        return text.Length <= MaxLength;
    }
}