namespace Domain.Models;

public class TgsRequest
{
    public byte[] EncryptedTicket { get; set; }
    public byte[] EncryptedAuthBlock { get; set; }
    public string Recipient { get; set; }
}
