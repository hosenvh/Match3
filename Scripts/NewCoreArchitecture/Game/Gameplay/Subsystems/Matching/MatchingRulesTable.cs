using Match3.Game.Gameplay.Core;
using System.Collections;
using System.Collections.Generic;

namespace Match3.Game.Gameplay.Matching
{
    public class MatchingRulesTable
    {

        //List<ColoredMatchingRule> matchRules = new List<ColoredMatchingRule>();
        ColoredMatchingRule coloredMatchinRule;

        public MatchingRulesTable()
        {
            coloredMatchinRule = new ColoredMatchingRule();
        }

        // TODO: Refactor this.
        public bool DoesMatch(TileStack tileStack1, TileStack tileStack2)
        {
            var stack1 = tileStack1.Stack();
            var stack2 = tileStack2.Stack();

            if (stack1.Count == 1 && stack2.Count == 1)
                return coloredMatchinRule.DoesMatch(stack1.Peek(), stack2.Peek());

            
            foreach (var tile1 in stack1)
            {
                foreach (var tile2 in stack2)
                {
                    if (coloredMatchinRule.DoesMatch(tile1, tile2))
                        return true;

                    if (tile2.GetComponent<TileMatchingProperties>(1).allowsMatchFallThrough == false)
                        break;
                }

                if (tile1.GetComponent<TileMatchingProperties>(1).allowsMatchFallThrough == false)
                    break;
            }
            

            return false;
        }

        //private bool DoesMatchBasedOnRules(Tile tile1, Tile tile2)
        //{
        //    return matchRules[0].DoesMatch(tile1, tile2);
        //    //var count = matchRules.Count;
        //    //for (int i = 0; i< count; ++i)
        //    //    if (matchRules[i].IsAppliedOn(tile1, tile2))
        //    //        return matchRules[i].DoesMatch(tile1, tile2);

        //    //return false;
        //}
    }
}