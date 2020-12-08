using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class DoorManager : MonoBehaviour
{

    [SerializeField] List<Transform> _doorPos;
    private const string _doorPrefab = "Prefabs/Door";


    private void Awake()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        foreach (var pos in _doorPos)
        {
            PhotonNetwork.Instantiate(_doorPrefab, pos.position, pos.rotation);
        }
    }
}
