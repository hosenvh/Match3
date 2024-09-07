
using Match3.Game.Gameplay.Core;

namespace Match3.Game.Gameplay.Input
{
    public class ActivteCommandInput : GameplayInputCommand
    {
        TileStack tileStack;

        public ActivteCommandInput(TileStack tileStack)
        {
            this.tileStack = tileStack;
        }

        public void Execute(GameplayController gameplayController)
        {
            if (tileStack == null || tileStack.IsDepleted() || tileStack.GetComponent<LockState>().IsLocked())
                return;

            gameplayController.
                FrameCenteredBlackBoard().
                GetComponent<UserRequestedTileStackActivationData>().targets.Add(tileStack);
        }
    }
}