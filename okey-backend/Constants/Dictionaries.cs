using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public class Dictionaries
{

    public static readonly Dictionary<string, Delegates.Move> moves = new()
    {

        ["00"] = Game.Instance.GetPieceFromLeft,
        ["01"] = Game.Instance.GetPieceFromMiddle,
        ["02"] = Game.Instance.ThrowPieceToRight,
        ["03"] = Game.Instance.OpenPair, //
        ["04"] = Game.Instance.OpenSeries, //
        ["05"] = Game.Instance.ThrowPieceToLeftAgain,
        ["06"] = Game.Instance.SortSeries, //
        ["07"] = Game.Instance.SortPairs, //
    };
}
