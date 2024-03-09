using DataEncryptionStandard;
using System.Text;

var key = Encoding.UTF8.GetBytes("12345678");
var value = Encoding.UTF8.GetBytes("12345678123451124542131");

var encrypted = DES.Encrypt(value, key);
Console.WriteLine(Encoding.UTF8.GetString(encrypted));
var decrypted = DES.Decrypt(encrypted, key);
Console.WriteLine(Encoding.UTF8.GetString(decrypted));
