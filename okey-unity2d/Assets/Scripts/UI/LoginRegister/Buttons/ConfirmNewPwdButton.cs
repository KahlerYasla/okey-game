using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmNewPwdButton : MonoBehaviour
{
    public GameObject newPasswordPanel, loginPanel, newPasswordText, newPasswordAgainText;

    public void Confirm()
    {
        string newPassword = newPasswordText.GetComponent<TMPro.TMP_InputField>().text;
        string newPasswordAgain = newPasswordAgainText.GetComponent<TMPro.TMP_InputField>().text;

        if (newPassword.Equals(newPasswordAgain))
        {
            Debug.Log("Password changed");
            newPasswordPanel.SetActive(false);
            loginPanel.SetActive(true);
        }
        else
        {
            Debug.Log("Passwords are not the same");
        }
    }
}
