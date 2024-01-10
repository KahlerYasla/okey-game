using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerificationConfirmButton : MonoBehaviour
{
    public GameObject verificationCodePanel, newPasswordPanel, verificationCodeText;

    public void Confirm()
    {
        string verificationCode = verificationCodeText.GetComponent<TMPro.TMP_InputField>().text;

        if (verificationCode.Equals("1234"))
        {
            Debug.Log("Code is correct");
            verificationCodePanel.SetActive(false);
            newPasswordPanel.SetActive(true);
        }
        else
        {
            Debug.Log("Code is wrong");
        }
    }
}
