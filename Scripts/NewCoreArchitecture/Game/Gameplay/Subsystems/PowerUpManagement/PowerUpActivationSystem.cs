

using Match3.Foundation.Base.EventManagement;
using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.HitManagement;
using Match3.Game.Gameplay.Input;
using Match3.Game.Gameplay.SubSystemsData.FrameData.ArtifactMechanic;
using Match3.Game.Gameplay.SubSystemsData.FrameData.ExplosionManagement;
using Match3.Game.Gameplay.SubSystemsData.FrameData.RainbowMechanic;
using Match3.Game.Gameplay.Tiles;
using System;

namespace Match3.Game.Gameplay.SubSystems.PowerUpManagement
{
    public struct PowerUpSystemKeyType : KeyType
    { }

    public interface PowerUpActivatedEvent : GameEvent
    { }




    [Before(typeof(ExplosionManagement.ExplosionActivationSystem))]
    [Before(typeof(RainbowMechanic.RainbowActivationSystem))]
    public class PowerUpActivationSystem : GameplaySystem
    {

        HandPowerUpActivationController handPowerUpActivationController;
        BroomPowerUpActivationController broomPowerUpActivationController;
        HammerPowerupActivationController hammerPowerUpActivationController;

        public PowerUpActivationSystem(GameplayController gameplayController) : base(gameplayController)
        {
        }

        public override void Start()
        {

            var powerUpPresentationHandler = gameplayController.GetPresentationHandler<PowerUpActivationPresentationHandler>();

            handPowerUpActivationController = new HandPowerUpActivationController(this, powerUpPresentationHandler, gameplayController);

            hammerPowerUpActivationController = new HammerPowerupActivationController(this, powerUpPresentationHandler);

            broomPowerUpActivationController = new BroomPowerUpActivationController(
                this, 
                powerUpPresentationHandler, 
                gameplayController.GameBoard().CellStackBoard());
        }

        public override void Update(float dt)
        {
            hammerPowerUpActivationController.Update();
            broomPowerUpActivationController.Update();
            handPowerUpActivationController.Update();
        }


        internal void ApplyPowerUpHitOn(CellStack target)
        {
            ReleaseTileStack(target);
            if (IsExplosive(target))
                GetFrameData<InternalExplosionActivationData>().targets.Add(target.CurrentTileStack());
            else if(IsRainbow(target))
                GetFrameData<DirectRainbowActivationRequestData>().targets.Add(target.CurrentTileStack());
            else
                GetFrameData<PowerUpActivationData>().affectedCellStacks.Add(target);

        }

        bool IsExplosive(CellStack target)
        {
            return QueryUtilities.HasAnyTile(target) && target.CurrentTileStack().Top() is ExplosiveTile;
        }

        bool IsRainbow(CellStack target)
        {
            return QueryUtilities.HasAnyTile(target) && target.CurrentTileStack().Top() is Rainbow;
        }

        internal void LockTileStack(CellStack target)
        {
            if (target.HasTileStack())
                target.CurrentTileStack().GetComponent<LockState>().LockBy<PowerUpSystemKeyType>();
        }

        void ReleaseTileStack(CellStack target)
        {
            if (target.HasTileStack())
                target.CurrentTileStack().GetComponent<LockState>().Release();
        }
    }
}