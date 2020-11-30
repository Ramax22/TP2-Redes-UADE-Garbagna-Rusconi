using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Connection : MonoBehaviourPunCallbacks
{
    MainMenuManager _menuManager;

    private void Awake()
    {
        _menuManager = GameObject.Find("MainMenuManager").GetComponent<MainMenuManager>();
    }

    #region ~~~ CREATE USER ~~~
    public void RequestCreateUser(string user, string pass)
    {
        photonView.RPC("RecibeCreateUser", GameServer.Instance.Server, user, pass, photonView.Owner);
    }

    [PunRPC]
    void RecibeCreateUser(string user, string pass, Player p)
    {
        _menuManager.CreateUser(user, pass, p, this);
    }

    public void ResponseCreateUser(string response, Player p)
    {
        photonView.RPC("RecibeResponseCreateUser", p, response);
    }

    [PunRPC]
    void RecibeResponseCreateUser(string response)
    {
        if (response.Contains("Success"))
            Debug.LogError("REGISTER OK - " + response);
        else
            Debug.LogError("REGISTER FAILS - " + response);
    }
    #endregion

    #region ~~~ FIND USER ~~~
    public void RequestFindUser(string user, string pass)
    {
        photonView.RPC("RecibeFindUser", GameServer.Instance.Server, user, pass, photonView.Owner);
    }

    [PunRPC]
    void RecibeFindUser(string user, string pass, Player p)
    {
        _menuManager.LogIn(user, pass, p, this);
    }

    public void ResponseFindUser(string user, string response, Player p)
    {
        if (GameServer.Instance.CheckIfPlayerAlreadyLogged(user)) response = "User already logged";

        if (response == "Server/Success") { GameServer.Instance.AddPlayerAndCheckGameForStart(p, user); }

        photonView.RPC("RecibeResponseFindUser", p, response, user);
    }

    [PunRPC]
    void RecibeResponseFindUser(string response, string user)
    {
        if (response == "Server/Success")
        {
            _menuManager.ChangeToloadingScreen();
            PhotonNetwork.LocalPlayer.NickName = user;
        }
        else Debug.LogError("LOGIN FAILED - " + response);
    }
    #endregion
}