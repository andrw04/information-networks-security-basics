using Microsoft.AspNetCore.Mvc;
using KerberosServer.Services;
using Domain.Models;
using Serilog;
using DataEncryptionStandard;
using System.Text;

namespace KerberosServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TicketGrantingServerController : ControllerBase
{
    private readonly KeyManagementService _keyManagementService;

    public TicketGrantingServerController(KeyManagementService keyManagementService)
    {
        _keyManagementService = keyManagementService;
    }

    [HttpPost("tgs")]
    public IActionResult GrantTicket([FromBody] TgsRequest request)
    {
        Log.Information($"Recieved a TGS request");
        var tgsKey = _keyManagementService.Keys["tgs"];
        Log.Information($"TGS key: {Encoding.UTF8.GetString(tgsKey)}");

        if (!_keyManagementService.Keys.TryGetValue(request.Recipient, out var recepientKey))
        {
            Log.Error("Request recepient was not registered yet");
            return NotFound();
        }

        var tgt = DES.Decrypt<Ticket>(request.EncryptedTicket, tgsKey);
        Log.Information($"Decrypted TGT");

        var cTgsKey = tgt.Key;

        var aut1 = DES.Decrypt<AuthBlock>(request.EncryptedAuthBlock, cTgsKey);
        Log.Information($"Decrypted Aut1");

        if (!tgt.From.Equals(aut1.Login, StringComparison.OrdinalIgnoreCase))
        {
            Log.Warning("Names in block are not equal");
            return BadRequest();
        }
        Log.Information($"TGT.c and Aut1.c are equal");

        if (tgt.TimeStamp + tgt.Expiration < aut1.Timestamp)
        {
            Log.Warning("TGT has expired");
            return BadRequest();
        }
        Log.Information($"TGT is not expired");

        var tgs = new Ticket()
        {
            From = aut1.Login,
            To = request.Recipient,
            Key = Encoding.UTF8.GetBytes(KeyManagementService.GenerateKey(8)),
            TimeStamp = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds(),
            Expiration = 60
        };
        Log.Information($"Generated TGS ticket: c='{tgs.From}', ss='{tgs.To}'," +
            $" t3='{tgs.TimeStamp}', p2='{tgs.TimeStamp}', SS key='{Encoding.UTF8.GetString(tgs.Key)}'");

        var tgsResponse = new TgsResponse()
        {
            EncryptedTicket = DES.Encrypt(tgs, recepientKey),
            Key = tgs.Key
        };
        Log.Information($"Encrypted TGS on TGS_SS key: {Encoding.UTF8.GetString(recepientKey)}");

        var encryptedTgsResponse = DES.Encrypt(tgsResponse, cTgsKey);
        Log.Information($"Encrypted TGS response on client TGS key: {Encoding.UTF8.GetString(cTgsKey)}");

        return Ok(Convert.ToBase64String(encryptedTgsResponse));
    }
}
