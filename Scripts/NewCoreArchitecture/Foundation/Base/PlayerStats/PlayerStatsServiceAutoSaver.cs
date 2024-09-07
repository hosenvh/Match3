using System;
using Match3.Foundation.Base.PlayerStats;
using Match3.Foundation.Base.ServiceLocating;
using UnityEngine;


namespace Match3.Foundation.Base.PlayerStats
{

    public class PlayerStatsServiceAutoSaver : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
                ServiceLocator.Find<PlayerStatsService>().SaveCurrentSession();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
                ServiceLocator.Find<PlayerStatsService>().SaveCurrentSession();
        }

        private void OnApplicationQuit()
        {
            ServiceLocator.Find<PlayerStatsService>().SaveCurrentSession();
        }
    }

}