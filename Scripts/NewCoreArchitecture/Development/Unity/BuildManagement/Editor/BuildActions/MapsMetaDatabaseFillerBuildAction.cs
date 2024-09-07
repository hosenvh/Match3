using Match3.Data.Map;
using UnityEditor;
using UnityEngine;



namespace Medrick.Development.Unity.BuildManagement
{

    [CreateAssetMenu(menuName = "Medrick/Build Management/Build Actions/Maps Meta Database Filler")]
    public class MapsMetaDatabaseFillerBuildAction : ScriptableBuildAction
    {

        public MapsMetaDatabase mapsMetaDatabase;
        public string[] pathsOfMaps;
        
        public override void Execute()
        {
            var maps = AssetEditorUtilities.FindComponentsInPrefabsAtPaths<State_Map>(pathsOfMaps);
            foreach (var map in maps)
            {
                var mapPath = AssetDatabase.GetAssetPath(map);
                mapPath = MakePathFromResources(mapPath);
                mapsMetaDatabase.InsertOrUpdateMapMetadata(new MapMetaData(map.MapId, mapPath));
            }
            EditorUtility.SetDirty(mapsMetaDatabase);
        }

        private string MakePathFromResources(string path)
        {
            path = path.Replace(".prefab", "");
            return path.Replace("Assets/Resources/", "");
        }
        
        public override void Revert()
        {
            
        }
    }
    
}


