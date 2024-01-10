using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class PasswordResetKey
{
    public int Id { get; set; }
    public string Key { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string? UserId { get; set; }
    public User User { get; set; } = null!;
}
