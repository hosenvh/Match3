using System;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.HitManagement;
using UnityEngine;


namespace Match3.Game.Gameplay.Tiles
{
    public class BalloonTiedTile : Tile
    {
        public Action onHit = delegate {  };

        private readonly Vector2Int  relationalDirectionFromItsMainBalloonTile;

        public BalloonTiedTile(Vector2Int relationalDirectionFromItsMainBalloonTile)
        {
            this.relationalDirectionFromItsMainBalloonTile = relationalDirectionFromItsMainBalloonTile;
        }

        public Vector2Int GetRelationalDirectionFromItsMainBalloonTile()
        {
            return relationalDirectionFromItsMainBalloonTile;
        }

        protected override void InternalHit(HitType hitType, HitCause hitCause)
        {
            base.InternalHit(hitType, hitCause);
            onHit.Invoke();
        }
    }
}