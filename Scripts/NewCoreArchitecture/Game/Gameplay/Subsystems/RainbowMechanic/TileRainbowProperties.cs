

using Match3.Foundation.Base.ComponentSystem;

namespace Match3.Game.Gameplay.RainbowMechanic
{
    public class TileRainbowProperties : Component
    {
        public enum MatchingRule
        {
            None,
            MatchWithRainbowNoOtherMatches,
            MatchWithRainbowFindOtherMatchesBySameColor,
            MatchWithRainbowFindOtherMatchesBySameType
        }

        public readonly MatchingRule matchingRule;

        public TileRainbowProperties(MatchingRule matchingRule)
        {
            this.matchingRule = matchingRule;
        }

        public bool DoesMatchWithRainbow()
        {
            return matchingRule != MatchingRule.None;
        }
    }
}