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
    [SerializeField] Vector3 movement;
    bool isDead;
    CharacterController cc;
    Vector3 mousePosition;
    int ammoCount; 
    Health hp;
    Player _lastHitBy;


    public CharacterController Cc { get => cc; set => cc = value; }

    

    private void Start()
    {
        //GameServer.Instance.photonView.RPC("CheckCamera", GameServer.Instance.Server, this, PhotonNetwork.LocalPlayer);
        

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
        }
    }

    void Update()
    {
        if (!photonView.IsMine || PhotonNetwork.IsMasterClient) return;

        if (!isDead)
        {
            //cc.Move(movement);
            if (!cc.isGrounded)
            {
                //movement.y -= 9.8f * Time.deltaTime * speed;
                //cc.Move(new Vector3(0, -9.8f, 0) * Time.deltaTime);
            }

            
        }
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
        //cc.Move(mov);
        transform.Translate(mov);
    }

    #endregion

    #region ~~~ CAMERA FUNCTIONS ~~~

    [PunRPC]
    public void ActivateCamera()
    {
        _myCamera.SetActive(true);
    }

    public void Aim(float vertical, float horizontal)
    {
        transform.Rotate(new Vector3(0, horizontal, 0));
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