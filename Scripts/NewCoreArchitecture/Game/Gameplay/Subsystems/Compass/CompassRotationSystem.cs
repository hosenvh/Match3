using System.Collections.Generic;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.RainbowMechanic;
using Match3.Game.Gameplay.Tiles;
using Match3.Game.Gameplay.SubSystems.General;
using Match3.Game.Gameplay.SubSystemsData.FrameData.General;
using Match3.Game.Gameplay.SubSystemsData.FrameData.RainbowMechanic;
using Match3.Game.Gameplay.Tiles.Explosives;
using static Match3.Game.Gameplay.QueryUtilities;


namespace Match3.Game.Gameplay.Subsystems.Compass
{
    public interface CompassPresentationPort : PresentationHandler
    {
        void Rotate(HashSet<CompassTile> allCompassBoard,Direction currentDirection);
    }

    [After(typeof(UserMovementManagementSystem))]
    [After(typeof(RainbowActivationSystem))]
    public class CompassRotationSystem : GameplaySystem
    {
        private readonly CompassBoardData compassBoardData;
        private readonly CompassPresentationPort compassPresentationPort;
        private readonly UserMovementData userMovementData;
        private readonly HashSet<CompassTile> compasses = new HashSet<CompassTile>();
        private int pendingChangeDirectionCount = 0;
        private readonly HashSet<CellStack> causesOfDelayedDeirectionChangeing = new HashSet<CellStack>();

        public CompassRotationSystem(GameplayController gameplayController) : base(gameplayController)
        {
            compassBoardData=GetSessionData<CompassBoardData>();
            compassPresentationPort = gameplayController.GetPresentationHandler<CompassPresentationPort>();
            userMovementData = GetFrameData<UserMovementData>();
            ReadCompassInBoard();
        }

        private void ReadCompassInBoard()
        {
            foreach (CellStack cellStack in gameplayController.GameBoard().ArrbitrayCellStackArray())
                if (HasTile<CompassTile>(cellStack))
                    compasses.Add(FindTile<CompassTile>(cellStack));
        }

        public override void Update(float dt)
        {
            ProcessReadyToRemoveCompasses();
            ChangeDirectionCompasses();
            ProcessPendingRotationCompasses();
        }

        private void ChangeDirectionCompasses()
        {
            bool moveHappened = userMovementData.moves > 0;
            bool rainbowActive = GetFrameData<RainbowActivationData>().activeRainbowes.Count>0;

            if (moveHappened)
            {
                if (rainbowActive || causesOfDelayedDeirectionChangeing.Count>0)
                {
                    pendingChangeDirectionCount++;

                    foreach (var activateRainbows in GetFrameData<RainbowActivationData>().activeRainbowes)  
                        causesOfDelayedDeirectionChangeing.Add(activateRainbows.Parent().Parent());
                }
                else
                   DoOneRotation();
            }
        }

        private void ProcessPendingRotationCompasses()
        {
            foreach (var rainbowActivated in GetFrameData<RainbowActivationData>().activatedRainbowCellStacks)
            {
                causesOfDelayedDeirectionChangeing.Remove(rainbowActivated);
            }
            if (causesOfDelayedDeirectionChangeing.Count == 0)
            {
                for (int i = 0; i < pendingChangeDirectionCount; i++)
                {
                    DoOneRotation();
                }
                pendingChangeDirectionCount = 0;
            }
        }

        private void ProcessReadyToRemoveCompasses()
        {
            foreach (var tileDestruction in GetFrameData<DestroyedObjectsData>().tiles)
            {
                if (tileDestruction is CompassTile compassTile)
                   compasses.Remove(compassTile);
            }
        }

        private void DoOneRotation()
        {
            switch (compassBoardData.currentDirection)
            {
                case Direction.Up:
                    compassBoardData.currentDirection=Direction.Right;
                    break;
                case Direction.Right:
                    compassBoardData.currentDirection=Direction.Down;
                    break;
                case Direction.Down:
                    compassBoardData.currentDirection=Direction.Left;
                    break;
                case Direction.Left:
                    compassBoardData.currentDirection=Direction.Up;
                    break;
            }
            compassPresentationPort.Rotate(compasses,compassBoardData.currentDirection);
        }
    }
}