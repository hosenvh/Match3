using System;
using I2.Loc;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.MainShop;
using Match3.Game.NeighborhoodChallenge;
using Match3.Game.ShopManagement;
using SeganX;
using UnityEngine;
using UnityEngine.Events;

namespace Match3.Presentation.NeighborhoodChallenge
{
    public class NeighbourChangeLevelPopupOpenedEvent : GameEvent {}

    public class NeighbourChangeLevelPopupClosedEvent : GameEvent
    {
        public bool confirmResult;

        public NeighbourChangeLevelPopupClosedEvent(bool confirmResult)
        {
            this.confirmResult = confirmResult;
        }
    }

    public class NCLevelChangingPresnetationController : MonoBehaviour
    {
        public LocalizedStringTerm levelChangingLocalizedMessage;
//        public string message;
        public UnityEvent onLevelChanged;

        private NCLevelChangingController controller;
        private global::Game gameManager;

        public void Awake()
        {
            controller = ServiceLocator.Find<NeighborhoodChallengeManager>().GetController<NCLevelChangingController>();
            gameManager = global::Game.gameManager;
        }

        public void ChangeLevel()
        {
            ServiceLocator.Find<EventManager>().Propagate(new NeighbourChangeLevelPopupOpenedEvent(), this);
            gameManager.OpenPopup<Popup_ConfirmBox>().Setup(
                string.Format(levelChangingLocalizedMessage, controller.ChangeCost()),
                ScriptLocalization.UI_General.Yes,
                ScriptLocalization.UI_General.No,
                true,
                (c) =>
                {
                    HandleConfirmation(c);
                    ServiceLocator.Find<EventManager>().Propagate(new NeighbourChangeLevelPopupClosedEvent(confirmResult: c), this);
                });


        }

        private void HandleConfirmation(bool isConfiremd)
        {
            if (isConfiremd == false)
                return;

            controller.ChangeLevel(
                onSuccess: () =>
                {
                    onLevelChanged.Invoke();
                    gameManager.OpenPopup<Popup_ConfirmBox>().Setup(ScriptLocalization.Message_NeighborhoodChallenge.LevelChanged, ScriptLocalization.UI_General.Ok, null, true, delegate { });
                },
                onNotEnoughCoin: () =>
                {
                    gameManager.OpenPopup<Popup_ConfirmBox>().Setup(ScriptLocalization.Message_Purchase.NotEnoughCoin, ScriptLocalization.UI_General.Ok, null, true, (confirm) =>
                    {
                        gameManager.shopCreator.TrySetupMainShop("game");
                    });
                });
        }

    }
}