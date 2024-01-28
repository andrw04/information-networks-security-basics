using EncryptionLibrary.Abstractions;
using EncryptionLibrary.Realizations;

string alphabet = "abcdefghijklmnopqrstuvwxyz";

IEncryptStrategy cesarCipher = new CesarCipher(alphabet, 3);

BaseEncryptor encryptor = new Encryptor(cesarCipher);

Console.WriteLine(encryptor.Encrypt("abz"));