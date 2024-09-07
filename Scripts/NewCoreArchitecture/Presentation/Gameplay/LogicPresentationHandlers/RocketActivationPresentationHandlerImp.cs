using System;
using System.Collections.Generic;
using KitchenParadise.Utiltiy.Base;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystems.ArtifactMechanic;
using Match3.Game.Gameplay.SubSystems.RocketBoxMechanic;
using Match3.Game.Gameplay.Tiles;
using Match3.Presentation.Gameplay.Core;
using Match3.Presentation.Gameplay.Tiles;
using UnityEngine;

namespace Match3.Presentation.Gameplay.LogicPresentationHandlers
{

    public class RocketActivationPresentationHandlerImp : MonoBehaviour, RocketBoxActivationPresentationHandler, ArtifactWithRocketActivationPort
    {
        public BoardPresenterNew boardPresenter;

        public float rocketSpeed;
        public float artifactRocketDelay;
        public Vector2 artifactRocketOffsetBound;
        public Transform targetTransform;

        public GameObject rocketTargetIconPrefab;
        public ParticleSystem explosionParticlePrefab;

        public AudioClip explosionAudioClip;

        public ParticleSystem artifactExplosionParticlePrefab;
        public float artifactExplosionSFXDelay;
        public List<SeekingRocketPresenter> rockets;


        void RocketBoxActivationPresentationHandler.Activate(
            RocketBox rocketBox, 
            List<CellStack> rocketTargets, 
            Action<CellStack> onTargetHit,
            Action onRocketBoxDepleted)
        {
            var tilePresenter = rocketBox.GetComponent<RocketBoxTilePresenter>();

            tilePresenter.PlayActivationEffect(
                () => OnRocketBoxActivationEffectCompleted(tilePresenter, rocketTargets, onTargetHit, onRocketBoxDepleted));
        }

        void ArtifactWithRocketActivationPort.HandleRocketActivationTo(ArtifactWithRocketMainCell artifactCell, List<CellStack> targets, Action<CellStack> onTargetHit)
        {
            this.Wait(artifactRocketDelay, () => CreateRocketsForArtifact(artifactCell, targets, onTargetHit));
        }

        private void CreateRocketsForArtifact(ArtifactWithRocketMainCell artifactCell, List<CellStack> targets, Action<CellStack> onTargetHit)
        {
            var size = artifactCell.Size();
            var logicalCenter = artifactCell.Parent().Position() + new Vector2(size.Witdth / 2f - 0.5f, size.Height / 2f - 0.5f);
            var presentationCenter = boardPresenter.BoardDimensions().LogicalPosToPresentaionalPos(logicalCenter);

            // TODO: Find a better way to sync the explosion sound with animation.
            this.Wait(artifactExplosionSFXDelay, () => PlaySound(explosionAudioClip));

            var particle = Instantiate(artifactExplosionParticlePrefab, targetTransform);

            particle.transform.position = presentationCenter;
            particle.transform.localScale = particle.transform.localScale * artifactCell.Scale();

            foreach (var target in targets)
            {
                var seekingRocket = Instantiate(rockets.RandomElement(), targetTransform);
                seekingRocket.transform.position =
                    presentationCenter
                    + new Vector2(UnityEngine.Random.Range(0, artifactRocketOffsetBound.x), UnityEngine.Random.Range(0, artifactRocketOffsetBound.y));
                InitiateRocket(target, seekingRocket, onTargetHit);
            }
        }

        private void OnRocketBoxActivationEffectCompleted(RocketBoxTilePresenter presenter, List<CellStack> rocketTargets, Action<CellStack> onTargetHit, Action onRocketBoxDepleted)
        {
            for(int i = 0; i< rocketTargets.Count; i++)
                InitiateRocket(rocketTargets[i], presenter.seekingRocketPresenters[i], onTargetHit);

            onRocketBoxDepleted();
        }

        private void InitiateRocket(CellStack target, SeekingRocketPresenter seekingRocket, Action<CellStack> onTargetHit)
        {
            var targetIcon = Instantiate(rocketTargetIconPrefab, targetTransform, false);
            var targetPosition = target.GetComponent<CellStackPresenter>().transform.position;
            targetIcon.transform.position = targetPosition;

            seekingRocket.transform.SetParent(targetTransform);
            seekingRocket.Move(targetPosition, rocketSpeed, () => HandleRocketReached(target, targetIcon, onTargetHit));
        }

        private void HandleRocketReached(CellStack target, GameObject targetIcon, Action<CellStack> onTargetHit)
        {
            Destroy(targetIcon);

            var explosionParticle = Instantiate(explosionParticlePrefab, targetTransform, false);
            explosionParticle.transform.position = target.GetComponent<CellStackPresenter>().transform.position;
            explosionParticle.gameObject.SetActive(true);
            explosionParticle.Play();
            ServiceLocator.Find<GameplaySoundManager>().TryPlay(explosionAudioClip);
            onTargetHit(target);
        }

        private void PlaySound(AudioClip audioClip)
        {
            ServiceLocator.Find<GameplaySoundManager>().TryPlay(audioClip);
        }

    }
}