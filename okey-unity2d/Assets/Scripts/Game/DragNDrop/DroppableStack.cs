using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DroppableStack : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData) // droppable stack is the right stack
    {
        if (eventData.pointerDrag != null)
        {
            eventData.pointerDrag.GetComponent<RectTransform>().transform.position = GetComponent<RectTransform>().transform.position;

            // change the status of the item
            eventData.pointerDrag.GetComponent<StatusPiece>().status = StatusPiece.Status.onRightStack;
        }

        Debug.Log("OnDropStack");
    }
}
