using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class BulletBehaviour : MonoBehaviour
{
    [SerializeField] float speed;

    private void Awake()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine("WaitToDestroy");
        }
    }
    private void Update()
    {
        if(PhotonNetwork.IsMasterClient)
        transform.Translate(0, 0, speed * Time.deltaTime);
    }

    IEnumerator WaitToDestroy()
    {
        yield return new WaitForSeconds(3f);
        PhotonNetwork.Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(PhotonNetwork.IsMasterClient)
        PhotonNetwork.Destroy(gameObject);
    }
}

