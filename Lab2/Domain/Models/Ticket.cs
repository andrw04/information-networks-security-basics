namespace Domain.Models;

public record Ticket
{
    public string From { get; set; }
    public string To { get; set; }
    public byte[] Key { get; set; }
    public long TimeStamp { get; set; }
    public long Expiration { get; set; }
}
