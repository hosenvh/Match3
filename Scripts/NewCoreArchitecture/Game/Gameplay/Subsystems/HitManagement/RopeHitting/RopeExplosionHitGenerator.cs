
using Match3.Game.Gameplay.CellAttachments;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Tiles;
using Match3.Utility.GolmoradLogging;
using UnityEngine;

namespace Match3.Game.Gameplay.HitManagement
{
    public partial class RopeHitGenerator
    {
        private class RopeExplosionHitGenerator
        {
            RopeHitGenerator mainRopeHitGenerator;

            public RopeExplosionHitGenerator(RopeHitGenerator mainRopeHitGenerator)
            {
                this.mainRopeHitGenerator = mainRopeHitGenerator;
            }

            public void GenerateHitsForExplosions(ExplosionActivationData explosionActivationData, HitGenerationSystem hitGenerationSystem)
            {
                foreach (var explosionData in explosionActivationData.explosionsHitData)
                {
                    hitGenerationSystem.BeginHitGroup();

                    foreach (var hitData in explosionData.hitsData)
                    {
                        var explosiveTile = explosionData.explosiveTile;
                        var explosionCenter = explosiveTile.Parent().Parent().Position();

                        mainRopeHitGenerator.ForeachRopesArroundHitDo(
                            (rope) => ProcessExplosionHit(rope, explosiveTile, explosionCenter, hitData),
                            hitData.cellStack);
                    }
                    hitGenerationSystem.EndHitGroup();
                }
            }

            private void ProcessExplosionHit(Rope rope, ExplosiveTile explosiveTile, Vector2 explosionCenter, DelayedCellHitData delayedHitData)
            {
                Vector2 ropePosition = rope.Owner().Position() + PositionOffsetOf(rope.placement);
                var distance = Mathf.Abs(ropePosition.x - explosionCenter.x)
                    + Mathf.Abs(ropePosition.y - explosionCenter.y);

                if (distance <= explosiveTile.EffectDistance())
                    mainRopeHitGenerator.TryGenerateHitsWithDelay(rope, delayedHitData.delay);
            }

            private Vector2 PositionOffsetOf(GridPlacement placement)
            {
                switch (placement)
                {
                    case GridPlacement.Down:
                        return new Vector2(0, 0.5f);
                    case GridPlacement.Left:
                        return new Vector2(-0.5f, 0);
                    default:
                        DebugPro.LogError<CoreGameplayLogTag>($"Placement {placement} is not supported");
                        return new Vector2(0, 0);
                }
            }
        }
    }
}