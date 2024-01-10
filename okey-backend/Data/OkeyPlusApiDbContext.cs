using Microsoft.EntityFrameworkCore;


public class OkeyPlusApiDbContext : DbContext
{
    public OkeyPlusApiDbContext(DbContextOptions<OkeyPlusApiDbContext> options) : base(options) { }

    public DbSet<User>? Users { get; set; }
    public DbSet<Verification>? Verifications { get; set; }
    public DbSet<PasswordResetKey>? PasswordResetKeys { get; set; }
    public DbSet<Settings>? Settings { get; set; }
    public DbSet<ChatMessage>? ChatMessages { get; set; }
}
