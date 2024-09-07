

using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.HitManagement;
using Match3.Game.Gameplay.SubSystemsData.FrameData.ArtifactMechanic;
using Match3.Game.Gameplay.Tiles;
using System;
using System.Collections.Generic;

namespace Match3.Game.Gameplay.SubSystems.ArtifactMechanic
{
    [Before(typeof(DestructionManagement.DestructionSystem))]
    [Before(typeof(HitManagement.HitGenerationSystem))]
    public class ArtifactDiscoverySystem : GameplaySystem
    {
        List<AbstractArtifactMainCell> artifactMainCells = new List<AbstractArtifactMainCell>();

        public ArtifactDiscoverySystem(GameplayController gameplayController) : base(gameplayController)
        {
            SetupArtifacts();
        }

        public override void Update(float dt)
        {
            for (int i = artifactMainCells.Count - 1; i >= 0; --i)
            {
                var artifact = artifactMainCells[i];
                if (CanBeDiscovered(artifact))
                {
                    Discover(artifact);
                    artifactMainCells.RemoveAt(i);
                }
            }

        }

        void Discover(AbstractArtifactMainCell artifact)
        {
            var destructionData = GetFrameData<DestructionData>();
            var hitData = GetFrameData<ArtifactHitData>();

            destructionData.cellsToDestroy.Add(artifact);
            hitData.cellStackToHitIfColored.Add(artifact.Parent());


            foreach (var slave in artifact.Slaves())
            {
                destructionData.cellsToDestroy.Add(slave);
                hitData.cellStackToHitIfColored.Add(slave.Parent());
            }
        }


        bool CanBeDiscovered(AbstractArtifactMainCell artifact)
        {
            if (IsOnTop(artifact) == false)
                return false;

            foreach (var slave in artifact.Slaves())
                if (IsOnTop(slave) == false)
                    return false;

            return true;
        }

        bool IsOnTop(Cell cell)
        {
            return cell.Parent().Top() == cell;
        }


        void SetupArtifacts()
        {
            foreach (var cellStack in gameplayController.GameBoard().ArrbitrayCellStackArray())
                TryAddArtifactIn(cellStack);
        }

        public void TryAddArtifactIn(CellStack cellStack)
        {
            var artifactMainCell = QueryUtilities.FindCell<AbstractArtifactMainCell>(cellStack);
            if (artifactMainCell != null)
                artifactMainCells.Add(artifactMainCell);
        }

    }
}