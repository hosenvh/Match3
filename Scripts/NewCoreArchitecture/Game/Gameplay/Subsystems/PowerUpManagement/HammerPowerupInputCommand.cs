using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystemsData.FrameData.PowerUpManagement;

namespace Match3.Game.Gameplay.SubSystems.PowerUpManagement
{
    public class HammerPowerUpInputCommand : Input.GameplayInputCommand
    {
        CellStack target;

        public HammerPowerUpInputCommand(CellStack target)
        {
            this.target = target;
        }

        public void Execute(GameplayController gameplayController)
        {
            gameplayController.
                FrameCenteredBlackBoard().
                GetComponent<PowerUpRequestData>().hammerTargets.Add(target);
        }
    }
}