

using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.CellAttachments;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystemsData.FrameData.PowerUpManagement;

namespace Match3.Game.Gameplay.SubSystems.PowerUpManagement
{
    public struct HammerPowerUpActivatedEvent : PowerUpActivatedEvent
    {
    }

    public class HammerPowerupActivationController
    {

        PowerUpActivationSystem system;

        PowerUpActivationPresentationHandler powerUpPresentationHandler;

        public HammerPowerupActivationController(PowerUpActivationSystem system, PowerUpActivationPresentationHandler powerUpPresentationHandler)
        {
            this.system = system;
            this.powerUpPresentationHandler = powerUpPresentationHandler;
        }

        public void Update()
        {
            foreach (var target in system.GetFrameData<PowerUpRequestData>().hammerTargets)
                TryActivateHammerOn(target);

        }


        private void TryActivateHammerOn(CellStack target)
        {
            if (IsHammerTarget(target))
            {
                system.LockTileStack(target);

                powerUpPresentationHandler.HandleHammer(target, () => system.ApplyPowerUpHitOn(target));
                ServiceLocator.Find<EventManager>().Propagate(new HammerPowerUpActivatedEvent(), this);
            }
        }

        private bool IsHammerTarget(CellStack target)
        {
            if (QueryUtilities.IsFullyFree(target))
            {
                if (QueryUtilities.HasAnyTile(target))
                    return target.CurrentTileStack().Top().GetComponent<TilePowerUpProperties>().isHammerTarget;
                else
                    return target.HasAttachment<LilyPadBud>() || target.Top().GetComponent<CellPowerUpProperties>().isHammerTarget;
                // TODO: Please fix this direct dependency to LilyPadBud.
                // NOTE: Changeing LilyPadBud To Cell will fix it.
            }
            else
                return false;
        }

    }
}