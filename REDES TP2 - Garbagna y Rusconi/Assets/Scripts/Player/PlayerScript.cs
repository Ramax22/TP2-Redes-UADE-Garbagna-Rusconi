using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviourPun
{
    [Header("Generic Values")]
    [SerializeField] float speed;
    [SerializeField] float initialhealth;
    [SerializeField] int initialAmmo;
    [SerializeField] GameObject _myCamera;
    [SerializeField] bool _quadDamage;
    [SerializeField] GameObject aimingPoint;
    [SerializeField] GameObject model;
    [SerializeField] ModelAnimation _modelAnim;
    
    bool isDead;
    CharacterController cc;
    Vector3 mousePosition;
    int ammoCount; 
    Health hp;
    Player _lastHitBy;
    bool isMovingVertical, isMovingHorizonal;
    Player _controlledBy;

    public CharacterController Cc { get => cc; set => cc = value; }
    public Player ControlledBy { get => _controlledBy; set => _controlledBy = value; }

    // movement vars
    Dictionary<string, bool> _moveInstructions;

    const string _moveForward = "up";
    const string _moveBackward = "down";
    const string _moveLeft = "left";
    const string _moveRight = "right";

    private void Start()
    {
        ammoCount = initialAmmo;

        cc = GetComponent<CharacterController>();
        hp = new Health(initialhealth, Die);
        isDead = false;

        if (photonView.IsMine)
        {
            _moveInstructions = new Dictionary<string, bool>();
            _moveInstructions.Add(_moveForward, false);
            _moveInstructions.Add(_moveBackward, false);
            _moveInstructions.Add(_moveLeft, false);
            _moveInstructions.Add(_moveRight, false);
        }
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        MoveEntity();
    }

    #region ~~~ HP FUNCTIONS ~~~
    [PunRPC]
    public void ChangeLife(float value, Player whoShoot)
    {
        _lastHitBy = whoShoot;
        hp.ChangeLife(value);
        photonView.RPC("UpdateHealth", _controlledBy, hp.HP);
    }

    [PunRPC]
    void UpdateHealth(float hp) { HUDManager.Instance.ChangeHPText((int)hp); }

    public void CallChangeLife(float value, Player owner, Player whoShoot)
    {
        photonView.RPC("ChangeLife", owner, value, whoShoot);
    }

    public void CallGetHP(Player owner)
    {
        photonView.RPC("GetHP", owner);
    }

    [PunRPC]
    public void GetHP()
    {
        hp.MaxLife();
        photonView.RPC("UpdateHealth", _controlledBy, hp.HP);
        //HUDManager.Instance.ChangeHPText((int)hp.HP);
    }

    void Die() //Vamos a tener que encontrar una manera de hacer bien la muerte
    {
        if (!PhotonNetwork.IsMasterClient) return;
        isDead = true;
        GameServer.Instance.RequestRespawn(_controlledBy);

        GameServer.Instance.RpcAddScore(_lastHitBy, _controlledBy);
        //HUDManager.Instance.ClearTexts();
        GameServer.Instance.ClearHUD(_controlledBy);
        PhotonNetwork.Destroy(gameObject);
    }
    #endregion

    #region ~~~ WEAPONS FUNCTIONS ~~~
    public void Shoot(Player p)
    {
        if (ammoCount > 0)
        {
            ammoCount--;
            photonView.RPC("UpdateAmmo", p, ammoCount);
            GameServer.Instance.NewShoot(_quadDamage, aimingPoint.transform, p);
            _modelAnim.SetFireBool();
        }
    }

    [PunRPC]
    void UpdateAmmo(int ammo) { HUDManager.Instance.ChangeAmmoText(ammo); }

    public void CallGetAmmo(Player owner, int value) { photonView.RPC("GetAmmo", owner, value); }

    public void GetAmmo(int value)
    {
        ammoCount += value;
        photonView.RPC("UpdateAmmo", _controlledBy, ammoCount);
    }
    #endregion

    #region ~~~ QUAD DAMAGE FUNCTIONS ~~~
    public void CallGetQuad(Player owner) { photonView.RPC("GetQuad", owner); }

    public void GetQuad()
    {
        _quadDamage = true;
        //HUDManager.Instance.EnableQuad();
        photonView.RPC("UpdateQuadMsg", _controlledBy, true);

        StartCoroutine(QuadDamageDuration());
    }

    [PunRPC]
    void UpdateQuadMsg(bool activate)
    {
        if (activate) HUDManager.Instance.EnableQuad();
        else HUDManager.Instance.DisableQuad();
    }

    //Actica esto cuando agarre el quad damage
    IEnumerator QuadDamageDuration()
    {
        yield return new WaitForSeconds(10);
        _quadDamage = false;
        photonView.RPC("UpdateQuadMsg", _controlledBy, false);
    }
    #endregion

    #region ~~~ MOVEMENT FUNCTIONS ~~~
    // Funcion que actualiza el movimiento del jugador
    void MoveEntity()
    {
        //Creo un nuevo vector
        Vector3 moveVector = new Vector3();

        //Agrego el movimiento vertical
        if (_moveInstructions[_moveForward]) moveVector += transform.forward;
        else if (_moveInstructions[_moveBackward]) moveVector += -transform.forward;

        //Agrego el movimiento horizontal
        if (_moveInstructions[_moveLeft]) moveVector += -transform.right;
        else if (_moveInstructions[_moveRight]) moveVector += transform.right;

        //Agrego la gravedad
        if (!cc.isGrounded) moveVector += new Vector3(0f, -8f, 0f);

        //Aplico el vector de movimiento
        var aux = moveVector.normalized * speed * Time.deltaTime;
        cc.Move(aux);

        //Animacion
        if (moveVector.z != 0 || moveVector.x != 0) _modelAnim.SetMovementBool();
        else _modelAnim.SetMovementBoolFalse();
    }

    public void AuthoritiveMovePress(string input)
    {
        if (input.Contains(_moveForward)) _moveInstructions[_moveForward] = true;
        if (input.Contains(_moveBackward)) _moveInstructions[_moveBackward] = true;
        if (input.Contains(_moveLeft)) _moveInstructions[_moveLeft] = true;
        if (input.Contains(_moveRight)) _moveInstructions[_moveRight] = true;
    }

    public void AuthoritiveMoveRelease(string input)
    {
        if (input.Contains(_moveForward)) _moveInstructions[_moveForward] = false;
        else if (input.Contains(_moveBackward)) _moveInstructions[_moveBackward] = false;
        else if (input.Contains(_moveLeft)) _moveInstructions[_moveLeft] = false;
        else _moveInstructions[_moveRight] = false;
    }
    #endregion

    #region ~~~ CAMERA FUNCTIONS ~~~
    [PunRPC]
    public void ActivateCamera()
    {
        _myCamera.SetActive(true);
    }

    public void Aim(float rot)
    {
        Vector3 newEulerRot = transform.localEulerAngles;
        newEulerRot.y = rot;

        transform.localEulerAngles = newEulerRot;
    }
    #endregion

    #region ~~~ ETC FUNCTIONS ~~~
    [PunRPC]
    public void SetSpawned()
    {
        PlayerManager.Instance.Spawned = true;
    }

    [PunRPC]
    void InitialConfig()
    {
        PlayerManager.Instance.Spawned = true;

        _myCamera.SetActive(true);

        model.SetActive(false);

        GameObject.Find("InputManager").transform.localEulerAngles = transform.localEulerAngles;

        ammoCount = initialAmmo;
        HUDManager.Instance.ChangeHPText((int)initialhealth);
        HUDManager.Instance.ChangeAmmoText(ammoCount);
        HUDManager.Instance.ChangeKillsText(0);
    }
    #endregion
}