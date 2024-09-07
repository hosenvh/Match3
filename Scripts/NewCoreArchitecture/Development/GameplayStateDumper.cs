using Match3.Game.Gameplay;
using Match3.Game.Gameplay.SubSystemsData.SessionData;
using Match3.Presentation.Gameplay;
using NiceJson;
using UnityEngine;

using static Match3.Game.Gameplay.QueryUtilities;

namespace Match3.Development
{
    public class GameplayStateDumper
    {
        private GameplayController gameplayController;
        private int levelIndex;

        public GameplayStateDumper(GameplayState gameplayState)
        {
            this.gameplayController = gameplayState.gameplayController;
            levelIndex = gameplayState.CurrentLevelIndex();
        }

        public void DumpToClipboard()
        {
            JsonObject root = new JsonObject();

            root.Add("LevelIndex", levelIndex);


            var tileLockState = new JsonArray();
            root.Add("TileLockStates", tileLockState);

            foreach (var cell in gameplayController.GameBoard().ArrbitrayCellStackArray())
                if (cell.HasTileStack() && IsFree(cell.CurrentTileStack()) == false)
                    tileLockState.Add($"{cell.Position()}: {cell.CurrentTileStack().GetComponent<LockState>().KeyType()}");
            

            var cellLockState = new JsonArray();
            root.Add("CellLockStates", cellLockState);

            foreach (var cell in gameplayController.GameBoard().ArrbitrayCellStackArray())
                if (IsFree(cell) == false)
                    tileLockState.Add($"{cell.Position()}: {cell.GetComponent<LockState>().KeyType()}");


            var activeSystems = new JsonArray();
            root.Add("ActiveSystem", activeSystems);

            foreach (var system in gameplayController.ActiveSystems())
                activeSystems.Add(system.GetType().Name);


            var inputController = gameplayController.SessionCenteredBlackBoard().GetComponent<InputControlData>();

            var inputLocks = new JsonArray();
            root.Add("InputLocks", inputLocks);

            foreach (var lockType in inputController.CurrentLocks())
                inputLocks.Add(lockType.Name);

            var logs = new JsonArray();
            root.Add("Logs", logs);

            foreach (var log in Object.FindObjectOfType<Reporter>().Logs())
                logs.Add($"{log.logType}:{log.condition}\n{log.stacktrace}");

            CopyToClipboard(root.ToJsonString());
        }


        void CopyToClipboard(string text)
        {
            TextEditor te = new TextEditor();
            te.text = text;
            te.SelectAll();
            te.Copy();
        }
    }
}