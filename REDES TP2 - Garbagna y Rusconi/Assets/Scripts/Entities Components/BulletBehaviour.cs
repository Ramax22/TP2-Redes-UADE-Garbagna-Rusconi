using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class BulletBehaviour : MonoBehaviourPunCallbacks
{
    [SerializeField] float speed;
    bool destroyed;

    private void Awake()
    {
        if (photonView.IsMine) StartCoroutine("WaitToDestroy");
    }
    private void Update()
    {
        if (photonView.IsMine) transform.Translate(0, 0, speed * Time.deltaTime);
    }

    IEnumerator WaitToDestroy()
    {
        yield return new WaitForSeconds(3f);
        if (!destroyed && photonView.IsMine) PhotonNetwork.Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (photonView.IsMine)
        {

            var player = other.transform.GetComponent<PlayerScript>();

            if (player)
            {
                player.ChangeLife(5f, PhotonNetwork.LocalPlayer);
            }

            PhotonNetwork.Destroy(gameObject);
            destroyed = true;
        }
    }
}

