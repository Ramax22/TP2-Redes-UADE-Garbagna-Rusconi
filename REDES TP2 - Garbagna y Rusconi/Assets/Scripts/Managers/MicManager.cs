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

    private void Awake()
    {
        _rec = FindObjectOfType<Recorder>();

        if (PhotonNetwork.IsMasterClient) _rec.TransmitEnabled = false;

        GetComponent<PhotonVoiceView>().enabled = false;
        GetComponent<Speaker>().enabled = false;
        StartCoroutine(CheckProperty());
    }
    
    IEnumerator CheckProperty()
    {
        yield return new WaitForSeconds(5);
        if (photonView.IsMine)
        {
            GetComponent<PhotonVoiceView>().enabled = true;
            GetComponent<Speaker>().enabled = true;
            _rec.TransmitEnabled = false;
            _rec.InterestGroup = 0;
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