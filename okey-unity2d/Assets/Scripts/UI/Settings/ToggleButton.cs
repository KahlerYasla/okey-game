using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleButton : MonoBehaviour
{
    private enum ToggleState
    {
        On,
        Off
    }

    private GameObject buttonHandle;
    private ToggleState toggleState;

    private void Awake()
    {
        buttonHandle = transform.GetChild(0).gameObject;
        toggleState = ToggleState.On;
        GetComponent<UnityEngine.UI.Image>().color = Color.green;
    }

    public void Toggle()
    {
        if (toggleState == ToggleState.Off)
        {
            buttonHandle.transform.localPosition = new Vector3(60, 0, 0);
            toggleState = ToggleState.On;
            GetComponent<UnityEngine.UI.Image>().color = Color.green;
        }
        else
        {
            buttonHandle.transform.localPosition = new Vector3(-60, 0, 0);
            toggleState = ToggleState.Off;
            GetComponent<UnityEngine.UI.Image>().color = Color.red;
        }
    }
}
