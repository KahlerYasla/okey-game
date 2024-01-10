
// singleton class
using System.Collections;
using System.Linq;
using System.Text.Json;

public class Game
{
    private static Game? _instance;
    private static readonly object _lock = new();

    private Game()
    {
    }

    public static Game Instance
    {
        get
        {
            lock (_lock)
            {
                _instance ??= new Game();

                return _instance;
            }
        }
    }
    //=================================================================================================================================================================
    public string Play(string message, Room room, int playerIndex, List<List<string>>? listOfValidGroups = null)
    {
        // parse the message 
        /* 
        first 0 and 1 characters are the move
        2 3 and 4 characters are the piece
        5 6 7 and 8 characters are the position. NA if the move doesn't require a position
        */

        string move = message[..2];
        string? piece;
        string? position;
        try
        {
            piece = message[2..5];
        }
        catch (System.Exception)
        {
            piece = null;
        }
        try
        {
            position = message[5..9];
        }
        catch (System.Exception)
        {
            position = null;
        }

        // check if the move is valid
        if (!Dictionaries.moves.ContainsKey(move))
        {
            Console.WriteLine(move);
            Console.WriteLine("Invalid move!");
            return "Invalid move!";
        }

        string gameLoopResponse = Dictionaries.moves[move](piece, position, room, playerIndex, listOfValidGroups);

        // return the result of the move: player's deck, middle grid, right grid, middle stack, cornersL
        return gameLoopResponse;
    }
    //=================================================================================================================================================================
    public string GetPieceFromLeft(string? piece, string? position, Room room, int playerIndex, List<List<string>>? listOfValidGroups)
    {
        Stack<string>[] cornersL = room.CornersL;
        string? pieceFromLeft;

        try
        {
            // get the piece from the left corner at the top
            pieceFromLeft = cornersL[playerIndex].Pop();
        }
        catch (Exception)
        {
            Console.WriteLine("No piece left!");
            return "No piece left!";
        }

        // if the piece is null, return
        if (pieceFromLeft == null)
        {
            Console.WriteLine("No piece left!");
            return "No piece left!";
        }

        // add the piece to the player's pieces
        room.Players[playerIndex].Pieces.Add(pieceFromLeft);

        GameLoopResponse result = new()
        {
            PlayerPieces = room.Players[playerIndex].Pieces,
            MiddleGrid = room.MiddleGrid,
            RightGrid = room.RightGrid,
            MiddlePiece = room.MiddleStack.Peek(),
            CornersL = room.CornersL,
            OkeyPiece = room.OkeyPiece
        };


        return JsonSerializer.Serialize(result);
    }
    //=================================================================================================================================================================
    public string GetPieceFromMiddle(string? piece, string? position, Room room, int playerIndex, List<List<string>>? listOfValidGroups)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("GetPieceFromMiddle()");
        Console.ResetColor();

        Stack<string> middleStack = room.MiddleStack;

        // get the piece from the middle stack
        string? pieceFromMiddle = middleStack.Pop();

        // if the piece is null, return
        if (pieceFromMiddle == null)
        {
            Console.WriteLine("No piece left!");
            return "No piece left!";
        }

        // add the piece to the player's pieces
        room.Players[playerIndex].Pieces.Add(pieceFromMiddle);

        GameLoopResponse result = new()
        {
            PlayerPieces = room.Players[playerIndex].Pieces,
            MiddleGrid = room.MiddleGrid,
            RightGrid = room.RightGrid,
            MiddlePiece = pieceFromMiddle,
            CornersL = room.CornersL,
            OkeyPiece = room.OkeyPiece
        };


