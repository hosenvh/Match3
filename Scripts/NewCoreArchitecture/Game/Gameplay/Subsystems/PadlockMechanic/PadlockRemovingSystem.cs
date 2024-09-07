using Match3.Game.Gameplay.Tiles;
using System;
using System.Collections.Generic;
using Match3.Utility.GolmoradLogging;
using static Match3.Game.Gameplay.QueryUtilities;

namespace Match3.Game.Gameplay.SubSystems.PadlockMechanic
{

    [Before(typeof(HitManagement.HitGenerationSystem))]
    public class PadlockRemovingSystem : GameplaySystem
    {
        List<Padlock> allPadlocks = new List<Padlock>();
        int lockedPadlocksCount = 0;

        public PadlockRemovingSystem(GameplayController gameplayController) : base(gameplayController)
        {
            foreach (var cellStack in gameplayController.GameBoard().ArrbitrayCellStackArray())
                if (HasTileOnTop<Padlock>(cellStack))
                {
                    var padlock = TopTile(cellStack) as Padlock;
                    allPadlocks.Add(padlock);
                    if (padlock.IsLocked())
                        lockedPadlocksCount++;
                }
        }

        public override void Start()
        {
            if (allPadlocks.Count == 0)
            {
                gameplayController.DeactivateSystem<PadlockUnlockingDetectionSystem>();
                gameplayController.DeactivateSystem<PadlockRemovingSystem>();
            }
        }

        public override void Update(float dt)
        {
            if (AllPadlocksAreUnlocked())
                RemovePadlocks();
        }

        private void RemovePadlocks()
        {
            var hitData = GetFrameData<PadlockHitData>();
            foreach(var padlock in allPadlocks)
            {
                padlock.EnableTotalDestruction();
                hitData.padlocks.Add(padlock);
            }
            allPadlocks.Clear();
        }

        private bool AllPadlocksAreUnlocked()
        {
            return lockedPadlocksCount <= 0;
        }

        public void AddUnlockedPadlock(Padlock padlock)
        {
            lockedPadlocksCount--;
            if (lockedPadlocksCount < 0)
                DebugPro.LogError<CoreGameplayLogTag>("Negative lockedPadlocksCount in PadlockRemovingSystem");
        }
    }
}