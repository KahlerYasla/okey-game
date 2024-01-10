
public class Settings
{
    public required string Id { get; set; }
    public required string UserId { get; set; }
    public bool FriendRequest { get; set; }
    public bool Chat { get; set; }
    public bool GameInvite { get; set; }
    public int HandCount { get; set; }
    public int SingleOrPaired { get; set; }
    public int GameHelp { get; set; }
    public int Folded { get; set; }
}
