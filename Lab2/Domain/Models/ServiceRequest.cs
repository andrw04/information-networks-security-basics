namespace Domain.Models;

public class ServiceRequest
{
    public byte[] EncryptedTicket { get; set; }
    public byte[] EncryptedAuthBlock { get; set; }
}
