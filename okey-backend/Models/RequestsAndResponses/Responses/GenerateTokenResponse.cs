using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class GenerateTokenResponse
{
    public required string Token { get; set; }
    public DateTime Expiration { get; set; }
}
