namespace EncryptionLibrary.Abstractions;

public abstract class BaseEncryptor(IEncryptStrategy? strategy)
{
    protected IEncryptStrategy? Strategy = strategy;
    public abstract string Encrypt(string text);
    public abstract string Decrypt(string text);
    public abstract void EncryptFile(string inputFile, string outputFile);
    public abstract void DecryptFile(string inputFile, string outputFile);
    public void SetStrategy(IEncryptStrategy strategy)
    {
        Strategy = strategy;
    }
}