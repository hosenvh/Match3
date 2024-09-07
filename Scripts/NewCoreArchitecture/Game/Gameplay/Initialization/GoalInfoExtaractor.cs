using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Tiles;
using System;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using System.Collections.Generic;
using Match3.Game.Gameplay.Core;

namespace Match3.Game.Gameplay.Initialization
{
    public class GoalInfoExtaractor
    {
        public struct GoalInfo
        {
            public GoalTargetType goalType;
            public int goalAmount;

            public GoalInfo(GoalTargetType goalType, int targetAmount)
            {
                this.goalType = goalType;
                this.goalAmount = targetAmount;
            }
        }

        public List<GoalInfo>  Extract(BoardConfig boardConfig)
        {
            var goals = new List<GoalInfo>();

            foreach (var goal in boardConfig.levelConfig.goals)
                goals.Add(new GoalInfo(goal.ExtractGoalType(), goal.goalAmount));

            return goals;
        }
    }
}