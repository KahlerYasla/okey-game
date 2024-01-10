using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SlidingCards : MonoBehaviour
{
    public GameObject content;
    private readonly float interval = 1638.4f;
    private readonly float startPositionLeft = 9828;
    readonly GameObject[] circles = new GameObject[6];
    public GameObject circlesParent;
    private readonly float[] centerPoints = new float[6];
    private int currentCardIndex = 0;


    public void Start()
    {
        centerPoints[0] = startPositionLeft;

        for (int i = 1; i < 6; i++)
        {
            centerPoints[i] = centerPoints[i - 1] - interval;
        }

        SlideTheCardToCenterSmoothly();
    }


    public void SlideTheCardToCenterSmoothly(string direction = "")
    {
        RectTransform contentRectTransform = content.GetComponent<RectTransform>();
        float contentLeftPosition = contentRectTransform.localPosition.x;

        // if the direction is left and the current card is not the last card
        if (direction == "left" && currentCardIndex < 5)
        {
            currentCardIndex++;
            // SMOOTHLY slide the card to the center
            StartCoroutine(TheMove(contentRectTransform, contentLeftPosition, centerPoints[currentCardIndex], 0.5f));
        }

        // if the direction is right and the current card is not the first card
        if (direction == "right" && currentCardIndex > 0)
        {
            currentCardIndex--;
            // SMOOTHLY slide the card to the center
            StartCoroutine(TheMove(contentRectTransform, contentLeftPosition, centerPoints[currentCardIndex], 0.5f));
        }

        // if the direction is not specified
        if (direction == "")
        {
            // slide the card to the center
            contentRectTransform.localPosition = new Vector3(centerPoints[currentCardIndex], 0, 0);
        }


        // create the circles
        for (int i = 0; i < 6; i++)
        {
            GameObject circle = new("circle" + i);
            circle.AddComponent<RectTransform>();
            circle.transform.SetParent(circlesParent.transform);
            circle.GetComponent<RectTransform>().localPosition = new Vector3(-100 + i * 40, 0, 0);
            circle.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);
            circle.AddComponent<UnityEngine.UI.Image>();
            circle.GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Sprites/Circle");
            circle.GetComponent<UnityEngine.UI.Image>().color = Color.white;

            circles[i] = circle;
        }

        // set the current card's circle color to white
        circles[currentCardIndex].GetComponent<UnityEngine.UI.Image>().color = Color.yellow;

    }

    IEnumerator TheMove(RectTransform rectTransform, float startPosition, float endPosition, float time)
    {
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            rectTransform.localPosition = new Vector3(Mathf.Lerp(startPosition, endPosition, (elapsedTime / time)), 0, 0);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rectTransform.localPosition = new Vector3(endPosition, 0, 0);
    }

    private bool sliding = false;
    private Vector2 touchStartPosition, touchEndPosition;
    public void Update()
    {

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // if touch phase is started on the content set sliding true
            if (touch.phase == TouchPhase.Began && RectTransformUtility.RectangleContainsScreenPoint(content.GetComponent<RectTransform>(), touch.position))
            {
                sliding = true;
                touchStartPosition = touch.position;
            }

            // if touch phase is ended set sliding false
            if (touch.phase == TouchPhase.Ended)
            {
                touchEndPosition = touch.position;

                if (sliding)
                {
                    SlideTheCardToCenterSmoothly(direction: touchStartPosition.x - touchEndPosition.x > 0 ? "left" : "right");
                }

                sliding = false;
            }
        }
    }
}
