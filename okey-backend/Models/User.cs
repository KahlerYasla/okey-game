
public class User
{
    public required string Id { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? EmailAddress { get; set; }
    public string? Password { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsVerified { get; set; }
    public bool Status { get; set; }
    public DateTime CreatedDate { get; set; }
    public double? Level { get; set; }
    public string? ProfileImagePath { get; set; }
}