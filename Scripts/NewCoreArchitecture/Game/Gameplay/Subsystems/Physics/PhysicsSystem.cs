using System;
using System.Collections.Generic;
using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Core;
using UnityEngine;

namespace Match3.Game.Gameplay.Physics
{

    public interface PhysicsKeyType : KeyType {}

    // TODO: Refactor the entire physis management.
    public partial class PhysicsSystem : GameplaySystem
    {
        const float FALL_ACCELERATION = 98;

        CellStackBoard cellStackBoard;
        GameBoard gameBoard;

        HashSet<CellStack> downwardBlockedCells = new HashSet<CellStack>();

        HashSet<Type> sidefallPriorities = new HashSet<Type>();


        DownwardFlowCalculator downWardFlowCalculator = new DownwardFlowCalculator();
        GravityEffectUpdater gravityEffectUpdater = new GravityEffectUpdater();
        TilePositionUpdater tilePositionUpdater = new TilePositionUpdater();


        public PhysicsSystem(GameplayController gameplayController) : base(gameplayController)
        {
            this.gameBoard = gameplayController.GameBoard();
            this.cellStackBoard = gameplayController.GameBoard().CellStackBoard();

            downWardFlowCalculator.Setup(this);
            gravityEffectUpdater.Setup(this);
            tilePositionUpdater.Setup(this);
        }

        public void SetSideFallPriorityFor<T>() where T : Tile
        {
            sidefallPriorities.Add(typeof(T));
        }

        public override void Update(float dt)
        {
            downWardFlowCalculator.Clear();
            gravityEffectUpdater.Update();
            tilePositionUpdater.Update(dt);
        }

        public bool CalculateCanTileStackFallThrough(CellStack cellStack)
        {
            return gravityEffectUpdater.CalculateCanTileStackFallThrough(cellStack);
        }

        public bool IsDownwardFlowStoppedFor(Vector2Int positon)
        {
            return downWardFlowCalculator.IsDownwardFlowStoppedFor(positon);
        }

        bool DoesBlockFlow(CellStack cellStack)
        {
            return HasBlockingTile(cellStack) || !IsCrossable(cellStack) || downwardBlockedCells.Contains(cellStack);
        }

        public void UpdatePhysicBlockageFor(CellStack owner)
        {
            gravityEffectUpdater.UpdatePhysicBlockageFor(owner);
        }

        bool HasBlockingTile(CellStack cellStack)
        {
            var currentTileStack = cellStack.CurrentTileStack();
            return currentTileStack != null
                && currentTileStack.IsDepleted() == false
                && currentTileStack.Top().GetComponent<TilePhysicalProperties>().isAffectedByGravity == false;
        }

        bool CanTileFallThrough(CellStack cellStack)
        {
            if (downwardBlockedCells.Contains(cellStack))
                return false;

            if (cellStack == null || cellStack.HasTileStack() || cellStack.GetComponent<LockState>().IsLocked())
                return false;

            var top = cellStack.Top();
 
            if (top.CanContainTile())
                return true;
            else
                return top.GetComponent<CellPhysicalProperties>().allowTileFallThrough;
        }

        bool IsCrossable(CellStack cellStack)
        {
            if (cellStack == null)
                return false;
            var top = cellStack.Top();
            return top.CanContainTile() || top.GetComponent<CellPhysicalProperties>().allowTileFallThrough;
        }

        bool IsProritizeForSideFalling(Tile tile)
        {
            return sidefallPriorities.Contains(tile.GetType());
        }

    }
}