using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public class Player
{
    public string? Id { get; set; }
    public string UserName { get; set; }
    public string ConnectionId { get; set; }
    public List<string> Pieces { get; set; }

    public Player(string? id, string userName, string connectionId, List<string> pieces)
    {
        Id = id;
        UserName = userName;
        ConnectionId = connectionId;
        Pieces = pieces;
    }
}
