using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterButton : MonoBehaviour
{
    public GameObject registerPanel, loginPanel;

    public void Register()
    {
        registerPanel.SetActive(true);
        loginPanel.SetActive(false);
    }
}
