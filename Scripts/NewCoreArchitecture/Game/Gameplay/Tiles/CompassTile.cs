using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.HitManagement;
using Match3.Game.Gameplay.SubSystems.LevelEnding;


namespace Match3.Game.Gameplay.Tiles
{
    public class CompassTile : Tile, DestructionBasedGoalObject
    {
        protected override bool InteralDoesAcceptDirectHit(HitCause hitCause)
        {
            return true;
        }

        protected override bool InteralDoesAcceptSideHit(HitCause hitCause)
        {
            return false;
        }
    }
}

