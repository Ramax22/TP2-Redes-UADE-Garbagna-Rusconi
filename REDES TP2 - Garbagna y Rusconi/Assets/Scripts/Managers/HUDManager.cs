﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class HUDManager : MonoBehaviourPunCallbacks
{
    public static HUDManager Instance;
    
    [SerializeField] Text hpText;
    [SerializeField] Text ammoText;
    [SerializeField] Text quadText;
    [SerializeField] Text killsText;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }

    public void ChangeHPText(int hp)
    {
        hpText.text = "HP: " + hp;
    }

    public void ChangeAmmoText(int ammo)
    {
        ammoText.text = "AMMO: " + ammo;
    }

    public void ChangeKillsText(int kills)
    {
        killsText.text = "KILLS: " + kills;
    }

    public void EnableQuad()
    {
        quadText.text = "QUAD DAMAGE";
    }

    public void DisableQuad()
    {
        quadText.text = "";
    }

    [PunRPC]
    public void ClearTexts()
    {
        ammoText.text = "";
        hpText.text = "";
        quadText.text = "";
    }

    public void SendClearText(Player client)
    {
        photonView.RPC("ClearTexts", client);
    }
}