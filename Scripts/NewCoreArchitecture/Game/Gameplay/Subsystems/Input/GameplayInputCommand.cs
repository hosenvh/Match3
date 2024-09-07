
using Match3.Game.Gameplay.Core;

namespace Match3.Game.Gameplay.Input
{
    public interface GameplayInputCommand
    {
        void Execute(GameplayController gameplayController);
    }
}