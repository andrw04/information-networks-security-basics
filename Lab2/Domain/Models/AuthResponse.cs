﻿namespace Domain.Models;

public class AuthResponse
{
    public byte[] EncryptedTicket { get; set; }
    public byte[] Key { get; set; }
}
