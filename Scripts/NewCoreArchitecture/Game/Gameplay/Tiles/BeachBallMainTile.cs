
using KitchenParadise.Utiltiy.Base;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.HitManagement;
using System;
using System.Collections.Generic;

namespace Match3.Game.Gameplay.Tiles
{
    public class BeachBallMainTile : CompositeTile
    {
        public event Action<TileColor> onColorRemoved = delegate { };

        HashSet<TileColor> remainingColors = new HashSet<TileColor>();

        public BeachBallMainTile(HashSet<TileColor> inistialColors) : base(new Size(2,2))
        {
            remainingColors.AddRange(inistialColors);
        }

        public bool IsReadyToBeDestoryed()
        {
            return remainingColors.IsEmpty();
        }

        protected override void InternalHit(HitType hitType, HitCause hitCause)
        {
            var color = ExtractColorFrom(hitCause);
            remainingColors.Remove(color);
            onColorRemoved(color);
        }

        private TileColor ExtractColorFrom(HitCause hitCause)
        {
            if (hitCause is MatchHitCause matchHitCause)
                return matchHitCause.matchColor;
            if(hitCause is RainbowHitCause rainbowHitCause)
                return rainbowHitCause.targetColor;

            return TileColor.None;
        }

        protected override bool InteralDoesAcceptDirectHit(HitCause hitCause)
        {
            return false;
        }

        protected override bool InteralDoesAcceptSideHit(HitCause hitCause)
        {
            return (hitCause is MatchHitCause matchHitCause && remainingColors.Contains(matchHitCause.matchColor)) ||
                 (hitCause is RainbowHitCause rainbowHitCause && remainingColors.Contains(rainbowHitCause.targetColor));
        }

        public HashSet<TileColor> RemainingColors()
        {
            return remainingColors;
        }

    }
}