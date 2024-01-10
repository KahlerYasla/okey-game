using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DroppableItem : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        StatusPiece statusPiece = GetComponent<StatusPiece>();
        StatusPiece.Status status = statusPiece.status;

        if (eventData.pointerDrag != null && eventData.pointerDrag.GetComponent<StatusPiece>().status == StatusPiece.Status.onWoodGrid) // switch on wood
        {
            Vector2 originalPosition = eventData.pointerDrag.GetComponent<DraggableItem>().originalPosition;
            eventData.pointerDrag.GetComponent<RectTransform>().transform.position = GetComponent<RectTransform>().transform.position;
            GetComponent<RectTransform>().transform.position = originalPosition;
        }
        else if (eventData.pointerDrag.GetComponent<StatusPiece>().status != StatusPiece.Status.onWoodGrid)
        {
            //SetPieces.SlidePiecesAndSetTheNewPieceOnWoodGrid16x2(gameObject, eventData.pointerDrag.GetComponent<RectTransform>().transform.position);

            // change the status of the item
            eventData.pointerDrag.GetComponent<StatusPiece>().status = StatusPiece.Status.onWoodGrid;
        }

        Debug.Log("OnDropItem");
    }
}
