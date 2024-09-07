using Match3.Game.Gameplay.CellAttachments;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.HitManagement;
using Match3.Game.Gameplay.RainbowMechanic;
using Match3.Game.Gameplay.SubSystemsData.FrameData.General;
using System;
using System.Collections.Generic;
using static Match3.Game.Gameplay.QueryUtilities;

namespace Match3.Game.Gameplay.SubSystems.LilyPadBudMechanic
{
    [After(typeof(HitManagement.HitApplyingSystem))]
    [After(typeof(General.UserMovementManagementSystem))]

    public class LilyPadBudGrowthSystem : GameplaySystem
    {
        List<LilyPadBud> lilyPadBuds = new List<LilyPadBud>();

        AppliedHitsData appliedHitsData;
        UserMovementData userMovementData;

        HashSet<LilyPadBud> grownLilyPads = new HashSet<LilyPadBud>();
        HashSet<LilyPadBud> toBeHitLilyPads = new HashSet<LilyPadBud>();

        CellStackBoard cellStackBoard;

        public LilyPadBudGrowthSystem(GameplayController gameplayController) : base(gameplayController)
        {
            foreach (var cellStack in gameplayController.GameBoard().ArrbitrayCellStackArray())
                if (cellStack.HasAttachment<LilyPadBud>())
                    lilyPadBuds.Add(cellStack.GetAttachment<LilyPadBud>());


            appliedHitsData = GetFrameData<AppliedHitsData>();
            userMovementData = GetFrameData<UserMovementData>();

            cellStackBoard = gameplayController.GameBoard().CellStackBoard();

            if (lilyPadBuds.Count == 0)
                gameplayController.DeactivateSystem<LilyPadBudGrowthSystem>();
        }

        public override void Start()
        {

        }

        public override void Reset()
        {
            if (lilyPadBuds.Count == 0)
                gameplayController.DeactivateSystem<LilyPadBudGrowthSystem>();
        }

        public override void Update(float dt)
        {
            ProcessGrowth();
            ProcessShrinking();
        }

        private void ProcessGrowth()
        {
            grownLilyPads.Clear();

            foreach (var cell in appliedHitsData.cellsStartedBeingHit)
            {
                var bud = cell.Parent().GetAttachment<LilyPadBud>();
                if (bud != null)
                    ProcessGrowthFor(bud);
            }
        }

        private void ProcessGrowthFor(LilyPadBud bud)
        {
            if (bud.IsFullyGrown())
                return;

            bud.Grow();

            grownLilyPads.Add(bud);

            if (bud.IsFullyGrown())
                lilyPadBuds.Remove(bud);
        }


        private void ProcessShrinking()
        {
            if (userMovementData.moves == 0)
                return;

            foreach (var bud in lilyPadBuds)
                if (grownLilyPads.Contains(bud) == false)
                    ProcesShrinkingFor(bud);
        }

        private void ProcesShrinkingFor(LilyPadBud bud)
        {
            if (bud.IsFullyShrunk() || bud.IsFullyGrown() || IsGoingToBeHit(bud.Owner(), cellStackBoard))
                return;

            bud.Shrink();
        }

    }
}