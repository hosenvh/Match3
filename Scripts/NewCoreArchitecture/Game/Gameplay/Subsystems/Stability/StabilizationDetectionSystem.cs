


using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystemsData.FrameData;

namespace Match3.Game.Gameplay.SubSystems.Stability
{
    public class StabilizationDetectionSystem : GameplaySystem
    {

        GameBoard gameBoard;


        StabilityData stabilityData;

        public StabilizationDetectionSystem(GameplayController gameplayController) : base(gameplayController)
        {
            gameBoard = gameplayController.GameBoard();
            stabilityData = GetFrameData<StabilityData>();
        }

        public override void Update(float dt)
        {
            bool isStable = true;



            foreach (var cellStack in gameBoard.leftToRightButtomUpCellStackArray)
            {
                if (IsStable(cellStack) == false)
                {
                    isStable = false;
                    break;
                }
            }

            stabilityData.wasStableLastChecked = isStable;

        }


        private bool IsStable(CellStack value)
        {
            var tileStack = value.CurrentTileStack();
            return tileStack == null || tileStack.componentCache.lockState.IsFree();
        }
    }
}