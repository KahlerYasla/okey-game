using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using UnityEngine;

// singleton class
public class Room
{
    private static Room instance;
    public static Room Instance
    {
        get
        {
            instance ??= new Room();

            return instance;
        }
    }

    public int MyIndexInGroup { get; set; }
    public List<string> PlayerNames { get; set; }
    public List<GameObject> Wood16x2 { get; set; }
    public Stack<GameObject> MiddleStack { get; set; }
    public List<List<string>> MiddleGrid { get; set; }
    public List<List<string>> RightGrid { get; set; }
    public Stack<GameObject>[] CornersL { get; set; }
    public string OkeyPiece { get; set; }

    // get the data from the server with socket
    private Room()
    {
        Wood16x2 = new();
        OkeyPiece = "";
        MiddleStack = new();
        MiddleGrid = new();
        RightGrid = new();
        CornersL = new Stack<GameObject>[4];

        for (int i = 0; i < 32; i++) // wood grid
        {
            Wood16x2.Add(new GameObject("Empty"));
        }

        for (int i = 0; i < 26; i++) // middle grid
        {
            // create 13 empty lists all elements are "12"
            MiddleGrid.Add(new List<string>(Enumerable.Repeat("", 13)));
        }

        for (int i = 0; i < 39; i++) // right grid
        {
            RightGrid.Add(new List<string>(Enumerable.Repeat("", 2)));
        }

    }

    public void ClearWood16x2()
    {
        // destroy the game objects
        for (int i = 0; i < 32; i++)
        {
            GameObject toBeDestroyed = Instance.Wood16x2[i];

            Instance.Wood16x2[i] = new GameObject("Empty");

            toBeDestroyed.name = "Destroyed";
            GameObject.Destroy(toBeDestroyed);
        }
    }
}

