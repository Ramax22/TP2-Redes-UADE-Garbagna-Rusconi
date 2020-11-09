﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.Demo.Cockpit;

public class GameServer : MonoBehaviourPun
{
    public static GameServer Instance;
    public string prefabAdress = "Prefabs/PlayerPrefabs/PlayerPrefab"; 
    [SerializeField] Player _server;
    [SerializeField] int playerCount;

    Dictionary<Player, PlayerScript> _dic = new Dictionary<Player, PlayerScript>();

    void Start()
    {
        if(Instance==null)
        {
            Instance = this;
            if(PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("SetServer", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer);
            }
        }
    }

    void Update()
    {
        
    }

    [PunRPC]
    void SetServer(Player s)
    {
        _server = s;
        Player currentClient = PhotonNetwork.LocalPlayer;
        if(_server != currentClient)
        {
            photonView.RPC("PlayerConnected", _server);
            //SpawnPlayer(currentClient);
        }
    }

    [PunRPC]
    void SpawnPlayer(Player client)
    {
        GameObject obj = PhotonNetwork.Instantiate(prefabAdress, Vector3.zero, Quaternion.identity);
        PlayerScript playerS = obj.GetComponent<PlayerScript>();
        if(playerS)
            _dic[client] = playerS;
    }

    [PunRPC]
    void PlayerConnected()
    {
        playerCount++;
    }
}
