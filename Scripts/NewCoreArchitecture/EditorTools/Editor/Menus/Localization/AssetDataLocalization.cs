using System.Text;
using I2.Loc;
using UnityEditor;
using UnityEngine;


namespace Match3.EditorTools.Editor.Menus.Localization
{
    // TODO: Refactor This F-Shit. 1st, the namings for public methods and the tool itself is awful, second, it has duplicate code with scenarioConfigEditor, which not only is not DRY but also is very error prone
    public class AssetDataLocalization
    {
        [MenuItem("Golmorad/Localization/Make Scenario Terms")]
        public static void MakeScenarioTerms()
        {
            var localizationSource = GetLanguageSourceAsset(LocalizationManager.ScenarioLanguageSource);
            var scenarioConfigs = AssetEditorUtilities.FindAssetsByType<ScenarioConfig>();
            StringBuilder termNameBuilder = new StringBuilder();

            var farsiLanguageIndex = GetFarsiLanguageIndex(localizationSource);

            foreach (var scenarioConfig in scenarioConfigs)
            {
                var pathParts = AssetDatabase.GetAssetPath(scenarioConfig).Split('/');
                var day = pathParts[pathParts.Length - 2];

                foreach (var scenarioItem in scenarioConfig.scenarioItems)
                {
                    if (scenarioItem.scenarioType == ScenarioType.Dialogue)
                    {
                        foreach (var dialogue in scenarioItem.dialogues)
                        {
                            termNameBuilder.Clear();
                            termNameBuilder.Append($"Scenario/Day{day}_Dialogue_");
                            termNameBuilder.Append(scenarioConfig.name.Replace(" ", "")
                                                 + "_SI" + scenarioConfig.scenarioItems.FindIndex(si => si == scenarioItem)
                                                 + "_D" + scenarioItem.dialogues.FindIndex(d => d == dialogue));

                            var termData = AddTermAndSetTranslation(localizationSource, farsiLanguageIndex, termNameBuilder.ToString(),
                                                                    dialogue.characterDialogue);
                            dialogue.characterLocalizedString = termData.Term;
                        }

                        EditorUtility.SetDirty(scenarioConfig);
                    }
                    else if (scenarioItem.scenarioType == ScenarioType.BubbleDialogue || scenarioItem.scenarioType == ScenarioType.Screen)
                    {
                        termNameBuilder.Clear();
                        var typeName = scenarioItem.scenarioType == ScenarioType.BubbleDialogue ? "DialogBubble" : "Screen";
                        termNameBuilder.Append($"Scenario/Day{day}_{typeName}_");
                        termNameBuilder.Append(scenarioConfig.name.Replace(" ", "")
                                             + "_SI" + scenarioConfig.scenarioItems.FindIndex(si => si == scenarioItem));

                        var termData = AddTermAndSetTranslation(localizationSource, farsiLanguageIndex, termNameBuilder.ToString(),
                                                                scenarioItem.string_0);
                        scenarioItem.localizedString_0 = termData.Term;

                        EditorUtility.SetDirty(scenarioConfig);
                    }
                }
            }

            EditorUtility.SetDirty(localizationSource);
            AssetDatabase.SaveAssets();

            localizationSource.mSource.UpdateDictionary();
        }



        [MenuItem("Golmorad/Localization/Make Tutorial Terms")]
        public static void MakeTermsForTutorials()
        {
            var localizationSource = GetLanguageSourceAsset(LocalizationManager.TutorialLanguageSource);
            var tutorialConfigs = AssetEditorUtilities.FindAssetsByType<TutorialConfig>();
            var farsiLanguageIndex = GetFarsiLanguageIndex(localizationSource);

            foreach (var tutorialConfig in tutorialConfigs)
            {
                if (string.IsNullOrEmpty(tutorialConfig.dialogueString)) continue;

                var termData = AddTermAndSetTranslation(localizationSource, farsiLanguageIndex, "Tutorial/" + tutorialConfig.name,
                                                        tutorialConfig.dialogueString);
                tutorialConfig.dialogueLocalizedString = termData.Term;

                EditorUtility.SetDirty(tutorialConfig);
            }

            EditorUtility.SetDirty(localizationSource);
            AssetDatabase.SaveAssets();

            localizationSource.mSource.UpdateDictionary();
        }


        [MenuItem("Golmorad/Localization/Make Task Terms")]
        public static void MakeTermsForTasks()
        {
            var localizationSource = GetLanguageSourceAsset(LocalizationManager.TaskLanguageSource);
            var taskConfigs = AssetEditorUtilities.FindAssetsByType<TaskConfig>();
            var farsiLanguageIndex = GetFarsiLanguageIndex(localizationSource);

            foreach (var taskConfig in taskConfigs)
            {
                if (string.IsNullOrEmpty(taskConfig.taskString)) continue;

                var termData = AddTermAndSetTranslation(localizationSource, farsiLanguageIndex, "Task/" + taskConfig.name,
                                                        taskConfig.taskString);
                taskConfig.taskLocalizedString = termData.Term;

                EditorUtility.SetDirty(taskConfig);
            }

            EditorUtility.SetDirty(localizationSource);
            AssetDatabase.SaveAssets();

            localizationSource.mSource.UpdateDictionary();
        }



        public static LanguageSourceAsset GetLanguageSourceAsset(string languageSourceName)
        {
            return Resources.Load<LanguageSourceAsset>(languageSourceName);
        }

        private static int GetFarsiLanguageIndex(LanguageSourceAsset languageSource)
        {
            return languageSource.mSource.GetLanguageIndex("Persian", true, false);
        }

        public static TermData AddTermAndSetTranslation(LanguageSourceAsset languageSource, int languageIndex, string termName, string translation)
        {
            var term = languageSource.mSource.AddTerm(termName, eTermType.Text, false);
            term.SetTranslation(languageIndex, translation);
            return term;
        }

    }

}