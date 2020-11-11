using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

public class MainMenuManager : MonoBehaviour
{ //Maximo largo de user y contraseña: 15 caracteres
    //Link: https://web.microsoftstream.com/video/5c917b19-cc6e-48b1-ad37-11c43560a635
    //Me quede en el 57:00
    //Las "pantallas" del menu
    [SerializeField] GameObject _mainScreen;
    [SerializeField] GameObject _loginScreen;
    [SerializeField] GameObject _registerScreen;

    private void Awake()
    {
        //Establezco el orden de pantallas
        ChangeToMainScreen();
    }

    #region ~~~ FUNCIONES DE CAMBIO DE PANELES ~~~
    //Ir a la pantalla pricipal
    public void ChangeToMainScreen()
    {
        _mainScreen.SetActive(true);
        _loginScreen.SetActive(false);
        _registerScreen.SetActive(false);
    }

    //Ir a la pantalla de Login
    public void ChangeToLoginScreen()
    {
        _mainScreen.SetActive(false);
        _loginScreen.SetActive(true);
        _registerScreen.SetActive(false);
    }

    //Ir a la pantalla de Register
    public void ChangeToRegisterScreen()
    {
        _mainScreen.SetActive(false);
        _loginScreen.SetActive(false);
        _registerScreen.SetActive(true);
    }
    #endregion

    #region ~~~ FUNCIONES DEL PANEL LOGIN ~~~
    //Funcion para crear un usuario
    IEnumerator CreateUserRequest(string username, string password, string url)
    {
        //Creo un formulario y agrego sus campos
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        //Hago un request
        UnityWebRequest request = UnityWebRequest.Post(url, form);

        //Espero a recibir el response
        yield return request.SendWebRequest();


    }

    void ConnectionErrorHandling(UnityWebRequest request)
    {
        //Si no tiene internet
        if (request.isNetworkError)
        {
            //Mostrar error
            return;
        }
    }
    #endregion
}