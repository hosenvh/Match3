using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystemsData.FrameData.General;
using Match3.Game.Gameplay.SubSystemsData.SessionData;
using Match3.Game.Gameplay.Tiles;
using System.Collections.Generic;

namespace Match3.Game.Gameplay.SubSystems.HoneyMechanic
{
    public class HoneyInBoardExpansionState : HoneyExpansionSystemState
    {
        UserMovementData userMovementData;
        List<CellStack> honeyCellStacks;
        List<CellStack> hardenedHoneyCellStacks;

        public HoneyInBoardExpansionState(HoneyExpansionSystem system, CellStackBoard cellStackBoard, ref List<CellStack> honeyCellStacks, ref List<CellStack> hardenedHoneyCellStacks) : base(system, cellStackBoard)
        {
            this.honeyCellStacks = honeyCellStacks;
            this.hardenedHoneyCellStacks = hardenedHoneyCellStacks;
            userMovementData = system.GetFrameData<UserMovementData>();
        }

        public override void Update()
        {

            if (UserMoved() && NoHoneyIsDestroyedOrBeingDestoryed() && NoHardenedHoneyIsDestroyedOrBeingDestoryed())
            {
                system.GetSessionData<InputControlData>().AddLockedBy<HoneySystemKeyType>();
                system.StartGrowingAHoneyTile();
            }
        }

        public bool UserMoved()
        {
            return userMovementData.moves > 0;
        }

        private bool NoHoneyIsDestroyedOrBeingDestoryed()
        {
            return NoTileIsDestroyed<Honey>() && NoTileIsBeingDestroyedIn(honeyCellStacks);
        }

        private bool NoHardenedHoneyIsDestroyedOrBeingDestoryed()
        {
            return NoTileIsDestroyed<HardenedHoney>() && NoTileIsBeingDestroyedIn(hardenedHoneyCellStacks);
        }

        private bool NoTileIsDestroyed<T>() where T : Tile
        {
            foreach (var tile in system.GetFrameData<DestroyedObjectsData>().tiles)
                if (tile is T)
                    return false;
            return true;

        }

        // NOTE: For now we assume if it is locked, it means it is going to be destroyed.
        bool NoTileIsBeingDestroyedIn(List<CellStack> cellstacks)
        {
            foreach (var cellStack in cellstacks)
                if (cellStack.CurrentTileStack().GetComponent<LockState>().IsLockedBy<HitManagement.HitApplyingKeyType>())
                    return false;

            foreach (var cellStack in cellstacks)
            {
                var pos = cellStack.Position();
                var top = cellStackBoard.DirectionalElementOf(pos, Direction.Up);
                var bottom = cellStackBoard.DirectionalElementOf(pos, Direction.Down);
                var left = cellStackBoard.DirectionalElementOf(pos, Direction.Left);
                var right = cellStackBoard.DirectionalElementOf(pos, Direction.Right);

                if (IsLockedByRainbow(top) || IsLockedByRainbow(bottom) || IsLockedByRainbow(left) || IsLockedByRainbow(right))
                    return false;
            }

            return true;
        }

        private bool IsLockedByRainbow(CellStack cellStack)
        {
            return
                cellStack != null
                && cellStack.HasTileStack()
                && cellStack.CurrentTileStack().GetComponent<LockState>().IsLockedBy<RainbowMechanic.RainbowActivateKeyType>();
        }

        public override void OnEnter()
        {
            
        }
    }
}