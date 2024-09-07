using Match3.Foundation.Base.Utility;
using Match3.Game.Gameplay;

namespace Match3.Game
{
    public struct GameplayControllerArgument : Argument
    {
        public readonly GameplayController gameplayController;

        public GameplayControllerArgument(GameplayController gameplayController)
        {
            this.gameplayController = gameplayController;
        }
    }
}