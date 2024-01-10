using System.Collections;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;


[Authorize]
public class ChatHub : Hub
{
    private OkeyPlusApiDbContext _context;

    // ConnectionId, UserName
    private static readonly Dictionary<string, string> playerConnectionIds = new();
    private static readonly List<Room> rooms = new();

    public ChatHub(OkeyPlusApiDbContext context)
    {
        _context = context;
    }
    //==================================================================================================================================================================
    public async Task JoinOrCreateGroup(string userName)
    {
        // add player to waiting list
        playerConnectionIds.Add(Context.ConnectionId, userName);

        // if there are 4 players in the waiting list, create a group
        if (playerConnectionIds.Count == 4)
        {
            string groupId;

            // generate random group Id
            groupId = Guid.NewGuid().ToString();

            Stack<string> generatedPieces = Room.GeneratePieces();

            // create a new room
            Room room = new()
            {
                Id = groupId,
                Players = new List<Player>(),
                OkeyPiece = Room.ChooseOkeyRandomly(ref generatedPieces),
                MiddleStack = new Stack<string>(),
                MiddleGrid = Room.GenerateMiddleGrid(), // 13 x 13
                RightGrid = Room.GenerateRightGrid(), // 6 x 13
                CornersL = new Stack<string>[4] { new(), new(), new(), new() },
                PiecesBeforeDeal = generatedPieces,  // 106 pieces -1 shown piece
            };

            // add players to the group
            foreach (var playerConnectionId in playerConnectionIds)
            {
                // add player to the group
                await Groups.AddToGroupAsync(playerConnectionId.Key, groupId);

                // create a new player
                Player player = new(null, playerConnectionId.Value, playerConnectionId.Key, new List<string>());

                // add player to the room
                room.Players!.Add(player);

            }

            // give players to 21 pieces each choose 1 player to give 1 more piece and he will have 22 and he will start the game        
            room.Players[0].Pieces.Add(room.PiecesBeforeDeal.Pop());

            for (int i = 0; i < 21; i++)
            {
                foreach (var player in room.Players)
                {
                    player.Pieces.Add(room.PiecesBeforeDeal.Pop());
                }
            }

            // rest of the pieces will be in the middle stack
            room.MiddleStack = room.PiecesBeforeDeal;

            // Send first GameLoopResponse to players
            foreach (var player in room.Players)
            {
                GameLoopResponse result = new()
                {
                    PlayerPieces = player.Pieces,
                    MiddleGrid = room.MiddleGrid,
                    RightGrid = room.RightGrid,
                    MiddlePiece = room.MiddleStack.Peek(),
                    CornersL = room.CornersL,
                    OkeyPiece = room.OkeyPiece,
                    IsTurnOfPlayer = false
                };

                // first player's turn
                if (player.UserName == room.Players[0].UserName)
                {
                    result.IsTurnOfPlayer = true;
                }

                string response = JsonSerializer.Serialize(result)!;

                await Clients.Client(player.ConnectionId).SendAsync("ReceiveMessage", "server", response);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nResponse: " + response);
                Console.ResetColor();
            }

            // add room to rooms
            rooms.Add(room);

            // clear waiting list
            playerConnectionIds.Clear();

            await GetPlayerNamesAndPositionsAtTheGroup(userName);
        }

    }
    //==================================================================================================================================================================
    class PlayerNameAndPosition
    {
        public required List<string> PlayerNames { get; set; }
    }

    public async Task GetPlayerNamesAndPositionsAtTheGroup(string userName)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\nGetPlayerNamesAndPositionsAtTheGroup()");
        Console.ResetColor();

        // find the room of the player sender
        Room room = rooms.First(x => x.Players!.Any(x => x.UserName == userName));

        // get the player index
        int playerIndex = room.Players!.ToList().FindIndex(x => x.UserName == userName);

        // create a new PlayerNameAndPosition object
        PlayerNameAndPosition playerNameAndPosition = new()
        {
            PlayerNames = new List<string>()
        };

