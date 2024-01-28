namespace EncryptionLibrary.Abstractions;

public interface IEncryptStrategy
{
    public string Encrypt(string text);
    public string Decrypt(string text);
}