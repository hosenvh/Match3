using Match3.Game.Gameplay.RainbowMechanic;
using Match3.Game.Gameplay.Tiles;
using Match3.Presentation.Gameplay.Core;
using System;
using UnityEngine;
using DG.Tweening;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Unity.TimeManagement;
using Match3.Game.Gameplay.Core;
using System.Collections.Generic;
using System.Collections;
using Match3.Presentation.Gameplay.Tiles;
using KitchenParadise.Utiltiy.Base;
using Match3.Game.Gameplay.Matching;
using Match3.Foundation.Unity.ObjectPooling;
using Match3.Game.Gameplay;
using Match3.Utility.GolmoradLogging;


namespace Match3.Presentation.Gameplay.RainbowGeneration
{
    public class ExplosionComboPresenterPool : UnityComponentObjectPool<RainbowExplosionComboPresenter>
    {
        protected override void ActiveObject(RainbowExplosionComboPresenter obj)
        {
            
        }

        protected override void DeactiveObject(RainbowExplosionComboPresenter obj)
        {
            obj.gameObject.transform.SetParent(targetTransform, false);
        }
    }

    public class RainbowPresentationHandler :
        MonoBehaviour,
        RainbowGenerationPresentationHandler,
        RainbowActivationPresentationHandler
    {
        public Transform outSideTransform;
        public RainbowExplosionComboPresenter comboPrefab;
        public float eneryMovementSpeed;
        public CurvedMovementInfo rainbowMovementInfo;

        public RainbowGeneratorPresenter rainbowGeneratorPresenter;
        public RainbowTargetActivationEffect rainbowTargetActivationEffectPrefab;
        public ParticleSystem doubleRainbowExplosionParticlePrefab;

        public ParticleSystem eneryParticlePrefab;
        public RectTransform effectContainer;

        GameplaySoundManager soundManager;

        ExplosionComboPresenterPool comboPresneterPool = new ExplosionComboPresenterPool();


        void Awake()
        {
            soundManager = ServiceLocator.Find<GameplaySoundManager>();

            comboPresneterPool.SetComponentPrefab(comboPrefab);
            comboPresneterPool.SetPoolTransform(outSideTransform);
            comboPresneterPool.Reserve(1);
        }


        public void HandleEnergyFlow(ExplosiveTile tile, Action onCompleted)
        {
            var effect = Instantiate(eneryParticlePrefab, this.effectContainer, false);
            effect.transform.position = tile.GetComponent<TilePresenter>().transform.position;

            effect.transform.
                DOMove(rainbowGeneratorPresenter.Center(), eneryMovementSpeed).
                SetSpeedBased(true).
                SetDelay(0.5f).
                OnComplete(() => ApplyEneryEffect(effect, onCompleted));
        }

        void OnDestroy()
        {
            ServiceLocator.Find<UnityTimeScheduler>().UnSchedule(this);
        }


        public void HandleCombo(Tile tile, int comboNumber)
        {
            if (comboNumber <= 1)
                return;

            var combo = comboPresneterPool.Acquire();
            combo.transform.SetParent(effectContainer);
            combo.transform.position = tile.Parent().GetComponent<TileStackPresenter>().transform.position;
            combo.Setup(comboNumber, () => comboPresneterPool.Release(combo));

        }

        void ApplyEneryEffect(ParticleSystem effect, Action onCompleted)
        {
            effect.Stop();
            ServiceLocator.Find<UnityTimeScheduler>().Schedule(1.5f, () => Destroy(effect.gameObject), this);
            onCompleted();
            rainbowGeneratorPresenter.UpdateFillAmount();
        }

        public void MoveTo(Type tileTypeToMove, CellStack target, Action onPlacedAction)
        {
            rainbowGeneratorPresenter.MoveTilePresentationTo(tileTypeToMove, target, effectContainer,rainbowMovementInfo, onPlaced: () =>
            {
                onPlacedAction.Invoke();
                rainbowGeneratorPresenter.UpdateFillAmount();
            });
            rainbowGeneratorPresenter.UpdateFillAmount();
        }

        public void HandleSingleRainbowActivation(Rainbow rainbow, List<TileStack> targets, Action onCompleted)
        {
            StartCoroutine(CreateSingleRainbowActivationEffect(rainbow, targets, onCompleted));
        }


        public void HandleDoubleRainbowActivation(Rainbow rainbow1, Rainbow rainbow2, Action onCompleted)
        {

            StartCoroutine(CreateDoubleRainbowActivationEffect(rainbow1, rainbow2, onCompleted));
        }

        IEnumerator CreateSingleRainbowActivationEffect(Rainbow rainbow, List<TileStack> targets, Action onCompleted)
        {

            soundManager.PlaySingleRainbowChargeEffectFor(rainbow);

            List<RainbowTargetActivationEffect> effects = new List<RainbowTargetActivationEffect>();
            var effectWait = new WaitForSeconds(0.05f);
            var rotation = Quaternion.identity;

            var rainbowPresenter = rainbow.GetComponent<RainbowTilePresenter>();
            rainbowPresenter.PlayActivation();

            var rainbowTileEffect = Instantiate(rainbowTargetActivationEffectPrefab, rainbowPresenter.transform.position, rotation, effectContainer);
            Color color = Color.white;
            if (targets.Count > 0)
                color = ColorFor(targets[0]);
            rainbowTileEffect.Setup(rainbowPresenter.transform.position, color);
            effects.Add(rainbowTileEffect);

            targets.Shuffle();
            foreach (var target in targets)
            {
                if (target.IsDestroyed())
                {
                    DebugPro.LogError<CoreGameplayLogTag>($"Target {target.Position()} is already destroyed");
                    continue;
                }

                var effect = Instantiate(rainbowTargetActivationEffectPrefab, rainbowPresenter.transform.position, rotation, effectContainer);
                //effect.transform.position = rainbowPresenter.transform.position;
                effect.Setup(target.GetComponent<TileStackPresenter>().transform.position, color);
                effects.Add(effect);
                yield return effectWait;
            }

            yield return new WaitForSeconds(0.7f);

            foreach (var effect in effects)
                effect.Explode();

            soundManager.PlaySingleRainbowExplosionEffectFor(rainbow);

            onCompleted();
        }


        IEnumerator CreateDoubleRainbowActivationEffect(Rainbow rainbow1, Rainbow rainbow2, Action onCompleted)
        {
            List<RainbowTargetActivationEffect> effects = new List<RainbowTargetActivationEffect>();
            var rotation = Quaternion.identity;

            var rainbow1Presenter = rainbow1.GetComponent<RainbowTilePresenter>();
            rainbow1Presenter.PlayActivation();
            var rainbow2Presenter = rainbow2.GetComponent<RainbowTilePresenter>();
            rainbow2Presenter.PlayActivation();

            var rainbow1TileEffect = Instantiate(rainbowTargetActivationEffectPrefab, rainbow1Presenter.transform.position, rotation, effectContainer);
            var rainbow2TileEffect = Instantiate(rainbowTargetActivationEffectPrefab, rainbow2Presenter.transform.position, rotation, effectContainer);


            rainbow1TileEffect.Setup(rainbow1Presenter.transform.position, Color.white);
            rainbow2TileEffect.Setup(rainbow2Presenter.transform.position, Color.white);

            effects.Add(rainbow1TileEffect);
            effects.Add(rainbow2TileEffect);

            soundManager.PlayDoubleRainbowChargeEffectFor(rainbow1);

            yield return new WaitForSeconds(0.7f);

            PlayDoubleRainbowExplosionFor(rainbow1Presenter, rainbow2Presenter);

            foreach (var effect in effects)
                effect.Explode();

            soundManager.PlayDoubleRainbowExplosionEffectFor(rainbow1);

            onCompleted();
        }

        private void PlayDoubleRainbowExplosionFor(RainbowTilePresenter rainbow1Presenter, RainbowTilePresenter rainbow2Presenter)
        {
            var particle = Instantiate(doubleRainbowExplosionParticlePrefab, effectContainer, false) as ParticleSystem;

            var main = particle.main;
            main.stopAction = ParticleSystemStopAction.Destroy;

            particle.transform.position = (rainbow1Presenter.transform.position + rainbow2Presenter.transform.position) / 2;
        }


        // TODO: Refactor this.
        private Color ColorFor(TileStack tileStack)
        {
            Color color = Color.white;

            TileColor tileColor = TileColor.Blue;

            foreach (var tile in tileStack.Stack())
            {
                if (tile is Nut)
                    return color;

                var colorComp = tile.GetComponent<TileColorComponent>();
                if (colorComp != null)
                {
                    tileColor = colorComp.color;
                    break;
                }


            }



            switch (tileColor)
            {
                case TileColor.Blue:
                    color = Color.blue;
                    break;
                case TileColor.Green:
                    color = Color.green;
                    break;
                case TileColor.Pink:
                    color = new Color(1f, .44f, .82f);
                    break;
                case TileColor.Purple:
                    color = new Color(.88f, 0f, .96f);
                    break;
                case TileColor.Red:
                    color = Color.red;
                    break;
                case TileColor.Yellow:
                    color = Color.yellow;
                    break;
            }

            return color;
        }
    }
}