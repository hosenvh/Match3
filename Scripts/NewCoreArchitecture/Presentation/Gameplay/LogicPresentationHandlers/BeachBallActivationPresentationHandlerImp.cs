using System;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Unity.TimeManagement;
using Match3.Game.Gameplay.BeachBallMechanic;
using Match3.Game.Gameplay.Tiles;
using Match3.Presentation.Gameplay.Core;
using Match3.Presentation.Gameplay.Tiles;
using UnityEngine;

namespace Match3.Presentation.Gameplay.LogicPresentationHandlers
{
    public class BeachBallActivationPresentationHandlerImp : MonoBehaviour, BeachBallActivationPresentationHandler
    {
        public float activationDelay;
        public void Activate(BeachBallMainTile beachBallMainTile, Action onCompleted)
        {
            ServiceLocator.Find<UnityTimeScheduler>().Schedule(
                activationDelay,
                () => beachBallMainTile.GetComponent<BeachBallTilePresenter>().PlayActivationEffect(onCompleted),
                this);
        }
    }
}