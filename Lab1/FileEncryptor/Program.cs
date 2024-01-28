using EncryptionLibrary.Abstractions;
using EncryptionLibrary.Realizations;

string alphabet = "abcdefghijklmnopqrstuvwxyz";

IEncryptStrategy cesarCipher = new CesarCipher(alphabet, 3);

BaseEncryptor encryptor = new Encryptor(cesarCipher);

string strForEncryption = "abX,.,/.,;lAKHfuhsdkjjqklwjtoehiubxckjnmlwnie";
string encryptedString = encryptor.Encrypt(strForEncryption);
Console.WriteLine(encryptedString);
string decryptedString = encryptor.Decrypt(encryptedString);
Console.WriteLine(decryptedString);
Console.WriteLine(strForEncryption == decryptedString);