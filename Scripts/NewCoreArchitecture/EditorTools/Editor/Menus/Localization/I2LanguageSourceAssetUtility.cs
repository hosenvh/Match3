using I2.Loc;
using I2.Loc.Match3Extensions;
using System.Collections.Generic;
using System.Linq;
using Match3.EditorTools.Editor.Base;
using UnityEditor;
using UnityEngine;

namespace Match3.EditorTools.Editor.Menus.Localization
{
    // TODO: Refactor this.
    public partial class I2LanguageSourceAssetUtility : EditorWindow
    {
        [MenuItem("Golmorad/Localization/I2 Language Source Asset Utility", isValidateFunction: false, priority: 1000)]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(I2LanguageSourceAssetUtility));
        }

        LanguageSourceAsset languageSource;

        string result;

        [SerializeField] LocalizedStringTerm localizeStringToSearch;

        SerializedProperty termToSearchProperty;

        [SerializeField] LocalizedStringTerm fontLocalizedString;
        [SerializeField] LocalizedStringTerm materialLocalizedString;

        SerializedProperty fontTermProperty;
        SerializedProperty materialTermProperty;

        public void Init()
        {
            var serializableObject = new SerializedObject(this);


            fontTermProperty = serializableObject.FindProperty(nameof(fontLocalizedString));
            materialTermProperty = serializableObject.FindProperty(nameof(materialLocalizedString));


            termToSearchProperty = serializableObject.FindProperty(nameof(localizeStringToSearch));
        }

        void OnHierarchyChange()
        {
            Init();
        }

        void Awake()
        {
            Init();
        }

        void OnGUI()
        {
            DrawFindUsage();
            DrawAssetUtility();
            DrawMaterialTermFixing();
            DrawMissingTranslationFixer();
            DrawScenarioLanguageSourceUtilities();
        }

        private void DrawFindUsage()
        {
            GUILayout.BeginVertical("Find Usage In Prefabs", "window");

            EditorGUILayout.PropertyField(termToSearchProperty);

            fontTermProperty.serializedObject.ApplyModifiedProperties();
            if (GUILayout.Button("Find Usage"))
                SearchFor(localizeStringToSearch.mTerm);


            GUILayout.EndVertical();
        }

        private void DrawAssetUtility()
        {
            GUILayout.BeginVertical("Asset Utility", "window");
            languageSource = (LanguageSourceAsset)EditorGUILayout.ObjectField("Source", languageSource, typeof(LanguageSourceAsset), allowSceneObjects: false);

            if (GUILayout.Button("Validate Assets"))
                ValidateAssets(languageSource);

            if (GUILayout.Button("Clean Up Assets"))
                CleanUpAssets(languageSource);

            GUILayout.Label(result);

            GUILayout.EndVertical();
        }

        private void DrawMaterialTermFixing()
        {
            GUILayout.BeginVertical("Find Usage", "window");

            EditorGUILayout.PropertyField(fontTermProperty);
            EditorGUILayout.PropertyField(materialTermProperty);

            fontTermProperty.serializedObject.ApplyModifiedProperties();
            if (GUILayout.Button("Add Material"))
                AddMaterial(fontLocalizedString.mTerm, materialLocalizedString.mTerm);

            GUILayout.EndVertical();
        }

        private void DrawMissingTranslationFixer()
        {
            GUILayout.BeginVertical("Missing Translations", "window");
            if (GUILayout.Button("Find Terms Without all 3 languages"))
                LogTermsWithMissedLanguages(FindTermsWithMissedLanguages());
            if (GUILayout.Button("Find AND Fix Terms Without all 3 languages"))
            {
                List<TermData> termsWithMissedLanguages = FindTermsWithMissedLanguages();
                LogTermsWithMissedLanguages(termsWithMissedLanguages);
                FixMissedLanguagesTo(termsWithMissedLanguages);
            }
            GUILayout.EndVertical();
        }

        private void DrawScenarioLanguageSourceUtilities()
        {
            GUILayout.BeginVertical("Scenario Language Source", "window");
            if (GUILayout.Button("Sort Scenario"))
                SortScenario();
            if (GUILayout.Button("Refresh Scenario Descriptions"))
                RefreshScenarioDescription();
            if (GUILayout.Button("Sort Scenario As I2 Default"))
                SortAsI2Default(languageSource);
            GUILayout.EndVertical();
        }

        private void SearchFor(string mTerm)
        {
            Debug.Log($" ------------ Usage for {mTerm} ---------------");
            foreach (var comp in AssetEditorUtilities.FindComponentsInScenesAndPrefabsByType<Localize>(true))
            {
                if (comp.SecondaryTerm.Equals(mTerm))
                    Debug.Log(comp.transform.root, comp.transform.root);
                
            }

            Debug.Log("--------------------------------------------");
        }

        private void AddMaterial(string fontTerm, string materialTerm)
        {
            BasicComponentEditorOperationUtilities.ModifyComponentsInPrefabs<Localize>(
                (comp) => comp.SecondaryTerm.Equals(fontTerm),
                (comp) => ApplyToPrefab(comp, materialTerm));

        }

        private void ApplyToPrefab(Localize comp, string materialTerm)
        {
            Undo.RecordObject(comp.gameObject, "Add Material");

            var ternaryTerm = comp.gameObject.GetComponent<TernaryTerm>();

            if (ternaryTerm == null)
                ternaryTerm = Undo.AddComponent<TernaryTerm>(comp.gameObject);


            Undo.RecordObject(ternaryTerm, "Add Material");

            ternaryTerm.SetTerm(materialTerm);
        }

        private void ValidateAssets(LanguageSourceAsset languageSource)
        {
            HashSet<Object> termsAssets = ExtractTermsAsset(languageSource.mSource, out bool hasMissingAssets);

            HashSet<Object> sourceAssets = new HashSet<Object>(languageSource.mSource.Assets);

            if (termsAssets.IsSubsetOf(sourceAssets) && hasMissingAssets == false)
                result = "Assets are valid";
            else
                result = "There are missing assets";
        }

        private void CleanUpAssets(LanguageSourceAsset languageSource)
        {
            HashSet<Object> termsAssets = ExtractTermsAsset(languageSource.mSource, out bool hasMissingAssets);

            languageSource.mSource.Assets.RemoveAll(asset => termsAssets.Contains(asset) == false);

            languageSource.mSource.UpdateAssetDictionary();
            UnityEditor.EditorUtility.SetDirty(languageSource);
        }

        private HashSet<Object> ExtractTermsAsset(LanguageSourceData languageData, out bool hasMissingAssets)
        {
            HashSet<Object> assets = new HashSet<Object>();
            hasMissingAssets = false;

            foreach (var termData in languageData.mTerms)
            {
                foreach (var translation in termData.Languages)
                {
                    var asset = languageData.FindAsset(translation);
                    if (asset != null)
                        assets.Add(asset);
                    else if (IsAsset(termData))
                    {
                        hasMissingAssets = true;
                        Debug.LogError($"Missing asset for term  {termData.Term}");
                    }
                }
            }

            return assets;
        }

        private bool IsAsset(TermData termData)
        {
            return termData.TermType != eTermType.Text;
        }


        private List<TermData> FindTermsWithMissedLanguages()
        {
            return languageSource.mSource.mTerms.FindAll(data => data.Languages.Length != 3);
        }

        private void LogTermsWithMissedLanguages(List<TermData> termsData)
        {
            Debug.Log($"{termsData.Count} Terms Found with missing languages");
            foreach (TermData data in termsData)
                Debug.LogError($"The Term {data.Term} has Missing Language.", languageSource);
        }

        private void FixMissedLanguagesTo(List<TermData> termDatas)
        {
            foreach (TermData termData in termDatas)
            {
                for (int i = termData.Languages.Length; i < 3; i++)
                    termData.Languages = termData.Languages.Concat(new[] {""}).ToArray();
            }
            EditorUtility.SetDirty(languageSource);
        }


        private void SortScenario()
        {
            LanguageSourceAsset scenarioSourceAsset = Resources.Load<LanguageSourceAsset>(LocalizationManager.ScenarioLanguageSource);

            List<ScenarioTermDataPackage> termDataPackages = new List<ScenarioTermDataPackage>(scenarioSourceAsset.mSource.mTerms.Count);
            foreach (TermData termData in scenarioSourceAsset.mSource.mTerms)
                termDataPackages.Add(new ScenarioTermDataPackage(termData));

            termDataPackages = termDataPackages.OrderBy(package => package.Index).ToList();

            scenarioSourceAsset.mSource.mTerms.Clear();
            foreach (ScenarioTermDataPackage termDataPackage in termDataPackages)
                scenarioSourceAsset.mSource.mTerms.Add(termDataPackage.OriginalData);

            EditorUtility.SetDirty(scenarioSourceAsset);
        }

        private void RefreshScenarioDescription()
        {
            LanguageSourceAsset scenarioSourceAsset = Resources.Load<LanguageSourceAsset>(LocalizationManager.ScenarioLanguageSource);
            List<ScenarioItem> allScenarioItems = AssetEditorUtilities.FindAssetsByType<ScenarioConfig>().SelectMany(scenarioConfig => scenarioConfig.scenarioItems).ToList();
            List<TermData> termsData = new List<TermData>(scenarioSourceAsset.mSource.mTerms);
            foreach (TermData termData in termsData)
            {
                ScenarioDialogueData scenarioDialogueData = null;
                ScenarioItem scenarioItem = allScenarioItems.Find(item => item.localizedString_0.mTerm == termData.Term);
                if (scenarioItem == null)
                {
                    foreach (ScenarioItem scenarioItemElement in allScenarioItems)
                    {
                        scenarioDialogueData = scenarioItemElement.dialogues.Find(dialogue => dialogue.characterLocalizedString.mTerm == termData.Term);
                        if (scenarioDialogueData != null)
                        {
                            scenarioItem = scenarioItemElement;
                            break;
                        }
                    }
                }
                if (scenarioItem != null)
                {
                    if (scenarioItem.scenarioType == ScenarioType.Screen)
                        termData.Description = $"Screen |             | {scenarioItem.scenarioType}";
                    else if (scenarioDialogueData == null)
                        termData.Description = $"{scenarioItem.character_0.ToString()} |             | {scenarioItem.scenarioType}";
                    else
                        termData.Description = $"{scenarioDialogueData.characterName} | {scenarioDialogueData.characterState} | {scenarioItem.scenarioType}";
                }
                else
                {
                    termData.Description = "NOT USED";
                    Debug.LogError($"Scenario Description Refresh Bug | In i2 Scenario Language source a term is found, without any corresponding item in scenario config files! Term in I2: {termData.Term}");
                }
            }

            scenarioSourceAsset.mSource.mTerms.Clear();
            foreach (TermData termData in termsData)
                scenarioSourceAsset.mSource.mTerms.Add(termData);

            EditorUtility.SetDirty(scenarioSourceAsset);

            Debug.Log("Scenario Description Finished");
        }

        private void SortAsI2Default(LanguageSourceAsset source)
        {
            source.mSource.mTerms.Sort((a, b) => string.CompareOrdinal(a.Term, b.Term));
            EditorUtility.SetDirty(source);
        }
    }
}
