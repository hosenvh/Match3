using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystemsData.FrameData.PowerUpManagement;

namespace Match3.Game.Gameplay.SubSystems.PowerUpManagement
{
    public class HandPowerUpInputCommand : Input.GameplayInputCommand
    {
        CellStack origin;
        CellStack destination;

        public HandPowerUpInputCommand(CellStack origin, CellStack destination)
        {
            this.origin = origin;
            this.destination = destination;
        }

        public void Execute(GameplayController gameplayController)
        {
            gameplayController.
             FrameCenteredBlackBoard().
             GetComponent<PowerUpRequestData>().handPowerUpTargets.Add(new HandPowerUpData(origin, destination));
        }
    }
}