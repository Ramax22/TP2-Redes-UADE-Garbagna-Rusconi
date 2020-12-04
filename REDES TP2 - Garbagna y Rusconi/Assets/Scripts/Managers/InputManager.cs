using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class InputManager : MonoBehaviour
{
    bool _canWalk;
    Vector3 movement;
    
    float horizontalRotation = 0;
    float verticalRotation = 0;
    float verticalRotationLimit = 80f;

    void Awake()
    {
        if (PhotonNetwork.IsMasterClient)
            Destroy(this.gameObject);
    }
    void Start()
    {
        StartCoroutine(Wait());
    }

    void Update()
    {
        if (PlayerManager.Instance.Spawned)
        {
            if (_canWalk)
            {
                var h = Input.GetAxis("Horizontal");
                var v = Input.GetAxis("Vertical");

                movement = new Vector3(h,0,v) * Time.deltaTime;

                if (movement != new Vector3(0, 0, 0))
                {
                    GameServer.Instance.photonView.RPC("RequestMovement", GameServer.Instance.Server, PhotonNetwork.LocalPlayer, movement);
                }

                verticalRotation -= Input.GetAxisRaw("Mouse Y");
                horizontalRotation = Input.GetAxisRaw("Mouse X");
                verticalRotation = Mathf.Clamp(verticalRotation, -80f, 80f);
                GameServer.Instance.photonView.RPC("RequestAim", GameServer.Instance.Server, PhotonNetwork.LocalPlayer, verticalRotation, horizontalRotation);
            }

            if (Input.GetButtonDown("Fire1"))
            {
                if (Cursor.lockState == CursorLockMode.None)
                    Cursor.lockState = CursorLockMode.Locked;

                GameServer.Instance.photonView.RPC("RequestShoot", GameServer.Instance.Server, PhotonNetwork.LocalPlayer);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }

    IEnumerator Wait()
    {
        while (true)
        {
            _canWalk = !_canWalk;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
