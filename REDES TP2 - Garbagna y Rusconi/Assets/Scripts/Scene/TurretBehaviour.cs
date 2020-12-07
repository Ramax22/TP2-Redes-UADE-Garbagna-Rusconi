using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TurretBehaviour : MonoBehaviour
{

    [SerializeField] float rotateSpeed;
    [SerializeField] private const string bulletPrefab = "Prefabs/BulletPrefab";
    [SerializeField] float minTime;
    [SerializeField] float maxTime;
    [SerializeField] float maxRotation;
    [SerializeField] float minRotation;
    [SerializeField] bool rotation;
    [SerializeField] Transform aimingPoint;

    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (minTime == 0)
            minTime = 3;
        if (maxTime == 0)
            maxTime = 6;

        maxRotation = transform.rotation.y + 45;
        minRotation = transform.rotation.y - 45;

        InvokeRepeating("ChangeRotation", Random.Range(15, 25), Random.Range(15, 25));
        InvokeRepeating("Shoot", Random.Range(3, 5), Random.Range(3, 5));
        rotateSpeed = Random.Range(20, 30);
    }

    private void Update()
    {
        if(PhotonNetwork.IsMasterClient)
        { 
            if (rotation)
                transform.Rotate(new Vector3(0, 1, 0) * rotateSpeed * Time.deltaTime);
            else
                transform.Rotate(new Vector3(0, -1, 0) * rotateSpeed * Time.deltaTime);

            if (transform.rotation.y >= maxRotation)
                rotation = !rotation;
            else if (transform.rotation.y <= minRotation)
                rotation = !rotation;
        }
    }

    public void ChangeRotation()
    {
        rotation = !rotation;
    }

   public void Shoot()
    {
        Debug.Log("Shoot");
        PhotonNetwork.Instantiate(bulletPrefab, aimingPoint.position, aimingPoint.rotation);
    }
}
