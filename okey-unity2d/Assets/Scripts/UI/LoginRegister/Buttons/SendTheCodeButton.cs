using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendTheCodeButton : MonoBehaviour
{
    public GameObject resetPwdPanel, verificationCodePanel, mailText;

    public void SendTheCode()
    {
        string mail = mailText.GetComponent<TMPro.TMP_InputField>().text;
        // TODO: send the code to the mail via API


        if (mail.Equals("a"))
        {
            Debug.Log("Code sent");
            resetPwdPanel.SetActive(false);
            verificationCodePanel.SetActive(true);
        }
        else
        {
            Debug.Log("mail format is wrong");
        }

    }
}
