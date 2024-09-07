using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Tiles;

namespace Match3.Game.Gameplay.Matching
{
    class ColoredMatchingRule : MatchingRule
    {
        public bool DoesMatch(Tile tile1, Tile tile2)
        {
            var colorComp1 = tile1.componentCache.colorComponent;
            if (colorComp1 == null)
                return false;

            var colorComp2 = tile2.componentCache.colorComponent;
            if (colorComp2 == null)
                return false;

            return colorComp1.color == colorComp2.color;

        }

        public bool IsAppliedOn(Tile tile1, Tile tile2)
        {
            return tile1 is ColoredBead && tile2 is ColoredBead;
        }
    }
}