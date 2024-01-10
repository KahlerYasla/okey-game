using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// handles mobile drag and drop
public class DraggableItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private CanvasGroup canvasGroup;
    public Vector2 originalPosition;
    private TagCalculations tagCalculations;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        tagCalculations = GameObject.Find("TagCalculations").GetComponent<TagCalculations>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        var status = GetComponent<StatusPiece>().status;

        if (status != StatusPiece.Status.onWoodGrid && status != StatusPiece.Status.onMiddleStackTop && status != StatusPiece.Status.onLeftStackTop)
        {
            return;
        }

        transform.position = Input.mousePosition;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        var status = GetComponent<StatusPiece>().status;

        // make the item front of all other items
        transform.SetAsLastSibling();

        canvasGroup.alpha = .6f;
        canvasGroup.blocksRaycasts = false;

        originalPosition = transform.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        tagCalculations.SetWoodDeckValueTagText();

        StatusPiece statusPieceTarget = eventData.pointerDrag.GetComponent<StatusPiece>();
        StatusPiece.Status statusTarget = statusPieceTarget.status;

        StatusPiece statusPiece = GetComponent<StatusPiece>();
        StatusPiece.Status status = statusPiece.status;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (eventData.pointerEnter == null)
        {
            Debug.Log("Dropped on nothing (null)");
        }
        // if the item is dropped to the space then return its original position
        if (eventData.pointerEnter.GetComponent<DroppableItem>() == null
        && eventData.pointerEnter.GetComponent<DroppableWood>() == null
        && eventData.pointerEnter.GetComponent<DroppableStack>() == null)
        {
            Debug.Log("Dropped on nothing");
            transform.position = originalPosition;
        }

        if (status != StatusPiece.Status.onWoodGrid && status != StatusPiece.Status.onMiddleStackTop && status != StatusPiece.Status.onLeftStackTop)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
}
