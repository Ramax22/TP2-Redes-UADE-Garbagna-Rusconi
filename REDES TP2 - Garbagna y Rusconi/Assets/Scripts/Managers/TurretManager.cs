using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TurretManager : MonoBehaviour
{
    public static TurretManager Instance;

    [SerializeField] List<Transform> _turretPos;
    private const string _turretPrefab = "Prefabs/Turret";


    private void Awake()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (Instance == null)
            Instance = this;

        foreach (var pos in _turretPos)
        {
            PhotonNetwork.Instantiate(_turretPrefab, pos.position, pos.rotation);
        }
    }
}
