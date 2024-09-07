using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using I2.Loc;
using RTLTMPro;
using SeganX;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Windows;
using Object = UnityEngine.Object;


namespace Match3.EditorTools.Editor.Menus.Localization
{
    public class LocalizationTextFinderWindow : EditorWindow
    {
        private enum ResultMode
        {
            None,
            FarsiScripts,
            FarsiScriptableObject,
            LocalTextInPrefab,
            LocalTextInScene,
            RtlTextMeshProInPrefab,
            RtlTextMeshProInScene,
            ComponentsWithFarsiInPrefabs,
            ComponentsWithFarsiInScenes
        }

        [Serializable]
        private class SearchResult
        {
            public GameObject rootGameObject;
            public string targetHierarchy;
        }

        private readonly string[] includedPaths = new string[] {"Assets/Configs/", "Assets/Resources/"};
        private readonly string[] excludedPaths = new string[]
                                                  {
                                                      "Assets/Resources/GameAnalytics", "Assets/Resources/I2Languages",
                                                      "Assets/Configs/MockServerConfiguration", "Assets/Configs/ServerConfigSO", "Assets/Configs/Cohorts/Offline_1/ServerConfigSO"
                                                  };

        private readonly string[] excludeScripts = new[] {"MonoEx.cs", "ContextMenu.cs", "CafebazaarMarket.cs", "RTLSupport.cs", "Utilities.cs"};

        private ResultMode showResultMode = ResultMode.None;

        private List<Object> assetsWithFarsiText = new List<Object>();
        private List<SearchResult> searchResults = new List<SearchResult>();

        private Vector2 resultScrollPos;
        private Color defaultColor;

        private HashSet<char> farsiChars = new HashSet<char>();
        readonly Regex farsiCharsRegex = new Regex("[\u0600-\u06ff]");



        [MenuItem("Golmorad/Localization/Farsi Text Finder")]
        private static void Init()
        {

            LocalizationTextFinderWindow window = (LocalizationTextFinderWindow) GetWindow(typeof(LocalizationTextFinderWindow));
            window.titleContent = new GUIContent("Farsi Finder");
            window.Show();
        }

        public LocalizationTextFinderWindow()
        {
            for (char i = '\u0600'; i <= '\u06ff'; i++)
            {
                farsiChars.Add(i);
            }
        }

        private void OnDestroy()
        {
            assetsWithFarsiText.Clear();
            searchResults.Clear();
            EditorUtility.UnloadUnusedAssetsImmediate();
        }

