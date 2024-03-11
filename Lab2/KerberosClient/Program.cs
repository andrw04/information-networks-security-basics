using DataEncryptionStandard;
using Domain.Models;
using KerberosClient;
using Serilog;
using System.Text;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var c = "testclient";
var serverName = "testserver";

Client client = new Client();
var regResponse = await client.Register(c);

if (regResponse == null)
{
    Log.Warning("Client didn't recieve key");
    Console.ReadKey();
    return;
}

var cKey = Encoding.UTF8.GetBytes(regResponse!);


// 1
Log.Information($"Client sent request to AS");
var encryptedTgtAndcTgsKey = await client.Authenticate(c);

if (encryptedTgtAndcTgsKey == null)
{
    Log.Warning($"Client didn't recieve TGT and TGS key from AS");
    Console.ReadKey();
    return;
}

Log.Information($"Client recieved TGT and TGS key from AS");
var tgtAndcTgsKey = DES.Decrypt<AuthResponse>(encryptedTgtAndcTgsKey!, cKey);

var tgsKey = tgtAndcTgsKey.Key;
Log.Information($"Client TGS key: '{Encoding.UTF8.GetString(tgsKey)}'");

// 3
var aut1 = new AuthBlock { Login = c, Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds() };
Log.Information($"Client generated Aut1: c='{c}', t2='{aut1.Timestamp}'");
var encryptedAut1 = DES.Encrypt(aut1, tgsKey);
Log.Information($"Aut1 ecnrypted on TGS key: '{Encoding.UTF8.GetString(tgsKey)}'");
var tgsRequest = new TgsRequest()
{
    EncryptedAuthBlock = encryptedAut1,
    EncryptedTicket = tgtAndcTgsKey.EncryptedTicket,
    Recipient = serverName,
};
Log.Information($"Client generated request to TGS, ID='{serverName}'");

// 4
Log.Information("Client sent request to TGS");
var encryptedTgsAndCSsKey = await client.RequestTgs(tgsRequest);

if (encryptedTgsAndCSsKey == null)
{
    Log.Warning($"Client didn't recieve TGS with SS key");
    Console.ReadKey();
    return;
}

Log.Information($"Client recieved TGS with SS key");
var tgsAndCSsKey = DES.Decrypt<AuthResponse>(encryptedTgsAndCSsKey!, tgsKey);

var cSsKey = tgsAndCSsKey.Key;
Log.Information($"Client SS key: '{Encoding.UTF8.GetString(cSsKey)}'");

// 5
var aut2 = new AuthBlock { Login = c, Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds() };
Log.Information($"Client generated Aut2: c='{c}', t4='{aut2.Timestamp}'");
var encryptedAut2 = DES.Encrypt(aut2, cSsKey);
Log.Information($"Aut2 encrypted on SS key: '{Encoding.UTF8.GetString(cSsKey)}'");

var serviceRequest = new ServiceRequest()
{
    EncryptedTicket = tgsAndCSsKey.EncryptedTicket,
    EncryptedAuthBlock = encryptedAut2
};

// 6
Log.Information($"Client sent request to SS");
var serverResponse = await client.RequestService(serviceRequest);

if (serverResponse == null)
{
    Log.Warning($"Client didn't recieve t4");
    Console.ReadKey();
    return;
}

Log.Information("Client recieved t4 from SS");
var t4 = DES.Decrypt<ServiceResponse>(serverResponse!, cSsKey);
Log.Information($"Client decrypted t4");

if (t4.Timestamp - 1 != aut2.Timestamp)
{
    Log.Error($"Service didn't pass verification. Timestamp is not equal");
    Console.ReadKey();
    return;
}

Log.Information($"t4 - 1 = Aut2.t4, '{t4.Timestamp} - 1' = '{aut2.Timestamp}'");

Log.Information("Success");

Console.ReadKey();
