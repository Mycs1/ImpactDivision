﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UiEvent;
using UiMsgs = UiEvent.UiMsgs;

public class HUDMgr : MonoBehaviour
{
    public Image hpImage;
    public Text ammo;
    public Text mag;
    public RawImage weaponCut;

    private void Awake()
    {
        Battle.hudMgr = this;
    }

    public void Init(C_UiEventMgr eventMgr)
    {
        eventMgr.BindEvent(typeof(UiMsgs.Hp), RefreshHp);
        eventMgr.BindEvent(typeof(UiMsgs.Ammo), RefreshAmmo);
        eventMgr.BindEvent(typeof(UiMsgs.WeaponCut), SetWeaponCut);
    }

    public void RefreshHp(UiMsg msg)
    {
        var hpMsg = msg as UiMsgs.Hp;
        hpImage.fillAmount = hpMsg.hp / hpMsg.hpMax;
    }

    public void RefreshAmmo(UiMsg msg)
    {
        var ammoMsg = msg as UiMsgs.Ammo;
        ammo.text = ammoMsg.ammo.ToString();
        mag.text = ammoMsg.mag.ToString();
    }

    public void SetWeaponCut(UiMsg msg)
    {
        var cutMsg = msg as UiMsgs.WeaponCut;
        weaponCut.texture = cutMsg.texture;
    }
}
