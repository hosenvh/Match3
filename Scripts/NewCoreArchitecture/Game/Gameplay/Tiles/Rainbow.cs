
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.HitManagement;
using Match3.Game.Gameplay.SubSystems.LevelEnding;

namespace Match3.Game.Gameplay.Tiles
{
    public class Rainbow : Tile, DestructionBasedGoalObject
    {
        private bool isActivated;

        protected override bool InteralDoesAcceptDirectHit(HitCause hitCause)
        {
            return false;
        }

        protected override bool InteralDoesAcceptSideHit(HitCause hitCause)
        {
            return false;
        }

        public void MarkAsActivated()
        {
            isActivated = true;
        }

        public bool IsActivated()
        {
            return isActivated;
        }
    }
}