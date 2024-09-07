using System.Collections;
using System.Collections.Generic;
using Match3.Data.Map;
using UnityEditor;
using UnityEngine;

namespace Medrick.Development.Unity.BuildManagement
{

    [CreateAssetMenu(menuName = "Medrick/Build Management/Build Actions/Maps IDs Database Filler")]
    public class MapsIDsDatabaseFillerBuildAction : ScriptableBuildAction
    {
        public MapsIDsDatabase mapsIDsDatabase;
        public string[] pathsOfMaps;
        
        public override void Execute()
        {
            var maps = AssetEditorUtilities.FindComponentsInPrefabsAtPaths<State_Map>(pathsOfMaps);
            foreach (var map in maps)
            {
                mapsIDsDatabase.InsertMapId(map.MapId);
            }
            EditorUtility.SetDirty(mapsIDsDatabase);
        }

        public override void Revert()
        {
            
        }
    }

}