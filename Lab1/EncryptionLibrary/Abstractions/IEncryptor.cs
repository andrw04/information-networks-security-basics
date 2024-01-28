namespace EncryptionLibrary.Abstractions;

public interface IEncryptor
{
    public string Encrypt();
    public string Decrypt();
    public void SetStrategy(IEncryptStrategy strategy);
}