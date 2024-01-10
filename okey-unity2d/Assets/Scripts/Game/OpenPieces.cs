using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using UnityEngine;

// singleton class
public class OpenPieces : MonoBehaviour
{
    public static TMPro.TextMeshProUGUI middleGridValueTagText, rightGridValueTagText;
    private static List<GameObject> wood16x2;
    public static List<List<GameObject>> listOfValidGroups = new();
    private static List<List<string>> middleGrid;
    private static List<List<string>> rightGrid;

    // the game objects that are on the middle grid and right grid 
    private static GameObject middleGridGameObject, rightGridGameObject;
    private Vector2 startingPositionOnMiddleGrid, startingPositionOnRightGrid;
    private readonly float columnWidth = 46.28f;
    private readonly int rowHeight = 57;
    private static Vector2[] dropPointsCoordinatesMiddleGrid, dropPointsCoordinatesRightGrid;
    readonly private GameObject[] dropPointsMiddleGrid = new GameObject[338];
    readonly private GameObject[] dropPointsRightGrid = new GameObject[78];


    private void Start()
    {
        wood16x2 = Room.Instance.Wood16x2;

        middleGrid = Room.Instance.MiddleGrid;
        rightGrid = Room.Instance.RightGrid;

        middleGridValueTagText = GameObject.Find("MiddleGridValueTagText").GetComponent<TMPro.TextMeshProUGUI>();
        rightGridValueTagText = GameObject.Find("RightGridValueTagText").GetComponent<TMPro.TextMeshProUGUI>();

        if (middleGridValueTagText == null || rightGridValueTagText == null)
        {
            Debug.LogError("middleGridValueTagText or rightGridValueTagText is null");
        }

        middleGridGameObject = GameObject.Find("MiddleGrid");
        rightGridGameObject = GameObject.Find("RightGrid");

        startingPositionOnMiddleGrid = new Vector2(-587, 350);
        startingPositionOnRightGrid = new Vector2(-115, 357);

        dropPointsCoordinatesMiddleGrid = new Vector2[338];
        dropPointsCoordinatesRightGrid = new Vector2[78];

        // middle grid has 26 rows and 13 columns ---------------------------------------------------------------------------------------------
        for (int i = 0; i < 169; i++)
        {
            dropPointsCoordinatesMiddleGrid[i] = startingPositionOnMiddleGrid + new Vector2((i % 13) * columnWidth, -(i / 13) * rowHeight);

            // create the drop point
            GameObject image = new("dropPointMiddleGrid" + i);
            image.transform.SetParent(middleGridGameObject.transform);
            image.AddComponent<RectTransform>();
            image.GetComponent<RectTransform>().localPosition = dropPointsCoordinatesMiddleGrid[i];
            image.GetComponent<RectTransform>().sizeDelta = new Vector2(10, 10);

            dropPointsMiddleGrid[i] = image;
        }
        for (int i = 169; i < 338; i++)
        {
            dropPointsCoordinatesMiddleGrid[i] = startingPositionOnMiddleGrid + new Vector2((i % 13) * columnWidth + 13 * columnWidth, -((i - 169) / 13) * rowHeight);

            // create the drop point
            GameObject image = new("dropPointMiddleGrid" + i);
            image.transform.SetParent(middleGridGameObject.transform);
            image.AddComponent<RectTransform>();
            image.GetComponent<RectTransform>().localPosition = dropPointsCoordinatesMiddleGrid[i];
            image.GetComponent<RectTransform>().sizeDelta = new Vector2(10, 10);

            dropPointsMiddleGrid[i] = image;
        }

        // right grid has 13 rows and 6 columns ---------------------------------------------------------------------------------------------
        for (int i = 0; i < 78; i++)
        {
            dropPointsCoordinatesRightGrid[i] = startingPositionOnRightGrid + new Vector2((i % 2) * columnWidth, -(((i / 2)) % 13) * rowHeight);

            // create the drop point
            GameObject image = new("dropPointRightGrid" + i);
            image.transform.SetParent(rightGridGameObject.transform);
            image.AddComponent<RectTransform>();
            image.GetComponent<RectTransform>().localPosition = dropPointsCoordinatesRightGrid[i];
            image.GetComponent<RectTransform>().sizeDelta = new Vector2(10, 10);

            dropPointsRightGrid[i] = image;
        }
    }
    //==============================================================================================================
    public static void SetPiecesFromLatestData(bool isMiddleNotRight)
    {
        Connection connection = Connection.GetInstance();
        GameData latestData = connection.latestData;
        var ColorCodesToHexCodeDictionary = Dictionaries.ColorCodesToHexCodeDictionary;

        if (isMiddleNotRight) // if it is middle grid
        {
            Debug.Log("latestData.MiddleGrid: " + JsonSerializer.Serialize(latestData.MiddleGrid));

            // set the pieces on the middle grid
            for (int i = 0; i < 26; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    if (latestData.MiddleGrid[i][j].Length != 3)
                    {
                        continue;
                    }

                    string piece = latestData.MiddleGrid[i][j];

                    Color pieceColor = ColorCodesToHexCodeDictionary[piece[0].ToString()];

                    int pieceNumber = int.Parse(piece[1..]);

                    // instantiate the piece from the prefab
                    GameObject pieceObject = Instantiate(Resources.Load<GameObject>("Prefabs/PiecePrefab"));

                    // set the piece name
                    pieceObject.name = piece;

                    // get the children heart and number and set the color of the heart and number of the piece
                    GameObject heart = pieceObject.transform.GetChild(1).gameObject;
                    GameObject number = pieceObject.transform.GetChild(0).gameObject;
                    GameObject jokerIcon = pieceObject.transform.GetChild(2).gameObject;

                    heart.GetComponent<UnityEngine.UI.Image>().color = pieceColor;

                    // text mesh pro
                    number.GetComponent<TMPro.TextMeshProUGUI>().text = pieceNumber.ToString();

                    // set text color
                    number.GetComponent<TMPro.TextMeshProUGUI>().color = pieceColor;

                    if (latestData.MiddleGrid[i][j] == latestData.OkeyPiece) // if it is okey piece remove the number and heart
                    {
                        heart.SetActive(false);
                        number.SetActive(false);
                    }
                    else if (pieceNumber == 0) // if it is joker piece remove the number and heart
                    {
                        number.SetActive(false);
                        jokerIcon.SetActive(true);
                        heart.SetActive(false);
                    }


                    // set the piece position on the middle grid
                    pieceObject.transform.SetParent(middleGridGameObject.transform);

                    // set scale to 0.5
                    pieceObject.transform.localScale = new Vector3(0.415f, 0.415f, 0.415f);

                    // set the piece position on the middle grid
                    pieceObject.transform.localPosition = dropPointsCoordinatesMiddleGrid[i * 13 + j];

                    // set unclickable
                    pieceObject.GetComponent<CanvasGroup>().blocksRaycasts = false;

                    // add the piece to the middle grid
                    middleGrid[i][j] = piece;
                }
            }
        }
        else // if it is right grid
        {
            Debug.Log("latestData.RightGrid: " + JsonSerializer.Serialize(latestData.RightGrid));

            // set the pieces on the right grid
            for (int i = 0; i < 39; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    if (latestData.RightGrid[i][j].Length != 3)
                    {
                        continue;
                    }

                    string piece = latestData.RightGrid[i][j];

                    Color pieceColor = ColorCodesToHexCodeDictionary[piece[0].ToString()];

                    int pieceNumber = int.Parse(piece[1..]);

                    // instantiate the piece from the prefab
                    GameObject pieceObject = Instantiate(Resources.Load<GameObject>("Prefabs/PiecePrefab"));

                    // set the piece name
                    pieceObject.name = piece;

                    // get the children heart and number and set the color of the heart and number of the piece
                    GameObject heart = pieceObject.transform.GetChild(1).gameObject;
                    GameObject number = pieceObject.transform.GetChild(0).gameObject;
                    GameObject jokerIcon = pieceObject.transform.GetChild(2).gameObject;

                    heart.GetComponent<UnityEngine.UI.Image>().color = pieceColor;

                    // text mesh pro
                    number.GetComponent<TMPro.TextMeshProUGUI>().text = pieceNumber.ToString();

                    // set text color
                    number.GetComponent<TMPro.TextMeshProUGUI>().color = pieceColor;

                    if (latestData.RightGrid[i][j] == latestData.OkeyPiece) // if it is okey piece remove the number and heart
                    {
                        heart.SetActive(false);
                        number.SetActive(false);
                    }
                    else if (pieceNumber == 0) // if it is joker piece remove the number and heart
                    {
                        number.SetActive(false);
                        jokerIcon.SetActive(true);
                        heart.SetActive(false);
                    }

                    // set the piece position on the right grid
                    pieceObject.transform.SetParent(rightGridGameObject.transform);

                    // set scale to 0.5
                    pieceObject.transform.localScale = new Vector3(0.415f, 0.415f, 0.415f);

                    // set the piece position on the right grid
                    pieceObject.transform.localPosition = dropPointsCoordinatesRightGrid[i * 2 + j];

                    // set unclickable
                    pieceObject.GetComponent<CanvasGroup>().blocksRaycasts = false;

                    // add the piece to the right grid
                    rightGrid[i][j] = piece;
                }

            }
        }
    }
    //==============================================================================================================
    public static void SendTheOpenRequest(bool isSeriesNotPairs)
    {
        Connection connection = Connection.GetInstance();

        List<List<string>> namesOfValidGroups = listOfValidGroups.Select(group => group.Select(piece => piece.name).ToList()).ToList();

        // eliminate 4 pieces groups or 2 pieces groups according to the isSeriesNotPairs
        if (isSeriesNotPairs)
        {
            namesOfValidGroups = namesOfValidGroups.Where(group => group.Count >= 3).ToList();
        }
        else
        {
            namesOfValidGroups = namesOfValidGroups.Where(group => group.Count == 2).ToList();
        }

        // eliminate the same groups make it 1
        for (int i = 0; i < namesOfValidGroups.Count; i++)
        {
            for (int j = i + 1; j < namesOfValidGroups.Count; j++)
            {
                if (namesOfValidGroups[i].SequenceEqual(namesOfValidGroups[j]))
                {
                    namesOfValidGroups.RemoveAt(j);
                    j--;
                }
            }
        }

        // send the open request
        connection.SendOpenRequest(isSeriesNotPairs, namesOfValidGroups);
    }
}