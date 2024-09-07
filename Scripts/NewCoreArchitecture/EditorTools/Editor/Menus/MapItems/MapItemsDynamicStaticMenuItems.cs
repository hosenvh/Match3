using UnityEditor;

using Match3.EditorTools.Base.MapItems;

namespace Match3.EditorTools.Editor.Menus.MapItems
{
    public static class MapItemsDynamicStaticMenuItems
    {

        [MenuItem("Golmorad/Map Items/Make All Dynamic", isValidateFunction: false, priority: 1)]
        public static void MakeAllMapItemsDynamic()
        {
            if (!IsInMapStatePrefab()) return;

            MakeUserSelectsDynamic(GetAllSingleUserSelects());
            MakeStatesDynamic(GetALlSingleStates());
        }

        [MenuItem("Golmorad/Map Items/Make All Static", isValidateFunction: false, priority: 2)]
        public static void MakeAllMapItemsStatic()
        {
            if (!IsInMapStatePrefab()) return;

            MakeUserSelectsStatic(GetAllSingleUserSelects());
            MakeStatesStatic(GetALlSingleStates());
        }

        private static bool IsInMapStatePrefab()
        {
            return UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null &&
                   UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot.GetComponent<State_Map>() != null;
        }

        private static MapItem_UserSelect_Single[] GetAllSingleUserSelects()
        {
            return UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot
                                     .GetComponentsInChildren<MapItem_UserSelect_Single>(true);
        }

        private static MapItem_State_Single[] GetALlSingleStates()
        {
            return UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot
                                     .GetComponentsInChildren<MapItem_State_Single>(true);
        }

        private static void MakeUserSelectsDynamic(MapItem_UserSelect_Single[] mapUserSelects)
        {
            foreach (var userSelect in mapUserSelects)
                MapItemResourceDynamicMaker.MakeDynamic(userSelect);
        }

        private static void MakeStatesDynamic(MapItem_State_Single[] mapStates)
        {
            foreach (var state in mapStates)
                MapItemResourceDynamicMaker.MakeDynamic(state);
        }

        private static void MakeUserSelectsStatic(MapItem_UserSelect_Single[] mapUserSelects)
        {
            foreach (var userSelect in mapUserSelects)
                MapItemResourceDynamicMaker.MakeStatic(userSelect);
        }

        private static void MakeStatesStatic(MapItem_State_Single[] mapStates)
        {
            foreach (var state in mapStates)
                MapItemResourceDynamicMaker.MakeStatic(state);
        }
    }

}