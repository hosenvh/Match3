using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.ServiceLocating;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrivacyPolicyController
{
    private bool haveToCheckPolicy = true;

    public PrivacyPolicyController()
    {
        ServiceLocator.Find<ConfigurationManager>().Configure(this);
    }

    public void SetHaveToCheckPolicy(bool value)
    {
        this.haveToCheckPolicy = value;
    }
    
    public void CheckPrivacyPolicy(Action onCheckComplete)
    {
        if (!haveToCheckPolicy || Base.gameManager.profiler.IsPrivacyPolicyChecked)
        {
            onCheckComplete();
            return;
        }

        Base.gameManager.OpenPopup<Popup_PrivacyPolicy>().Setup(true, result =>
        {
            if (result)
            {
                Base.gameManager.profiler.IsPrivacyPolicyChecked = true;
                Base.gameManager.ClosePopup();
                onCheckComplete();
            }
            else
            {
                Application.Quit();
            }
        });
    }
    
}
