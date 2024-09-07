using System;
using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.ExplosionManagement;
using Match3.Game.Gameplay.RainbowMechanic;
using Match3.Game.Gameplay.SubSystems.VacuumCleanerMechanic;
using Match3.Game.Gameplay.Tiles;
using static Match3.Game.Gameplay.ActionUtilites;

namespace Match3.Game.Gameplay.HitManagement.DirectHitHandlers
{
    public class VacuumCleanerDirectHitHandler : MechanicHitGenerator
    {
        struct VacuumCleanerHitCause : HitCause
        { }

        ExplosionActivationGatheringSystem explosionActivationGatheringSystem;
        RainbowActivationGatheringSystem rainbowActivationGatheringSystem;

        public VacuumCleanerDirectHitHandler(HitGenerationSystem system, GameplayController gameplayController) : base(system)
        {
            this.explosionActivationGatheringSystem = gameplayController.GetSystem< ExplosionActivationGatheringSystem>();
            this.rainbowActivationGatheringSystem = gameplayController.GetSystem<RainbowActivationGatheringSystem>();
        }

        public override void GenerateHits()
        {
            foreach (var cellstack in system.GetFrameData<VacuumCleanerHitData>().cellStacks)
            {
                if (cellstack.HasTileStack())
                    TryHitAllTiles(cellstack.CurrentTileStack());

                TryHitAllGrasses(cellstack);
            }
        }

        private void TryHitAllGrasses(CellStack cellStack)
        {
            if (cellStack.Top() is GrassCell grass)
                for (int i = 0; i < grass.CurrentLevel(); ++i)
                    system.ForceGenerateCellStackHitFor(cellStack);
        }

        private void TryHitAllTiles(TileStack currentTileStack)
        {
            foreach (var tile in currentTileStack.Stack())
            {
                if (tile is ExplosiveTile)
                {
                    explosionActivationGatheringSystem.Enqueue(currentTileStack);
                }
                else if (tile is Rainbow)
                {
                    rainbowActivationGatheringSystem.Enqueue(currentTileStack);
                }
                else
                {
                    int hitCount = tile.CurrentLevel();

                    if (tile.GetComponent<VacuumCleanerMechanicProperties>().vacuumCleanerMustHitOnlyOnce)
                        hitCount = 1;

                    for (int i = 0; i < hitCount; ++i)
                    {
                        system.BeginHitGroup();
                        system.TryGenerateTileStackHit(
                            currentTileStack,
                            new VacuumCleanerHitCause(),
                            HitType.Direct);
                        system.EndHitGroup();
                    }
                }
            }
        }
    }
}