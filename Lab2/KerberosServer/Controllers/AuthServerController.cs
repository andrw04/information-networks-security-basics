using DataEncryptionStandard;
using Domain.Models;
using KerberosServer.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Text;

namespace KerberosServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthServerController : ControllerBase
{
    private readonly KeyManagementService _keyManagementService;

    public AuthServerController(KeyManagementService keyManagementService)
    {
        _keyManagementService = keyManagementService;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] string login)
    {
        var key = KeyManagementService.GenerateKey(8);
        var binaryKey = Encoding.UTF8.GetBytes(key);

        if (_keyManagementService.Keys.TryAdd(login, binaryKey))
        {
            Log.Information($"Registered user: '{login}' with key: '{key}'");
            return Ok(key);
        }

        return BadRequest();
    } 

    [HttpPost("authenticate")]
    public IActionResult Authenticate([FromBody] string login)
    {
        Log.Information($"Recieved an authentication request from user: '{login}'");

        if (!_keyManagementService.Keys.ContainsKey(login))
        {
            Log.Information($"User: '{login}' is not found");
            return NotFound();
        }

        var cTgsKey = KeyManagementService.GenerateKey(8);

        var tgt = new Ticket()
        {
            From = login,
            To = "tgs",
            Key = Encoding.UTF8.GetBytes(cTgsKey),
            TimeStamp = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds(),
            Expiration = (long)TimeSpan.FromHours(1).TotalSeconds
        };

        Log.Information($"Generated TGT: from='{login}', To='tgs', Key='{cTgsKey}', t1={tgt.TimeStamp}, p1={tgt.Expiration}");


        var encryptedTgt = DES.Encrypt(tgt, _keyManagementService.Keys["tgs"]);
        Log.Information($"Enctypted TGT on key: '{Encoding.UTF8.GetString(_keyManagementService.Keys["tgs"])}'");

        var authResponse = new AuthResponse()
        {
            EncryptedTicket = encryptedTgt,
            Key = Encoding.UTF8.GetBytes(cTgsKey)
        };
        Log.Information($"Generated auth response");

        var encryptedAuthResponse = DES.Encrypt(authResponse, _keyManagementService.Keys[login]);
        Log.Information($"Encrypted auth response" +
            $" on key: '{Convert.ToBase64String(_keyManagementService.Keys[login])}'");

        return Ok(Convert.ToBase64String(encryptedAuthResponse));
    }
}
