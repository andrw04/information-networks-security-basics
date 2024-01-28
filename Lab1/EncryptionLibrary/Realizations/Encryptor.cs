using EncryptionLibrary.Abstractions;

namespace EncryptionLibrary.Realizations;

public class Encryptor : BaseEncryptor
{
    public Encryptor(IEncryptStrategy strategy) : base(strategy) { }
    
    public override string Encrypt(string text)
    {
        return Strategy.Encrypt(text);
    }

    public override string Decrypt(string text)
    {
        return Strategy.Decrypt(text);
    }
}