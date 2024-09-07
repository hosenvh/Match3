using System;
using Match3.Foundation.Base.ServiceLocating;
using UnityEngine;

namespace Match3.MoreGames
{
    public class SettingsMerchButton : MonoBehaviour
    {
        private void OnEnable()
        {
            if (HasTargetLink() == false)
                gameObject.SetActive(false);
        }

        private bool HasTargetLink()
        {
            var serverConfigData = ServiceLocator.Find<ServerConfigManager>().data;
            return serverConfigData.config.merchShopLink.IsNullOrEmpty() == false;
        }
    }
}