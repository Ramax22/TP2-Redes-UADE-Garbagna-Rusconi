using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ElevatorBehaviour : MonoBehaviourPunCallbacks
{
    float _speed;
    int _direction;
    Rigidbody _rb;

    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        _speed = Random.Range(1, 3);
        _direction = Random.Range(0, 1);
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (_direction == 1) _rb.velocity = Vector3.up * _speed;
        else _rb.velocity = Vector3.down * _speed;

        if (transform.position.y >= 11) _direction = 0;
        else if (transform.position.y <= 0.5) _direction = 1;
    }
}