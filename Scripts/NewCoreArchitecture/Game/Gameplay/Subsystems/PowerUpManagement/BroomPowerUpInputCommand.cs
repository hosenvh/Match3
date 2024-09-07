using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystemsData.FrameData.PowerUpManagement;

namespace Match3.Game.Gameplay.SubSystems.PowerUpManagement
{
    public class BroomPowerUpInputCommand : Input.GameplayInputCommand
    {
        CellStack target;

        public BroomPowerUpInputCommand(CellStack target)
        {
            this.target = target;
        }

        public void Execute(GameplayController gameplayController)
        {
            gameplayController.
                FrameCenteredBlackBoard().
                GetComponent<PowerUpRequestData>().broomTargets.Add(target);
        }
    }
}