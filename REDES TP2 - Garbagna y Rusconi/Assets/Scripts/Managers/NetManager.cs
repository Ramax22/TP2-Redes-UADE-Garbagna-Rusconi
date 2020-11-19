using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class NetManager : MonoBehaviourPunCallbacks
{
    //[SerializeField] Text _username;

    public void SetUpGame() //En la clase 5 el flaco hace esta misma funcion, pero la llama "ConnectedToRoom" por si llegas a revisar el codigo de él
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        //Creo las options de la room para que sea de 4 jugadores
        RoomOptions rO = new RoomOptions();
        rO.MaxPlayers = 12;

        PhotonNetwork.LocalPlayer.NickName = "usuario" + Random.Range(0, 100).ToString();

        //Creo la room
        PhotonNetwork.JoinOrCreateRoom("GameRoom", rO, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        //Cargo la scene del juego, a esto hay que agregarle cosas de la clase 5
        //PhotonNetwork.LoadLevel("LoginScene");
        PhotonNetwork.LoadLevel("LobbyScene");
    }
}