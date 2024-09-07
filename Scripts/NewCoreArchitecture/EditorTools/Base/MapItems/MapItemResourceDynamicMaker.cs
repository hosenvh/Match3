
#if UNITY_EDITOR

using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


namespace Match3.EditorTools.Base.MapItems
{
    // NOTE: This whole pipeline must be redesigned.
    public static class MapItemResourceDynamicMaker
    {
        static string RootToResourcesPath = "Resources";

        private static string GetUserSelectsResourceFolderPath(string mapId) => $"MapItems/{mapId}/UserSelects";
        private static string GetStatesResourceFolderPath(string mapId) => $"MapItems/{mapId}/States";
        
        
        public static void MakeStatic(MapItem_State_Single state)
        {

            if (state.EDITORIgnoreDynamicResourceLoading)
            {
                Debug.LogWarningFormat(state.gameObject, "Dynamicifaction is disabled for {0}", state.gameObject.name);
                return;
            }

            if (state.mapItem_ElementsPaths.Count == 0)
                return;

            state.mapItem_Elements = new MapItem_Element[state.mapItem_ElementsPaths.Count];

            Undo.RecordObject(state, "Make Static");

            for(int i = 0; i< state.mapItem_ElementsPaths.Count; ++i)
            {
                var elementPath = state.mapItem_ElementsPaths[i];

                var resource = Resources.Load<MapItem_Element>(elementPath);
                var currentElement = GameObject.Instantiate(resource, state.transform, false);
                currentElement.gameObject.name = resource.gameObject.name;
                state.mapItem_Elements[i] = currentElement;
            }

            state.mapItem_ElementsPaths.Clear();

        }

        public static void MakeDynamic(MapItem_State_Single state)
        {
            TrimEndingSpace(UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot
                .GetComponentsInChildren<MapItem_State_Single>(true));
            
            if (state.EDITORIgnoreDynamicResourceLoading)
            {
                Debug.LogWarningFormat(state.gameObject, "Dynamicifaction is disabled for {0}", state.gameObject.name);
                return;
            }

            if (state.mapItem_Elements.Length == 0)
                return;
            
            if (CheckForValidity(state) == false)
                return;
            
            Undo.RecordObject(state, "Make Dynamic");

            FixNames(state.mapItem_Elements);

            var mapId = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot.GetComponent<State_Map>().MapId;
            var nameWithId = state.name + $"_{state.GetItemId().ToString()}";
            var resourceFolderPath = Path.Combine(GetStatesResourceFolderPath(mapId), nameWithId);
            var assetFolderPath = Path.Combine(RootToResourcesPath, GetStatesResourceFolderPath(mapId), nameWithId);
            CreateDirectoryFor(assetFolderPath);

            foreach (var element in state.mapItem_Elements)
            {
                if (element == null)
                    continue;

                var fullAssetPath = Path.Combine("Assets",assetFolderPath, element.gameObject.name);
                var resourcesAssetPath = Path.Combine(resourceFolderPath, element.gameObject.name);

                UnityEditor.PrefabUtility.SaveAsPrefabAssetAndConnect(
                    element.gameObject, 
                    string.Format("{0}.prefab", fullAssetPath), 
                    UnityEditor.InteractionMode.AutomatedAction, 
                    out bool isSucceded);
                if (isSucceded == false)
                {
                    Debug.LogErrorFormat("Coudn't create prefab for {0} in {1}", element.gameObject.name, state.gameObject.name);
                    return;
                }

                state.mapItem_ElementsPaths.Add(resourcesAssetPath);

                UnityEngine.Object.DestroyImmediate(element.gameObject);
            }

            state.mapItem_Elements = new MapItem_Element[0];
        }



        public static void MakeDynamic(MapItem_UserSelect_Single userSelect)
        {
            TrimEndingSpace(UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot
                .GetComponentsInChildren<MapItem_UserSelect_Single>(true));

            if (userSelect.EDITORIgnoreDynamicResourceLoading)
            {
                Debug.LogWarningFormat(userSelect.gameObject, "Dynamification is disabled for {0}", userSelect.gameObject.name);
                return;
            }
            if (userSelect.mapItem_Elements.Count == 0)
                return;

            
            if (!CheckForValidity(userSelect))
                return;

            Undo.RecordObject(userSelect, "Make Dynamic");

            FixNames(userSelect.mapItem_Elements);
            
            var mapId = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot.GetComponent<State_Map>().MapId;
            var nameWithId = userSelect.name + $"_{userSelect.GetItemId().ToString()}";
            var resourceFolderPath = Path.Combine(GetUserSelectsResourceFolderPath(mapId), nameWithId);
            var assetFolderPath = Path.Combine(RootToResourcesPath, GetUserSelectsResourceFolderPath(mapId), nameWithId);

            CreateDirectoryFor(assetFolderPath);

            foreach (var element in userSelect.mapItem_Elements)
            {
                if (element == null)
                    continue;

                var fullAssetPath = Path.Combine("Assets", assetFolderPath, element.gameObject.name);
                var resourcesAssetPath = Path.Combine(resourceFolderPath, element.gameObject.name);

                UnityEditor.PrefabUtility.SaveAsPrefabAssetAndConnect(
                    element.gameObject,
                    string.Format("{0}.prefab", fullAssetPath),
                    UnityEditor.InteractionMode.AutomatedAction,
                    out bool isSucceded);
                if (isSucceded == false)
                {
                    Debug.LogErrorFormat("Couldn't create prefab for {0} in {1}", element.gameObject.name, userSelect.gameObject.name);
                    return;
                }

                userSelect.mapItem_ElementsPaths.Add(resourcesAssetPath);

                Object.DestroyImmediate(element.gameObject);
            }

            userSelect.mapItem_Elements.Clear();
        }

