using System;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystemsData.FrameData;
using Match3.Game.Gameplay.SubSystemsData.FrameData.ArtifactMechanic;
using Match3.Game.Gameplay.SubSystemsData.FrameData.ExplosionManagement;
using Match3.Game.Gameplay.SubSystemsData.FrameData.RainbowMechanic;
using Match3.Game.Gameplay.Tiles;

namespace Match3.Game.Gameplay.SubSystems.WinSequence
{
    public class ActivatingState : State
    {
        const float INTERVAL =0.25f;

        float duration;

        CellStack[] cellStacks;

        BoardStabilityCalculator boardStabilityCalculator;

        protected override void InternalSetup()
        {
            cellStacks = gpc.GameBoard().leftToRightButtomUpCellStackArray;

            boardStabilityCalculator = new BoardStabilityCalculator(gpc.GameBoard());

        }

        public override void Update(float dt)
        {
            duration += dt;

            if(duration >= INTERVAL)
            {
                bool wasThereAnyExplosivesOrRainbows = false;
                bool activationApplied = false;

                foreach (var cellStack in cellStacks)
                {
                    TryActivateExplosion(cellStack, ref activationApplied, ref wasThereAnyExplosivesOrRainbows);

                    if(activationApplied)
                    {
                        duration = 0;
                        return;
                    }

                    TryActivateRaninbow(cellStack, ref activationApplied, ref wasThereAnyExplosivesOrRainbows);

                    if (activationApplied)
                    {
                        duration = 0;
                        return;
                    }
                }

                if (wasThereAnyExplosivesOrRainbows == false && boardStabilityCalculator.CalculateIsStable())
                    system.OnActivationFinished();
            }
        }

        private void TryActivateRaninbow(CellStack cellStack, ref bool activationApplied, ref bool wasThereAnyRainbow)
        {
            if (QueryUtilities.HasTileOnTop<Rainbow>(cellStack.CurrentTileStack()) == false)
                return;

            wasThereAnyRainbow = true;

            if (QueryUtilities.IsFullyFree(cellStack) == false)
                return;

            activationApplied = true;

            system.GetFrameData<DirectRainbowActivationRequestData>().targets.Add(cellStack.CurrentTileStack());
        }

        private void TryActivateExplosion(CellStack cellStack, ref bool activationApplied, ref bool wasThereAnyExplosive)
        {
            if (QueryUtilities.HasTileOnTop<ExplosiveTile>(cellStack.CurrentTileStack()) == false)
                return;
     
            wasThereAnyExplosive = true;

            if (QueryUtilities.IsFullyFree(cellStack) == false)
                return;

            activationApplied = true;

            system.GetFrameData<InternalExplosionActivationData>().targets.Add(cellStack.CurrentTileStack());

        }
    }
}