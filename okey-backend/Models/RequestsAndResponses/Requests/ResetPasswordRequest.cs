using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public class ResetPasswordRequest
{
    public string UserId { get; set; } = null!;
    public string PasswordResetKey { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}
