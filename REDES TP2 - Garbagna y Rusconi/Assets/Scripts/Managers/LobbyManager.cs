using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] Text textField;

    void Start()
    {
        Invoke("ChangeText", 2);   
    }


    void ChangeText()
    {
        if (GameServer.Instance.GameStart == false)
            textField.text = "Waiting for more players...";
        else
            textField.text = "Connecting...";
    }
}