        void OnGUI()
        {

            // ---------------------------- Farsi Texts In Scripts ---------------------------- \\

            if (GUILayout.Button("Find Farsi Containing Scripts"))
            {
                assetsWithFarsiText.Clear();
                assetsWithFarsiText = FindFarsiContainingScripts();
                showResultMode = ResultMode.FarsiScripts;
                EditorUtility.UnloadUnusedAssetsImmediate();
            }

            GUILayout.Space(5);


            // ---------------------------- Farsi Texts In Scriptable Objects ---------------------------- \\

            if (GUILayout.Button("Find Farsi Containing Scriptable Objects"))
            {
                assetsWithFarsiText.Clear();
                assetsWithFarsiText = FindFarsiContainingScriptableObjects();
                showResultMode = ResultMode.FarsiScriptableObject;
                EditorUtility.UnloadUnusedAssetsImmediate();
            }

            GUILayout.Space(5);


            // ---------------------------- Farsi Fields In Components ---------------------------- \\

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Find Components With Farsi Field In Prefabs"))
            {
                searchResults.Clear();
                searchResults = FindAllComponentsWithFarsiFieldsInPrefabs();
                showResultMode = ResultMode.ComponentsWithFarsiInPrefabs;
                EditorUtility.UnloadUnusedAssetsImmediate();
            }

            GUILayout.Space(5);

            if (GUILayout.Button("Find Components With Farsi Field In Scenes"))
            {
                searchResults.Clear();
                searchResults = FindAllComponentsWithFarsiFieldsInScenes();
                showResultMode = ResultMode.ComponentsWithFarsiInScenes;
                EditorUtility.UnloadUnusedAssetsImmediate();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(5);


            // ---------------------------- LocalText Usages ---------------------------- \\

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Find LocalText Usages In Prefabs"))
            {
                searchResults.Clear();
                searchResults = FindAllComponentUsageInPrefabs<LocalText>();
                showResultMode = ResultMode.LocalTextInPrefab;
                EditorUtility.UnloadUnusedAssetsImmediate();
            }

            GUILayout.Space(5);

            if (GUILayout.Button("Find LocalText Usages In Scenes"))
            {
                searchResults.Clear();
                searchResults = FindAllComponentUsagesInScenes<LocalText>();
                showResultMode = ResultMode.LocalTextInScene;
                EditorUtility.UnloadUnusedAssetsImmediate();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            // ---------------------------- RTLTextMeshPro Usages ---------------------------- \\

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Find RTLTextMeshPro Usages In Prefabs"))
            {
                searchResults.Clear();
                searchResults = FindAllComponentUsageInPrefabs<RTLTextMeshPro>();
                showResultMode = ResultMode.RtlTextMeshProInPrefab;
                EditorUtility.UnloadUnusedAssetsImmediate();
            }

            GUILayout.Space(5);

            if (GUILayout.Button("Find RTLTextMeshPro Usages In Scenes"))
            {
                searchResults.Clear();
                searchResults = FindAllComponentUsagesInScenes<RTLTextMeshPro>();
                showResultMode = ResultMode.RtlTextMeshProInScene;
                EditorUtility.UnloadUnusedAssetsImmediate();
            }
            GUILayout.EndHorizontal();

            // ---------------------------- Draw Results ---------------------------- \\

            GUILayout.Space(5);
            DrawResults();

        }


        void DrawResults()
        {
            if (showResultMode == ResultMode.None) return;

            defaultColor = GUI.color;
            GUI.color = Color.yellow;
            switch (showResultMode)
            {
                case ResultMode.FarsiScripts:
                    GUILayout.Label($"Found ({assetsWithFarsiText.Count}) Scripts With Farsi Texts:", EditorStyles.boldLabel);
                    break;
                case ResultMode.FarsiScriptableObject:
                    GUILayout.Label($"Found ({assetsWithFarsiText.Count}) Scriptable Objects With Farsi Texts:", EditorStyles.boldLabel);
                    break;
                case ResultMode.ComponentsWithFarsiInPrefabs:
                    GUILayout.Label($"Found ({searchResults.Count}) Components With Farsi Fields In Prefabs:", EditorStyles.boldLabel);
                    break;
                case ResultMode.ComponentsWithFarsiInScenes:
                    GUILayout.Label($"Found ({searchResults.Count}) Components With Farsi Fields In Scenes:", EditorStyles.boldLabel);
                    break;
                case ResultMode.LocalTextInPrefab:
                    GUILayout.Label($"Found ({searchResults.Count}) LocalText Usages In Prefabs:", EditorStyles.boldLabel);
                    break;
                case ResultMode.RtlTextMeshProInPrefab:
                    GUILayout.Label($"Found ({searchResults.Count}) RTLTextMeshPro Usages In Prefabs:", EditorStyles.boldLabel);
                    break;
                case ResultMode.LocalTextInScene:
                    GUILayout.Label($"Found ({searchResults.Count}) LocalText Usages In Scenes:", EditorStyles.boldLabel);
                    break;
                case ResultMode.RtlTextMeshProInScene:
                    GUILayout.Label($"Found ({searchResults.Count}) RTLTextMeshPro Usages In Scenes:", EditorStyles.boldLabel);
                    break;
            }
            GUI.color = defaultColor;


            if (((showResultMode == ResultMode.FarsiScripts || showResultMode == ResultMode.FarsiScriptableObject) && assetsWithFarsiText.Count == 0)
             || (showResultMode != ResultMode.FarsiScripts && showResultMode != ResultMode.FarsiScriptableObject && searchResults.Count == 0))
            {
                defaultColor = GUI.color;
                GUI.color = Color.green;
                GUILayout.Label("Nothing Found!");
                GUI.color = defaultColor;
                return;
            }


            GUILayout.BeginVertical(EditorStyles.helpBox);
            resultScrollPos = GUILayout.BeginScrollView(resultScrollPos);

            switch (showResultMode)
            {
                case ResultMode.FarsiScripts:
                    foreach (var script in assetsWithFarsiText)
                    {
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("Select", GUILayout.Width(100)))
                            Selection.activeObject = script;
                        GUILayout.Label($"Farsi Containing Script: ({script.name}.cs)");
                        GUILayout.EndHorizontal();
                    }
                    break;
                case ResultMode.FarsiScriptableObject:
                    foreach (var so in assetsWithFarsiText)
                    {
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("Select", GUILayout.Width(100)))
                            Selection.activeObject = so;
                        GUILayout.Label($"Farsi Containing ScriptableObject: ({so.name}.asset)");
                        GUILayout.EndHorizontal();
                    }
                    break;
                case ResultMode.ComponentsWithFarsiInPrefabs:
                case ResultMode.LocalTextInPrefab:
                case ResultMode.RtlTextMeshProInPrefab:
                    foreach (var result in searchResults)
                    {
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("Select", GUILayout.Width(100)))
                            Selection.activeObject = result.rootGameObject;
                        GUILayout.Label($"{result.rootGameObject.name} - {result.targetHierarchy}");
                        GUILayout.EndHorizontal();
                    }
                    break;
                case ResultMode.ComponentsWithFarsiInScenes:
                case ResultMode.RtlTextMeshProInScene:
                case ResultMode.LocalTextInScene:
                    foreach (var result in searchResults)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label($"{result.targetHierarchy}");
                        GUILayout.EndHorizontal();
                    }
                    break;
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }




