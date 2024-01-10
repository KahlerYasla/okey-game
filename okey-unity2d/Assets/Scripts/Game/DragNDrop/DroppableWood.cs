using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DroppableWood : MonoBehaviour, IDropHandler
{
    private Vector2 startingPositionOnWoodGrid16x2;
    private readonly float columnWidth = 105.6f;
    private readonly int rowHeight = 157;
    private Vector2[] dropPointsCoordinates;
    private GameObject[] dropPoints = new GameObject[32];

    public GameObject[] DropPoints
    {
        get { return dropPoints; }
    }

    private void Awake()
    {
        dropPointsCoordinates = new Vector2[32];
        startingPositionOnWoodGrid16x2 = new Vector2(-790, 80);

        for (int i = 0; i < 32; i++)
        {
            dropPointsCoordinates[i] = startingPositionOnWoodGrid16x2 + new Vector2((i % 16) * columnWidth, -(i / 16) * rowHeight);

            // create the drop point
            GameObject image = new("dropPoint" + i);
            image.transform.SetParent(transform);
            image.AddComponent<RectTransform>();
            image.GetComponent<RectTransform>().localPosition = dropPointsCoordinates[i];
            image.GetComponent<RectTransform>().sizeDelta = new Vector2(10, 10);

            dropPoints[i] = image;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");

        // get the dropped item position
        Vector2 droppedItemPosition = eventData.pointerDrag.GetComponent<RectTransform>().transform.position;

        // get the closest drop point
        float minDistance = float.MaxValue;
        int minDistanceIndex = 0;

        for (int i = 0; i < 32; i++)
        {
            float distance = Vector2.Distance(droppedItemPosition, dropPoints[i].GetComponent<RectTransform>().transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                minDistanceIndex = i;
            }
        }

        // set the dropped item position to the closest drop point
        eventData.pointerDrag.GetComponent<RectTransform>().transform.position = dropPoints[minDistanceIndex].GetComponent<RectTransform>().transform.position;
        Debug.Log("Dropped item position: " + eventData.pointerDrag.GetComponent<RectTransform>().transform.position);
    }
}
