using Match3.EditorTools.Editor.Menus.Utility.BoardFinder.Conditions.Base;
using Match3.EditorTools.Editor.Utility;
using Match3.Game.Gameplay.SubSystems.LevelEnding;


namespace Match3.EditorTools.Editor.Menus.Utility.BoardFinder.Conditions
{
    public class LevelDifficultyCondition : BoardFinderCondition
    {
        private readonly DifficultyLevel levelDifficulty;

        public LevelDifficultyCondition(DifficultyLevel levelDifficulty)
        {
            this.levelDifficulty = levelDifficulty;
        }

        public bool IsSatisfied(BoardConfig boardConfig)
        {
            return boardConfig.levelConfig.difficultyLevel == levelDifficulty;
        }
    }

    public class LevelDifficultyConditionDrawer : BoardFinderConditionDrawer<LevelDifficultyCondition>
    {
        private DifficultyLevel levelDifficulty;

        protected override void DrawConditionsInternal()
        {
            EditorUtilities.InsertEnumDropDown(ref levelDifficulty, label: "Level Difficulty");
        }

        protected override LevelDifficultyCondition GetSelectedCondition()
        {
            return new LevelDifficultyCondition(levelDifficulty);
        }
    }
}