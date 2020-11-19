using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpSpawnsManager : MonoBehaviour
{
    [SerializeField] List<GameObject> spawnPoints;
    [SerializeField] GameObject quadSpawnPoint;

    void Start()
    {
        GameServer.Instance.GetSpawnPoints(spawnPoints, quadSpawnPoint);    
    }

    
}
