using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class LoginButton : MonoBehaviour
{
    public GameObject emailText, passwordText;

    // if the password and email are both "regant" then login
    public void Login()
    {
        string passMail = "a";
        string email = emailText.GetComponent<TMP_InputField>().text;
        string password = passwordText.GetComponent<TMP_InputField>().text;

        email = email.ToString();

        if (email.Equals(passMail))
        {
            // login
            Debug.Log("Login");
            SceneChanger.ChangeScene("Home");
        }
        else
        {
            Debug.Log("Wrong email or password " + "email:\"" + email + "\" password:\"" + password + "\"");
        }
    }
}
