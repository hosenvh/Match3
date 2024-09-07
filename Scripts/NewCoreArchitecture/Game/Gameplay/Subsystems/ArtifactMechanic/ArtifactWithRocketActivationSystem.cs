

using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystems.RocketMechanic;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Game.Gameplay.SubSystems.ArtifactMechanic
{
    public interface ArtifactWithRocketActivationPort : PresentationHandler
    {
        void HandleRocketActivationTo(ArtifactWithRocketMainCell artifactCell, List<CellStack> targets, Action<CellStack> onTargetHit);
    }

    public struct ArtifactWithRocketActivationSystemKey : KeyType
    {

    }

    [After(typeof(DestructionManagement.DestructionSystem))]
    public class ArtifactWithRocketActivationSystem : GameplaySystem
    {
        RocketTargetFinder rocketTargetFinder;
        DestroyedObjectsData destroyedObjectsData;

        ArtifactWithRocketActivationPort activationPort;
        RocketHitHandlingSystem hitHandlingSystem;

        public ArtifactWithRocketActivationSystem(GameplayController gameplayController, RocketTargetFinder rocketTargetFinder) : base(gameplayController)
        {
            this.rocketTargetFinder = rocketTargetFinder;

            destroyedObjectsData = GetFrameData<DestroyedObjectsData>();

            activationPort = gameplayController.GetPresentationHandler<ArtifactWithRocketActivationPort>();
        }

        public override void Start()
        {
            hitHandlingSystem = gameplayController.GetSystem<RocketHitHandlingSystem>();
        }

        public override void Update(float dt)
        {
            foreach (var cell in destroyedObjectsData.cells)
                if (cell is ArtifactWithRocketMainCell)
                    ActivateSeekingRocketsFor(cell as ArtifactWithRocketMainCell);
        }

        private void ActivateSeekingRocketsFor(ArtifactWithRocketMainCell artifactWithRocketMainCell)
        {
            var size = artifactWithRocketMainCell.Size();
            var neededRockets = size.Witdth * size.Height;

            var targets = rocketTargetFinder.FindTargets(neededRockets);
            ActionUtilites.LockTileStacksBy<ArtifactWithRocketActivationSystemKey>(targets);

            activationPort.HandleRocketActivationTo(artifactWithRocketMainCell, targets, HandleHit);
        }

        private void HandleHit(CellStack cellStack)
        {
            hitHandlingSystem.HandleArfiactWithRocketHitTo(cellStack);
        }
    }
}