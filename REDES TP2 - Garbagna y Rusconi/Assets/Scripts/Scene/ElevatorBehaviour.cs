using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorBehaviour : MonoBehaviour
{
    float _speed;
    int _direction;
    Rigidbody _rb;

    private void Start()
    {
        _speed = Random.Range(1, 7);
        _direction = Random.Range(0, 1);
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (_direction == 1) _rb.velocity = Vector3.up * _speed;
        else _rb.velocity = Vector3.down * _speed;

        if (transform.position.y >= 11) _direction = 0;
        else if (transform.position.y <= 0.5) _direction = 1;
    }
}