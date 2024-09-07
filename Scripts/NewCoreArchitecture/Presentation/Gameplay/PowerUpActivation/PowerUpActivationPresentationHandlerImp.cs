using System;
using System.Collections;
using System.Collections.Generic;
using Match3.Game.Gameplay.Core;
using Match3.Presentation.Gameplay.Core;
using UnityEngine;

namespace Match3.Presentation.Gameplay.PowerUpActivation
{
    public class PowerUpActivationPresentationHandlerImp : MonoBehaviour, Game.Gameplay.SubSystems.PowerUpManagement.PowerUpActivationPresentationHandler
    {

        public HammerPowerUpEffect hammerPowerUpEffect;
        public BroomPowerUpEffect broomPowerUpEffect;
        public HandPowerUpEffect handPowerUpEffect;

        public RectTransform targetContainer;


        public void HandleHammer(CellStack target, Action onCompleted)
        {
            var effect = Instantiate(hammerPowerUpEffect, targetContainer, false);
            effect.transform.position = target.GetComponent<CellStackPresenter>().transform.position;
            effect.Play(onCompleted);
        }


        public void HandleBroom(List<CellStack> horizontals, List<CellStack> verticals, Action<CellStack> onCompletedOnStack, Action onCompleted)
        {
            var effect = Instantiate(broomPowerUpEffect, targetContainer, false);
            effect.Play(horizontals, verticals, onCompletedOnStack, onCompleted);
        }

        public void HandleHand(CellStack origin, CellStack destination, Action onCompleted)
        {
            var effect = Instantiate(handPowerUpEffect, targetContainer, false);
            effect.Play(origin, destination, onCompleted);
        }
    }
}