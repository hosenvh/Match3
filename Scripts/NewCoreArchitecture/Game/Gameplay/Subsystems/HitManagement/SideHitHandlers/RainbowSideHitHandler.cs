using System;
using System.Collections.Generic;
using KitchenParadise.Utiltiy.Base;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Matching;
using Match3.Game.Gameplay.SubSystemsData.FrameData.RainbowMechanic;
using Match3.Game.Gameplay.Tiles;

namespace Match3.Game.Gameplay.HitManagement.SideHitHandlers
{
    public class RainbowSideHitHandler : SideHitHandler
    {
        public RainbowSideHitHandler(HitGenerationSystem system) : base(system)
        {
        }


        public override void GenerateHits()
        {
            var activationData = system.GetFrameData<RainbowActivationData>();

            if (IsEmpty(activationData))
                return;

            var hitCause = new RainbowHitCause(RainbowColorExtractor.ExtractFrom(activationData));

            sideCellStacksCache.Clear();

            GatherSidesOf(activationData.tileStackHits, ref sideCellStacksCache);
            GatherSidesOf(activationData.activatedRainbowCellStacks, ref sideCellStacksCache);

            TryGenerateSideHitsForGroup(sideCellStacksCache, hitCause);
        }

        private bool IsEmpty(RainbowActivationData activationData)
        {
            return activationData.tileStackHits.IsEmpty() && activationData.activatedRainbowCellStacks.IsEmpty();
        }
    }
}