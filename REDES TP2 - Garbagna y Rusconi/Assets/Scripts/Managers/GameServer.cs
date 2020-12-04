using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class GameServer : MonoBehaviourPun
{
    //Variables
    [SerializeField] Player _server;
    [SerializeField] int playersNeeded;
    [SerializeField] bool gameStart = false;
    [SerializeField] float _matchMaxTime;
    public static GameServer Instance;
    public string prefabAdress = "Prefabs/PlayerPrefabs/PlayerPrefab"; 
    Dictionary<Player, PlayerScript> _dic = new Dictionary<Player, PlayerScript>();
    Dictionary<PlayerScript, Player> _dicInverse = new Dictionary<PlayerScript, Player>();
    Dictionary<Player, int> _scoreDic;
    int _myScore;
    float _matchTime;
    bool _startCountingTime;
    bool _corutineAlreadyCalled = false;

    public List<Player> loggedPlayers;
    public List<string> _loggedUsernames;

    private void Awake()
    {
        _loggedUsernames = new List<string>();
        DontDestroyOnLoad(this.gameObject);
        if (playersNeeded == 0) playersNeeded = 4;
    }

    void Start()
    {
        if(Instance == null)
        {
            Instance = this;
            if (PhotonNetwork.IsMasterClient)
            {
                loggedPlayers = new List<Player>();
                _scoreDic = new Dictionary<Player, int>();
                Player currentClient = PhotonNetwork.LocalPlayer;
                photonView.RPC("SetServer", RpcTarget.AllBuffered, currentClient);
                _matchTime = 0;
                _startCountingTime = false;
            }
            else
            {
                _myScore = 0;
            }
        }
    }

    private void Update()
    {
        if (_startCountingTime) MatchDuration();
    }

    #region ~~~ ACTION SCRIPTS ~~~
    //Funcion que le avisa al server que un usuario disparo
    public void Shoot(int _view, bool hasQuadDamage) { photonView.RPC("RpcShoot", _server, _view, hasQuadDamage); }

    //Disparo por parte del server
    [PunRPC]
    void RpcShoot(int client, bool hasQuadDamage)
    {
        var whoShoot = PhotonView.Find(client); //Agarro el Player que disparó
        var sender = whoShoot.transform; //Agarro el transform del Player que disparo

        //Disparo un Raycast a ver si el usuario acertó algo
        if (Physics.Raycast(sender.position, sender.forward, out RaycastHit hit, 60))
        {
            Debug.DrawLine(sender.position, hit.point, Color.green, 0.5f);

            //intento agarrar el script, si tiene, significa que le acertó a un jugador
            var playerScript = hit.collider.GetComponent<PlayerScript>();
            if (playerScript != null)
            {
                //Agarro el Player del Owner
                var owner = hit.collider.GetComponent<PhotonView>().Owner;
                var damage = 5; //dejo el base damage
                if (hasQuadDamage) damage *= 4; //Si tiene quaddamage, se multiplica el daño por 4

                //Le digo al jugador acertado que fue dañado
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
        _myScore = newScore;
        HUDManager.Instance.ChangeKillsText(newScore);
    }

    public void SendScore()
    {
        if (PhotonNetwork.LocalPlayer != _server) return;

        //Investigo el Dict, y agrego a una lista al ganador o ganadores
        Player winner = null;
        int nKills = 0;
        List<Player> _numberOfWinners = new List<Player>();
        foreach (var p in _scoreDic)
        {
            if (winner == null) { 
                winner = p.Key;
                nKills = p.Value;
                _numberOfWinners.Add(p.Key);
                continue;
            }

            if (p.Value > nKills)
            {
                _numberOfWinners.Clear();
                winner = p.Key;
                nKills = p.Value;
                _numberOfWinners.Add(winner);
            } else if (p.Value == nKills)
            {
                _numberOfWinners.Add(p.Key);
            }
        }

        //Saco a los ganadores del diccionario de puntos
        foreach (var gameWinner in _numberOfWinners)
        {
            _scoreDic.Remove(gameWinner);
        }

        //Envio a los perdedores sus estadisticas
        foreach (var item in _scoreDic)
        {
            photonView.RPC("RecibeScore", item.Key, 0, item.Value);
        }

        //Chequeo si hubo empate o si gano alguien
        if (_numberOfWinners.Count > 1)
        {
            //Aca significa que hubo empate
            foreach (var tieGuy in _numberOfWinners)
            {
                photonView.RPC("RecibeScore", tieGuy, 1, nKills);
            }
        } else
        {
            //Aca significa que gano 1 solo
            photonView.RPC("RecibeScore", winner, 2, nKills);
        }
    }

    [PunRPC]
    void RecibeScore(int gameStatus, int kills) //status: 0 - Lose || 1 - Tie || 2 - Win
    {
        ScoreManager score = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        score.SetWinLoseText(gameStatus);
        score.SetKillCount(kills);
    }
    #endregion

    #region ~~~ SPAWN PLAYERS ~~~
    //Solicita spawnear
    public void SpawnRequest(Player p) { photonView.RPC("SpawnPlayer", _server, p); }

    //Esto se llama cuando el jugador quiere respawnear despues de morir
    public void RequestRespawn() { StartCoroutine(RespawnWait()); }

    //Funcion que espera 3 segundos, y solicita respawnear al jugador
    IEnumerator RespawnWait()
    {
        yield return new WaitForSeconds(3);
        SpawnRequest(PhotonNetwork.LocalPlayer);
    }

    //Funcion desde el server que spawnea un jugador y le pasa la propiedad al cliente
    [PunRPC]
    void SpawnPlayer(Player client)
    {
        if (client != _server)
        {
            if (SpawnsManager.Instance.Counter >= SpawnsManager.Instance.SpawnPoints.Count)
                SpawnsManager.Instance.Counter = 0;

            GameObject spawnPos = SpawnsManager.Instance.SpawnPoints[SpawnsManager.Instance.Counter];
            SpawnsManager.Instance.Counter++;

            GameObject obj = PhotonNetwork.Instantiate(prefabAdress, spawnPos.transform.position, spawnPos.transform.rotation);
            //obj.GetPhotonView().TransferOwnership(client);
            PlayerScript playerS = obj.GetComponent<PlayerScript>();
            if (playerS)
            {
                _dic[client] = playerS; //aca guarda al cliente y su script player de su gameobject
                _dicInverse[playerS] = client; //aca lo mismo pero al revez
            }
            playerS.photonView.RPC("SetSpawned", client);
            playerS.photonView.RPC("ActivateCamera", client);
        }
    }

    [PunRPC]
    void PlayerConnected(Player p) { _scoreDic.Add(p, 0); } //Agrego al diccionario del server al jugador y sus kills 

    public void AddPlayerAndCheckGameForStart(Player p, string username)
    {
        loggedPlayers.Add(p);
        _loggedUsernames.Add(username);
        if (loggedPlayers.Count >= playersNeeded && gameStart == false && !_corutineAlreadyCalled)
        {
            StartCoroutine(WaitSecondToStartGame());
            _corutineAlreadyCalled = true;
        }
    }

    public bool CheckIfPlayerAlreadyLogged(string user)
    {
        return _loggedUsernames.Contains(user);
    }

    IEnumerator WaitSecondToStartGame()
    {
        yield return new WaitForSeconds(10);
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player == Server) continue;

            if (!loggedPlayers.Contains(player)) PhotonNetwork.CloseConnection(player);
        }
        photonView.RPC("InitializeGame", RpcTarget.AllBuffered);
    }
    #endregion

    #region ~~~ GAME FUNCTIONS ~~~
    //Funcion que inicializa el juego
    [PunRPC]
    void InitializeGame()
    {
        gameStart = true;
        PhotonNetwork.LoadLevel("GameScene");
        _startCountingTime = true;
    }
    
    //Funcion que hace que un cliente (el masterclient) se haga server
    [PunRPC]
    void SetServer(Player s)
    {
        _server = s;
        Player currentClient = PhotonNetwork.LocalPlayer;
        if (_server != currentClient) photonView.RPC("PlayerConnected", _server, currentClient);
    }

    //Funcion que controla la duración de la partida
    void MatchDuration()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        _matchTime += Time.deltaTime;
        if (_matchTime >= _matchMaxTime)
        {
            photonView.RPC("LoadResultScreen", RpcTarget.AllBuffered);
            _startCountingTime = false;
        }
    }

    [PunRPC]
    void LoadResultScreen()
    {
        PhotonNetwork.LoadLevel("ResultScene");
    }
    #endregion

    #region ~~~ MOVEMENT REQUEST ~~~

    [PunRPC]
    public void RequestMovement(Player client, Vector3 mov)
    {
        if(_dic[client])
        _dic[client].Move(mov);
    }

    [PunRPC]
    public void RequestShoot(Player client)
    {
        if (_dic[client])
            _dic[client].Shoot();
    }

    [PunRPC]
    public void RequestAim(Player client, float vertical, float horizontal)
    {
        if (_dic[client])
            _dic[client].Aim(vertical, horizontal);
    }

    #endregion

    #region ~~~ CAMERA REQUEST ~~~

    [PunRPC]
    public void ActivateCamera(PlayerScript p)
    {
        p.ActivateCamera();
    }

    [PunRPC]
    public void CheckCamera(PlayerScript script, Player client)
    {
        if(_dic[client] == script)
        {
            script.photonView.RPC("ActivateCamera", client);
        }
    }

    

    #endregion

    public bool GameStart { get => gameStart; }
    public Player Server { get => _server; }
    public Dictionary<Player, PlayerScript> Dic { get => _dic; set => _dic = value; }
    public Dictionary<PlayerScript, Player> DicInverse { get => _dicInverse; set => _dicInverse = value; }
}