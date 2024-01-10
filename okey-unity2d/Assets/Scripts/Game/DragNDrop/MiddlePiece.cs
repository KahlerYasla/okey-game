using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MiddlePiece : MonoBehaviour, IPointerDownHandler
{
    private static Connection connection;

    private void Start()
    {
        connection = Connection.GetInstance();

        // set status to onMiddleStackTop
        gameObject.GetComponent<StatusPiece>().status = StatusPiece.Status.onMiddleStackTop;

        // set the piece's parent to the Pieces game object
        gameObject.transform.SetParent(GameObject.Find("Pieces").transform);

        // set scale to 1
        gameObject.transform.localScale = new Vector3(1, 1, 1);

        // set the piece's position
        gameObject.transform.localPosition = new Vector3(450, 334.267f, 0);

        // set the piece's name
        gameObject.name = "MiddlePiece";

        Room.Instance.MiddleStack.Push(gameObject);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GameObject piece = Room.Instance.MiddleStack.Pop();

        // renew the middle piece
        GameObject newMiddlePiece = Instantiate(Resources.Load<GameObject>("Prefabs/PiecePrefab"));
        newMiddlePiece.AddComponent<MiddlePiece>();

        // set the newMiddlePiece the first child
        newMiddlePiece.transform.SetAsFirstSibling();

        // set the piece's name
        piece.name = connection.latestData.MiddlePiece;

        // get the children heart and number and set the color of the heart and number of the piece
        GameObject heart = piece.transform.GetChild(1).gameObject;
        GameObject number = piece.transform.GetChild(0).gameObject;
        GameObject jokerIcon = piece.transform.GetChild(2).gameObject;

        // get the piece color
        Color pieceColor = Dictionaries.ColorCodesToHexCodeDictionary[piece.name[0].ToString()];
        int pieceNumber = int.Parse(piece.name[1..]);

        heart.GetComponent<UnityEngine.UI.Image>().color = pieceColor;

        // text mesh pro
        number.GetComponent<TMPro.TextMeshProUGUI>().text = pieceNumber.ToString();

        // set text color
        number.GetComponent<TMPro.TextMeshProUGUI>().color = pieceColor;

        if (connection.latestData.MiddlePiece == connection.latestData.OkeyPiece) // if it is okey piece remove the number and heart
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

    }
}