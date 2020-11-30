using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviourPunCallbacks
{
    //Maximo largo de user y contraseña: 15 caracteres

    //Las "pantallas" del menu
    [Header("Menu screens")]
    [SerializeField] GameObject _mainScreen;
    [SerializeField] GameObject _loginScreen;
    [SerializeField] GameObject _registerScreen;
    [SerializeField] GameObject _waitingScreen;

    //Objetos de la pantalla Login
    [Header("Login Screen Objects")]
    [SerializeField] InputField _usernameFieldLoginScreen;
    [SerializeField] InputField _passwordFieldLoginScreen;

    //Objetos de la pantalla Register
    [Header("Register Screen Objects")]
    [SerializeField] InputField _usernameFieldRegisterScreen;
    [SerializeField] InputField _passwordFieldRegisterScreen;

    //Variables
    string _urlCreateUser;
    string _urlFindUser;
    bool _isLogged;
    string _conn = "Prefabs/Connection";
    Connection _connObj;

    private void Awake()
    {
        //
        if (PhotonNetwork.IsMasterClient)
        {
            _isLogged = true;
        }
        else
        {
            //Establezco el orden de pantallas
            ChangeToMainScreen();

            //Seteo las URL
            _urlCreateUser = "http://localhost/redes/createUser.php";
            _urlFindUser = "http://localhost/redes/findUser.php";

            //
            _isLogged = false;

            //Instancio un elemento para hacer la conexion con el Server
            _connObj = PhotonNetwork.Instantiate(_conn, Vector3.zero, new Quaternion()).GetComponent<Connection>();
        }
    }

    #region ~~~ FUNCIONES DE CAMBIO DE PANELES ~~~
    //Ir a la pantalla pricipal
    public void ChangeToMainScreen()
    {
        _mainScreen.SetActive(true);
        _loginScreen.SetActive(false);
        _registerScreen.SetActive(false);
        _waitingScreen.SetActive(false);
    }

    //Ir a la pantalla de Login
    public void ChangeToLoginScreen()
    {
        _mainScreen.SetActive(false);
        _loginScreen.SetActive(true);
        _registerScreen.SetActive(false);
        _waitingScreen.SetActive(false);
    }

    //Ir a la pantalla de Register
    public void ChangeToRegisterScreen()
    {
        _mainScreen.SetActive(false);
        _loginScreen.SetActive(false);
        _registerScreen.SetActive(true);
        _waitingScreen.SetActive(false);
    }

    public void ChangeToloadingScreen()
    {
        _mainScreen.SetActive(false);
        _loginScreen.SetActive(false);
        _registerScreen.SetActive(false);
        _waitingScreen.SetActive(true);
    }
    #endregion

    #region ~~~ FUNCIONES GENERICAS ~~~
    //Funcion que verifica si hay errores de conexion en la DB
    void ConnectionErrorHandling(UnityWebRequest request)
    {
        //Si no tiene internet
        if (request.isNetworkError)
        {
            Debug.LogError("ERROR: Can't reach server");
            Debug.LogError(request.error);
            return;
        }

        //Si no encontró la página
        if (request.isHttpError)
        {
            Debug.LogError("ERROR: Server not found");
            return;
        }
    }
    #endregion

    #region ~~~ FUNCIONES DEL PANEL REGISTER ~~~
    //Funcion para crear un usuario
    IEnumerator CreateUserRequest(string username, string password, Player p, Connection conn)
    {
        //Creo un formulario y agrego sus campos
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        //Hago un request
        UnityWebRequest request = UnityWebRequest.Post("http://localhost/redes/createUser.php", form);

        //Espero a recibir el response
        yield return request.SendWebRequest();

        //Reviso que no haya errores
        ConnectionErrorHandling(request);

        //Agarro el response, y voy al éxito o al error
        var res = request.downloadHandler.text;

        RpcOnRegisterFinish(res, p, conn);
    }

    //Funcion para enviar la cosnulta al server
    public void RequestCreateUser()
    {
        _connObj.RequestCreateUser(_usernameFieldRegisterScreen.text, _passwordFieldRegisterScreen.text);
    }

    //Funcion que crea al usuario en la db
    public void CreateUser(string user, string pass, Player p, Connection connObj)
    {
        //TODO: REALIZAR VERIFICACIONES DESDE FRONTEND

        //Ejecuto la corutina 
        StartCoroutine(CreateUserRequest(user, pass, p, connObj));
    }

    void RpcOnRegisterFinish(string response, Player p, Connection conn)
    {
        conn.ResponseCreateUser(response, p);
    }
    #endregion

    #region ~~~ FUNCIONES DEL PANEL LOGIN ~~~
    //Funcion para mandar el request al login y trabajar con el response
    IEnumerator LogInRequest(string username, string password, Player p, Connection conn)
    {
        //Creo el formulario
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        //Mando el request
        UnityWebRequest request = UnityWebRequest.Post("http://localhost/redes/findUser.php", form);

        //Espero al response
        yield return request.SendWebRequest();

        //Reviso si no hay errores de conexion
        ConnectionErrorHandling(request);

        //Agarro el response, y voy al éxito o al error
        var res = request.downloadHandler.text;

        RpcOnLoginFinish(res, p, conn, username);
    }

    void RpcOnLoginFinish(string response, Player p, Connection conn, string user)
    {
        conn.ResponseFindUser(user, response, p);
    }

    public void RequestFindUser()
    {
        _connObj.RequestFindUser(_usernameFieldLoginScreen.text, _passwordFieldLoginScreen.text);
    }

    //Funcion para loguearse
    public void LogIn(string user, string pass, Player P, Connection conn)
    {
        //TODO: FRONTEND CHECK
        StartCoroutine(LogInRequest(user, pass, P, conn));
    }
    #endregion
}