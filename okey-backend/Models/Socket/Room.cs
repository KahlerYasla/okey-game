using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;


public class Room
{
    public string? Id { get; set; }
    public List<Player>? Players { get; set; }
    public string? OkeyPiece { get; set; }
    public Stack<string>? MiddleStack { get; set; }
    public List<List<string>>? MiddleGrid { get; set; }
    public List<List<string>>? RightGrid { get; set; }
    public Stack<string>[]? CornersL { get; set; }
    public Stack<string>? PiecesBeforeDeal { get; set; }
    public int turnOfPlayerIndex = 0;


    public static Stack<string> GeneratePieces()
    {
        Stack<string> pieces = new();

        // black, orange, red, green
        string[] colors = { "b", "o", "r", "g" };

        // generate pieces
        for (int i = 0; i < 4; i++)
        {
            for (int j = 1; j <= 13; j++)
            {
                if (j < 10)
                {
                    pieces.Push(colors[i] + "0" + j);
                    continue;
                }
                pieces.Push(colors[i] + j);
            }
            for (int j = 1; j <= 13; j++)
            {
                if (j < 10)
                {
                    pieces.Push(colors[i] + "0" + j);
                    continue;
                }
                pieces.Push(colors[i] + j);
            }
        }

        // generate joker pieces
        pieces.Push("j00");
        pieces.Push("j00");


        // shuffle pieces
        Random random = new();
        pieces = new Stack<string>(pieces.OrderBy(x => random.Next()));

        return pieces;
    }

    // 26 x 13
    public static List<List<string>> GenerateMiddleGrid()
    {
        List<List<string>> middleGrid = new();

        for (int i = 0; i < 26; i++)
        {
            middleGrid.Add(new List<string>());
            for (int j = 0; j < 13; j++)
            {
                middleGrid[i].Add("");
            }
        }

        return middleGrid;
    }

    // 39 x 2
    public static List<List<string>> GenerateRightGrid()
    {
        List<List<string>> rightGrid = new();

        for (int i = 0; i < 39; i++)
        {
            rightGrid.Add(new List<string>());
            for (int j = 0; j < 2; j++)
            {
                rightGrid[i].Add("");
            }
        }

        return rightGrid;
    }

    public static string ChooseOkeyRandomly(ref Stack<string> pieces)
    {
        Random random = new();
        int index = random.Next(0, pieces.Count);

        if (pieces.ElementAt(index)[..1] == "j")
        {
            return ChooseOkeyRandomly(ref pieces);
        }

        // remove the first okey -1 piece from the pieces beacuse it is the show piece
        string showPieceColor = pieces.ElementAt(index)[..1];
        string showPieceNumber = (Convert.ToInt32(pieces.ElementAt(index)[1..]) - 1).ToString();

        if (showPieceNumber == "0")
        {
            showPieceNumber = "13";
        }

        showPieceNumber = showPieceNumber.Length == 1 ? "0" + showPieceNumber : showPieceNumber;

        string showPiece = showPieceColor + showPieceNumber;

        Console.WriteLine("\nShow piece: " + showPiece);

        // remove the first occurence of the show piece from the pieces. There are 2 show pieces in the pieces. Let stay the 1 of them.
        pieces = new Stack<string>(pieces.Where(x => x != showPiece));
        pieces.Push(showPiece);

        return pieces.ElementAt(index);
    }
}

