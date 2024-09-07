using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystemsData.FrameData.ExplosionManagement;
using System.Collections.Generic;
using static Match3.Game.Gameplay.QueryUtilities;

namespace Match3.Game.Gameplay.ExplosionManagement
{
    public class ExplosionActivationGatheringSystem : GameplaySystem
    {
        List<TileStack> gatheredTileStacks = new List<TileStack>();
        List<DelayedActivationData> delayedGatheredTileStakcs = new List<DelayedActivationData>();
        public ExplosionActivationGatheringSystem(GameplayController gameplayController) : base(gameplayController)
        {
        }

        public override void Update(float dt)
        {
            for (int i = gatheredTileStacks.Count - 1; i >=0; --i)
            {
                var tileStack = gatheredTileStacks[i];
                if (IsFree(tileStack))
                {
                    GetFrameData<InternalExplosionActivationData>().targets.Add(tileStack);
                    gatheredTileStacks.RemoveAt(i);
                }
            }

            for (int i = delayedGatheredTileStakcs.Count - 1; i >= 0; --i)
            {
                var data = delayedGatheredTileStakcs[i];

                GetFrameData<InternalExplosionActivationData>().delayedTargets.Add(data);
                delayedGatheredTileStakcs.RemoveAt(i);
            }
        }

        public void Enqueue(TileStack tileStack)
        {
            gatheredTileStacks.Add(tileStack);
        }

        public void Enqueue(TileStack tileStack, float delay)
        {
            delayedGatheredTileStakcs.Add(new DelayedActivationData(tileStack, cause: null, delay));
        }
    }
}