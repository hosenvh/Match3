using Match3.Foundation.Unity;
using PandasCanPlay.HexaWord.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using static Medrick.Development.Unity.BuildManagement.GolmoradAssetChangerBuildAction;

namespace Medrick.Development.Unity.BuildManagement
{
    [CreateAssetMenu(menuName = "Medrick/Build Management/Configs/Task Icon Database")]
    public class AssetLocalizationDatabase : ScriptableObject
    {
        [System.Serializable]
        public struct TaskIconConfigEntry
        {
            public TaskConfig taskConfig;
            public ResourceSpriteAsset iranIconSpriteResource;
            [FormerlySerializedAs("turkeyIconSpriteResource")] public ResourceSpriteAsset internationalIconSpriteResource;

            public ResourceSpriteAsset IconSpriteFor(Country targetCountry)
            {
                switch (targetCountry)
                {
                    case Country.Iran:
                        return iranIconSpriteResource;
                    case Country.International:
                        return internationalIconSpriteResource;
                    default:
                        throw new Exception("Not defined");
                }
            }
        }

        [System.Serializable]
        public struct ScenarioAudioEntry
        {
            [System.Serializable]
            public struct ScenarioItemAudioEntry
            {
                [System.Serializable]
                public struct DialogueAudioEntry
                {
                    [System.Serializable]
                    public struct LocalizedAudioClip
                    {
                        public Country country;
                        public AudioClip audioClip;
                    }

                    public int dialogueIndex;
                    public List<LocalizedAudioClip> localizedAudioClip;

                    public AudioClip AudioClipFor(Country targetCountry)
                    {
                        return localizedAudioClip.Find(localizedAudioClip => localizedAudioClip.country == targetCountry).audioClip;
                    }
                }

                public int scenarioItemIndexInConfig;
                public List<DialogueAudioEntry> dialogueAudioEntries;
            }

            public ScenarioConfig config;
            public List<ScenarioItemAudioEntry> scenarioItemAudioEntries;
        }

        [ArrayElementTitle(nameof(TaskIconConfigEntry.taskConfig))]
        [SerializeField] private List<TaskIconConfigEntry> taskIconEntries;
        [ArrayElementTitle(nameof(ScenarioAudioEntry.config))]
        [SerializeField] private List<ScenarioAudioEntry> scenarioItemAudioEntries;

        public List<TaskIconConfigEntry> TaskIconEntries()
        {
            return taskIconEntries;
        }

        public List<ScenarioAudioEntry> ScenarioAudioEntries()
        {
            return scenarioItemAudioEntries;
        }
    }
}