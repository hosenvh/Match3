using System.Collections.Generic;
using KitchenParadise.Utiltiy.Base;
using Match3.EditorTools.Editor.Menus.Utility.BoardFinder.Conditions;
using Match3.EditorTools.Editor.Menus.Utility.BoardFinder.Conditions.Base;
using Match3.EditorTools.Editor.Utility;
using UnityEditor;
using UnityEngine;


namespace Match3.EditorTools.Editor.Menus.Utility.BoardFinder
{
    public class LevelFinderWindow : EditorWindow
    {
        private static readonly Vector2 MinWindowSize = new Vector2(700, 450);
        private static readonly Vector2 MaxWindowSize = new Vector2(700, 1000);

        private List<BoardFinderCondition> selectedConditions;
        private List<BoardFinderConditionDrawer> conditionDrawers;
        private BoardFinder boardFinder;

        [MenuItem("Golmorad/Utility/Level Finder", priority = 535)]
        public static void OpenWindow()
        {
            var window = OpenUnityWindow();
            SetupWindowSize();

            LevelFinderWindow OpenUnityWindow() => GetWindow<LevelFinderWindow>(title: "Level Finder");

            void SetupWindowSize()
            {
                window.minSize = MinWindowSize;
                window.maxSize = MaxWindowSize;
            }
        }

        private void OnEnable()
        {
            InitializeFields();

            void InitializeFields()
            {
                InitializeConditionDrawers();
                InitializeBoardFinder();

                void InitializeConditionDrawers()
                {
                    selectedConditions = new List<BoardFinderCondition>();
                    conditionDrawers = new List<BoardFinderConditionDrawer>()
                       {
                           new BoardSizeConditionDrawer(),
                           new MoveCountConditionDrawer(),
                           new LevelDifficultyConditionDrawer(),
                           new LevelGoalConditionDrawer(),
                           new LevelGoalConditionDrawer(),
                           new AvailableCellConditionDrawer(),
                           new AvailableTileConditionDrawer()
                       };
                }

                void InitializeBoardFinder()
                {
                    boardFinder = new BoardFinder();
                }
            }
        }

        private void OnGUI()
        {
            InsertInitialEmptySpace();

            DrawConditions();
            FetchSelectedConditions();
            DrawSectionForLoggingBoardsWithSatisfiedConditions();
        }

        private void InsertInitialEmptySpace() => EditorUtilities.InsertEmptySpace(pixels: 30);

        private void DrawConditions()
        {
            conditionDrawers.DoForEachElement(conditionDrawer =>
            {
                conditionDrawer.DrawCondition();
                InsertConditionsEmptySpace();
            });

            void InsertConditionsEmptySpace() => EditorUtilities.InsertEmptySpace(pixels: 25);
        }

        private void FetchSelectedConditions()
        {
            selectedConditions.Clear();
            foreach (BoardFinderConditionDrawer conditionDrawer in conditionDrawers)
            {
                BoardFinderCondition condition = conditionDrawer.GetSelectedCondition();
                selectedConditions.Add(condition);
            }
        }

        private void DrawSectionForLoggingBoardsWithSatisfiedConditions()
        {
            EditorUtilities.InsertButton(title: "Find Boards", onClick: LogBoardsWithSatisfiedConditions);

            void LogBoardsWithSatisfiedConditions()
            {
                if (selectedConditions.Count == 0)
                {
                    LogChooseSomeConditions();
                    return;
                }
                List<BoardConfig> foundBoards = boardFinder.FindBoardsBasedOnConditions(selectedConditions);
                if (foundBoards.Count == 0)
                    LogNoBoardsFound();
                else
                    foundBoards.DoForEachElement(todo: LogBoardIsSatisfyingConditions);

                void LogChooseSomeConditions()
                {
                    Debug.LogError(message: "Dude!!! WTF? For god sake, fist apply some conditions and then ask me to find your target boards, otherwise all boards are satisfied guineas! :/");
                }

                void LogNoBoardsFound()
                {
                    Debug.Log(message: "Board Finder | No Board Found based on selected conditions!");
                }

                void LogBoardIsSatisfyingConditions(BoardConfig boardConfig)
                {
                    Debug.Log(message: $"Board Finder | Board {boardConfig.name} is satisfying selected conditions", boardConfig);
                }
            }
        }
    }
}