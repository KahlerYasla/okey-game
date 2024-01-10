using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class PasswordResetKeyRequest
{
    public User? UserInstance { get; set; }
    public string? Email { get; set; }
}
