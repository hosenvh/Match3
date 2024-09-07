

using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.HitManagement;
using Match3.Game.Gameplay.SubSystemsData.FrameData.General;
using System.Collections.Generic;

namespace Match3.Game.Gameplay.SubSystems.General
{
    [Before(typeof(HitGenerationSystem))]
    public class GeneralHitActivatingSystem : GameplaySystem
    {
        List<CellStack> pendingCellStackHits = new List<CellStack>();

        GeneralHitData generalHitData;

        public GeneralHitActivatingSystem(GameplayController gameplayController) : base(gameplayController)
        {
            generalHitData = GetFrameData<GeneralHitData>();
        }

        public override void Update(float dt)
        {
            foreach (var cellStack in pendingCellStackHits)
                generalHitData.cellStacksToHit.Add(cellStack);
            
            pendingCellStackHits.Clear();
        }

        public void ActivateHit(CellStack target)
        {
            pendingCellStackHits.Add(target);
        }
    }
}