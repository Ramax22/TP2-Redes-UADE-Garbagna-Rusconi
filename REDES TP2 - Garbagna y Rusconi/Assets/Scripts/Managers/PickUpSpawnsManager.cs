using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PickUpSpawnsManager : MonoBehaviour
{
    [SerializeField] List<Transform> _spawnPos;
    [SerializeField] Transform _quadSpawnPos;

    private const string _ammoDirectory = "Prefabs/PickUpPrefabs/PickUpAmmo";
    private const string _hpDirectory = "Prefabs/PickUpPrefabs/PickUpHP2";
    private const string _quadDirectory = "Prefabs/PickUpPrefabs/PickUpQuad";

    private void Awake()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        StartCoroutine(WaitToSpawn());
    }

    IEnumerator WaitToSpawn()
    {
        yield return new WaitForSeconds(20);
        Spawn();
        StartCoroutine(WaitToRespawn());
    }

    IEnumerator WaitToRespawn()
    {
        yield return new WaitForSeconds(20);
        Spawn();
        StartCoroutine(WaitToRespawn());
    }

    void Spawn()
    {
        for (int i = 0; i < _spawnPos.Count; i++)
        {
            if (i % 2 == 0) PhotonNetwork.Instantiate(_hpDirectory, _spawnPos[i].position, _spawnPos[i].rotation);
            else PhotonNetwork.Instantiate(_ammoDirectory, _spawnPos[i].position, _spawnPos[i].rotation);
        }
        PhotonNetwork.Instantiate(_quadDirectory, _quadSpawnPos.position, _quadSpawnPos.rotation);
    }
}