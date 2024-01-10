using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackButton : MonoBehaviour
{
    public GameObject loginPanel, registerPanel;

    public void Back()
    {
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
    }
}
