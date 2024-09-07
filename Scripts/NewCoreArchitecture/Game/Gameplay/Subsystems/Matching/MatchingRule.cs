using Match3.Game.Gameplay.Core;

namespace Match3.Game.Gameplay.Matching
{
    public interface MatchingRule
    {
        bool IsAppliedOn(Tile tile1, Tile tile2);

        bool DoesMatch(Tile tile1, Tile tile2);

    }
}