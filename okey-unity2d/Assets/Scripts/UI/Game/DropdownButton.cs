using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropdownButton : MonoBehaviour
{
    public GameObject dropdownContent;

    public void OnClick()
    {
        dropdownContent.SetActive(!dropdownContent.activeSelf);
    }
}
