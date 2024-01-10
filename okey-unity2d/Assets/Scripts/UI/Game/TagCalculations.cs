using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using UnityEngine;

public class TagCalculations : MonoBehaviour
{
    public TMPro.TextMeshProUGUI woodDeckValueTagText;
    private List<GameObject> woodGrid;
    public int woodDeckValue = 0;

    public void SetWoodDeckValueTagText()
    {
        OpenPieces.listOfValidGroups = new();

        woodDeckValue = 0;

        Debug.Log("SetWoodDeckValueTagText()");

        woodGrid = Room.Instance.Wood16x2;

        List<List<GameObject>> dividedWoodGrid = new();
        List<GameObject> temp = new();
        for (int i = 0; i < woodGrid.Count; i++) // divide the wood grid into parts where dividing parts are null
        {
            if (woodGrid[i].name == "Empty" || i == woodGrid.Count - 1)
            {
                dividedWoodGrid.Add(temp);
                temp = new List<GameObject>();
                continue;
            }
            else
            {
                temp.Add(new GameObject(woodGrid[i].name));
            }
        }

        Debug.Log("dividedWoodGrid: " + JsonSerializer.Serialize(dividedWoodGrid.Select(x => x.Select(y => y.name).ToList()).ToList()));

        // if there any joker in the divided wood grid, convert it's name to the okey piece name
        int jokerCount = 0;
        for (int i = 0; i < dividedWoodGrid.Count; i++)
        {
            for (int j = 0; j < dividedWoodGrid[i].Count; j++)
            {
                if (dividedWoodGrid[i][j].name == "j00")
                {
                    dividedWoodGrid[i][j].name = Room.Instance.OkeyPiece;
                    jokerCount++;
                }
            }
        }

        int seriesCount = 0, pairsCount = 0;
        bool isSeriesNotPairs = true;
        // set the isSeriesOrPairs from valid elements of the divided wood grid
        for (int i = 0; i < dividedWoodGrid.Count; i++)
        {
            if (dividedWoodGrid[i].Count >= 3)
            {
                seriesCount++;
            }
            else if (dividedWoodGrid[i].Count == 2)
            {
                pairsCount++;

            }
        }
        isSeriesNotPairs = seriesCount > pairsCount;

        // if it is series calculate the wood deck value of the valid series that are pieces different from each other with the same color and ascending order or different color and same number
        // if it is pairs calculate the wood deck value of the valid pairs that are pieces same with each other with the same color 
        // if there is any Room.Instance.OkeyPiece in the dividedWoodGrid count it as a proper piece
        if (isSeriesNotPairs) // if it is series
        {
            bool isDifferentColorSeries = true;
            bool isSameColorSeries = true;

            foreach (List<GameObject> validGroup in dividedWoodGrid) // for each pieces in the divided wood grid
            {
                if (validGroup.Count < 3 || validGroup.Count > 4) // if the pieces count is less than 3 continue
                {
                    continue;
                }

                Debug.Log("valid group :" + JsonSerializer.Serialize(validGroup.Select(x => x.name).ToList()));

                for (int i = 0; i < validGroup.Count - 1; i++) // find the series is different color or same color
                {
                    if (
                    (
                    validGroup[i].name[0] != validGroup[i + 1].name[0] // check the colors if it is different
                    && validGroup[i].name[1..] == validGroup[i + 1].name[1..] // check the numbers if it is same
                    )
                    || validGroup[i].name == Room.Instance.OkeyPiece
                    || validGroup[i + 1].name == Room.Instance.OkeyPiece
                    && isDifferentColorSeries // if the previous pieces are different color series
                    ) // colors are different and the numbers are same
                    {
                        isDifferentColorSeries = true;
                    }
                    else
                    {
                        isDifferentColorSeries = false;
                    }

                    if (
                    (
                    validGroup[i].name[0] == validGroup[i + 1].name[0]
                    && int.Parse(validGroup[i].name[1..]) == (int.Parse(validGroup[i + 1].name[1..]) - 1)
                    )
                    || validGroup[i].name == Room.Instance.OkeyPiece
                    || validGroup[i + 1].name == Room.Instance.OkeyPiece
                    && isSameColorSeries // if the previous pieces are same color series
                    ) // colors are same and the numbers are ascending order
                    {
                        isSameColorSeries = true;
                    }
                    else
                    {
                        isSameColorSeries = false;
                    }

                }

                if (isDifferentColorSeries || isSameColorSeries)
                {
                    for (int i = 0; i < validGroup.Count; i++)
                    {
                        woodDeckValue += int.Parse(validGroup[i].name[1..]);
                    }
                    OpenPieces.listOfValidGroups.Add(validGroup);
                }

                isDifferentColorSeries = true;
                isSameColorSeries = true;
            }

            if (woodDeckValue < int.Parse(OpenPieces.middleGridValueTagText.text)) // you can not open the pieces if the wood deck value is less than the middle grid value
            {
                Debug.Log("the value :" + woodDeckValue + " is less than the middle grid value: " + OpenPieces.middleGridValueTagText.text);
                OpenPieces.listOfValidGroups = new();
            }
            else
            {
                Debug.Log("the value :" + woodDeckValue + " is greater than the middle grid value: " + OpenPieces.middleGridValueTagText.text);
            }

            Debug.Log(JsonSerializer.Serialize(OpenPieces.listOfValidGroups.Select(x => x.Select(y => y.name).ToList()).ToList()));
        }
        else // if it is pairs
        {
            foreach (List<GameObject> pieces in dividedWoodGrid)
            {
                if (pieces.Count == 2 && (pieces[0].name[0] == pieces[1].name[0] // or one of them is okey piece
                || pieces[0].name == Room.Instance.OkeyPiece
                || pieces[1].name == Room.Instance.OkeyPiece))
                {
                    OpenPieces.listOfValidGroups.Add(pieces);
                    woodDeckValue += 1;
                }
            }

            if (woodDeckValue < int.Parse(OpenPieces.rightGridValueTagText.text)) // you can not open the pieces if the wood deck value is less than the middle grid value
            {
                Debug.Log("the value :" + woodDeckValue + " is less than the right grid value: " + OpenPieces.rightGridValueTagText.text);
                OpenPieces.listOfValidGroups = new();
            }
            else
            {
                Debug.Log("the value :" + woodDeckValue + " is greater than the right grid value: " + OpenPieces.rightGridValueTagText.text);
            }

        }

        // if there is any joker convert it back to the joker
        for (int i = 0; i < dividedWoodGrid.Count; i++)
        {
            for (int j = 0; j < dividedWoodGrid[i].Count; j++)
            {
                if (dividedWoodGrid[i][j].name == Room.Instance.OkeyPiece && jokerCount > 0)
                {
                    dividedWoodGrid[i][j].name = "j00";
                    jokerCount--;
                }
            }
        }

        woodDeckValueTagText.text = woodDeckValue.ToString();

        SetLeftAndRightValuesTagsTexts();
    }


    public TMPro.TextMeshProUGUI leftGridValueTagText, rightGridValueTagText;
    public void SetLeftAndRightValuesTagsTexts()
    {
        // Sum of the numbers of the pieces in the middle grid
        leftGridValueTagText.text = Mathf.Max(Room.Instance.MiddleGrid.Sum(x => x.Sum(y => int.Parse(getNumberOfPiece(y)))), 101).ToString();

        static string getNumberOfPiece(string y)
        {
            try
            {
                return y[1..];
            }
            catch (System.Exception)
            {
                return "0";
            }
        }

        // Count of the lists that are not empty
        rightGridValueTagText.text = Mathf.Max(Room.Instance.RightGrid.Sum(x => x.Any(y => y != "") ? 1 : 0), 5).ToString();
    }
}
