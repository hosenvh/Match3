using Match3.Development.Base.DevelopmentConsole;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game;
using Match3.Game.Effects;
using Match3.Game.Gameplay;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Input;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using Match3.Game.Gameplay.SubSystemsData.FrameData.LevelEnding;
using Match3.Presentation.Gameplay;
using UnityEngine;
using static Base;


namespace Match3.Development.DevOptions
{
    [DevOptionGroup(groupName: "Gameplay", priority: 7)]
    public class GameplayDevOptions : DevelopmentOptionsDefinition
    {
        [DevOption(commandName: "Load Level", color: DevOptionColor.Cyan, shouldAutoClose: true)]
        public static void LoadLevel(int level)
        {
            SetLevel(level - 1);
            LoadCurrentLevel();
        }
        
        [DevOption(commandName: "Load Next")]
        [ShortCut(KeyCode.LeftShift, KeyCode.N)]
        public static void LoadNextLevel()
        {
            GoToNextLevel();
            LoadCurrentLevel();
        }

        [DevOption(commandName: "Load Previous")]
        [ShortCut(KeyCode.LeftShift, KeyCode.B)]
        public static void LoadPreviousLevel()
        {
            GoToPreviousLevel();
            LoadCurrentLevel();
        }

        [DevOption(commandName: "Reload Level")]
        [ShortCut(KeyCode.LeftShift, KeyCode.P)]
        public static void LoadCurrentLevel()
        {
            var boardConfig = gameManager.levelManager.GetLevelConfig(gameManager.profiler.LastUnlockedLevel);
            var selectedLevelIndex = gameManager.profiler.LastUnlockedLevel;
            ServiceLocator.Find<GameTransitionManager>().GoToLevel(boardConfig, selectedLevelIndex);
        }

        [DevOption(commandName: "Force Lose", color: DevOptionColor.Yellow, shouldAutoClose: true)]
        public static void ForceLoseLevel()
        {
            ConsumeLevelResources();
            SetRemainingMoves(0);
        }

        [DevOption(commandName: "Force Win", color: DevOptionColor.Yellow, shouldAutoClose: true)]
        public static void ForceWinLevel()
        {
            ConsumeLevelResources();
            SetRemainingGoals(0);
        }

        private static void ConsumeLevelResources()
        {
            var successfulActivationData = GameplayController().FrameCenteredBlackBoard().GetComponent<SuccessfulUserActivationData>();

            var gameBoard = GameplayController().GameBoard();

            TileStack targetTileStack = null;
            foreach (var cellStack in gameBoard.leftToRightTopDownCellStackArray)
                if (cellStack.HasTileStack())
                    targetTileStack = cellStack.CurrentTileStack();

            // Adding a fake successful activation.
            successfulActivationData.targets.Add(targetTileStack);
        }

        [DevOption(commandName: "Set Remaining Moves")]
        public static void SetRemainingMoves(int moveAmount)
        {
            var movementStopCondition = GameplayController().GetSystem<LevelStoppingSystem>().FindStoppingCondition<MovementStopCondition>();
            var movesToAddData = GameplayController().FrameCenteredBlackBoard().GetComponent<MovesToAddData>();

            movesToAddData.amount = moveAmount - movementStopCondition.RemainingMovements();
        }

        [DevOption(commandName: "Set Remaining Goals")]
        public static void SetRemainingGoals(int amount)
        {
            var levelStoppingSystem = GameplayController().GetSystem<LevelStoppingSystem>();
            
            foreach(var goalTracker in levelStoppingSystem.AllGoals())
                goalTracker.SetRemainingAmount(amount);
        }


        [DevOption(commandName: "Load Levels Consecutively")]
        public static void LoadLevelsConsecutively(int startLevel, int finishLevel, float intervalTime)
        {
            Object.FindObjectOfType<OpenLevelsConsecutivelyTool>().StartLoading(startLevel, finishLevel, intervalTime);
        }

