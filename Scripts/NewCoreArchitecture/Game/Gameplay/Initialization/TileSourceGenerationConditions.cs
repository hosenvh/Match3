using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Physics;
using Match3.Game.Gameplay.SubSystems.ButterflyMechanic;
using Match3.Game.Gameplay.SubSystemsData.SessionData;
using Match3.Game.Gameplay.TileGeneration;
using Match3.Game.Gameplay.Tiles;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Match3.Game.Gameplay.QueryUtilities;

namespace Match3.Game.Gameplay.Initialization
{
    public class EmptinessTileSourceGenerationConditinon : TileSourceGenerationCondition
    {
        public bool IsSatisfied(CellStack cellStack, GameplayController gpc)
        {
            return cellStack.HasTileStack() == false;
        }
    }

    public class ButterflyTileSourceGenerationCondition : TileSourceGenerationCondition
    {
        public bool IsSatisfied(CellStack cellStack, GameplayController gpc)
        {
            if (DidntJustFinishedButterflyMovement(gpc))
                return false;

            return
                IsFullyFree(cellStack) &&
                (HasAnyTile(cellStack) == false || TopTile(cellStack).GetComponent<TileButterflyMechanicProperties>().canButterflyBeGeneratedOn);
        }

        private bool DidntJustFinishedButterflyMovement(GameplayController gpc)
        {
            return 
                gpc.GetSystem<ButterflyBoardMovementSystem>().JustFinishedButterflyMovement() == false;
        }
    }

}