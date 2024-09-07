using System;
using I2.Loc;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay;
using Match3.Game.Gameplay.Input;
using Match3.Game.MainShop;
using Match3.Presentation.Gameplay.Core;
using Match3.Profiler;
using SeganX;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Match3.Presentation.Gameplay.PowerUpActivation
{


    public abstract class PowerUpActivator : MonoBehaviour
    {
        public int index;
        public Image reserverdRewardSymbol;
        
        public LocalText availableItemsText;

        public UnityEvent onUnlocked;
        public UnityEvent onLocked;

        public UnityEvent onAvailable;
        public UnityEvent onNotAvailable;

        public UnityEvent onSelected;
        public UnityEvent onDeselected;
        protected GameplayController gameplayController;
        PowerUpSelectionInputHandler inputHandler;
        InputSystem inputSystem;

        public void Setup(PowerUpSelectionInputHandler inputHandler, GameplayController gameplayController)
        {
            this.inputHandler = inputHandler;
            this.gameplayController = gameplayController;
            this.inputSystem = gameplayController.GetSystem<InputSystem>();
      
            UpdateState();
        }

        public void UpdateState()
        {
            availableItemsText.SetText(Base.gameManager.profiler.GetPowerupCount(index).ToString());
            if (IsAvailable())
                onAvailable.Invoke();
            else
                onNotAvailable.Invoke();

            if (IsUnlocked())
                onUnlocked.Invoke();
            else
                onLocked.Invoke();
            
            reserverdRewardSymbol.gameObject.SetActive(DoesReservedRewardExist());
        }

        private bool IsAvailable()
        {
            return IsInInventory();
        }

        public void TryUse()
        {
            if (IsUnlocked())
            {
                if (IsInInventory())
                {
                    onSelected.Invoke();
                    inputHandler.EnablePowerUpSelection(this);
                }
                else
                {
                    // TODO: Refactor this fuckity fuck.
                    Base.gameManager.OpenPopup<Popup_ConfirmBox>().Setup(
                        string.Format(ScriptLocalization.Message_Purchase.AskPurchasePowerUp, Base.gameManager.profiler.GetPowerupPrice(index)), 
                        ScriptLocalization.UI_General.Yes, ScriptLocalization.UI_General.No, true, (confirm) =>
                    {
                        if (confirm)
                        {
                            if (Base.gameManager.profiler.CoinCount >= Base.gameManager.profiler.GetPowerupPrice(index))
                            {
                                Base.gameManager.profiler.ChangeCoin(-Base.gameManager.profiler.GetPowerupPrice(index), "buy powerup: " + index);
                                Base.gameManager.profiler.ChangePowerupCount(index, 3);
                                ServiceLocator.Find<EventManager>().Propagate(new PowerUpPurchaseSucceededEvent(index), this);
                                UpdateState();
                                TryUse();
                            }
                            else
                            {
                                Base.gameManager.OpenPopup<Popup_ConfirmBox>().Setup(
                                    ScriptLocalization.Message_Purchase.NotEnoughCoin, 
                                    ScriptLocalization.UI_General.Ok, 
                                    null, 
                                    true, 
                                    (confirm2) =>
                                    {
                                         Base.gameManager.shopCreator.TrySetupMainShop("PowerUp", UpdateState);
                                    });
                                ServiceLocator.Find<EventManager>().Propagate(new PowerUpPurchaseFailedEvent(index), this);
                            }
                        }
                        else
                            ServiceLocator.Find<EventManager>().Propagate(new PowerUpPurchaseClosedEvent(index), this);
                    });

                    ServiceLocator.Find<EventManager>().Propagate(new PowerUpPurchaseOpenedEvent(index), this);
                }
            }
            else
                Base.gameManager.OpenPopup<Popup_ConfirmBox>().Setup(
                    string.Format(ScriptLocalization.Message_GamePlay.PowerUpUnlockLevel, Base.gameManager.profiler.powerupUnlockLevels[index]), 
                    ScriptLocalization.UI_General.Ok, null, true, null);
        }




        protected void Execute(GameplayInputCommand command)
        {
            inputSystem.Apply(command);
            inputHandler.Disable();
            onDeselected.Invoke();
        }


        private bool IsInInventory()
        {
            return DoesPowerupExistInProfile() || DoesReservedRewardExist();

            bool DoesPowerupExistInProfile()
            {
                return Base.gameManager.profiler.GetPowerupCount(index) > 0;
            }

        }

        private bool DoesReservedRewardExist()
        {
            LevelReservedRewardsHandler profiler = Base.gameManager.levelSessionProfiler.PowerUpsReservedRewardsHandler;
            Type powerupRewardType = RewardConversionUtility.GetPowerUpRewardTypeFromPowerUpIndex(index);
            return profiler.DoesRewardOfTypeExist(powerupRewardType);
        }
        
        private bool IsUnlocked()
        {
            return Base.gameManager.profiler.LastUnlockedLevel >= Base.gameManager.profiler.powerupUnlockLevels[index] - 1;
        }


        public abstract void OnCellStackSelected(CellStackPresenter target);

        public abstract bool AcceptsSelection(CellStackPresenter cellStackPresenter);

        protected abstract void ResetActivationState();


        public void OnSelectionCanceled()
        {
            ResetActivationState();
            onDeselected.Invoke();
        }
    }

}