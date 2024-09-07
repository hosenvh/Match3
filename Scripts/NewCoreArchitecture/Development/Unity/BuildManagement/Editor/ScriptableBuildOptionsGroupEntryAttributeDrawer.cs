using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Medrick.Development.Unity.BuildManagement
{

    [CustomPropertyDrawer(typeof(ScriptableBuildOptionsGroupEntryAttribute))]
    public class ScriptableBuildOptionsGroupEntryAttributeDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var obj = GetActualObjectForSerializedProperty<ScriptableBuildOptionsPreset.GroupOptionEntry>(fieldInfo, property);

            var optionNames = new string[obj.group.BuildOptions().Count() +1];

            optionNames[0] = "Ignore";

            for (int i = 1; i < optionNames.Length; ++i)
                optionNames[i] = obj.group.buildOptions[i-1].Name();

            EditorGUI.LabelField(position, $"OptionGroup : { obj.group.Name()}");

            obj.optionIndex =  EditorGUI.Popup(new Rect(position.x, position.y + 20, position.width, 20), obj.optionIndex + 1, optionNames) -1;

            var optionIndexProperty = property.FindPropertyRelative(nameof(ScriptableBuildOptionsPreset.GroupOptionEntry.optionIndex));

            optionIndexProperty.intValue = obj.optionIndex;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 100;
        }
        public static T GetActualObjectForSerializedProperty<T>(FieldInfo fieldInfo, SerializedProperty property) where T : class
        {
            var obj = fieldInfo.GetValue(property.serializedObject.targetObject);
            if (obj == null) { return null; }

            T actualObject = null;
            if (obj.GetType().IsArray)
            {
                var index = Convert.ToInt32(new string(property.propertyPath.Where(c => char.IsDigit(c)).ToArray()));
                actualObject = ((T[])obj)[index];
            }
            else if (obj is IList<T>)
            {
                var index = Convert.ToInt32(new string(property.propertyPath.Where(c => char.IsDigit(c)).ToArray()));
                actualObject = ((IList<T>)obj)[index];
            }
            else
            {
                actualObject = obj as T;
            }
            return actualObject;
        }

    }
}