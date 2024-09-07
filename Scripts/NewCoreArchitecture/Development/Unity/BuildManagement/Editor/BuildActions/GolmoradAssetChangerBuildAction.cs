using I2.Loc;
using Match3.Foundation.Unity;
using System.Collections.Generic;
using Newtonsoft.Json.Utilities;
using UnityEngine;

namespace Medrick.Development.Unity.BuildManagement
{
    // NOTE: IF OTHER COUNTRIES ARE ADDED, MAKE THIS EXTENDABLE (DON"T USE ENUM). OR CHANGE THE SYSTEM COMPLETELY
    // NOTE: This is only a temporary solution. 
    [CreateAssetMenu(menuName = "Medrick/Build Management/Build Actions/Task Icon Changer")]
    public class GolmoradAssetChangerBuildAction : ScriptableBuildAction
    {
        public enum Country
        {
            Iran,
            International,
        }

        [System.Serializable]
        private struct ConfigIconEntry
        {
            public TaskConfig taskConfig;
            public ResourceSpriteAsset iconSpriteResource;
        }

        [SerializeField] Country targetCountry;
        [SerializeField] private AssetLocalizationDatabase database;

        public override void Execute()
        {
            foreach(var entry in database.TaskIconEntries())
            {
                entry.taskConfig.iconSpriteResource = entry.IconSpriteFor(targetCountry);
                UnityEditor.EditorUtility.SetDirty(entry.taskConfig);
            }
            
            foreach (var audioEntry in database.ScenarioAudioEntries())
            {
                var scenarioConfig = audioEntry.config;
                foreach (var scenarioItemAudioEntry in audioEntry.scenarioItemAudioEntries)
                {
                    var scenarioItem = scenarioConfig.scenarioItems[scenarioItemAudioEntry.scenarioItemIndexInConfig];
                    if (scenarioItem.scenarioType == ScenarioType.Audio)
                    {
                        scenarioItem.audioClip_0 = scenarioItemAudioEntry.dialogueAudioEntries[0].AudioClipFor(targetCountry);
                    }
                }

                UnityEditor.EditorUtility.SetDirty(audioEntry.config);
            }
        }

        public override void Revert()
        {

        }
    }
}