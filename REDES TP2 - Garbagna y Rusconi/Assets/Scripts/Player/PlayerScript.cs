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
    Health hp;
    //GameManager _gameManager;

    //[SerializeField] CameraBehaviour cB;
    private void Awake()
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
            //cB = Camera.main.gameObject.GetComponentInParent<CameraBehaviour>();
            //cB.GetPlayer(this.gameObject);
        }

        isDead = false;
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        if (!isDead)
        {
            var h = Input.GetAxis("Horizontal");
            var v = Input.GetAxis("Vertical");

            movement = new Vector3(h, 0, v) * Time.deltaTime * speed;
            cc.Move(movement);
            if (!cc.isGrounded)
            {
                cc.Move(new Vector3(0, -9.8f, 0) * Time.deltaTime);
            }
        }

        if (Input.GetKeyDown(KeyCode.O)) photonView.RPC("RespawnRPC", RpcTarget.All);
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
