using Match3.Data.Map;
using UnityEditor;
using UnityEngine;


namespace Medrick.Development.Unity.BuildManagement
{

    [CreateAssetMenu(menuName = "Medrick/Build Management/Build Actions/Maps Items Database Filler")]
    public class MapItemsDatabaseFillerBuildAction : ScriptableBuildAction
    {
        public MapsItemsDatabase mapsItemsDatabase;
        public string[] pathsOfMaps;
        
        public override void Execute()
        {
            var maps = AssetEditorUtilities.FindComponentsInPrefabsAtPaths<State_Map>(pathsOfMaps);
            foreach (var map in maps)
            {
                var allMapUserSelects = map.GetComponentsInChildren<MapItem_UserSelect>();
                var allMapStates = map.GetComponentsInChildren<MapItem_State>();
            
                foreach (var userSelect in allMapUserSelects)
                {
                    mapsItemsDatabase.InsertOrUpdateUserSelect(map.MapId, userSelect);
                }
            
                foreach (var mapState in allMapStates)
                {
                    mapsItemsDatabase.InsertOrUpdateState(map.MapId, mapState);
                }
            }

            EditorUtility.SetDirty(mapsItemsDatabase);
        }

        public override void Revert()
        {
            
        }
    }

}