        // send player names and positions to all players in the group
        for (int i = 0; i < room.Players!.Count; i++)
        {
            playerNameAndPosition.PlayerNames.Add(room.Players[i].UserName);
        }

        string response = JsonSerializer.Serialize(playerNameAndPosition)!;

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\nResponse: " + response);
        Console.ResetColor();

        await Clients.Caller.SendAsync("ReceivePlayerNamesAndPositionsAtTheGroup", response);
    }
    //==================================================================================================================================================================
    string response = "";
    public async Task SendMessage(string userNameOfSender, string? userNameOfReceiver, string? groupId, string message)
    {
        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.WriteLine("\nRequest: " + message);
        Console.ResetColor();

        string? receiverConnectionId = null;

        bool playedGetPieceFromLeft = false;
        string leftPiece = "";
        bool sorting = false;

        // if receiver is not specified, send message to all players in the group
        if (userNameOfReceiver == null)
        {
            // find the room of the player sender
            Room room = rooms.First(x => x.Players.Any(x => x.UserName == userNameOfSender));

            // get the player index
            int playerIndex = room.Players.ToList().FindIndex(x => x.UserName == userNameOfSender);

            if (groupId == null) // playing a move
            {
                // if it is turn of the player continue otherwise return
                if (userNameOfSender != room.Players[room.turnOfPlayerIndex / 2].UserName)
                {
                    response = "It is not your turn!";
                    await Clients.Caller.SendAsync("ReceiveMessage", "server", response);

                    return;
                }
                else if (message[..2] == "06")
                {
                    // sort series
                    sorting = true;
                }
                else if (message[..2] == "07")
                {
                    // sort pairs
                    sorting = true;
                }
                else // it is turn of the player
                {
                    // if it is turn of the player, increase the turnOfPlayerIndex. every player will play 2 times in a round
                    if (room.turnOfPlayerIndex % 2 == 0 && message[..2] != "00" && message[..2] != "01"
                        && room.Players[room.turnOfPlayerIndex / 2].Pieces.Count != 22) // user is throwing or opening a piece and he has 21 pieces
                    {
                        response = "First you should get a piece from the left or middle!";
                        await Clients.Caller.SendAsync("ReceiveMessage", "server", response);

                        return;
                    }
                    else if (room.Players[room.turnOfPlayerIndex / 2].Pieces.Count == 22 && room.turnOfPlayerIndex % 2 == 0)
                    {
                        if (message[..2] == "00" || message[..2] == "01")
                        {
                            response = "You should throw a piece to the right or you should open your deck!";
                            await Clients.Caller.SendAsync("ReceiveMessage", "server", response);

                            return;
                        }
                        else
                        {
                            // user is the first player in the round so he will play 1 times and throw a piece to the right
                            room.turnOfPlayerIndex++;
                        }
                    }
                    else if (room.turnOfPlayerIndex % 2 == 1 && (message[..2] == "00" || message[..2] == "01"))
                    {
                        response = "You have already got a piece from the left or middle!";
                        await Clients.Caller.SendAsync("ReceiveMessage", "server", response);

                        return;
                    }
                    else if (room.turnOfPlayerIndex % 2 == 1 && playedGetPieceFromLeft && (message[..2] != "02" || message[..2] != "03" || message[..2] != "04"))
                    {
                        response = "You have got a piece from the left so you should throw a piece to the left again or open your deck!";
                        await Clients.Caller.SendAsync("ReceiveMessage", "server", response);
                    }
                    else if (message[..2] == "00") // if he is getting a piece from the left he has to open his deck or throw the same piece to the left again
                    {
                        playedGetPieceFromLeft = true;
                        leftPiece = room.CornersL[room.turnOfPlayerIndex / 2].Peek()!;
                    }

                    room.turnOfPlayerIndex++;

                    if (room.turnOfPlayerIndex >= room.Players.Count * 2)
                    {
                        room.turnOfPlayerIndex = 0;
                    }

                    if (message[..2] == "05")
                    {
                        message = "05" + leftPiece;
                        room.turnOfPlayerIndex--;
                    }
                }

                response = Game.Instance.Play(room: room, message: message, playerIndex: playerIndex);

                GameLoopResponse result = JsonSerializer.Deserialize<GameLoopResponse>(response)!;

                result.IsTurnOfPlayer = false;

                // if it is the turn of the player set it true
                if (room.Players[room.turnOfPlayerIndex / 2].UserName == userNameOfSender)
                {
                    result.IsTurnOfPlayer = true;
                }

                response = JsonSerializer.Serialize(result)!;

                if (message[..2] != "01")
                {
                    result.MiddlePiece = null;
                }

                if (sorting) // send the result only the caller
                {
                    await Clients.Caller.SendAsync("ReceiveMessage", "server", response);
                    return;
                }
                else // send result to all players in the group one by one with their own pieces
                {
                    for (int i = 0; i < room.Players.Count; i++)
                    {
                        result.PlayerPieces = room.Players[i].Pieces;

                        response = JsonSerializer.Serialize(result)!;

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\nResponse: " + response);
                        Console.ResetColor();

                        await Clients.Client(room.Players[i].ConnectionId).SendAsync("ReceiveMessage", "server", response);
                    }
                }

                return;

            }
            await Clients.Group(groupId).SendAsync("ReceiveMessage", userNameOfSender, message);
        }
        else
        {
            // get connection Id of the receiver
            receiverConnectionId = playerConnectionIds.FirstOrDefault(x => x.Value == userNameOfReceiver).Key;

            // send message to the specified player
            await Clients.User(receiverConnectionId).SendAsync("ReceiveMessage", userNameOfSender, message);
        }

        // save message to database
        var chatMessage = new ChatMessage
        {
            Date = DateTime.Now.ToUniversalTime(),
            Sender = userNameOfSender,
            Receiver = receiverConnectionId,
            GroupId = groupId,
            Message = message
        };

        _context!.ChatMessages!.Add(chatMessage);
        await _context.SaveChangesAsync();
    }
    //==================================================================================================================================================================
    public async Task OpenRequest(string userNameOfSender, bool isSeriesOrPairs, List<List<string>> validGroups)
    {
        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.WriteLine(JsonSerializer.Serialize(validGroups));
        Console.ResetColor();

        // find the room of the player sender
        Room room = rooms.First(x => x.Players.Any(x => x.UserName == userNameOfSender));

        // get the player index
        int playerIndex = room.Players.ToList().FindIndex(x => x.UserName == userNameOfSender);

        // if it is turn of the player continue otherwise return
        if (userNameOfSender != room.Players[room.turnOfPlayerIndex / 2].UserName)
        {
            response = "It is not your turn!";
            await Clients.Caller.SendAsync("ReceiveMessage", "server", response);

            return;
        }

        string moveIndex = isSeriesOrPairs ? "04" : "03";

        string resultString = Game.Instance.Play(room: room, message: moveIndex, playerIndex: playerIndex, listOfValidGroups: validGroups);
        GameLoopResponse result = JsonSerializer.Deserialize<GameLoopResponse>(resultString)!;

        result.IsTurnOfPlayer = false;

        // if it is the turn of the player set it true
        if (room.Players[room.turnOfPlayerIndex / 2].UserName == userNameOfSender)
        {
            result.IsTurnOfPlayer = true;
        }

        response = JsonSerializer.Serialize(result)!;

        // send result to all players in the group one by one with their own pieces
        for (int i = 0; i < room.Players.Count; i++)
        {
            result.PlayerPieces = room.Players[i].Pieces;

            response = JsonSerializer.Serialize(result)!;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nResponse: " + response);
            Console.ResetColor();

            await Clients.Client(room.Players[i].ConnectionId).SendAsync("ReceiveMessage", "server", response);
        }

        return;
    }



}
