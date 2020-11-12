using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.Demo.Cockpit;
using UnityEngine.SceneManagement;

public class GameServer : MonoBehaviourPun
{
    public static GameServer Instance;
    public string prefabAdress = "Prefabs/PlayerPrefabs/PlayerPrefab"; 
    [SerializeField] Player _server;
    [SerializeField] int playerCount;

    Dictionary<Player, PlayerScript> _dic = new Dictionary<Player, PlayerScript>();
    Dictionary<PlayerScript, Player> _dicInverse = new Dictionary<PlayerScript, Player>();

    bool gameStart = false;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

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
            photonView.RPC("PlayerConnected", _server, currentClient);
        }
    }

    [PunRPC]
    void SpawnPlayer(Player client)
    {
        if(client!=_server)
        { 
            GameObject obj = PhotonNetwork.Instantiate(prefabAdress, Vector3.zero, Quaternion.identity);
            PlayerScript playerS = obj.GetComponent<PlayerScript>();
            if(playerS)
            { 
                _dic[client] = playerS;
                _dicInverse[playerS] = client;
            }
        }
    }

    [PunRPC]
    void PlayerConnected(Player p)
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount - 1 >= 4 && gameStart==false)
        {
            gameStart = true;
            photonView.RPC("EnterGameScene", RpcTarget.AllBuffered);
        }
        
            
    }

    [PunRPC]
    void EnterGameScene()
    {
        if (SceneManager.GetActiveScene().name != "GameScene")
        {
            SceneManager.LoadScene("GameScene");
        }
    }
}
