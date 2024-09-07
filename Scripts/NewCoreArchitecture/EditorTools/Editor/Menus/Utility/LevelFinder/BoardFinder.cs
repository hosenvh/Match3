using System.Collections.Generic;
using System.Linq;
using KitchenParadise.Utiltiy.Base;
using Match3.Data.Configuration;
using Match3.EditorTools.Editor.Menus.Utility.BoardFinder.Conditions.Base;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Unity;


namespace Match3.EditorTools.Editor.Menus.Utility.BoardFinder
{
    public class BoardFinder
    {
        private List<BoardConfig> boardConfigs = new List<BoardConfig>();

        public List<BoardConfig> FindBoardsBasedOnConditions(List<BoardFinderCondition> conditions)
        {
            LoadBoardConfigs();

            List<BoardConfig> result = new List<BoardConfig>();

            boardConfigs.DoForEachElement(
                todo: boardConfig =>
                {
                    if (AreAllConditionsSatisfiedForBoard(boardConfig))
                        result.Add(boardConfig);
                });

            return result;

            bool AreAllConditionsSatisfiedForBoard(BoardConfig boardConfig)
            {
                foreach (BoardFinderCondition condition in conditions)
                    if (condition.IsSatisfied(boardConfig) == false)
                        return false;
                return true;
            }
        }

        private void LoadBoardConfigs()
        {
            if(AreBoardsAlreadyLoaded())
                return;

            LevelsAndTaskConfigurer configurer = FindBoardsConfigurer();
            boardConfigs = GetBoardsConfigs(configurer);

            bool AreBoardsAlreadyLoaded() => boardConfigs.Count != 0;

            LevelsAndTaskConfigurer FindBoardsConfigurer()
            {
                return (LevelsAndTaskConfigurer) ServiceLocator.Find<ConfigurationManager>().FindConfigurer<LevelManager>();
            }

            List<BoardConfig> GetBoardsConfigs(LevelsAndTaskConfigurer boardsConfigurer)
            {
                ResourceBoardConfigAsset[] boardsConfigsAssets = boardsConfigurer.GetBoardsConfigs();
                return boardsConfigsAssets.Select(configAsset => configAsset.Load()).ToList();
            }
        }
    }
}