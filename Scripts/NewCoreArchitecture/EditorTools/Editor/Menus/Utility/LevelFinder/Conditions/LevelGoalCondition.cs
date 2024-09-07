using System;
using System.Collections.Generic;
using Match3.EditorTools.Editor.Drawers;
using Match3.EditorTools.Editor.Menus.Utility.BoardFinder.Conditions.Base;
using Match3.EditorTools.Editor.Utility;
using Match3.Foundation.Base.DataStructures;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Tiles;
using static BoardConfig;


namespace Match3.EditorTools.Editor.Menus.Utility.BoardFinder.Conditions
{
    public class LevelGoalCondition : BoardFinderCondition
    {
        private readonly int goalIndex;
        private readonly Type goalType;
        private readonly TileColor goalColor;
        private readonly Bound<int> goalCountBound;

        public LevelGoalCondition(int goalIndex, Type goalType, TileColor goalColor, Bound<int> goalCountBound)
        {
            this.goalIndex = goalIndex;
            this.goalType = goalType;
            this.goalColor = goalColor;
            this.goalCountBound = goalCountBound;
        }

        public bool IsSatisfied(BoardConfig boardConfig)
        {
            List<GoalInfo> goals = GetLevelGoals(boardConfig);
            if (DoesSelectedGoalIndexExist(goals) == false)
                return false;

            var goal = GetSelectedGoal(goals);

            return IsGoalCountSatisfied(goal)
                && IsGoalTypeSatisfied(goal)
                && IsGoalColorSatisfied(goal);
        }

        private List<GoalInfo> GetLevelGoals(BoardConfig boardConfig)
        {
            return boardConfig.levelConfig.goals;
        }

        private bool DoesSelectedGoalIndexExist(List<GoalInfo> goals)
        {
            return goalIndex < goals.Count && goalIndex >= 0;
        }

        private GoalInfo GetSelectedGoal(List<GoalInfo> goals)
        {
            return goals[goalIndex];
        }

        private bool IsGoalCountSatisfied(GoalInfo goal)
        {
            return goalCountBound.Contains(goal.goalAmount);
        }

        private bool IsGoalTypeSatisfied(GoalInfo goal)
        {
            return ReflectionEditorUtilities.GetType(goal.goalType) == goalType;
        }

        private bool IsGoalColorSatisfied(GoalInfo goal)
        {
            return goalColor == TileColor.None || goal.color == goalColor;
        }
    }

    public class LevelGoalConditionDrawer : BoardFinderConditionDrawer<LevelGoalCondition>
    {
        private int goalIndex;
        private Type goalType = typeof(ColoredBead);
        private TileColor goalColor = TileColor.None;
        private Bound<int> goalCountBound = new Bound<int>();
        private TypeDropdownDrawer typeAttributeDrawer = new TypeDropdownDrawer(targetTypes: new List<Type> {typeof(Tile), typeof(Cell)}, excludingTypes: new List<Type>(), includeAbstracts: false, showPartialNames: true);

        protected override void DrawConditionsInternal()
        {
            DrawInVerticallyArea(
                toDraw: () =>
                {
                    DrawInHorizontallyArea(
                        toDraw: () =>
                        {
                            DrawGoalIndexCondition();
                            DrawGoalTypeCondition();
                            DrawGoalColorCondition();
                        });

                    DrawInHorizontallyArea(toDraw: DrawGoalCountCondition);
                });
        }

        private void DrawGoalIndexCondition()
        {
            EditorUtilities.InsertIntField(ref goalIndex, label: "Goal Index:");
        }

        private void DrawGoalTypeCondition()
        {
            EditorUtilities.InsertTypeSelectionDropDown(ref goalType, label: "Goal Type:", typeAttributeDrawer, labelsWidth: 70);
        }

        private void DrawGoalColorCondition()
        {
            EditorUtilities.InsertEnumDropDown(ref goalColor, label: "Goal Color:", labelsWidth: 70);
        }

        private void DrawGoalCountCondition()
        {
            EditorUtilities.InsertIntBoundField(ref goalCountBound, label: "Goal Count");
        }

        protected override LevelGoalCondition GetSelectedCondition()
        {
            return new LevelGoalCondition(goalIndex, goalType, goalColor, goalCountBound);
        }
    }
}