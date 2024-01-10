using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using UnityEngine;

public class SetPieces : MonoBehaviour
{
    private static Connection connection;
    public static GameObject woodGrid16x2;
    public static GameObject showPiece;
    public static List<GameObject> cornerStacks = new();

    void Start()
    {
        woodGrid16x2 = GameObject.Find("Pieces");
        showPiece = GameObject.Find("ShowPiece");

        for (int i = 0; i < 4; i++)
        {
            cornerStacks.Add(GameObject.Find("CornerStack" + i));
        }

        connection = Connection.GetInstance();

        // call setPiecesOnWoodGrid16x2() when the latestData is not null wait for 0.1 second and call it again until it is not null
        // this coroutine is for the first setup then it is not updating the pieces
        StartCoroutine(WaitForLatestData());
    }

    public IEnumerator WaitForLatestData()
    {
        while (connection.latestData == null)
        {
            yield return new WaitForSeconds(0.15f);
        }

        // set the okey show piece color and number
        string okeyPieceColorCode = connection.latestData.OkeyPiece[0].ToString();

        string okeyPieceNumber = connection.latestData.OkeyPiece[1..].ToString();
        okeyPieceNumber = (int.Parse(okeyPieceNumber) - 1).ToString();

        if (okeyPieceNumber == "0")
        {
            okeyPieceNumber = "13";
        }

        GameObject okeyPieceHeart = showPiece.transform.GetChild(1).gameObject;
        GameObject okeyPieceNumberText = showPiece.transform.GetChild(0).gameObject;

        okeyPieceHeart.GetComponent<UnityEngine.UI.Image>().color = Dictionaries.ColorCodesToHexCodeDictionary[okeyPieceColorCode];
        okeyPieceNumberText.GetComponent<TMPro.TextMeshProUGUI>().text = okeyPieceNumber;
        okeyPieceNumberText.GetComponent<TMPro.TextMeshProUGUI>().color = Dictionaries.ColorCodesToHexCodeDictionary[okeyPieceColorCode];
    }

    public static void SetPiecesOnWoodGrid16x2(bool sorting = false)
    {
        if (connection.latestData.MiddleGrid != null || connection.latestData.RightGrid != null)
        {
            bool isMiddleNotRight = connection.latestData.MiddleGrid != null;

            OpenPieces.SetPiecesFromLatestData(isMiddleNotRight);
        }

        List<string> names = Room.Instance.Wood16x2.ConvertAll(x => x.name);

        Room.Instance.ClearWood16x2();

        GameData latestData = connection.latestData;
        var ColorCodesToHexCodeDictionary = Dictionaries.ColorCodesToHexCodeDictionary;

        for (int i = 0; i < latestData.PlayerPieces.Count; i++)
        {

            if (latestData.PlayerPieces[i] == "000")
            {
                continue;
            }

            string piece = latestData.PlayerPieces[i];

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

            if (latestData.PlayerPieces[i] == latestData.OkeyPiece) // if it is okey piece remove the number and heart
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


            // set the piece position on the wood grid
            pieceObject.transform.SetParent(woodGrid16x2.transform);

            // set scale to 1
            pieceObject.transform.localScale = new Vector3(1, 1, 1);

            // set the piece position on the wood grid
            pieceObject.GetComponent<RectTransform>().localPosition = new Vector2(-790 + (i % 16) * 105.6f, 80 - (i / 16) * 157);

            Room.Instance.Wood16x2[i] = pieceObject;
        }

        Debug.Log("Room.Instance.Wood16x2 names after set pieces ~> " + JsonSerializer.Serialize(Room.Instance.Wood16x2.ConvertAll(x => x.name)));

        TagCalculations tagCalculations = GameObject.Find("TagCalculations").GetComponent<TagCalculations>();
        tagCalculations.SetWoodDeckValueTagText();

        SetCornerStacks();
    }

    public static void SetCornerStacks()
    {

    }
}
