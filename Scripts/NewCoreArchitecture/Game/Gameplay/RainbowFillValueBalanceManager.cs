using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using UnityEngine;

namespace Match3.Game.Gameplay
{
    public class RainbowFillValueBalanceManager : Service, EventListener
    {
        const string BALANCE_KEY = "RainbowFillValueBalance";

        int currentBalanceLevel;

        float[] levelValues = { 0, 0.05f, 0.10f, 0.15f };


        public RainbowFillValueBalanceManager()
        {
            ServiceLocator.Find<EventManager>().Register(this);
            currentBalanceLevel = PlayerPrefs.GetInt(BALANCE_KEY, 0);
        }

        public void OnEvent(GameEvent evt, object sender)
        {
            if(evt is LevelEndedEvent)
            {
                var result = evt.As<LevelEndedEvent>().result;
                if (result == LevelResult.Lose)
                    IncrementBalanceLevel();
                else
                    ResetBalanceLevel();
            }
        }

        void IncrementBalanceLevel()
        {
            SetBalanceLevel(UnityEngine.Mathf.Min(currentBalanceLevel + 1, levelValues.Length - 1));
        }

        void ResetBalanceLevel()
        {
            SetBalanceLevel(0);
        }

        void SetBalanceLevel(int level)
        {
            currentBalanceLevel = level;
            PlayerPrefs.SetInt(BALANCE_KEY, currentBalanceLevel);
        }

        public float CurrentFillValueFor(float baseValue)
        {
            return (baseValue + levelValues[currentBalanceLevel]);
        }
    }
}