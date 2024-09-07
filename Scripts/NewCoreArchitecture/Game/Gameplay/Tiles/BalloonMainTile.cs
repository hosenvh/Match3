using System.Collections.Generic;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.HitManagement;
using Match3.Game.Gameplay.SubSystems.LevelEnding;


namespace Match3.Game.Gameplay.Tiles
{
    public class BalloonMainTile : Tile, DestructionBasedGoalObject
    {
        private HashSet<BalloonTiedTile> tiedTiles = new HashSet<BalloonTiedTile>();

        public void AddTiedTiles(BalloonTiedTile tiedTile)
        {
            tiedTiles.Add(tiedTile);
        }

        protected override void InternalHit(HitType hitType, HitCause hitCause)
        {
        }

        public bool IsReadyToFly()
        {
            return tiedTiles.Count == 0;
        }

        public HashSet<BalloonTiedTile> GetTiedTiles()
        {
            return tiedTiles;
        }

        public void RemoveTiedTile(BalloonTiedTile tile)
        {
            tiedTiles.Remove(tile);
        }
    }
}