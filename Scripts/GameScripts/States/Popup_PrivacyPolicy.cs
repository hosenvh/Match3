using System;
using System.Collections;
using System.Collections.Generic;
using SeganX;
using UnityEngine;
using UnityEngine.UI;

public class Popup_PrivacyPolicy : GameState
{

    [SerializeField] private GameObject confirmBtn = default;

    [SerializeField] private GameObject closeBtn = default;
    
    
    private System.Action<bool> onResult;
    private const string PrivatePolicyLink = "https://policies.google.com/privacy";

    
    
    public Popup_PrivacyPolicy Setup(bool confirmMode, System.Action<bool> onResult)
    {
        confirmBtn.SetActive(confirmMode);
        closeBtn.SetActive(!confirmMode);
        this.onResult = onResult;
        return this;
    }

    public void Accept()
    {
        gameManager.fxPresenter.PlayClickAudio();
        onResult.Invoke(true);
    }

    public void Discard()
    {
        onResult.Invoke(false);
    }

    public void OpenPrivacyPolicyLink()
    {
        Application.OpenURL(PrivatePolicyLink);
    }

    public void CloseFromSettings()
    {
        gameManager.fxPresenter.PlayClickAudio();
        base.Back();
    }

    public override void Back()
    {
        // Do Nothing
    }
}
