
using Match3.Game.Gameplay.Core;
using System;
using System.Collections.Generic;
using static KitchenParadise.Utiltiy.Base.CollectionExtensions;

namespace Match3.Game.Gameplay.HitManagement
{
    public abstract class SideHitHandler : MechanicHitGenerator
    {
        protected HashSet<CellStack> sideCellStacksCache = new HashSet<CellStack>();

        SideTileFinder sideTileFinder;

        public SideHitHandler(HitGenerationSystem system) : base(system)
        {
            this.sideTileFinder = new SideTileFinder(system.GameBoard());

            sideCellStacksCache.SetCapacity(128,() => new CellStack(0,0));
        }

        protected void GatherSidesOf(HashSet<TileStack> tileStacks, ref HashSet<CellStack> gatheredSides)
        {
            foreach (var tileStack in tileStacks)
                GatherSidesOf(tileStack, ref gatheredSides);
        }

        protected void GatherSidesOf(List<TileStack> tileStacks, ref HashSet<CellStack> gatheredSides)
        {
            foreach (var tileStack in tileStacks)
                GatherSidesOf(tileStack, ref gatheredSides);
        }

        protected void GatherSidesOf(ICollection<CellStack> cellStacks, ref HashSet<CellStack> output)
        {
            // TODO: Check for garbage.
            foreach (var cellStack in cellStacks)
                sideTileFinder.Find(cellStack, ref output);
        }

        protected void TryGenerateSideHitsForGroup(HashSet<CellStack> cellStacks, HitCause hitCause)
        {
            system.BeginHitGroup();

            foreach (var cellStack in cellStacks)
            {
                system.TryGenerateCellStackHit(cellStack, hitCause, HitType.Side);

                if(cellStack.HasTileStack())
                    system.TryGenerateTileStackHit(
                        cellStack.CurrentTileStack(),
                        hitCause,
                        HitType.Side);
            }

            system.EndHitGroup();
        }

        void GatherSidesOf(TileStack tileStack, ref HashSet<CellStack> gatheredSides)
        {
            if (tileStack.IsDepleted() || tileStack.Top().GetComponent<TileHitProperties>().SuppressesHitToSideHit())
                return;

            sideTileFinder.Find(tileStack, ref gatheredSides);
        }
    }
}