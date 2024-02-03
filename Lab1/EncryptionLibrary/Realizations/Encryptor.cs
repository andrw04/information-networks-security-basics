using EncryptionLibrary.Abstractions;

namespace EncryptionLibrary.Realizations;

public class Encryptor : BaseEncryptor
{
    public Encryptor() : base(null) { }
    public Encryptor(IEncryptStrategy strategy) : base(strategy) { }
    
    public override string Encrypt(string text)
    {
        if (Strategy is  null)
            throw new InvalidOperationException("");
        return Strategy.Encrypt(text);
    }

    public override string Decrypt(string text)
    {
        if (Strategy is null)
            throw new InvalidOperationException("");
        return Strategy.Decrypt(text);
    }

    public override void EncryptFile(string inputFile, string outputFile)
    {
        if (Strategy is null)
            throw new InvalidOperationException();
        
        using var streamReader = new StreamReader(inputFile);
        using var streamWriter = new StreamWriter(outputFile, true);

        while (!streamReader.EndOfStream)
        {
            var line = streamReader.ReadLine()!;
            var encrypted = Strategy.Encrypt(line);
            streamWriter.WriteLine(encrypted);
        }
    }

    public override void DecryptFile(string inputFile, string outputFile)
    {
        if (Strategy is null)
            throw new InvalidOperationException();
        
        using var streamReader = new StreamReader(inputFile);
        using var streamWriter = new StreamWriter(outputFile, true);

        while (!streamReader.EndOfStream)
        {
            var line = streamReader.ReadLine()!;
            var decrypted = Strategy.Decrypt(line);
            streamWriter.WriteLine(decrypted);
        }
    }
}