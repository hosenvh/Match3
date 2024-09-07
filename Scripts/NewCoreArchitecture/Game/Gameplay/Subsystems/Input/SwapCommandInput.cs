
using System;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Input;
using Match3.Game.Gameplay.Swapping;

namespace Match3.Game.Gameplay.Input
{
    public class SwapCommandInput : GameplayInputCommand
    {
        TileStack tileStack;
        Direction direction;

        public SwapCommandInput(TileStack stack, Direction direction)
        {
            this.tileStack = stack;
            this.direction = direction;

        }

        public void Execute(GameplayController gameplayController)
        {
            if (tileStack == null)
                return;

            gameplayController.
                FrameCenteredBlackBoard().
                GetComponent<RequestedSwapsData>().
                data.Add(new SwapRequestData(tileStack, direction));

        }


    }
}