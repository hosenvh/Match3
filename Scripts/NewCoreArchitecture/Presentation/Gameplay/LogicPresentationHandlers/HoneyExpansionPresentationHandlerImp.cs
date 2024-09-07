using System;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystems.HoneyMechanic;
using Match3.Presentation.Gameplay.Core;
using UnityEngine;

namespace Match3.Presentation.Gameplay.LogicPresentationHandlers
{

    public class HoneyExpansionPresentationHandlerImp : MonoBehaviour, HoneyExpansionPresentationHandler
    {
        public HoneyExpansionEffect honeyExpansionEffectPrefab;

        public Transform targetTransform;

        public void HandleGrowth(CellStack cellstack, Action onCompleted)
        {
            var effect = Instantiate(honeyExpansionEffectPrefab, targetTransform, false);
            effect.transform.position = cellstack.GetComponent<CellStackPresenter>().transform.position;

            effect.Play(onCompleted);
        }
    }
}