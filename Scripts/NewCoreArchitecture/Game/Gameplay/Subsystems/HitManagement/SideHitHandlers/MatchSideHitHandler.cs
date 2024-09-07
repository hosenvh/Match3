using KitchenParadise.Utiltiy.Base;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Matching;
using Match3.Game.Gameplay.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Match3.Game.Gameplay.HitManagement.SideHitHandlers
{

    public class MatchSideHitHandler : SideHitHandler
    {

        public MatchSideHitHandler(HitGenerationSystem system) : base(system)
        {
        }

        public override void GenerateHits()
        {
            foreach (var match in system.GetFrameData<CreatedMatchesData>().data)
            {
                var hitCause = new MatchHitCause(ExtractMatchColor(match));

                sideCellStacksCache.Clear();
                GatherSidesOf(match.tileStacks, ref sideCellStacksCache);

                TryGenerateSideHitsForGroup(sideCellStacksCache, hitCause);
            }
        }

        private TileColor ExtractMatchColor(Match match)
        {
            foreach (var tile in match.tileStacks.FirstElement().Stack())
            {
                var colorComp = tile.componentCache.colorComponent;
                if (colorComp != null)
                    return colorComp.color;
            }

            return TileColor.None;
        }
    }
}