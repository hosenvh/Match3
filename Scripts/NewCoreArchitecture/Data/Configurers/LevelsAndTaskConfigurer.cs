using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Unity;
using Match3.Foundation.Unity.Configuration;
using UnityEngine;


namespace Match3.Data.Configuration
{
    [CreateAssetMenu(menuName = "Match3/Configurations/LevelsAndTaskConfigurer")]
    public class LevelsAndTaskConfigurer : ScriptableConfiguration, Configurer<LevelManager>, Configurer<TaskManager>
    {
        [SerializeField] private int lastPublishedLevelIndex;
        [SerializeField] private int lastTaskId;
        [SerializeField] private ResourceDayConfigAsset[] dayConfigs;
        [SerializeField] private ResourceBoardConfigAsset[] levelConfigs;

        public void Configure(LevelManager entity)
        {
            entity.SetLevels(levelConfigs);
            entity.SetLastPublishedLevelIndex(lastPublishedLevelIndex);
        }

        public void Configure(TaskManager entity)
        {
            entity.SetDaysConfigs(dayConfigs);
            entity.SetLastTaskId(lastTaskId);
        }


        public override void RegisterSelf(ConfigurationManager manager)
        {
            manager.Register<LevelManager>(this);
            manager.Register<TaskManager>(this);
        }

        public int GetLastPublishedLevelIndex()
        {
            return lastPublishedLevelIndex;
        }

        public int GetLastPublishedTaskIndex()
        {
            return lastTaskId;
        }

        public ResourceDayConfigAsset[] GetDayConfigs()
        {
            return dayConfigs;
        }

        public ResourceBoardConfigAsset[] GetBoardsConfigs()
        {
            return levelConfigs;
        }
    }
}
