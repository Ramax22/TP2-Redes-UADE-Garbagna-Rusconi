using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class GameServer : MonoBehaviourPun
{
    public static GameServer Instance;
    public string prefabAdress = "Prefabs/PlayerPrefabs/PlayerPrefab"; 
    [SerializeField] Player _server;
    [SerializeField] int playersNeeded;

    Dictionary<Player, PlayerScript> _dic = new Dictionary<Player, PlayerScript>();
    Dictionary<PlayerScript, Player> _dicInverse = new Dictionary<PlayerScript, Player>();

    [SerializeField] bool gameStart = false;

    Dictionary<Player, int> _scoreDic;
    int _myScore;

    //Team Vars
    /*Dictionary<Player, int> _dicTeam = new Dictionary<Player, int>();
    List<Player> listTeamOne = new List<Player>();
    List<Player> listTeamTwo = new List<Player>();
    int playerTeam = 0;*/

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (playersNeeded == 0)
            playersNeeded = 4;
    }

    void Start()
    {
        if(Instance==null)
        {
            Instance = this;
            if(PhotonNetwork.IsMasterClient)
            {
                _scoreDic = new Dictionary<Player, int>();
                Player currentClient = PhotonNetwork.LocalPlayer;
                //_server = currentClient;
                //Debug.Log(_server);
                photonView.RPC("SetServer", RpcTarget.AllBuffered, currentClient);
            }
            else
            {
                _myScore = 0;
            }
        }
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
    void PlayerConnected(Player p)
    {
        Debug.Log("User " + p + " connected");
        /*if(listTeamOne.Count<= listTeamTwo.Count)
        { 
            _dicTeam.Add(p, 1);
            listTeamOne.Add(p);
        }
        else
        { 
            _dicTeam.Add(p, 2);
            listTeamTwo.Add(p);
        }

        photonView.RPC("SetTeam", p,_dicTeam[p]);*/

        _scoreDic.Add(p, 0);

        if (PhotonNetwork.CurrentRoom.PlayerCount - 1 >= playersNeeded && gameStart==false)
        {
            //gameStart = true;
            //PhotonNetwork.LoadLevel("GameScene");
            photonView.RPC("InitializeGame", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void InitializeGame()
    {
        gameStart = true;
        PhotonNetwork.LoadLevel("GameScene");
    }

    /*[PunRPC]
    void SetTeam(int team)
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            playerTeam = team;
        }
    }*/

    #region ~~~ ACTION SCRIPTS ~~~
    public void Shoot(int _view, bool hasQuadDamage)
    {
        photonView.RPC("RpcShoot", _server, _view, hasQuadDamage);
    }

    [PunRPC]
    void RpcShoot(int client, bool hasQuadDamage)
    {
        var whoShoot = PhotonView.Find(client);
        var sender = whoShoot.transform;
        var foward = sender.forward + sender.forward;
        //if (Physics.Raycast(sender.position + sender.forward, foward, out RaycastHit hit, 60))
        if (Physics.Raycast(sender.position, sender.forward, out RaycastHit hit, 60))
        {
            Debug.DrawLine(sender.position, hit.point, Color.green, 0.5f);

            var playerScript = hit.collider.GetComponent<PlayerScript>();
            if (playerScript != null)
            {
                var owner = hit.collider.GetComponent<PhotonView>().Owner;
                var damage = 5;
                if (hasQuadDamage) damage *= 4;

                playerScript.CallChangeLife(damage, owner, whoShoot.Owner);
            }
        }
    }
    #endregion

    #region ~~~ SCORE FUNCTIONS ~~~
    public void AddScore(Player p)
    {
        photonView.RPC("RpcAddScore", _server, p);
    }

    [PunRPC]
    void RpcAddScore(Player p)
    {
        _scoreDic[p]++;
        photonView.RPC("AddScoreLocal", p, _scoreDic[p]);
    }

    [PunRPC]
    void AddScoreLocal(int newScore)
    {
        Debug.LogError("My new score is " + newScore);
    }
    #endregion

    //Player Spawn

    public void SpawnRequest(Player p)
    {
        photonView.RPC("SpawnPlayer", _server, p);
    }

    public void RequestRespawn()
    {
        StartCoroutine(RespawnWait());
    }

    IEnumerator RespawnWait()
    {
        yield return new WaitForSeconds(3);
        SpawnRequest(PhotonNetwork.LocalPlayer);
    }

    [PunRPC]
    void SpawnPlayer(Player client)
    {
        if (client != _server)
        {
            GameObject spawnPos;
            //Aca, dependiendo del equipo, lo instancia en un lado u otro
            /*if(_dicTeam[client]==1)
            {
                //Aca se resetea el ponter el vector3 donde se instancia a los jugadores
                if (SpawnsManager.Instance.CounterOne >= SpawnsManager.Instance.TeamOneSpawns.Count)
                    SpawnsManager.Instance.CounterOne = 0;

                spawnPos = SpawnsManager.Instance.TeamOneSpawns[SpawnsManager.Instance.CounterOne];
                SpawnsManager.Instance.CounterOne++;
            }
            else
            {
                //Aca se resetea el ponter el vector3 donde se instancia a los jugadores
                if (SpawnsManager.Instance.CounterTwo >= SpawnsManager.Instance.TeamTwoSpawns.Count)
                    SpawnsManager.Instance.CounterTwo = 0;

                spawnPos = SpawnsManager.Instance.TeamTwoSpawns[SpawnsManager.Instance.CounterTwo];
                SpawnsManager.Instance.CounterTwo++;
            }*/
            if (SpawnsManager.Instance.Counter >= SpawnsManager.Instance.SpawnPoints.Count)
                SpawnsManager.Instance.Counter = 0;

            spawnPos = SpawnsManager.Instance.SpawnPoints[SpawnsManager.Instance.Counter];
            SpawnsManager.Instance.Counter++;


            GameObject obj = PhotonNetwork.Instantiate(prefabAdress, spawnPos.transform.position, spawnPos.transform.rotation);
            obj.GetPhotonView().TransferOwnership(client);
            PlayerScript playerS = obj.GetComponent<PlayerScript>();
            if (playerS)
            {
                _dic[client] = playerS; //aca guarda al cliente y su script player de su gameobject
                _dicInverse[playerS] = client; //aca lo mismo pero al revez
            }
        }
    }

    public bool GameStart { get => gameStart; }
    //public Dictionary<Player, int> DicTeam { get => _dicTeam; }
    //public int PlayerTeam { get => playerTeam; }
    public Player Server { get => _server; set => _server = value; }
}