        return JsonSerializer.Serialize(result);
    }
    //=================================================================================================================================================================
    public string ThrowPieceToRight(string? piece, string? position, Room room, int playerIndex, List<List<string>>? listOfValidGroups)
    {
        Stack<string>[] cornersL = room.CornersL;

        // get the piece which is given in arguments from the player's pieces
        int pieceIndex = room.Players[playerIndex].Pieces.ToList().FindIndex(x => x == piece);

        if (pieceIndex == -1)
        {
            Console.WriteLine("No such piece!");
            return "No such piece!";
        }

        string? pieceFromPlayer = room.Players[playerIndex].Pieces[pieceIndex];

        // if the piece is null, return
        if (pieceFromPlayer == null)
        {
            Console.WriteLine("No such piece!");
            return "No such piece!";
        }

        // remove the piece from the player's pieces from the list
        room.Players[playerIndex].Pieces.RemoveAt(pieceIndex);

        // add the piece to the next player's corner
        cornersL[(playerIndex + 1) % 4].Push(pieceFromPlayer);

        GameLoopResponse result = new()
        {
            PlayerPieces = room.Players[playerIndex].Pieces,
            MiddleGrid = room.MiddleGrid,
            RightGrid = room.RightGrid,
            MiddlePiece = room.MiddleStack.Peek(),
            CornersL = room.CornersL,
            OkeyPiece = room.OkeyPiece
        };

        return JsonSerializer.Serialize(result);
    }
    //=================================================================================================================================================================
    public string OpenPair(string? piece, string? position, Room room, int playerIndex, List<List<string>> listOfValidGroups)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\nOpenPair()\n");
        Console.ResetColor();

        Console.WriteLine("Count of valid groups: " + listOfValidGroups.Count);

        int emptyRowIndex = 0;

        // find the latest empty list
        for (int i = 0; i < room.RightGrid!.Count; i++) // loop through the rows = 26
        {
            // if there is any element that contains not 0 characters then it is what we are looking for
            if (room.RightGrid[i].Any(x => x.Length != 0))
            {
                emptyRowIndex = i;
                break;
            }
        }

        // place a group from the listOfValidGroups to the latest empty list
        for (int i = 0; i < listOfValidGroups.Count; i++) // loop through the of all valid groups
        {
            for (int j = 0; j < listOfValidGroups[i].Count; j++) // loop through the pieces of the group
            {
                // place the piece to the right grid
                room.RightGrid[emptyRowIndex][j] = listOfValidGroups[i][j];
            }

            emptyRowIndex++;
        }

        // remove the groups with from the player's pieces
        for (int i = 0; i < listOfValidGroups.Count; i++) // loop through the of all valid groups
        {
            for (int j = 0; j < listOfValidGroups[i].Count; j++) // loop through the pieces of the group
            {
                room.Players![playerIndex].Pieces.Remove(listOfValidGroups[i][j]);
            }
        }

        var result = new GameLoopResponse
        {
            PlayerPieces = room.Players[playerIndex].Pieces,
            MiddleGrid = null,
            RightGrid = room.RightGrid,
            MiddlePiece = room.MiddleStack.Peek(),
            CornersL = room.CornersL,
            OkeyPiece = room.OkeyPiece
        };

        return JsonSerializer.Serialize(result);
    }
    //=================================================================================================================================================================
    public string OpenSeries(string? piece, string? position, Room room, int playerIndex, List<List<string>> listOfValidGroups)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\nOpenSeries()\n");
        Console.ResetColor();

        Console.WriteLine("Count of valid groups: " + listOfValidGroups.Count);

        int emptyRowIndex = 0;

        // find the latest empty list
        for (int i = 0; i < room.MiddleGrid!.Count; i++) // loop through the rows = 26
        {
            // if there is any element that contains not 0 characters then it is what we are looking for
            if (room.MiddleGrid[i].Any(x => x.Length != 0))
            {
                emptyRowIndex = i;
                break;
            }
        }

        // place a group from the listOfValidGroups to the latest empty list
        for (int i = 0; i < listOfValidGroups.Count; i++) // loop through the of all valid groups
        {
            bool isGroupSameNumberSerie = true; // are all the pieces in the group have the same number
            isGroupSameNumberSerie = listOfValidGroups[i].Select(x => x[1..]).Distinct().Count() == 1;

            bool isGroupStartsWithLessThan7 = true; // is the group starts with a number less than 7
            isGroupStartsWithLessThan7 = int.Parse(listOfValidGroups[i][0][1..]) < 7;

            Console.WriteLine("\nvalid group " + i.ToString() + JsonSerializer.Serialize(listOfValidGroups[i]));

            if (isGroupStartsWithLessThan7)
            {
                int firstPieceIndexInGroup = int.Parse(listOfValidGroups[i][0][1..]);
                if (firstPieceIndexInGroup == 0)
                {
                    firstPieceIndexInGroup = int.Parse(listOfValidGroups[i][1][1..]) - 2;
                }
                else if (firstPieceIndexInGroup == int.Parse(room.OkeyPiece[1..]))
                {
                    firstPieceIndexInGroup = int.Parse(listOfValidGroups[i][1][1..]) - 2;
                }

                firstPieceIndexInGroup--;

                for (int j = 0; j < listOfValidGroups[i].Count; j++) // loop through the pieces of the group
                {
                    // place the piece to the middle grid
                    room.MiddleGrid[emptyRowIndex][j + firstPieceIndexInGroup] = listOfValidGroups[i][j];
                }
            }
            else
            {
                int latestPieceIndexInGroup = int.Parse(listOfValidGroups[i][listOfValidGroups[i].Count - 1][1..]);
                if (latestPieceIndexInGroup == 0)
                {
                    latestPieceIndexInGroup = int.Parse(listOfValidGroups[i][listOfValidGroups[i].Count - 2][1..]);
                }
                else if (latestPieceIndexInGroup == int.Parse(room.OkeyPiece[1..]))
                {
                    latestPieceIndexInGroup = int.Parse(listOfValidGroups[i][listOfValidGroups[i].Count - 2][1..]);
                }

                latestPieceIndexInGroup--;

                for (int j = 0; j < listOfValidGroups[i].Count; j++) // loop through the pieces of the group
                {
                    // place the piece to the middle grid
                    room.MiddleGrid[emptyRowIndex][latestPieceIndexInGroup - listOfValidGroups[i].Count + j + 1] = listOfValidGroups[i][j];
                }
            }

            emptyRowIndex++;
        }

        // remove the groups with from the player's pieces
        for (int i = 0; i < listOfValidGroups.Count; i++) // loop through the of all valid groups
        {
            for (int j = 0; j < listOfValidGroups[i].Count; j++) // loop through the pieces of the group
            {
                room.Players![playerIndex].Pieces.Remove(listOfValidGroups[i][j]);
            }
        }

        var result = new GameLoopResponse
        {
            PlayerPieces = room.Players[playerIndex].Pieces,
            MiddleGrid = room.MiddleGrid,
            RightGrid = null,
            MiddlePiece = room.MiddleStack.Peek(),
            CornersL = room.CornersL,
            OkeyPiece = room.OkeyPiece
        };

        return JsonSerializer.Serialize(result);
    }
    //=================================================================================================================================================================
    public string ThrowPieceToLeftAgain(string? piece, string? position, Room room, int playerIndex, List<List<string>>? listOfValidGroups)
    {
        Stack<string>[] cornersL = room.CornersL;

        // get the piece which is given in arguments from the player's pieces
        int pieceIndex = room.Players[playerIndex].Pieces.ToList().FindIndex(x => x == piece);
        string? pieceFromPlayer = room.Players[playerIndex].Pieces[pieceIndex];

        // if the piece is null, return
        if (pieceFromPlayer == null)
        {
            Console.WriteLine("No such piece!");
            return "No such piece!";
        }

        // remove the piece from the player's pieces
        room.Players[playerIndex].Pieces.RemoveAt(pieceIndex);

        // add the piece to the next player's corner
        cornersL[(playerIndex + 3) % 4].Push(pieceFromPlayer);

        GameLoopResponse result = new()
        {
            PlayerPieces = room.Players[playerIndex].Pieces,
            MiddleGrid = room.MiddleGrid,
            RightGrid = room.RightGrid,
            MiddlePiece = room.MiddleStack.Peek(),
            CornersL = room.CornersL,
            OkeyPiece = room.OkeyPiece
        };

        return JsonSerializer.Serialize(result);
    }
    //=================================================================================================================================================================
    // sort the pieces of the player with series groups
    // rule: all possible groups are grouped with at least 3 pieces with the numbers are (in ascending order with same colors and different numbers) or (all the different color with same numbers).
    // the return value value will be the most valuable sorted deck for the player. Consists of the groups and the remaining pieces. 
    // the value of the deck is calculated by the sum of the each group's value
    // each group's value is calculated by the sum of the numbers of the pieces in the group.
    public string SortSeries(string? p, string? position, Room room, int playerIndex, List<List<string>>? listOfValidGroups)
    {
        List<string> playerPieces = new(room.Players[playerIndex].Pieces);

        Console.WriteLine("\n\nPlayer pieces: " + JsonSerializer.Serialize(playerPieces));

        // find the count of the okeyPiece in the player's pieces
        int okeyCount = playerPieces.Count(x => x == room.OkeyPiece);

        int turnedOkeyPieceCount = 0;

        #region // --------------------- convert the okey pieces to j00 ---------------------
        // if there are okey pieces in player's pieces find the piece okey piece with code j00 and convert it to room.OkeyPiece
        if (playerPieces.Count(x => x == "j00") > 0)
        {
            // if there are okey pieces in player's pieces find the piece okey piece with code j00 and convert it to room.OkeyPiece
            for (int i = 0; i < playerPieces.Count; i++)
            {
                if (playerPieces[i] == "j00")
                {
                    playerPieces[i] = room.OkeyPiece;
                    turnedOkeyPieceCount++;
                }
            }
        }

        Console.WriteLine("\nOkey count: " + okeyCount);
        Console.WriteLine("\nTurned okey piece count (Joker count): " + turnedOkeyPieceCount);
        #endregion // --------------------- convert the okey pieces to j00 ---------------------

        List<List<string>> allPossibleGroups = new();
        List<List<List<string>>> allPossibleGroupCombinations = new();
        List<List<string>> mostValuableGroupCombination = new();
        List<string> remainingPieces = new();

        List<List<string>> seriesCandidates = new();

        // sort the pieces by number
        playerPieces.Sort((x, y) => int.Parse(x[1..]).CompareTo(int.Parse(y[1..])));

        #region // --------------------- get all possible groups with at least 3 pieces with the numbers are (all the different color with same numbers) ---------------------
        // get all possible groups with at least 3 pieces with the numbers are (all the different color with same numbers).
        for (int i = 0; i < playerPieces.Count; i++)
        {
            List<string> group = new()
            {
                playerPieces[i]
            };
            for (int j = i + 1; j < playerPieces.Count; j++)
            {
                if (playerPieces[i][1..] == playerPieces[j][1..])
                {
                    group.Add(playerPieces[j]);
                }
            }

            // if the group has 2 pieces add it to the seriesCandidates list
            if (group.Count >= 2)
            {
                seriesCandidates.Add(group);
            }

            // if the group has at least 3 pieces and all the pieces are different colors each other, add it to the allPossibleGroups list
            if (group.Count >= 3 && group.Select(x => x[0]).Distinct().Count() == group.Count)
            {
                allPossibleGroups.Add(group);
            }
        }

        #endregion // --------------------- get all possible groups with at least 3 pieces with the numbers are (all the different color with same numbers) ---------------------

        Console.WriteLine("\n\nAll possible groups (all the different color with same numbers): " + JsonSerializer.Serialize(allPossibleGroups));

        // sort the pieces by color
        playerPieces.Sort((x, y) => x[0].CompareTo(y[0]));

        // sort each color by number in ascending order by its inside
        Dictionary<string, List<string>> colors = new();

        for (int i = 0; i < playerPieces.Count; i++)
        {
            // if color is not in the colors list, add it
            if (!colors.ContainsKey(playerPieces[i][0].ToString()))
            {
                colors.Add(playerPieces[i][0].ToString(), new List<string>());
            }

            // add the piece to the color
            colors[playerPieces[i][0].ToString()].Add(playerPieces[i]);
        }

        // sort the pieces by number in ascending order
        playerPieces.Sort((x, y) => int.Parse(x[1..]).CompareTo(int.Parse(y[1..])));

        Console.WriteLine("\n\nPlayer pieces sorted: " + JsonSerializer.Serialize(playerPieces));

        #region // --------------------- get all possible groups with at least 3 pieces with the numbers are (in ascending order with same colors and all are unique number in the group) ---------------------
        // get all possible groups with at least 3 pieces with the numbers are (in ascending order with same colors and all are unique number in the group).
        for (int i = 0; i < playerPieces.Count; i++)
        {
            List<string> group = new()
            {
                playerPieces[i]
            };
            for (int j = i + 1; j < playerPieces.Count; j++)
            {
                if (playerPieces[i][0] == playerPieces[j][0] && int.Parse(group[^1][1..]) + 1 == int.Parse(playerPieces[j][1..]))
                {
                    // if the piece already added dont add
                    if (!group.Contains(playerPieces[j]))
                    {
                        group.Add(playerPieces[j]);
                    }
                }
            }

            // if the group has 2 pieces add it to the seriesCandidates list
            if (group.Count >= 2)
            {
                seriesCandidates.Add(group);
            }

            // if the group has at least 3 pieces and all the pieces are different numbers each other, add it to the allPossibleGroups list
            if (group.Count >= 3)
            {
                allPossibleGroups.Add(group);
            }
        }

        #endregion // ------------------- get all possible groups with at least 3 pieces with the numbers are (in ascending order with same colors and all are unique number in the group) -----------------

        #region //-------------------- find the max 2 values of the seriesCandidates and eliminate others --------------------------
        int maxSeriesCandidatesValue0 = 0, maxSeriesCandidatesValue1 = 0;

        foreach (var seriesCandidate in seriesCandidates)
        {
            int seriesCandidateValue = 0;

            foreach (var pieceInSeriesCandidate in seriesCandidate)
            {
                seriesCandidateValue += int.Parse(pieceInSeriesCandidate[1..]);
            }

            if (seriesCandidateValue > maxSeriesCandidatesValue0)
            {
                maxSeriesCandidatesValue1 = maxSeriesCandidatesValue0;
                maxSeriesCandidatesValue0 = seriesCandidateValue;
            }
            else if (seriesCandidateValue > maxSeriesCandidatesValue1)
            {
                maxSeriesCandidatesValue1 = seriesCandidateValue;
            }
        }
        seriesCandidates = seriesCandidates.Where(x => x.Sum(y => int.Parse(y[1..])) == maxSeriesCandidatesValue0 || x.Sum(y => int.Parse(y[1..])) == maxSeriesCandidatesValue1).ToList();

        // sort the seriesCandidates by ascending order of the sum of the numbers of the pieces in the group
        seriesCandidates.Sort((x, y) => x.Sum(z => int.Parse(z[1..])).CompareTo(y.Sum(z => int.Parse(z[1..]))));

        Console.WriteLine("\n\nSeries candidates: " + JsonSerializer.Serialize(seriesCandidates));
        Console.WriteLine();

        // add the seriesCandidates to the allPossibleGroups limited by the player's pieces room.OkeyPiece count
        foreach (var seriesCandidate in seriesCandidates)
        {
            if (okeyCount > 0)
            {
                seriesCandidate.Add(room.OkeyPiece);
                allPossibleGroups.Add(seriesCandidate);
                okeyCount--;
            }
        }

        // eliminate the groups has more than 4 pieces
        allPossibleGroups = allPossibleGroups.Where(x => x.Count <= 4).ToList();

        #endregion //-------------------- find the max 2 values of the seriesCandidates and eliminate others --------------------------


        Console.WriteLine("\n\nAll possible groups (in ascending order with same colors and different numbers) and other rule added: " + JsonSerializer.Serialize(allPossibleGroups));
        Console.WriteLine();

        if (allPossibleGroups.Count == 0)
        {
            Console.WriteLine("No possible groups!");
            return JsonSerializer.Serialize(new GameLoopResponse
            {
                PlayerPieces = playerPieces,
                MiddleGrid = room.MiddleGrid,
                RightGrid = room.RightGrid,
                MiddlePiece = room.MiddleStack.Peek(),
                CornersL = room.CornersL,
                OkeyPiece = room.OkeyPiece
            });
        }

        #region // --------------------- get all possible group combinations ---------------------
        // get all possible group combinations with groups however each piece can be used only the number of its count in the player's pieces among all the groups
        // 2^allPossibleGroups.Count
        int maxCount = (int)Math.Pow(2, allPossibleGroups.Count) - 1;

        Console.WriteLine("\n\nMax count: " + maxCount);
        Console.WriteLine();

        for (int i = 1; i <= maxCount; i++)
        {
            List<List<string>> groupCombination = new();

            // get the binary representation of the number
            string binary = Convert.ToString(i, 2);

            // add 0s to the beginning of the binary representation to make it 4 digits
            while (binary.Length < allPossibleGroups.Count)
            {
                binary = "0" + binary;
            }

            // get the groups according to the binary representation
            for (int j = 0; j < binary.Length; j++)
            {
                if (binary[j] == '1')
                {
                    groupCombination.Add(allPossibleGroups[j]);
                }
            }

            // eliminate the combinations which has the total count of a piece more than the count of the piece in the player's pieces
            bool isValid = true;

            foreach (var pieceInPlayerPieces in playerPieces)
            {
                int countOfPieceInPlayerPieces = playerPieces.Count(x => x == pieceInPlayerPieces);
                int countOfPieceInGroupCombination = groupCombination.SelectMany(x => x).Count(x => x == pieceInPlayerPieces);

                if (countOfPieceInGroupCombination > countOfPieceInPlayerPieces)
                {
                    isValid = false;
                    break;
                }
            }

            if (!isValid)
            {
                continue;
            }

            allPossibleGroupCombinations.Add(groupCombination);

        }

        #endregion // --------------------- get all possible group combinations ---------------------


        Console.WriteLine("\n\nAll possible group combinations: " + JsonSerializer.Serialize(allPossibleGroupCombinations));
        Console.WriteLine();

        // find the most valuable group combination
        int maxGroupCombinationValue = 0;

        foreach (var groupCombination in allPossibleGroupCombinations)
        {
            int groupCombinationValue = 0;

            foreach (var group in groupCombination)
            {
                int groupValue = 0;

                foreach (var pieceInGroup in group)
                {
                    if (pieceInGroup == room.OkeyPiece)
                    {
                        groupValue += 50;
                        continue;
                    }

                    groupValue += int.Parse(pieceInGroup[1..]);
                }

                groupCombinationValue += groupValue;
            }

            if (groupCombinationValue > maxGroupCombinationValue)
            {
                maxGroupCombinationValue = groupCombinationValue;
                mostValuableGroupCombination = groupCombination;
            }
        }

        Console.WriteLine("\n\nMost valuable group combination: " + JsonSerializer.Serialize(mostValuableGroupCombination));
        Console.WriteLine();

        Dictionary<string, int> eachPieceCountOfPlayer = new();

        // get the count of each piece in the player's pieces
        foreach (var pieceInPlayerPieces in playerPieces)
        {
            if (!eachPieceCountOfPlayer.ContainsKey(pieceInPlayerPieces))
            {
                eachPieceCountOfPlayer.Add(pieceInPlayerPieces, playerPieces.Count(x => x == pieceInPlayerPieces));
            }
        }

        // set the remaining pieces after maxGroupCombinationValue
        foreach (var group in mostValuableGroupCombination)
        {
            foreach (var pieceInGroup in group)
            {
                eachPieceCountOfPlayer[pieceInGroup]--;
            }
        }

        // get the remaining pieces
        foreach (var piece in eachPieceCountOfPlayer)
        {
            for (int i = 0; i < piece.Value; i++)
            {
                remainingPieces.Add(piece.Key);
            }
        }

        // add the remaining pieces to the most valuable group combination
        mostValuableGroupCombination.Add(remainingPieces);

        // add 000 between the groups
        for (int i = 0; i < mostValuableGroupCombination.Count - 1; i++)
        {
            mostValuableGroupCombination[i].Add("000");
        }

        // convert mostValuableGroupCombination to a 1 dimension list
        List<string> mostValuableGroupCombinationLast = mostValuableGroupCombination.SelectMany(x => x).ToList();

        Console.WriteLine("\n\nMost valuable group combination: " + JsonSerializer.Serialize(mostValuableGroupCombination));
        Console.WriteLine();

        Console.WriteLine("\n\nMost valuable group combination last: " + JsonSerializer.Serialize(mostValuableGroupCombinationLast));
        Console.WriteLine();

        #region // if there are room.OkeyPiece in player's pieces find the pieces convert them to j00
        for (int i = 0; i < turnedOkeyPieceCount; i++)
        {
            int okeyIndex = mostValuableGroupCombinationLast.IndexOf(room.OkeyPiece);
            Console.WriteLine("\n\nOkey index: " + okeyIndex + "\n");
            mostValuableGroupCombinationLast[okeyIndex] = "j00";
        }
        #endregion // if there are room.OkeyPiece in player's pieces find the pieces convert them to j00

        GameLoopResponse result = new()
        {
            PlayerPieces = mostValuableGroupCombinationLast,
            MiddleGrid = null,
            RightGrid = room.RightGrid,
            MiddlePiece = room.MiddleStack.Peek(),
            CornersL = room.CornersL,
            OkeyPiece = room.OkeyPiece
        };

        return JsonSerializer.Serialize(result);
    }

    //=================================================================================================================================================================
    // sort the pieces of the player with pair groups
    // rule: all possible groups are grouped with excactly 2 pieces with the same number and color.
    // the return value value will be the groups and the remaining pieces.
    public string SortPairs(string? piece, string? position, Room room, int playerIndex, List<List<string>>? listOfValidGroups)
    {
        List<string> playerPieces = new(room.Players[playerIndex].Pieces);

        // find the count of the okeyPiece in the player's pieces
        int okeyCount = playerPieces.Count(x => x == room.OkeyPiece);

        int turnedOkeyPieceCount = 0;

        #region // --------------------- convert the okey pieces to j00 ---------------------
        // if there are okey pieces in player's pieces find the piece okey piece with code j00 and convert it to room.OkeyPiece
        if (playerPieces.Count(x => x == "j00") > 0)
        {
            // if there are okey pieces in player's pieces find the piece okey piece with code j00 and convert it to room.OkeyPiece
            for (int i = 0; i < playerPieces.Count; i++)
            {
                if (playerPieces[i] == "j00")
                {
                    playerPieces[i] = room.OkeyPiece;
                    turnedOkeyPieceCount++;
                }
            }
        }
        #endregion // --------------------- convert the okey pieces to j00 ---------------------

        List<List<string>> allGroups = new();
        List<string> remainingPieces = new();

        // find all possible pairs and add them to the allGroups list
        allGroups = playerPieces.GroupBy(x => x).Where(x => x.Count() == 2).Select(x => x.ToList()).ToList();

        // find remaining pieces and add them to the remainingPieces list from playerPieces
        remainingPieces = playerPieces.Where(x => !allGroups.SelectMany(y => y).Contains(x)).ToList();

        // find the biggest 2 remaining pieces and pair them with room.OkeyPiece as much as room.OkeyPiece count
        if (okeyCount > 0)
        {
            // find the biggest 2 remaining pieces
            List<string> biggest2RemainingPieces = remainingPieces.OrderByDescending(x => int.Parse(x[1..])).Take(2).ToList();
            List<List<string>> biggest2RemainingPiecesPairs = new();

            // add room.OkeyPiece to the biggest 2 remaining pieces as much as room.OkeyPiece count
            for (int i = 0; i < okeyCount; i++)
            {
                remainingPieces.Remove(biggest2RemainingPieces[i]);
                remainingPieces.Remove(room.OkeyPiece);

                biggest2RemainingPiecesPairs.Add(new List<string> { biggest2RemainingPieces[i], room.OkeyPiece });
            }

            allGroups.Add(biggest2RemainingPiecesPairs.SelectMany(x => x).ToList());
        }

        allGroups.Add(remainingPieces);

        // sort the allGroups by the sum of the numbers of the pieces in the group
        allGroups.Sort((x, y) => x.Sum(z => int.Parse(z[1..])).CompareTo(y.Sum(z => int.Parse(z[1..]))));

        Console.WriteLine("\n\nall groups sorted for pair:" + JsonSerializer.Serialize(allGroups));
        Console.WriteLine();

        // add 000 between the groups
        for (int i = 0; i < allGroups.Count - 1; i++)
        {
            // if group is empty continue
            if (allGroups[i].Count == 2)
            {
                allGroups[i].Add("000");
            }
        }

        Console.WriteLine("\n\nall groups sorted for pair and added 000s:" + JsonSerializer.Serialize(allGroups));
        Console.WriteLine();

        // convert allGroups to a 1 dimension list
        List<string> allGroupsLast = allGroups.SelectMany(x => x).ToList();

        #region // if there are room.OkeyPiece in player's pieces find the pieces convert them to j00
        for (int i = 0; i < turnedOkeyPieceCount; i++)
        {
            int okeyIndex = allGroupsLast.IndexOf(room.OkeyPiece);
            Console.WriteLine("\n\nOkey index: " + okeyIndex + "\n");
            allGroupsLast[okeyIndex] = "j00";
        }
        #endregion // if there are room.OkeyPiece in player's pieces find the pieces convert them to j00

        GameLoopResponse result = new()
        {
            PlayerPieces = allGroupsLast,
            MiddleGrid = null,
            RightGrid = room.RightGrid,
            MiddlePiece = room.MiddleStack.Peek(),
            CornersL = room.CornersL,
            OkeyPiece = room.OkeyPiece
        };

        return JsonSerializer.Serialize(result);
    }

}
