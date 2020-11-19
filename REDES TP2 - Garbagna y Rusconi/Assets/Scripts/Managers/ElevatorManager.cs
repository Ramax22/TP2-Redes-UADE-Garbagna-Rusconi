using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ElevatorManager : MonoBehaviourPunCallbacks
{
    [SerializeField] List<Transform> _elevatorPos;
    private const string _elevatorPrefab = "Prefabs/Elevator";

    private void Awake()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        foreach (var pos in _elevatorPos)
        {
            PhotonNetwork.Instantiate(_elevatorPrefab, pos.position, pos.rotation);
        }
    }
}