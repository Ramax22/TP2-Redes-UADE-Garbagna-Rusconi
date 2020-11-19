using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class ScoreManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TextMeshProUGUI _winLoseText;
    [SerializeField] Text _killsText;

    private void Awake()
    {
        if (GameServer.Instance.Server == PhotonNetwork.LocalPlayer) StartCoroutine(WaitClientsToLoad());
    }

    IEnumerator WaitClientsToLoad()
    {
        yield return new WaitForSeconds(5);
        RequestForScore();
    }

    void RequestForScore()
    {
        GameServer.Instance.SendScore();
    }

    public void SetWinLoseText(int state)
    {
        if (state == 0) _winLoseText.text = "<color=red> YOU LOSE </color>";
        else if(state == 1) _winLoseText.text = "<color=orange> TIE </color>";
        else _winLoseText.text = "<color=green> YOU WIN </color>";
    }

    public void SetKillCount(int kills)
    {
        _killsText.text = "You killed: " + kills + " enemies";
    }
}