using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class GameData
{
    public List<string>? PlayerPieces { get; set; }
    public List<List<string>>? MiddleGrid { get; set; }
    public List<List<string>>? RightGrid { get; set; }
    public string? MiddlePiece { get; set; }
    public List<List<string>>? CornersL { get; set; }
    public string? OkeyPiece { get; set; }
}
