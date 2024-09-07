

using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.HitManagement;
using Match3.Game.Gameplay.Tiles;
using System.Collections.Generic;
using static Match3.Game.Gameplay.QueryUtilities;

namespace Match3.Game.Gameplay.SubSystems.PadlockMechanic
{

    [After(typeof(HitManagement.HitApplyingSystem))]
    public class PadlockUnlockingDetectionSystem : GameplaySystem
    {
        HashSet<Padlock> loackedPadlocks = new HashSet<Padlock>();

        public PadlockUnlockingDetectionSystem(GameplayController gameplayController) : base(gameplayController)
        {
            foreach (var cellStack in gameplayController.GameBoard().ArrbitrayCellStackArray())
                if (HasTileOnTop<Padlock>(cellStack) && (TopTile(cellStack) as Padlock).IsLocked())
                    loackedPadlocks.Add(TopTile(cellStack) as Padlock);
        }

        public override void Start()
        {

        }

        public override void Update(float dt)
        {
            foreach (var tile in GetFrameData<AppliedHitsData>().tilesFinishedBeingHit)
                if (tile is Padlock padlock && IsPadlockJustUnlocked(padlock))
                    AddToUnlockedPadlocks(padlock);

        }

        private void AddToUnlockedPadlocks(Padlock padlock)
        {
            loackedPadlocks.Remove(padlock);
            gameplayController.GetSystem<PadlockRemovingSystem>().AddUnlockedPadlock(padlock);

        }

        private bool IsPadlockJustUnlocked(Padlock padlock)
        {
            return loackedPadlocks.Contains(padlock) && padlock.IsLocked() == false;
        }
    }
}