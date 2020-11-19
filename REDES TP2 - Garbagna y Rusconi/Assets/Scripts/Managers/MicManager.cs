using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Voice.Unity;
using Photon.Pun;
using Photon.Voice.PUN;

public class MicManager : MonoBehaviour
{
    Recorder _rec;
    PhotonVoiceView photonVoiceView;

    private void Awake()
    {
        _rec = FindObjectOfType<Recorder>();

        _rec.TransmitEnabled = false;

        _rec.InterestGroup = 0;
        photonVoiceView = GetComponent<PhotonVoiceView>();
        photonVoiceView.enabled = false;
        photonVoiceView.enabled = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            _rec.TransmitEnabled = true;
        }
        else if (Input.GetKeyUp(KeyCode.V))
        {
            _rec.TransmitEnabled = false;
        }
    }
}