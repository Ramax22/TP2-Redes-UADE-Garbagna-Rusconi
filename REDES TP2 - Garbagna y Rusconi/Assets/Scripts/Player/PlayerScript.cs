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
            //cB = Camera.main.gameObject.GetComponentInParent<CameraBehaviour>();
            //cB.GetPlayer(this.gameObject);
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

        
    }

    [PunRPC]
    public void ChangeLife(float value)
    {
        hp.ChangeLife(value);

        //if (photonView.IsMine) hpText.text = ("HP: " + hp.HP);
    }

    void Die() //Vamos a tener que encontrar una manera de hacer bien la muerte
    {
        isDead = true;
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

    public void GetGun(GameObject gunObj)
    {
        gunObj.transform.SetParent(aimingPoint.transform);
        Instantiate(gunObj);
        gunList.Add(gunObj);
        gunScript = gunObj.GetComponent<GunManager>();
    }

    public void EquipGun(GameObject gun)
    {
        if (equippedGun != null)
        {
            equippedGun.SetActive(false);
            equippedGun = null;
        }
    }

    public bool IsDead
    {
        get { return isDead; }
    }

    public void getHpText(Text t)
    {
        if (photonView.IsMine)
        {
            //hpText = t;
        }
    }

    public void GetDamaged(float value)
    {
        photonView.RPC("ChangeLife", RpcTarget.All, value);
    }

    public void MaxHp()
    {
        photonView.RPC("MaxHpRPC", RpcTarget.All);
    }

    [PunRPC]
    public void MaxHpRPC()
    {
        hp.MaxLife();
        if (photonView.IsMine)
        {
            //hpText.text = ("HP: " + hp.HP);
        }
    }
}
