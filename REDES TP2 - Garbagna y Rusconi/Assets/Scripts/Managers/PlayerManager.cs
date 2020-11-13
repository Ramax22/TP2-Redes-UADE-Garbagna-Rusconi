using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    [SerializeField] int team = 0;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] bool spawned = false;

    private void Start()
    {
        if(PhotonNetwork.LocalPlayer!=GameServer.Instance.Server)
        { 
            if(Instance==null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }

            Invoke("SetUp", 3);
        }
    }

    void SetUp()
    {
        team = GameServer.Instance.PlayerTeam;
        GameServer.Instance.SpawnRequest(PhotonNetwork.LocalPlayer);
    }

}
