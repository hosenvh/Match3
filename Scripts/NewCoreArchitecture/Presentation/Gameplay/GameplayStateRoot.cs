using UnityEngine;
using UnityEngine.UI;
using Match3.Presentation.Gameplay.Core;
using Match3.Presentation.Gameplay.PowerUpActivation;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using System;

namespace Match3.Presentation.Gameplay
{
    public class GameplayStateRoot : MonoBehaviour
    {
        public GameplayState gameplayState;

        public LevelStatePresentationController levelStatePresentationController;
        public CanvasScaler gameCanvasScaler;
        public CanvasScaler otherCanvasScaler;
        public CanvasScaler backCanvasScaler;
        public BoardPresenterNew boardPresenter;
        public PowerUpContainer powerUpContainer;
        public GameObject generalPresentationHandlersContainer;
        public HedgesBounceController hedgesBounceController;

        [Space(10)]
        public LevelDifficultyUIController levelDifficultyUIController;

        public void OnFinishGameClick()
        {
            gameplayState.OnFinishGameClick();
        }
    }
}