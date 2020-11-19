using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Voice.Unity;
using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Realtime;

public class MicManager : MonoBehaviourPunCallbacks
{
    Recorder _rec;
    PhotonVoiceView photonVoiceView;

    private void Awake()
    {
        if (photonView.IsMine)
        {
            _rec = FindObjectOfType<Recorder>();

            _rec.TransmitEnabled = false;

            _rec.InterestGroup = 0;
            /* photonVoiceView = GetComponent<PhotonVoiceView>();
             photonVoiceView.enabled = false;
             photonVoiceView.enabled = true;*/
        }
        else
        {
            GetComponent<PhotonVoiceView>().enabled = false;
            GetComponent<Speaker>().enabled = false;
        }


        
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

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