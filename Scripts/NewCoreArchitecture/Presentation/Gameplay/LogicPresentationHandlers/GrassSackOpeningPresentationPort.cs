using System;
using System.Collections;
using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystems.GrassSackMechanic;
using Match3.Presentation.Gameplay.Core;
using UnityEngine;

namespace Match3.Presentation.Gameplay.LogicPresentationHandlers
{
    public class GrassSackOpeningPresentationPort : MonoBehaviour, GrassSackOpeningPort
    {

        public float propagationRate;

        public void ProcessArtifactOpening(ArtifactMainCell artifact, Action<ArtifactMainCell> onCompleted)
        {
            var presenter = artifact.GetComponent<CellPresenter>();
            presenter.gameObject.SetActive(false);
            StartCoroutine(Delay(propagationRate * 2, () => { onCompleted(artifact); presenter.gameObject.SetActive(true); }));
        }

        public void ProcessGrassCreatingIn(CellStack cellStack, int distanceFromOrigin, Action<CellStack> onCompleted)
        {
            StartCoroutine(Delay(distanceFromOrigin * propagationRate, () => onCompleted(cellStack)));
        }

        private IEnumerator Delay(float waitTime, Action action)
        {
            yield return new WaitForSeconds(waitTime);
            action.Invoke();
        }
    }
}