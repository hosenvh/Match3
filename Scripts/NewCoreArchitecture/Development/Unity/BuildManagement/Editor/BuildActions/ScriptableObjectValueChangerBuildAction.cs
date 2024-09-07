using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Medrick.Development.Unity.BuildManagement
{
    [CreateAssetMenu(menuName = "Medrick/Build Management/Build Actions/ScriptableObjectValueChangerBuildAction")]
    public class ScriptableObjectValueChangerBuildAction : ScriptableBuildAction
    {
        [System.Serializable]
        public struct BooleanEntry
        {
            public string field;
            public bool value;
        }

     

        public List<ScriptableObject> targets;

        public List<BooleanEntry> booleanEntries;

        public override void Execute()
        {
            foreach(var target in targets)
            {
                var serObj = new SerializedObject(target);
                serObj.Update();

                foreach (var entry in booleanEntries)
                    ApplyValue(entry, serObj);

                serObj.ApplyModifiedProperties();

            }


            foreach (var target in targets)
                UnityEditor.EditorUtility.SetDirty(target);
        }

        private void ApplyValue(BooleanEntry entry, SerializedObject serObj)
        {
            serObj.FindProperty(entry.field).boolValue = entry.value;
        }

        public override void Revert()
        {
            
        }


    }
}