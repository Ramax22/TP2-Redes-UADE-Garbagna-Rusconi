﻿using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/*
    https://web.microsoftstream.com/video/5c917b19-cc6e-48b1-ad37-11c43560a635
    time. 02:26:00
 */

public class MainMenuManager : MonoBehaviour
{ //Maximo largo de user y contraseña: 15 caracteres
    //Las "pantallas" del menu
    [SerializeField] GameObject _mainScreen;
    [SerializeField] GameObject _loginScreen;
    [SerializeField] GameObject _registerScreen;

    //Objetos de la pantalla Login
    [SerializeField] InputField _usernameFieldLoginScreen;
    [SerializeField] InputField _passwordFieldLoginScreen;

    //Objetos de la pantalla Register
    [SerializeField] InputField _usernameFieldRegisterScreen;
    [SerializeField] InputField _passwordFieldRegisterScreen;

    //Variables
    string _urlCreateUser;
    string _urlFindUser;

    private void Awake()
    {
        //Establezco el orden de pantallas
        ChangeToMainScreen();

        //Seteo las URL
        _urlCreateUser = "http://localhost/tp2_redes_garbagna_rusconi/createUser.php";
        _urlFindUser = "http://localhost/tp2_redes_garbagna_rusconi/findUser.php";
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

    #region ~~~ FUNCIONES GENERICAS ~~~
    //Funcion que verifica si hay errores de conexion en la DB
    void ConnectionErrorHandling(UnityWebRequest request)
    {
        //Si no tiene internet
        if (request.isNetworkError)
        {
            Debug.Log("ERROR: Can't reach server");
            return;
        }

        //Si no encontró la página
        if (request.isHttpError)
        {
            Debug.Log("ERROR: Server not found");
            return;
        }
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

        //Reviso que no haya errores
        ConnectionErrorHandling(request);

        //Muestro en consola lo que devuelve la consulta
        Debug.Log(request.downloadHandler.text);
    }

    //Funcion que crea al usuario en la db
    public void CreateUser()
    {
        //TODO: REALIZAR VERIFICACIONES DESDE FRONTEND

        //Ejecuto la corutina 
        StartCoroutine(CreateUserRequest(_usernameFieldRegisterScreen.text, _passwordFieldRegisterScreen.text, _urlCreateUser));

        //TODO: QUE SE LOGUE SOLO
    }
    #endregion

    #region ~~~ FUNCIONES DEL PANEL REGISTER ~~~
    //Funcion para mandar el request al login y trabajar con el response
    IEnumerator LogInRequest(string username, string password, string url)
    {
        //Creo el formulario
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        //Mando el request
        UnityWebRequest request = UnityWebRequest.Post(url, form);

        //Espero al response
        yield return request.SendWebRequest();

        //Reviso si no hay errores de conexion
        ConnectionErrorHandling(request);

        Debug.Log(request.downloadHandler.text);
    }

    //Funcion para loguearse
    public void LogIn()
    {
        //TODO: FRONTEND CHECK

        StartCoroutine(LogInRequest(_usernameFieldLoginScreen.text, _passwordFieldLoginScreen.text, _urlFindUser));
    }
    #endregion
}