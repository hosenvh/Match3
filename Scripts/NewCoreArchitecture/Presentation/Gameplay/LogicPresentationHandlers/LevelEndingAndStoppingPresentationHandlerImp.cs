using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Unity.TimeManagement;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using Match3.Presentation.Gameplay.GoalManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using Match3.Game.Effects;
using UnityEngine;

namespace Match3.Presentation.Gameplay.LogicPresentationHandlers
{
    public class LevelEndingAndStoppingPresentationHandlerImp : MonoBehaviour, LevelEndingPresentationHandler, LevelStoppingPresentationHandler
    {
        public GameplayStateRoot gameplayStateRoot;
        public GainedCoinsPresentationController coinsPresentationController;
        public ClosingBoardEffectController closingBoardEffectController;

        Popup_TouchDisabler touchDisabler;

        public void HandleGameLost(StopConditinon losingCause)
        {
            ServiceLocator.Find<UnityTimeScheduler>().Schedule(
            0.8f, () =>
            {
                global::Base.gameManager.ClosePopup(touchDisabler);
                gameplayStateRoot.gameplayState.HandleGameEnding(LevelResult.Lose, losingCause, -1);
            },
            this);
        }

        public void HandleGamePreWin(Action onComepleted)
        {
            ServiceLocator.Find<UnityTimeScheduler>().Schedule(0.5f, onComepleted, this);
        }

        public void HandleGameWin(int finalScore)
        {
            coinsPresentationController.UpdateCoins(isFinal: true);

            var boardSize = gameplayStateRoot.gameplayState.gameplayController.GameBoard().CellStackBoard().Size();

            closingBoardEffectController.CloseBoard(boardSize, () =>
            {
                ServiceLocator.Find<UnityTimeScheduler>().Schedule(
                    0.5f, () =>
                    {
                        global::Base.gameManager.ClosePopup(touchDisabler);
                        gameplayStateRoot.gameplayState.HandleGameEnding(LevelResult.Win, null, finalScore);
                    },
                    this);
            });

        }

        public void HandleRewardDoubling(int addedScore)
        {
            gameplayStateRoot.gameplayState.HandleRewardDoubling(addedScore);
        }

        public void HandleStopping()
        {
            touchDisabler = global::Base.gameManager.OpenPopup<Popup_TouchDisabler>();
        }
    }
}