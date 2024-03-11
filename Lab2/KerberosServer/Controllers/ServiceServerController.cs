using DataEncryptionStandard;
using Domain.Models;
using KerberosServer.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Text;

namespace KerberosServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ServiceServerController : ControllerBase
{
    private readonly KeyManagementService _keyManagementService;
    private readonly SessionService _sessionService;
    public ServiceServerController(KeyManagementService keyManagementService, SessionService sessionService)
    {
        _keyManagementService = keyManagementService;
        _sessionService = sessionService;
    }

    [HttpPost("ss")]
    public IActionResult ServiceResponse([FromBody] ServiceRequest request)
    {
        Log.Information("Recieved a SS request");
        var tgsKey = _keyManagementService.Keys["testserver"];

        var ticket = DES.Decrypt<Ticket>(request.EncryptedTicket, tgsKey);
        Log.Information("Decrypted TGS");

        var cSsKey = ticket.Key;
        Log.Information($"Client SS key: {Encoding.UTF8.GetString(cSsKey)}");
        var aut2 = DES.Decrypt<AuthBlock>(request.EncryptedAuthBlock, cSsKey);
        Log.Information($"Decrypted Aut2");

        if (!ticket.From.Equals(aut2.Login, StringComparison.OrdinalIgnoreCase))
        {
            Log.Warning("Names in block are not equal");
            return BadRequest();
        }
        Log.Information($"TGS.c and Au2.c are equal");

        if (ticket.TimeStamp + ticket.Expiration < aut2.Timestamp)
        {
            Log.Warning("TGT has expired");
            return BadRequest();
        }
        Log.Information("TGT is not expired");

        Log.Information("Added client session key to database");
        _sessionService.Keys.Add(ticket.From, cSsKey);

        var t4 = aut2.Timestamp + 1;
        Log.Information($"Changed t4: t4 + 1, {t4} + 1");

        var serviceResponse = new ServiceResponse()
        {
            Timestamp = t4,
        };

        var encryptedServiceResponse = DES.Encrypt(serviceResponse, cSsKey);
        Log.Information($"Encrypted t4 on client SS key: {Encoding.UTF8.GetString(cSsKey)}");

        return Ok(Convert.ToBase64String(encryptedServiceResponse));
    }
}
