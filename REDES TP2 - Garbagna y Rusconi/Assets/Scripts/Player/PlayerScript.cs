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
    
    
    bool isDead;
    CharacterController cc;
    Vector3 mousePosition;
    int ammoCount; 
    Health hp;
    Player _lastHitBy;
    bool isMovingVertical, isMovingHorizonal;


    public CharacterController Cc { get => cc; set => cc = value; }

    // movement vars
    Dictionary<string, bool> _moveInstructions;

    const string _moveForward = "up";
    const string _moveBackward = "down";
    const string _moveLeft = "left";
    const string _moveRight = "right";



    private void Start()
    {
        //GameServer.Instance.photonView.RPC("CheckCamera", GameServer.Instance.Server, this, PhotonNetwork.LocalPlayer);

        //Esto es para que el input manager este como "mirando" hacia el mismo lugar, y no se rompa la rotación
        if (!PhotonNetwork.IsMasterClient) GameObject.Find("InputManager").transform.localEulerAngles = transform.localEulerAngles;

        cc = GetComponent<CharacterController>();

        hp = new Health(initialhealth, Die);

        /*if (!photonView.IsMine)
        {
            _myCamera.SetActive(false);
        }*/

        isDead = false;

        if (photonView.IsMine)
        {
            ammoCount = initialAmmo;
            HUDManager.Instance.ChangeHPText(Mathf.RoundToInt(hp.HP));
            HUDManager.Instance.ChangeAmmoText(ammoCount);
            HUDManager.Instance.ChangeKillsText(0);
            //Cursor.lockState = CursorLockMode.Locked;

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

        Move2();

        //if (!isDead)
        //{

        //    transform.Translate(movement);
        //    //cc.Move(movement);
        //    //if (!cc.isGrounded)
        //    //{
        //    //    //movement.y -= 9.8f * Time.deltaTime * speed;
        //    //    //cc.Move(new Vector3(0, -9.8f, 0) * Time.deltaTime);
        //    //}
        //}
    }

    #region ~~~ HP FUNCTIONS ~~~
    [PunRPC]
    public void ChangeLife(float value, Player whoShoot)
    {
        _lastHitBy = whoShoot;
        hp.ChangeLife(value);
        

        HUDManager.Instance.ChangeHPText((int)hp.HP);

        //if (photonView.IsMine) hpText.text = ("HP: " + hp.HP);
    }

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
        HUDManager.Instance.ChangeHPText((int)hp.HP);
    }

    void Die() //Vamos a tener que encontrar una manera de hacer bien la muerte
    {
        isDead = true;
        GameServer.Instance.RequestRespawn();
        //Esto lo tiene que hacer el server de alguna manera
        if(photonView.IsMine)
        { 
            HUDManager.Instance.ClearTexts();
            Cursor.lockState = CursorLockMode.None;
        }
        GameServer.Instance.AddScore(_lastHitBy);
        HUDManager.Instance.ClearTexts();
        PhotonNetwork.Destroy(gameObject);
    }
    #endregion

    #region ~~~ WEAPONS FUNCTIONS ~~~
    
    public void Shoot()
    {
        if (ammoCount > 0)
        {
            ammoCount--;
            HUDManager.Instance.ChangeAmmoText(ammoCount);
            GameServer.Instance.Shoot(photonView.ViewID, _quadDamage);
        }
    }

    public void CallGetAmmo(Player owner, int value)
    {
        photonView.RPC("GetAmmo", owner, value);
    }

    [PunRPC]
    void GetAmmo(int value)
    {
        ammoCount += value;
        HUDManager.Instance.ChangeAmmoText(ammoCount);
    }

    public void AmmoChange()
    {
        ammoCount = initialAmmo;
    }
    #endregion

    #region ~~~ QUAD DAMAGE FUNCTIONS ~~~

    public void CallGetQuad(Player owner)
    {
        photonView.RPC("GetQuad", owner);
    }

    [PunRPC]
    public void GetQuad()
    {
        _quadDamage = true;
        HUDManager.Instance.EnableQuad();
        StartCoroutine(QuadDamageDuration());
    }

    //Actica esto cuando agarre el quad damage
    IEnumerator QuadDamageDuration()
    {
        yield return new WaitForSeconds(10);
        _quadDamage = false;
        HUDManager.Instance.DisableQuad();
    }
    #endregion

    #region ~~~ MOVEMENT FUNCTIONS ~~~

    public void Move(Vector3 mov)
    {
        mov *= speed;
        cc.Move(mov);
        //transform.Translate(mov);
    }

    void Move2()
    {
        Vector3 moveVector = new Vector3();

        if (_moveInstructions[_moveForward]) moveVector += transform.forward;
        else if (_moveInstructions[_moveBackward]) moveVector += -transform.forward;

        if (_moveInstructions[_moveLeft]) moveVector += -transform.right;
        else if (_moveInstructions[_moveRight]) moveVector += transform.right;

        

        var aux = moveVector.normalized * speed * Time.deltaTime;
        cc.Move(aux);
    }

    public void AuthoritiveMovePress(string input)
    {
        if (input.Contains(_moveForward)) _moveInstructions[_moveForward] = true;
        if (input.Contains(_moveBackward)) _moveInstructions[_moveBackward] = true;
        if (input.Contains(_moveLeft)) _moveInstructions[_moveLeft] = true;
        if (input.Contains(_moveRight)) _moveInstructions[_moveRight] = true;

        /*
        if (verMove == "up") movement += transform.forward;
        else if (verMove == "down") movement += -transform.forward;

        if (horMove == "left") movement += -transform.right;
        else if (horMove == "right") movement += transform.right;

        if (verMove == "up") movement += Vector3.forward;
        else if (verMove == "down") movement += Vector3.back;

        if (horMove == "left") movement += Vector3.left;
        else if (horMove == "right") movement += Vector3.right;
        */
    }
    public void AuthoritiveMoveRelease(string input)
    {
        if (input.Contains(_moveForward)) _moveInstructions[_moveForward] = false;
        else if (input.Contains(_moveBackward)) _moveInstructions[_moveBackward] = false;
        else if (input.Contains(_moveLeft)) _moveInstructions[_moveLeft] = false;
        else _moveInstructions[_moveRight] = false;

        /*if (verMove == "up" || verMove == "down") movement.z = 0;
        else if (horMove == "left" || horMove == "right") movement.x = 0;*/
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
        //aimingPoint.transform.Rotate(new Vector3(vertical, 0, 0));
    }

    #endregion

    #region ~~~ ETC FUNCTIONS ~~~

    [PunRPC]
    public void SetSpawned()
    {
        PlayerManager.Instance.Spawned = true;
    }

    #endregion
}