using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.HitManagement;
using Match3.Game.Gameplay.Tiles;
using System;
using System.Collections.Generic;
using static Match3.Game.Gameplay.ActionUtilites;
using static Match3.Game.Gameplay.QueryUtilities;
using Match3.Game.Gameplay.SubSystems.RocketMechanic;

namespace Match3.Game.Gameplay.SubSystems.RocketBoxMechanic
{
    public interface RocketBoxActivationPresentationHandler : PresentationHandler
    {
        void Activate(RocketBox rocketBox, List<CellStack> rocketTargets, Action<CellStack> onTargetHit, Action onRocketBoxDepleted);
    }

    [After(typeof(HitManagement.HitApplyingSystem))]
    [Before(typeof(DestructionManagement.DestructionSystem))]
    public class RocketBoxActivationSystem : GameplaySystem
    {
        const int ROCKETS_PER_BOX = 3;

        List<RocketBox> toBeActivatedRocketBoxes = new List<RocketBox>();
        HashSet<RocketBox> pendingRocketBoxes = new HashSet<RocketBox>();
        RocketBoxActivationPresentationHandler presentationHandler;
        RocketHitHandlingSystem hitHandlingSystem;
        RocketTargetFinder rocketTargetFinder;


        public RocketBoxActivationSystem(GameplayController gameplayController, RocketTargetFinder rocketTargetFinder) : base(gameplayController)
        {
            this.rocketTargetFinder = rocketTargetFinder;
            presentationHandler = gameplayController.GetPresentationHandler<RocketBoxActivationPresentationHandler>();
        }

        public override void Start()
        {
            hitHandlingSystem = gameplayController.GetSystem<RocketHitHandlingSystem>();
        }

        public override void Update(float dt)
        {
            toBeActivatedRocketBoxes.Clear();

            foreach (var tile in GetFrameData<AppliedHitsData>().tilesStartedBeingHit)
                if (tile is RocketBox rocketBox
                    && IsFree(rocketBox.Parent())
                    && rocketBox.IsDestroyed() == false
                    && pendingRocketBoxes.Contains(rocketBox) == false)
                {
                    toBeActivatedRocketBoxes.Add(rocketBox);
                    pendingRocketBoxes.Add(rocketBox);
                }

            if (toBeActivatedRocketBoxes.Count > 0)
                Activate(toBeActivatedRocketBoxes);

        }

        private void Activate(List<RocketBox> rocketBoxes)
        {
            for (int i = 0; i < rocketBoxes.Count; ++i)
            {
                var rocketBox = rocketBoxes[i];
                Lock<RocketBoxActivationKeyType>(rocketBox.Parent());

                var targets = rocketTargetFinder.FindTargets(ROCKETS_PER_BOX);
                LockTileStacksBy<RocketBoxActivationKeyType>(targets);

                if (targets.Count > 0)
                {
                    presentationHandler.Activate(
                        rocketBox,
                        targets,
                        HandleHit,
                        () => DestroyRocketBox(rocketBox));
                }
                else
                {
                    DestroyRocketBox(rocketBox);
                }
            }
        }

        void HandleHit(CellStack target)
        {
            hitHandlingSystem.HandleRocketBoxHitTo(target);
        }

        void DestroyRocketBox(RocketBox rocketBox)
        {
            pendingRocketBoxes.Remove(rocketBox);
            rocketBox.ForceDestroy();

            Unlock(rocketBox.Parent());

            rocketBox.Parent().Pop();
            GetFrameData<DestructionData>().tilesToDestroy.Add(rocketBox);
        }
    }

}