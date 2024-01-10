using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableItem : MonoBehaviour
{
    public void DestroyItem()
    {
        Destroy(gameObject);
    }
}
