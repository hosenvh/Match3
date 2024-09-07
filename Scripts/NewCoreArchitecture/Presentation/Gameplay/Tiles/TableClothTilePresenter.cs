using System;
using System.Collections.Generic;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Matching;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using Match3.Game.Gameplay.Tiles;
using Match3.Presentation.Gameplay.Core;
using RTLTMPro;
using UnityEngine;


namespace Match3.Presentation.Gameplay.Tiles
{
    public class TableClothTilePresenter : TilePresenter
    {
        [Serializable]
        public struct BasketInfo
        {
            public GameObject root;
            public List<GoalTypePrefabDatabase> goalObjects;
            public List<RTLTextMeshPro> counterTexts;
            public List<GameObject> completedVisuals;
        }

        public RectTransform tileTransform;
        public Transform basketTransform;
        public BasketInfo singleBasket;
        public BasketInfo doubleBasket;

        public AnimationPlayer animationPlayer;
        public AnimationClip bounceAnimationClip;
        public AnimationClip removalAnimationClip;

        BasketInfo currentBasket;
        TableClothMainTile tableClothMainTile;

        protected override void InternalSetup()
        {
            tableClothMainTile = tile as TableClothMainTile;

            SetupSize();
            SetupBasket();
            SetupCounters();
        }

        private void OnDestroy()
        {
            tableClothMainTile.firstTarget.onFillChanged -= TryUpdateFirstTargetCounter;
            tableClothMainTile.secondTarget.onFillChanged -= TryUpdateSecondTargetCounter;
        }

        public void PlayRemovelEffect(Action onCompleted)
        {
            animationPlayer.Play(removalAnimationClip, onCompleted);
        }

        private void SetupSize()
        {
            var sizeDelta = tileTransform.sizeDelta;
            sizeDelta = new Vector2(sizeDelta.x * tableClothMainTile.Size().Witdth + 4, sizeDelta.y * tableClothMainTile.Size().Height + 4);
            tileTransform.sizeDelta = sizeDelta;
        }

        private void SetupBasket()
        {
            if (HasTwoGoals(tableClothMainTile))
                currentBasket = doubleBasket;
            else
                currentBasket = singleBasket;

            currentBasket.root.SetActive(true);
            TryActivateGoal(currentBasket, 0, tableClothMainTile.firstTarget);
            TryActivateGoal(currentBasket, 1, tableClothMainTile.secondTarget);
        }

        private void TryActivateGoal(BasketInfo currentBasket, int index, TableClothMainTile.TargetHandler target)
        {
            if (currentBasket.goalObjects.Count > index)
                currentBasket.goalObjects[index].ActivateObjectFor(target.goalType);
        }

        private bool HasTwoGoals(TableClothMainTile tableClothMainTile)
        {
            return tableClothMainTile.secondTarget.isActive;
        }

        private void SetupCounters()
        {
            TryUpdateCounter(0, tableClothMainTile.firstTarget);
            TryUpdateCounter(1, tableClothMainTile.secondTarget);

            tableClothMainTile.firstTarget.onFillChanged += TryUpdateFirstTargetCounter;
            tableClothMainTile.secondTarget.onFillChanged += TryUpdateSecondTargetCounter;
        }

        private void TryUpdateCounter(int index, TableClothMainTile.TargetHandler target)
        {
            if (currentBasket.counterTexts.Count > index && currentBasket.completedVisuals.Count > index)
            {
                animationPlayer.Play(bounceAnimationClip);

                int count = target.targetNumber - target.CurrentFill();
                currentBasket.counterTexts[index].text = count.ToString();

                currentBasket.counterTexts[index].gameObject.SetActive(count != 0);
                currentBasket.completedVisuals[index].SetActive(count == 0);
            }
        }

        private void TryUpdateFirstTargetCounter(int fill)
        {
            TryUpdateCounter(0, tableClothMainTile.firstTarget);
        }

        private void TryUpdateSecondTargetCounter(int fill)
        {
            TryUpdateCounter(1, tableClothMainTile.secondTarget);
        }

        public Transform GoalTransform(Tile tile)
        {
            if (tableClothMainTile.firstTarget.goalType.Includes((GoalObject) tile))
                return currentBasket.goalObjects[0].transform;
            else
                return currentBasket.goalObjects[1].transform;
        }
    }
}
