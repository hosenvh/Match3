using Match3.Game.Gameplay.Input;
using Match3.Game.Gameplay.SubSystemsData.FrameData.General;
using Match3.Game.Gameplay.SubSystemsData.FrameData.LevelEnding;
using System;

namespace Match3.Game.Gameplay.SubSystems.LevelEnding
{
    public class MovementStopCondition : StopConditinon
    {
        public event Action<int> remainingMovesChanged =delegate { };
        int remainingMoves;

        public MovementStopCondition(int maxMoves)
        {
            this.remainingMoves = maxMoves;
        }


        public bool IsSatisfied()
        {
            return remainingMoves <= 0;
        }

        public void Update(float dt, LevelStoppingSystem system)
        {
            var usedMoves = system.GetFrameData<UserMovementData>().moves;
            var movesToAdd = system.GetFrameData<MovesToAddData>().amount;


            if (usedMoves != 0 || movesToAdd != 0)
            {
                remainingMoves += movesToAdd;
                remainingMoves -= usedMoves;
                remainingMovesChanged(remainingMoves);
            }
        }

        public int RemainingMovements()
        {
            return remainingMoves;
        }

        public void AddExtraMove(int moves)
        {
            remainingMoves += moves;
            remainingMovesChanged(remainingMoves);
        }
    }
}