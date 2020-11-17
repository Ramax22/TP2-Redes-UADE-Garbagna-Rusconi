using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviourPun
{
    [SerializeField] float speed;
    [SerializeField] CharacterController cc;
    [SerializeField] Vector3 movement;
    [SerializeField] Vector3 mousePosition;
    [SerializeField] bool isDead;
    [SerializeField] float initialhealth;
    //[SerializeField] Text hpText;
    [SerializeField] GameObject aimingPoint;
    [SerializeField] float horizontalRotation = 0;
    [SerializeField] float verticalRotation = 0;
    [SerializeField] float verticalRotationLimit = 80f;
    Health hp;
    [SerializeField] GameObject equippedGun;
    [SerializeField] GunManager gunScript;
    [SerializeField] List<GameObject> gunList;
    [SerializeField] int ammoCount;
    [SerializeField] AudioListener _audioListener;
    [SerializeField] GameObject gunPoint;
    [SerializeField] GameObject _myCamera;
    //GameManager _gameManager;

    //[SerializeField] CameraBehaviour cB;
    private void Start()
    {
        cc = GetComponent<CharacterController>();

        if (PhotonNetwork.IsMasterClient)
        {
            //_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            //_gameManager.AddPlayerToList(this);
        }

        hp = new Health(initialhealth, Die);
        if (photonView.IsMine)
        {
            aimingPoint.SetActive(true);
            _audioListener.enabled = true;
            //cB = Camera.main.gameObject.GetComponentInParent<CameraBehaviour>();
            //cB.GetPlayer(this.gameObject);
        }
        else
        {
            if (_audioListener != null) _audioListener.enabled = false; //Hago esto para desactivar los listeners de otros players
        }

        if (!photonView.IsMine)
        {
            _myCamera.SetActive(false);
        }

        isDead = false;
    }

    void Update()
    {
        if (!photonView.IsMine || PhotonNetwork.IsMasterClient) return;

        if (!isDead)
        {
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
            aimingPoint.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
            gameObject.transform.Rotate(new Vector3(0, horizontalRotation, 0));
        }

        //if (Input.GetKeyDown(KeyCode.Space)) Die(); //TESTING
    }

    #region ~~~ HP FUNCTIONS ~~~
    [PunRPC]
    public void ChangeLife(float value)
    {
        hp.ChangeLife(value);

        //if (photonView.IsMine) hpText.text = ("HP: " + hp.HP);
    }

    void Die() //Vamos a tener que encontrar una manera de hacer bien la muerte
    {
        isDead = true;
        //Esto lo tiene que hacer el server de alguna manera
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

    public void GetDamaged(float value, Player client)
    {
        photonView.RPC("ChangeLife", client, value); //aca se lo manda al cliente
        if (GameServer.Instance.Server == PhotonNetwork.LocalPlayer) ChangeLife(value); //esto solo lo ejecuta el server, pero solamente para tener controlada la
        //Vida del jugador al que esta lastimando (tipo, lo pienso por si hay cheats).

        //photonView.RPC("ChangeLife", RpcTarget.All, value);
    }
    #endregion

    #region ~~~ WEAPONS FUNCTIONS ~~~
    public void GetGun(GameObject gunObj)
    {
        gunObj.transform.SetParent(gunPoint.transform);
        Instantiate(gunObj);
        gunList.Add(gunObj);
    }

    public void EquipGun(GameObject gun)
    {
        if (equippedGun != null)
        {
            equippedGun.SetActive(false);
            equippedGun = gun;
            gun.SetActive(true);
            gunScript = gun.GetComponent<GunManager>();
        }
    }

    public void Reload()
    {
        if(equippedGun!=null)
        {
            if(gunScript.CanReload()==true && ammoCount>0)
            { 
                int ammoNeeded = gunScript.AmmoNeeded();
                if(ammoNeeded<=ammoCount) //HAY QUE AGREGAR UN CHECKEO DE AMMOTYPE SI HACEMOS OTRA ARMA
                {
                    ammoCount -= ammoNeeded;
                    gunScript.Reload(ammoNeeded);
                }
                else
                {
                    gunScript.Reload(ammoCount);
                    ammoCount = 0;
                }
                //AGREGAR QUE DESPUES DE RECARGAR CHECKEE CUANTA AMMO HAY EN EL ARMA PARA ACTUALIZAR EL HUD
            }
        }
    }

    //Send shoot to player (desp comento que es, es para no olvidarme que ahora rindo)

    //CHE PADRELANDIA, cuando haga el disparo, haga que revise que el CanShoot del gunmanager revise true, sino no dispare. Tengo armada parte de la logica del behaviour para el disparo (menos el raycast) en GunInterface, revise ahi
    #endregion

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ FUNCIONES SIN REFERENCIAS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //public bool IsDead
    //{
    //    get { return isDead; }
    //}

    //public void getHpText(Text t)
    //{
    //    if (photonView.IsMine)
    //    {
    //        //hpText = t;
    //    }
    //}

    //public void MaxHp()
    //{
    //    photonView.RPC("MaxHpRPC", RpcTarget.All);
    //}

    //[PunRPC]
    //public void MaxHpRPC()
    //{
    //    hp.MaxLife();
    //    if (photonView.IsMine)
    //    {
    //        //hpText.text = ("HP: " + hp.HP);
    //    }
    //}
}