using System;
using System.Collections.Generic;
using DG.Tweening;
using KitchenParadise.Presentation;
using KitchenParadise.Utiltiy.Base;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Unity.TimeManagement;
using Match3.Game.Gameplay;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.RainbowMechanic;
using Match3.Presentation.Gameplay.Core;
using Match3.Utility.GolmoradLogging;
using PandasCanPlay.HexaWord.Utility;
using UnityEngine;
using UnityEngine.UI;


namespace Match3.Presentation.Gameplay.RainbowGeneration
{
    public class RainbowGeneratorPresenter : MonoBehaviour
    {
        [Serializable]
        private class MovingTilePresentation
        {
            [TypeAttribute(new[] {typeof(Cell), typeof(Tile), typeof(CellAttachment)}, includeAbstracts: false, showPartialName: true)]
            [SerializeField] private string tileType;
            [SerializeField] private GameObject movingVisual;

            public Type TileType => Type.GetType(tileType);
            public GameObject MovingVisual => movingVisual;
        }

        private const float TILE_MOVING_INITIAL_DELAY = 0.5f;

        [SerializeField] private Transform center;
        [SerializeField] private List<Image> fillerImages;
        [SerializeField] private List<MovingTilePresentation> movingTilesPresentations;

        private RainbowGenerationSystem rainbowGenerationSystem;
        private UnityTimeScheduler timeScheduler;

        public void Setup(GameplayController gpc)
        {
            rainbowGenerationSystem = gpc.GetSystem<RainbowGenerationSystem>();
        }

        private void Start()
        {
            timeScheduler = ServiceLocator.Find<UnityTimeScheduler>();
            UpdateFillAmount();
        }

        public void UpdateFillAmount()
        {
            float fillAmount = rainbowGenerationSystem.HasToBeCreatedRainbow() ? 1 : rainbowGenerationSystem.CurrentFillAmount();
            foreach (Image fillerImage in fillerImages)
                fillerImage.fillAmount = fillAmount;
        }

        public void MoveTilePresentationTo(Type tileTypeToMove, CellStack targetCellStack, Transform movementContainer, CurvedMovementInfo movementInfo, Action onPlaced)
        {
            GameObject movingVisualPrefab = GetMovingVisualFor(tileTypeToMove);
            var movingVisual = Instantiate(movingVisualPrefab, movementContainer, worldPositionStays: true);
            movingVisual.gameObject.SetActive(false);

            var presenter = targetCellStack.GetComponent<CellStackPresenter>();

            PlayMovingVisualAnimation();
            ScheduleActivatingVisualOnMovementStart();

            void PlayMovingVisualAnimation()
            {
                movingVisual.transform
                            .DOScale(endValue: 1, duration: 0.3f)
                            .SetDelay(0.8f);

                movingVisual.transform
                            .DOLocalRotate(Vector3.zero, duration: 0.3f)
                            .SetDelay(0.7f);

                movingVisual.transform
                            .DoMoveWithForce(movementInfo.force, movementInfo.forceAngle, presenter.transform.position, movementInfo.speed, false)
                            .SetSpeedBased(true)
                            .SetEase(movementInfo.curve)
                            .SetDelay(TILE_MOVING_INITIAL_DELAY)
                            .OnComplete(ApplyTilePlacement);
            }

            void ScheduleActivatingVisualOnMovementStart()
            {
                timeScheduler.Schedule(
                    TILE_MOVING_INITIAL_DELAY,
                    callback: () =>
                    {
                        if (IsMovingVisualAvailable())
                            movingVisual.SetActive(true);

                        bool IsMovingVisualAvailable() => movingVisual != null;
                    }, this);
            }

            void ApplyTilePlacement()
            {
                Destroy(movingVisual.gameObject);
                onPlaced.Invoke();
            }
        }

        private GameObject GetMovingVisualFor(Type tileTypeToMove)
        {
            movingTilesPresentations.DoRotateShift();

            var movingTilePresentation = movingTilesPresentations.Find(presentation => presentation.TileType == tileTypeToMove);
            if (movingTilePresentation == null)
            {
                DebugPro.LogError<CoreGameplayLogTag>($"While generating tiles from rainbow generation, No MovingVisual find for {tileTypeToMove}");
                movingTilePresentation = movingTilesPresentations[0];
            }

            return movingTilePresentation.MovingVisual;
        }


        public Vector3 Center()
        {
            return center.position;
        }
    }
}