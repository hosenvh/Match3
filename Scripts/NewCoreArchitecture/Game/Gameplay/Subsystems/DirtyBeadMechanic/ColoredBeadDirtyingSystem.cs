using System;
using Match3.Game.Gameplay.HitManagement;
using Match3.Game.Gameplay.Matching;
using Match3.Game.Gameplay.Tiles;

namespace Match3.Game.Gameplay.SubSystems.GardenMechanic
{
    [After(typeof(MatchDetectionSystem))]
    [Before(typeof(HitGenerationSystem))]
    public class ColoredBeadDirtyingSystem : GameplaySystem
    {

        CreatedMatchesData createdMatchesData;

        public ColoredBeadDirtyingSystem(GameplayController gameplayController) : base(gameplayController)
        {
            createdMatchesData = GetFrameData<CreatedMatchesData>();
        }

        public override void Update(float dt)
        {
            foreach (var match in createdMatchesData.data)
                if (HasDirtyBead(match))
                    MakeAllTopCleanBeadsDirty(match);
        }


        private bool HasDirtyBead(Match match)
        {
            foreach (var tileStack in match.tileStacks)
            {
                var coloredBead = QueryUtilities.FindTile<ColoredBead>(tileStack);
                if (coloredBead != null && coloredBead.IsDirty())
                    return true;
            }

            return false;
        }

        private void MakeAllTopCleanBeadsDirty(Match match)
        {
            foreach (var tileStack in match.tileStacks)
            {
                if (tileStack.Top() is ColoredBead coloredBead)
                    if (coloredBead.IsClean())
                        coloredBead.SetDirtinessState(ColoredBead.DirtinessState.Dirty);
            }
        }

    }
}