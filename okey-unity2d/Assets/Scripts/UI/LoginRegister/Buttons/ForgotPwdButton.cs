using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForgotPwdButton : MonoBehaviour
{
    public GameObject loginPanel, forgotPwdPanel;

    public void ForgotPwd()
    {
        loginPanel.SetActive(false);
        forgotPwdPanel.SetActive(true);
    }
}
