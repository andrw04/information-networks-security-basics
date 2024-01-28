using EncryptionLibrary.Abstractions;

namespace EncryptionLibrary.Realizations;

public class Encryptor(IEncryptStrategy strategy) : IEncryptor
{
    public string Encrypt()
    {
        throw new NotImplementedException();
    }

    public string Decrypt()
    {
        throw new NotImplementedException();
    }

    public void SetStrategy(IEncryptStrategy strategy1)
    {
        strategy = strategy1;
    }
}