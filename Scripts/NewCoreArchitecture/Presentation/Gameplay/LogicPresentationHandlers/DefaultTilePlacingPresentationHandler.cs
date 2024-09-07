using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Unity.TimeManagement;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystems.TileRandomPlacement;
using Match3.Presentation.Gameplay.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Presentation.Gameplay.LogicPresentationHandlers
{
    // TODO: Refactor this.
    public class DefaultTilePlacingPresentationHandler : MonoBehaviour, DefaultTilePlacerPresentationHandler
    {
        public BoosterCreationEffect effectPrefab;
        public RectTransform targetTransform;


        public float initalDelay;
        public float intervalDelay;
        public float placementOffsetDelay;



        List<CellStack> internalCellStacks;

        Counter counter;

        Action<int> onPlaced;

        bool forceStop;

        public void PlaceAll(List<TileToPlace> tilesToPlace, Action<TileToPlace> onEachTilePlaced, Action onCompleted)
        {
            List<Tile> tiles = new List<Tile>(tilesToPlace.Count);
            List<CellStack> targets = new List<CellStack>(tilesToPlace.Count);

            foreach (TileToPlace toPlace in tilesToPlace)
            {
                tiles.Add(toPlace.tile);
                targets.Add(toPlace.targetCellStack);
            }

            PlaceSequence(tiles, targets, onEachTilePlaced: i => onEachTilePlaced.Invoke(tilesToPlace[i]), onCompleted);
        }

        public void PlaceSequence(List<Tile> tiles, List<CellStack> targets, Action<int> onEachTilePlaced, Action onCompleted)
        {
            PlaceSequenceWithStartDelay(tiles, targets, onEachTilePlaced, onCompleted, initalDelay);
        }

        public void PlaceSequenceWithStartDelay(List<Tile> tiles, List<CellStack> targets, Action<int> onPlaced, Action onCompleted, float startDelay)
        {
            this.onPlaced = onPlaced;
            this.internalCellStacks = new List<CellStack>(targets);
            this.counter = new Counter(targets.Count, onCompleted);

            forceStop = false;

            ServiceLocator.Find<UnityTimeScheduler>().Schedule(startDelay, () => PlaceATile(0), this);
        }

        public void PlaceSingle(Tile tile, CellStack target, Action onCompleted)
        {
            this.internalCellStacks = new List<CellStack> { target };
            this.onPlaced = delegate { };
            this.counter = new Counter(1, onCompleted);

            PlaceATile(0);
        }

        void OnDestroy()
        {
            ServiceLocator.Find<UnityTimeScheduler>().UnSchedule(this);
        }

        private void PlaceATile(int currentIndex)
        {
            var cellStack = internalCellStacks[currentIndex];
            var effect = Instantiate(effectPrefab, targetTransform, false);
            effect.Play();
            effect.transform.position = cellStack.GetComponent<CellStackPresenter>().transform.position;


            ServiceLocator.Find<UnityTimeScheduler>().Schedule(
                placementOffsetDelay,
                () => { onPlaced(currentIndex); counter.Decrement(); },
                this);


            if (forceStop)
                counter.SetAmount(1);
            else if(currentIndex +1 < internalCellStacks.Count)
                ServiceLocator.Find<UnityTimeScheduler>().Schedule(intervalDelay, () => PlaceATile(currentIndex+1), this);
        }

        public void StopPlacing()
        {
            forceStop = true;
        }
    }
}