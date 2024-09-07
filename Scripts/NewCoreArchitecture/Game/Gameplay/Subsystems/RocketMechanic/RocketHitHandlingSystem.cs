using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystems.ArtifactMechanic;
using Match3.Game.Gameplay.SubSystems.RocketMechanic;
using Match3.Game.Gameplay.SubSystemsData.FrameData.General;
using Match3.Game.Gameplay.SubSystemsData.FrameData.RocketMechanic;
using Match3.Game.Gameplay.SubSystems.RocketBoxMechanic;
using System.Collections.Generic;


namespace Match3.Game.Gameplay.SubSystems.RocketMechanic
{
    // TODO: Make this generic.
    [Before(typeof(HitManagement.HitGenerationSystem))]
    public class RocketHitHandlingSystem : GameplaySystem
    {
        List<CellStack> rocketBoxPendingHits = new List<CellStack>();
        List<CellStack> artifactWithRocketPendingHits = new List<CellStack>();

        public RocketHitHandlingSystem(GameplayController gameplayController) : base(gameplayController)
        {
        }

        // TODO: Refactor this.
        public override void Update(float dt)
        {
            foreach (var cellStack in rocketBoxPendingHits)
            {
                if (cellStack.HasTileStack())
                {
                    var lockState = cellStack.CurrentTileStack().componentCache.lockState;
                    if (lockState.IsLockedBy<RocketBoxActivationKeyType>())
                        lockState.Release();
                }
                GetFrameData<RocketHitData>().cellStacks.Add(cellStack);
            }

            foreach (var cellStack in artifactWithRocketPendingHits)
            {
                if (cellStack.HasTileStack())
                {
                    var lockState = cellStack.CurrentTileStack().componentCache.lockState;
                    if (lockState.IsLockedBy<ArtifactWithRocketActivationSystemKey>())
                        lockState.Release();
                }
                GetFrameData<RocketHitData>().cellStacks.Add(cellStack);
            }

            rocketBoxPendingHits.Clear();
            artifactWithRocketPendingHits.Clear();
        }

        public void HandleRocketBoxHitTo(CellStack target)
        {
            rocketBoxPendingHits.Add(target);
        }

        public void HandleArfiactWithRocketHitTo(CellStack target)
        {
            artifactWithRocketPendingHits.Add(target);
        }
    }
}