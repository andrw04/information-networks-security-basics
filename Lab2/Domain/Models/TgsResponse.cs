namespace Domain.Models;

public class TgsResponse
{
    public byte[] EncryptedTicket { get; set; }
    public byte[] Key { get; set; }
}
