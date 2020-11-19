using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Voice.Unity;

public class MicSelect : MonoBehaviour
{
    Recorder _recorder;
    public Dropdown _drop;
    void Start()
    {
        _recorder = FindObjectOfType<Recorder>();
        List<Dropdown.OptionData> list = new List<Dropdown.OptionData>();
        for (int i = 0; i < Microphone.devices.Length; i++)
        {
            Dropdown.OptionData data = new Dropdown.OptionData();
            data.text = Microphone.devices[i];
            list.Add(data);
        }
        _drop.AddOptions(list);
    }
    public void SetMic(int i)
    {
        var currMic = Microphone.devices[i];
        _recorder.UnityMicrophoneDevice = currMic;
    }
}