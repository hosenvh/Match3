using I2.Loc;
using System.Collections.Generic;
using UnityEngine;

namespace Medrick.Development.Unity.BuildManagement
{
    [CreateAssetMenu(menuName = "Medrick/Build Management/Build Actions/I2 Language Changer")]
    public class I2LanguageChangerBuildAction : ScriptableBuildAction
    {
        public string languageNameToEnable;

        public override void Execute()
        {
            LocalizationManager.CurrentLanguage = languageNameToEnable;
            SetLanguagesSourcesActiveness(isActive: true);
        }

        public override void Revert()
        {
            SetLanguagesSourcesActiveness(isActive: false);
        }

        private void SetLanguagesSourcesActiveness(bool isActive)
        {
            foreach (var languageSource in GlobalLanguageSources())
            {
                var languageData = FindLanguageDataFor(languageSource, languageNameToEnable);
                if (languageData != null)
                {
                    languageData.SetEnabled(isActive);
                    UnityEditor.EditorUtility.SetDirty(languageSource);
                }
                else
                    Debug.LogError($"LanguageSource {languageSource} doesn't have the language {languageNameToEnable}");
            }
        }

        private List<LanguageSourceAsset> GlobalLanguageSources()
        {
            var languageSources = new List<LanguageSourceAsset>();

            foreach (var path in LocalizationManager.GlobalSources)
                languageSources.Add(Resources.Load<LanguageSourceAsset>(path));

            return languageSources;
        }
		

        private LanguageData FindLanguageDataFor(LanguageSourceAsset languageSourceAsset,string languageName)
        {
            return languageSourceAsset.mSource.mLanguages.Find(l => l.Name.Equals(languageName));
        }
    }
}