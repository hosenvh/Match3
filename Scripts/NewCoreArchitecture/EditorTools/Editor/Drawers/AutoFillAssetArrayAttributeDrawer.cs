using UnityEditor;
using UnityEngine;


namespace Match3.EditorTools.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(AutoFillAssetArrayAttribute))]
    public class AutoFillAssetArrayAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            EditorGUI.BeginProperty(position, label, property);

            //if (GUILayout.Button("All", GUILayout.MaxWidth(40))) TransformCopyAll();

            EditorGUI.EndProperty();


        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 30;
        }
    }
}