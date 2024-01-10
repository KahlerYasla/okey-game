using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusPiece : MonoBehaviour
{
    public enum Status { onWoodGrid, onCornerStack, onRightStack, onLeftStack, onMiddleStack, onMiddleStackTop, onLeftStackTop };
    public Status status = Status.onWoodGrid;
}
