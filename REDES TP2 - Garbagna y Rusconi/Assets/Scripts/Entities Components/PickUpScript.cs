using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class PickUpScript : MonoBehaviour
{
    Animator anim;
    [SerializeField] int type;
    //0 es HP, 1 es ammo, 2 es quad

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        if (PhotonNetwork.IsMasterClient)
            Invoke("FastAnim", 5);
    }


    public void FastAnim()
    {
        Invoke("AutoDestroy", 5);
    }

    public void AutoDestroy()
    {
        if(PhotonNetwork.IsMasterClient)
        PhotonNetwork.Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            PlayerScript pS;
            pS = other.gameObject.GetComponent<PlayerScript>();
            if(pS!=null)
            {
                GameServer.Instance.GotPickUp(pS, type);
                AutoDestroy();
            }
        }
    }

}