        private List<Object> FindFarsiContainingScripts()
        {
            List<Object> tempAssetsWithFarsiText = new List<Object>();
            string[] assetPaths = AssetDatabase.GetAllAssetPaths();

            foreach (string assetPath in assetPaths)
            {
                if (IsAssetIntendedScript(assetPath))
                {
                    var monoObject = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);

                    if (IsContainFarsi(monoObject.text))
                        tempAssetsWithFarsiText.Add(monoObject);
                }
            }

            return tempAssetsWithFarsiText;
        }

        private bool IsAssetIntendedScript(string assetPath)
        {
            return assetPath.EndsWith(".cs") && !excludeScripts.Any(excludeScript => assetPath.EndsWith(excludeScript));
        }


        private List<Object> FindFarsiContainingScriptableObjects()
        {
            List<Object> tempAssetsWithFarsiText = new List<Object>();
            string[] assetPaths = AssetDatabase.GetAllAssetPaths();

            foreach (string assetPath in assetPaths)
            {
                if (!ShouldProcessAssetPath(assetPath) || !assetPath.EndsWith(".asset")) continue;

                var asset = AssetDatabase.LoadMainAssetAtPath(assetPath);
                bool isNonLocalizedWithFarsi = false;

                if (asset is ScenarioConfig scenarioConfig)
                {
                    foreach (var scenarioItem in scenarioConfig.scenarioItems)
                    {
                        if (scenarioItem.scenarioType == ScenarioType.BubbleDialogue ||
                            scenarioItem.scenarioType == ScenarioType.Screen)
                        {
                            isNonLocalizedWithFarsi = !string.IsNullOrEmpty(scenarioItem.string_0) && string.IsNullOrEmpty(scenarioItem.localizedString_0);
                        }
                        else if (scenarioItem.scenarioType == ScenarioType.Dialogue)
                            isNonLocalizedWithFarsi = scenarioItem.dialogues.Any(dialogue =>
                                                                                     !string.IsNullOrEmpty(dialogue.characterDialogue) && string.IsNullOrEmpty(dialogue.characterLocalizedString));
                    }
                }
                else if (asset is TaskConfig taskConfig)
                {
                    isNonLocalizedWithFarsi = !string.IsNullOrEmpty(taskConfig.taskString) && string.IsNullOrEmpty(taskConfig.taskLocalizedString);
                }
                else if (asset is TutorialConfig tutorialConfig)
                {
                    isNonLocalizedWithFarsi = !string.IsNullOrEmpty(tutorialConfig.dialogueString) && string.IsNullOrEmpty(tutorialConfig.dialogueLocalizedString);
                }
                else
                {
                    var scriptableObjectString = Encoding.ASCII.GetString(File.ReadAllBytes(assetPath));
                    scriptableObjectString = DecodeEncodedNonAsciiCharacters(scriptableObjectString);
                    isNonLocalizedWithFarsi = IsContainFarsi(scriptableObjectString);
                }

                if (isNonLocalizedWithFarsi)
                    tempAssetsWithFarsiText.Add(AssetDatabase.LoadMainAssetAtPath(assetPath));
            }

            return tempAssetsWithFarsiText;
        }

        private bool ShouldProcessAssetPath(string assetPath)
        {
            return IsIncluded(assetPath) && !IsExcluded(assetPath);
        }

        private bool IsIncluded(string assetPath)
        {
            foreach (var includedPath in includedPaths)
                if (assetPath.StartsWith(includedPath))
                    return true;
            return false;
        }

        private bool IsExcluded(string assetPath)
        {
            foreach (var excludedPath in excludedPaths)
                if (assetPath.StartsWith(excludedPath))
                    return true;
            return false;
        }

        private List<SearchResult> FindAllComponentsWithFarsiFieldsInPrefabs()
        {
            return FindAllComponentsWithFarsiFields(GetAllPrefabs().Cast<GameObject>().ToArray(), true);
        }

        private List<SearchResult> FindAllComponentsWithFarsiFieldsInScenes()
        {
            var startScene = SceneManager.GetActiveScene().path;
            List<SearchResult> tempSearchResults = new List<SearchResult>();
            var activeScenesPath = GetAllActiveScenesPath();
            foreach (var scenePath in activeScenesPath)
            {
                var rootGameObjects = EditorSceneManager.OpenScene(scenePath).GetRootGameObjects();
                tempSearchResults.AddRange(FindAllComponentsWithFarsiFields(rootGameObjects, false));
            }

            if (!string.IsNullOrEmpty(startScene))
                EditorSceneManager.OpenScene(startScene);

            return tempSearchResults;
        }

        private List<SearchResult> FindAllComponentsWithFarsiFields(GameObject[] gameObjects, bool includePrefabs)
        {
            List<SearchResult> tempSearchResults = new List<SearchResult>();

            StringBuilder objectHierarchy = new StringBuilder();

            foreach (var gameObject in gameObjects)
            {
                var componentsOnPrefab = gameObject.GetComponentsInChildren<MonoBehaviour>(true);
                foreach (var component in componentsOnPrefab)
                {
                    if (!includePrefabs && PrefabUtility.IsPartOfAnyPrefab(component.gameObject)) continue;
                    if (component is LocalText || component is RTLTextMeshPro || component is Text) continue;

                    if (IsContainFarsi(JsonUtility.ToJson(component)))
                    {
                        GetGameObjectHierarchy(component.gameObject, ref objectHierarchy);
                        tempSearchResults.Add(new SearchResult
                                              {
                                                  rootGameObject = gameObject,
                                                  targetHierarchy = objectHierarchy.ToString()
                                              });
                        objectHierarchy.Clear();
                    }
                }
            }

            return tempSearchResults;
        }


        private List<SearchResult> FindAllComponentUsageInPrefabs<T>() where T : Component
        {
            List<SearchResult> tempSearchResults = new List<SearchResult>();

            var allPrefabs = GetAllPrefabs();
            var prefabsWithTargetUsage = ExtractPrefabsWithComponent<T>(allPrefabs);

            StringBuilder stringBuilder = new StringBuilder();

            foreach (var prefab in prefabsWithTargetUsage)
            {
                GameObject rootGameObject = (GameObject) prefab;
                var allTargetComponents = rootGameObject.GetComponentsInChildren<T>(true);
                foreach (var targetComponent in allTargetComponents)
                {
                    if (IsLocalized(targetComponent))
                        continue;

                    var childGameObject = targetComponent.gameObject;
                    GetGameObjectHierarchy(childGameObject, ref stringBuilder);

                    tempSearchResults.Add(new SearchResult
                                          {rootGameObject = rootGameObject, targetHierarchy = stringBuilder.ToString()});

                    stringBuilder.Clear();
                }
            }

            return tempSearchResults;
        }

        private bool IsLocalized<T>(T targetComponent) where T : Component
        {
            return targetComponent.GetComponent<Localize>() != null;
        }

        private List<SearchResult> FindAllComponentUsagesInScenes<T>() where T : Component
        {
            List<SearchResult> tempSearchResults = new List<SearchResult>();

            var scenesPath = GetAllActiveScenesPath();
            var startingScenePath = SceneManager.GetActiveScene().path;

            foreach (string scenePath in scenesPath)
            {
                var scene = EditorSceneManager.OpenScene(scenePath);
                var rootGameObjects = scene.GetRootGameObjects();

                StringBuilder stringBuilder = new StringBuilder();

                foreach (var rootGameObject in rootGameObjects)
                {
                    if (PrefabUtility.IsPartOfAnyPrefab(rootGameObject)) continue;
                    var components = rootGameObject.GetComponentsInChildren<T>(true);

                    foreach (var component in components)
                    {
                        var userGameObject = component.gameObject;

                        if (PrefabUtility.IsPartOfAnyPrefab(userGameObject)) continue;

                        GetGameObjectHierarchy(userGameObject, ref stringBuilder);

                        stringBuilder.Insert(0, $"{scenePath.Remove(0, scenePath.LastIndexOf('/') + 1)} - ");

                        tempSearchResults.Add(new SearchResult
                                              {
                                                  rootGameObject = rootGameObject, targetHierarchy = stringBuilder.ToString()
                                              });

                        stringBuilder.Clear();
                    }
                }
            }

            if (!string.IsNullOrEmpty(startingScenePath))
                EditorSceneManager.OpenScene(startingScenePath);

            return tempSearchResults;
        }



        // -------------------------------------------------- Utilities -------------------------------------------------- \\


        private string DecodeEncodedNonAsciiCharacters(string value)
        {
            return Regex.Replace(
                value,
                @"\\u(?<Value>[a-zA-Z0-9]{4})",
                m => ((char) int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString());
        }




        private bool IsContainFarsi(string text)
        {
            foreach (var c in text)
                if (farsiChars.Contains(c))
                    return true;

            return false;
            //return regex.IsMatch(text);
        }

        private void GetGameObjectHierarchy(GameObject gameObject, ref StringBuilder stringBuilder)
        {
            stringBuilder.Append(gameObject.name);
            Transform parent = gameObject.transform.parent;

            while (parent != null)
            {
                stringBuilder.Insert(0, "/");
                stringBuilder.Insert(0, parent.name);

                parent = parent.parent;
            }
        }

        private Object[] GetAllPrefabs()
        {

            List<Object> assets = new List<Object>(1000);
            string[] guids = AssetDatabase.FindAssets("t:Prefab");
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                assets.Add(AssetDatabase.LoadAssetAtPath<Object>(assetPath));
            }
            return assets.ToArray();
        }

        private Object[] ExtractPrefabsWithComponent<T>(Object[] prefabs) where T : Component
        {
            List<Object> prefabsWithLocalText = new List<Object>();
            foreach (var prefab in prefabs)
            {
                GameObject go = (GameObject) prefab;
                var comp = go.GetComponentInChildren<T>(true);

                if (comp != null)
                {
                    prefabsWithLocalText.Add(prefab);
                }
            }

            return prefabsWithLocalText.ToArray();
        }

        private string[] GetAllActiveScenesPath()
        {
            return (from scene in EditorBuildSettings.scenes where scene.enabled select scene.path).ToArray();
        }

    }

}