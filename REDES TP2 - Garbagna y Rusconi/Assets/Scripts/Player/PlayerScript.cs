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

    bool isDead;
    CharacterController cc;
    Vector3 movement;
    Vector3 mousePosition;
    int ammoCount; 
    float horizontalRotation = 0;
    float verticalRotation = 0;
    float verticalRotationLimit = 80f;
    Health hp;
    Player _lastHitBy;

    private void Start()
    {
        cc = GetComponent<CharacterController>();

        hp = new Health(initialhealth, Die);

        if (!photonView.IsMine)
        {
            _myCamera.SetActive(false);
        }

        isDead = false;

        if (photonView.IsMine)
        {
            ammoCount = initialAmmo;
            HUDManager.Instance.ChangeHPText(Mathf.RoundToInt(hp.HP));
            HUDManager.Instance.ChangeAmmoText(ammoCount);
            HUDManager.Instance.ChangeKillsText(0);
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void Update()
    {
        if (!photonView.IsMine || PhotonNetwork.IsMasterClient) return;

        if (!isDead)
        {
            //MOVEMENT & CAMERA
            var h = Input.GetAxis("Horizontal") * transform.right;
            var v = Input.GetAxis("Vertical") * transform.forward;
            movement = (h + v) * Time.deltaTime * speed;
            cc.Move(movement);
            if (!cc.isGrounded)
            {
                cc.Move(new Vector3(0, -9.8f, 0) * Time.deltaTime);
            }
            verticalRotation -= Input.GetAxisRaw("Mouse Y");
            horizontalRotation = Input.GetAxisRaw("Mouse X");
            verticalRotation = Mathf.Clamp(verticalRotation, -80f, 80f);
            transform.Rotate(new Vector3(0, horizontalRotation, 0));

            //SHOOT
            if (Input.GetButtonDown("Fire1"))
            {
                if(Cursor.lockState==CursorLockMode.None)
                Cursor.lockState = CursorLockMode.Locked;

                Shoot();
            }

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
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

        PhotonNetwork.Destroy(gameObject);
        print("Muerto");
        /*if (photonView.IsMine)
        {
            Vector3 deadPos = new Vector3(500, 500, 500);
            transform.position = deadPos;
            //cB.StopFollowing();
            hpText.text = ("Dead");
        }*/

        //if (PhotonNetwork.IsMasterClient) _gameManager.CheckEndgame();
    }

    /*public void GetDamaged(float value, Player client)
    {
        photonView.RPC("ChangeLife", client, value); //aca se lo manda al cliente
        if (GameServer.Instance.Server == PhotonNetwork.LocalPlayer) ChangeLife(value); //esto solo lo ejecuta el server, pero solamente para tener controlada la
        //Vida del jugador al que esta lastimando (tipo, lo pienso por si hay cheats).

        //photonView.RPC("ChangeLife", RpcTarget.All, value);
    }*/
    #endregion

    #region ~~~ WEAPONS FUNCTIONS ~~~
    
    void Shoot()
    {
        if (ammoCount > 0)
        {
            ammoCount--;
            HUDManager.Instance.ChangeAmmoText(ammoCount);
            GameServer.Instance.Shoot(photonView.ViewID, _quadDamage);
        }
    }

    public void AmmoChange()
    {

    }

    //public void Reload()
    //{
    //    if(equippedGun!=null)
    //    {
    //        if(gunScript.CanReload()==true && ammoCount>0)
    //        { 
    //            int ammoNeeded = gunScript.AmmoNeeded();
    //            if(ammoNeeded<=ammoCount) //HAY QUE AGREGAR UN CHECKEO DE AMMOTYPE SI HACEMOS OTRA ARMA
    //            {
    //                ammoCount -= ammoNeeded;
    //                gunScript.Reload(ammoNeeded);
    //            }
    //            else
    //            {
    //                gunScript.Reload(ammoCount);
    //                ammoCount = 0;
    //            }
    //            //AGREGAR QUE DESPUES DE RECARGAR CHECKEE CUANTA AMMO HAY EN EL ARMA PARA ACTUALIZAR EL HUD
    //        }
    //    }


    //CHE PADRELANDIA, cuando haga el disparo, haga que revise que el CanShoot del gunmanager revise true, sino no dispare. Tengo armada parte de la logica del behaviour para el disparo (menos el raycast) en GunInterface, revise ahi
    #endregion

    #region ~~~ QUAD DAMAGE FUNCTIONS ~~~
    //Actica esto cuando agarre el quad damage
    IEnumerator QuadDamageDuration()
    {
        HUDManager.Instance.EnableQuad();
        yield return new WaitForSeconds(10);
        _quadDamage = false;
        HUDManager.Instance.DisableQuad();
    }
    #endregion
}