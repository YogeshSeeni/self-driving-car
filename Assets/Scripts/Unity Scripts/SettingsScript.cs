using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SettingsScript : MonoBehaviour
{
    public TextMeshProUGUI ipText;
    public TextMeshProUGUI portText;
    public TMP_InputField ipInputField;
    public TMP_InputField portInputField;

    private void Start()
    {
        if (PlayerPrefs.GetString("IP", "none") != "none")
        {
            ipInputField.text = PlayerPrefs.GetString("IP");
        }

        if (PlayerPrefs.GetInt("Port", 0) != 0)
        {
            portInputField.text = PlayerPrefs.GetInt("Port").ToString();
        }
    }
    public void SaveIPAddress()
    {
        PlayerPrefs.SetString("IP", ipText.text.Substring(0, ipText.text.Length - 1));
    }

    public void SavePort()
    {
        PlayerPrefs.SetInt("Port", Convert.ToInt32(portText.text.Substring(0, portText.text.Length - 1)));
    }

    public void Back()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 3);
    }
}
