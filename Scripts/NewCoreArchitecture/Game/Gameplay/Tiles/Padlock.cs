
using System;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.HitManagement;

namespace Match3.Game.Gameplay.Tiles
{
    public class Padlock : Tile
    {
        public enum Status
        {
            Opened,
            Closed,
        }
        bool canBeTotallyDestored;

        public Padlock(Status status) : base(initialLevel: status == Status.Opened? 1 : 2)
        {
            canBeTotallyDestored = false;
        }

        protected override bool InteralDoesAcceptDirectHit(HitCause hitCause)
        {
            return canBeTotallyDestored == true || CurrentLevel() > 1 ;
        }

        protected override bool InteralDoesAcceptSideHit(HitCause hitCause)
        {
            return canBeTotallyDestored == true || CurrentLevel() > 1;
        }

        public bool IsLocked()
        {
            return this.CurrentLevel() > 1;
        }

        public void EnableTotalDestruction()
        {
            canBeTotallyDestored = true;
        }
    }
}