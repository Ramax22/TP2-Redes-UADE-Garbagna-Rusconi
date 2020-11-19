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

    public void ResponseFindUser(string response, Player p)
    {
        if (response == "Server/Success")
        {
            Debug.LogError("SUCCESS");
            GameServer.Instance.loggedPlayers.Add(p);
        }

        photonView.RPC("RecibeResponseFindUser", p, response);
    }

    [PunRPC]
    void RecibeResponseFindUser(string response)
    {
        if (response == "Server/Success") Debug.LogError("ta bien");
        else Debug.LogError("LOGIN FAILED - " + response);
    }
    #endregion
}