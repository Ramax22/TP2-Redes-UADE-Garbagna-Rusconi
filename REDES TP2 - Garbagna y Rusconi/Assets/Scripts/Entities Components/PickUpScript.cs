using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class PickUpScript : MonoBehaviourPunCallbacks
{
    [SerializeField] int type;
    Animator _anim;

    void Start()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        _anim = GetComponentInChildren<Animator>();
        StartCoroutine(WaitToMoveFaster());
    }

    public void MoveFaster()
    {
        _anim.SetBool("SetBool", true);
    }

    IEnumerator WaitToMoveFaster()
    {
        yield return new WaitForSeconds(5);
        MoveFaster();
        StartCoroutine(WaitToDestroy());
    }

    IEnumerator WaitToDestroy()
    {
        yield return new WaitForSeconds(5);
        PhotonNetwork.Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        var photonId = other.transform.GetComponent<PhotonView>().Owner;
        var player = other.transform.GetComponent<PlayerScript>();

        if (player)
        {
            if (type == 0) player.GetAmmo(10); //AMMO
            else if (type == 1) player.CallGetHP(photonId); //LIFE
            else player.CallGetQuad(photonId); //QuadDamage

            PhotonNetwork.Destroy(gameObject);
        }
    }
}
