using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public static class Delegates
{
    public delegate string Move(string? piece, string? position, Room room, int playerIndex, List<List<string>> listOfValidGroups);

}
