

using System;
using System.Collections.Generic;
using KitchenParadise.Utiltiy.Base;
using Match3.Game.Gameplay.Core;

namespace Match3.Game.Gameplay.TileGeneration
{
    public class SpecialTileTacker
    {
        GameplayController gameplayController;

        Dictionary<Type, int> tilesCurrentAmount = new Dictionary<Type, int>();
        Dictionary<Type, int> tileRemainingAmount = new Dictionary<Type, int>();
        Dictionary<Type, int> remainingTurns = new Dictionary<Type, int>();

        HashSet<Type> typesToTrack = new HashSet<Type>();
        Dictionary<Type, SpecialTileGenerationInfo> typeGenerationInfos = new Dictionary<Type, SpecialTileGenerationInfo>();

        HashSet<Type> alwaysReadyForGenerationTypes = new HashSet<Type>();

        public void Setup(List<SpecialTileGenerationInfo> generationInfos, CellStackBoard cellStackBoard)
        {
            generationInfos.ForEach(l => typesToTrack.Add(l.tileType));

            foreach(var generationInfo in generationInfos)
            {
                var type = generationInfo.tileType;
                typesToTrack.Add(type);
                tilesCurrentAmount[type] = 0;
                typeGenerationInfos[type] = generationInfo;

                if (generationInfo.minGenerationTurn == 0 && generationInfo.maxGenerationTurn == 0)
                    alwaysReadyForGenerationTypes.Add(type);
            }

            foreach(var element in new LeftToRightTopDownGridIterator<CellStack>(cellStackBoard))
            {
                if (element.value.HasTileStack() == false)
                    continue;

                foreach (var tile in element.value.CurrentTileStack().Stack())
                {
                    var tileType = tile.GetType();
                    if (typesToTrack.Contains(tileType))
                        tilesCurrentAmount[tileType]++;
                }
            }


            foreach (var limit in generationInfos)
                tileRemainingAmount[limit.tileType] = limit.inLevelLimit - tilesCurrentAmount[limit.tileType];

        }

        public void UpdateRemovals(DestroyedObjectsData destroyedTileStacksData)
        {
            foreach (var tile in destroyedTileStacksData.tiles)
            {
                if (typesToTrack.Contains(tile.GetType()))
                    tilesCurrentAmount[tile.GetType()]--;
            }
        }

        public bool DoesTileTypeNeedsGeneration(Type type)
        {
            return IsBelowInBoardLimit(type) && NotReachedLevelLimit(type);
        }
        public bool CanTileTypeBeGenrated(Type type)
        {
            return DoesTileTypeNeedsGeneration(type) && IsTurnFor(type);
        }

        private bool IsTurnFor(Type type)
        {
            if (alwaysReadyForGenerationTypes.Contains(type))
                return true;

            return remainingTurns.ContainsKey(type) && remainingTurns[type] <= 0;
        }

        bool IsBelowInBoardLimit(Type type)
        {
            return tilesCurrentAmount[type] < typeGenerationInfos[type].inBoardLimit;
        }

        bool NotReachedLevelLimit(Type type)
        {
            return tileRemainingAmount[type] > 0;
        }

        public void TryIncrementTileAmount(Type type)
        {
            if (typesToTrack.Contains(type))
            {
                tilesCurrentAmount[type]++;
                tileRemainingAmount[type]--;
                remainingTurns.Remove(type);
            }
        }

        public void TryStartGenerationTurnFor(Type type)
        {
            if (remainingTurns.ContainsKey(type))
                return;

            var info = typeGenerationInfos[type];
            var turns = UnityEngine.Random.Range(info.minGenerationTurn, info.maxGenerationTurn + 1);
            remainingTurns[type] = turns;

        }

        public void AdvanceTurnsFor(Type tileType)
        {
            if(remainingTurns.ContainsKey(tileType))
                remainingTurns[tileType]--;
        }
    }
}