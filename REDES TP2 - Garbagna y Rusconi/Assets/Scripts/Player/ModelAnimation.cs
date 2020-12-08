using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ModelAnimation : MonoBehaviourPunCallbacks
{
    Animator _anim;

    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        _anim = GetComponent<Animator>();
    }

    public void SetFireBool() { if (photonView.IsMine) _anim.SetBool("isShooting", true); }

    public void ResetShootBool() { if (photonView.IsMine) _anim.SetBool("isShooting", false); }

    public void SetMovementBool() { if (photonView.IsMine) _anim.SetBool("isMoving", true); }
    public void SetMovementBoolFalse() { if (photonView.IsMine) _anim.SetBool("isMoving", false); }
}