using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public class ChatMessage
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public required string Sender { get; set; }
    public string? Receiver { get; set; }
    public string? GroupId { get; set; }
    public required string Message { get; set; }
}