        [DevOption(commandName: "Stop Consecutive Level Loading")]
        public static void StopConsecutiveLevelLoading()
        {
            Object.FindObjectOfType<OpenLevelsConsecutivelyTool>().StopLoading();
        }

        public static void GoToNextLevel()
        {
            gameManager.profiler.LastUnlockedLevel =
               Mathf.Min(gameManager.levelManager.TotalLevels()-1, gameManager.profiler.LastUnlockedLevel +1);
        }

        public static void GoToPreviousLevel()
        {
            gameManager.profiler.LastUnlockedLevel =
                Mathf.Max(0,gameManager.profiler.LastUnlockedLevel -1);
        }

        public static void SetLevel(int level)
        {
            gameManager.profiler.LastUnlockedLevel = level;
        }

        [DevOption(commandName: "Copy Gameplay State")]
        public static void DumpGameplayState()
        {
            new GameplayStateDumper(Object.FindObjectOfType<GameplayState>()).DumpToClipboard();
        }


        [DevOption(commandName: "Log Locks")]
        public static void LogLocks()
        {
            var gameBoard = GameplayController().GameBoard();
            foreach (var cellStack in gameBoard.leftToRightTopDownCellStackArray)
            {
                if (cellStack.HasTileStack())
                {
                    var tileLockState = cellStack.CurrentTileStack().GetComponent<LockState>();
                    if (tileLockState.IsLocked())
                        Debug.LogFormat("Tile in {0} is locked by {1}", cellStack.Position(), tileLockState.KeyType().Name);
                }

                if(cellStack.componentCache.lockState.IsLocked())
                    Debug.LogFormat("CellStack in {0} is locked by {1}", cellStack.Position(), cellStack.componentCache.lockState.KeyType().Name);
            }

        }

        [DevOption(commandName: "Toggle Downward Flow")]
        public static void ToggleDownwardFlow()
        {
            Object.FindObjectOfType<DownwardFlowPresenter>().Toggle();
        }

        [DevOption(commandName: "Log Item Lock")]
        public static void LogItemLock(int x, int y)
        {
            var cellStack = Object.FindObjectOfType<GameplayState>().gameplayController.GameBoard().CellStackBoard()[x, y];

            Debug.Log($"CellStack at ({x},{y}) is locked by {cellStack.GetComponent<LockState>().KeyType()}");

            if (cellStack.HasTileStack())
                Debug.Log($"TileStack at ({x},{y}) is locked by {cellStack.CurrentTileStack().GetComponent<LockState>().KeyType()}");
        }

        [DevOption(commandName: "Toggle Tile Indices")]
        public static void ToggleTileIndices()
        {
            Object.FindObjectOfType<CellIndicesPresenter>().Toggle();
        }

        private static GameplayState GameState()
        {
            return (gameManager.CurrentState as GameplayState);
        }

        private static GameplayController GameplayController()
        {
            return GameState().gameplayController;
        }

        [DevOption(commandName: "Close Board", ignore: true)]
        public static void CloseBoard()
        {
            var cellSTack = Object.FindObjectOfType<GameplayState>().gameplayController.GameBoard().CellStackBoard();
            Vector2 boardSize = new Vector2(cellSTack.Width(), cellSTack.Height());
            Object.FindObjectOfType<ClosingBoardEffectController>().CloseBoard(boardSize, () => { Debug.Log("-------------- Board Closed -------------- "); });
        }

        private static bool usePooling;

        [DevOption(commandName: "Toggle Pool", ignore: true)]
        public static void TogglePool()
        {
            usePooling = !usePooling;
            Debug.Log($"Pooling is {usePooling}");
        }

        public static bool enableProfiling;

        [DevOption(commandName: "Toggle Profiling", ignore: true)]
        public static void ToggleProfiling()
        {
            enableProfiling = !enableProfiling;
            Debug.Log($"Profiling is {enableProfiling}");
        }
    }
}