        public static void MakeStatic(MapItem_UserSelect_Single userState)
        {

            if (userState.EDITORIgnoreDynamicResourceLoading)
            {
                Debug.LogWarningFormat(userState.gameObject, "Dynamization is disabled for {0}", userState.gameObject.name);
                return;
            }

            if (userState.mapItem_ElementsPaths.Count == 0)
                return;

            userState.mapItem_Elements = new List<MapItem_Element>();
  

            Undo.RecordObject(userState, "Make Static");

            foreach(var elementPath in userState.mapItem_ElementsPaths)
            {
                var resource = Resources.Load<MapItem_Element>(elementPath);
                if(resource == null)
                {
                    Debug.LogError($"Couldn't load/instantiate Resource at path: {elementPath}. The Map Item Is: {userState.gameObject.name}. Single Click on this error to select it.", userState.gameObject);
                    continue;
                }
                var currentElement = GameObject.Instantiate(resource, userState.transform, false);
                currentElement.gameObject.name = resource.name;
                userState.mapItem_Elements.Add(currentElement);
            }

            userState.mapItem_ElementsPaths.Clear();

        }

        private static bool CheckForValidity(MapItem_UserSelect_Single userSelect)
        {
            foreach (var element in userSelect.mapItem_Elements)
            {
                if (element.TouchGraphicGameObject() != null && IsNotIn(element.TouchGraphicGameObject(), element.gameObject))
                {
                    Debug.LogErrorFormat(element,"Touch Object is not inside {0} of {1}", element.name, userSelect.name);
                    return false;
                }

                var userSelectItems = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot
                    .GetComponentsInChildren<MapItem_UserSelect_Single>(true);
                var itemId = userSelect.GetItemId();
                foreach (var userSelectItem in userSelectItems)
                {
                    if (userSelectItem.GetItemId() == itemId && userSelectItem != userSelect)
                    {
                        Debug.LogError(
                            $"UserSelect {userSelect.gameObject.name} and {userSelectItem.gameObject.name} has a same {itemId} Id.");
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool CheckForValidity(MapItem_State_Single state)
        {
            foreach (var element in state.mapItem_Elements)
            {
                if(element.TouchGraphicGameObject() != null && IsNotIn(element.TouchGraphicGameObject(), element.gameObject))
                {
                    Debug.LogErrorFormat("Touch Object is not inside {0} of {1}", element.name, state.name);
                    return false;
                }
            }

            var userStates = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot
                .GetComponentsInChildren<MapItem_State_Single>(true);
            var itemId = state.GetItemId();
            foreach (var userSelectItem in userStates)
            {
                if (userSelectItem.GetItemId() == itemId && userSelectItem != state)
                {
                    Debug.LogError(
                        $"State {state.gameObject.name} and {userSelectItem.gameObject.name} has a same {itemId} Id.");
                    return false;
                }
            }
            
            return true;
        }

        private static bool IsNotIn(GameObject target, GameObject owner)
        {
            var current = target.transform;
            while(current.transform.parent != null)
            {
                if (current.transform.parent == owner.transform)
                    return false;
                current = current.transform.parent;
            }

            return true;
        }

        private static void CreateDirectoryFor(string assetFolderpath)
        {
            var fullPath = Path.Combine(Application.dataPath, assetFolderpath);

            Debug.Log(fullPath);
            if (Directory.Exists(fullPath) == false)
                Directory.CreateDirectory(fullPath);
        }

        public static void TrimEndingSpace<T>(IEnumerable<T> elements) where T : MonoBehaviour
        {
            foreach (var elem in elements)
            {
                elem.gameObject.name = elem.gameObject.name.TrimEnd(' ');
            }
        }

        public static void FixNames<T>(IEnumerable<T> elements) where T : MonoBehaviour
        {
            Dictionary<string, List<T>> namesMap = new Dictionary<string, List<T>>();
            foreach (var elem in elements)
            {
                elem.gameObject.name = elem.gameObject.name.TrimEnd(' ');
                
                var name = elem.gameObject.name.ToLower();
                if (namesMap.ContainsKey(name) == false)
                    namesMap.Add(name, new List<T>());
                namesMap[name].Add(elem);
            }

            bool neededFixing = false;
            foreach(var entry in namesMap)
            {
                for (int i = 1; i < entry.Value.Count; ++i)
                {
                    entry.Value[i].gameObject.name += i;
                    
                    // NOTE: Since the renaming may affect the dynamic path, the map item (State or UserSelect) is forced to be static.
                    MakeStaticDueToRenaming(entry.Value[i]);

                    neededFixing = true;
                }
            }

            if (neededFixing)
                FixNames(elements);
        }

        private static void MakeStaticDueToRenaming(MonoBehaviour monoBehaviour)
        {
            switch(monoBehaviour)
            {
                case MapItem_State_Single state:
                    Debug.LogError($"Making {state} static due to renaming. Please make it dynamic later", state);
                    MakeStatic(state);
                    break;
                case MapItem_UserSelect_Single userSelect:
                    Debug.LogError($"Making {userSelect} static due to renaming. Please make it dynamic later", userSelect);
                    MakeStatic(userSelect);
                    break;
            }
        }
    }
}
#endif