#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using Match3.Utility.Editor;

namespace Match3.Foundation.Unity
{
    public class ResourceAssetDrawer<T> : ListSupportedPropertyDrawer where T : Object
    {
        SerializedProperty resourcePath;

        protected override void DrawCustomProperty(Rect position, SerializedProperty property, GUIContent label)
        {
            resourcePath = property.FindPropertyRelative(nameof(ResourceAsset<T>.resourcePath));

            var targetObject = LoadTargetObject(property);

            var displayName = property.displayName;

            targetObject = UnityEditor.EditorGUI.ObjectField(position, displayName, targetObject, typeof(T), false);

            if (targetObject != null)
            {
                string outGUID;
                long outLocalID;

                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(targetObject, out outGUID, out outLocalID);
                var objPath = AssetDatabase.GUIDToAssetPath(outGUID);

                if (AssetEditorUtilities.IsInResourcesFolder(objPath))
                {
                    resourcePath.stringValue = AssetEditorUtilities.RelativeAssetPathInResources(objPath);
                }
                else
                {
                    Debug.LogError($"Object {targetObject.name} is not in Resources folder", targetObject);
                    ClearData();
                }
            }
            else
                ClearData();
        }

        private void TryInsert(SerializedProperty collection, T resource)
        {
            int index = collection.arraySize;
            string path = AssetDatabase.GetAssetPath(resource);

            if (AssetEditorUtilities.IsInResourcesFolder(path))
            {
                string guid = AssetDatabase.AssetPathToGUID(path);

                collection.InsertArrayElementAtIndex(index);

                collection.GetArrayElementAtIndex(index).FindPropertyRelative(nameof(ResourceAsset<T>.resourcePath)).stringValue = AssetEditorUtilities.RelativeAssetPathInResources(path);

                EditorUtility.SetDirty(collection.serializedObject.targetObject);
            }
        }

        protected override Object IsValidToPerformDrag(Object targetObject, SerializedProperty property)
        {
            var parent = property.FindParent();

            if (parent != null && parent.isArray)
                return targetObject as T;
            else
                return null;
        }

        protected override void PerformDrag(Object targetObject, SerializedProperty property)
        {
            var parent = property.FindParent();
            if (parent != null && parent.isArray)
                TryInsert(parent, targetObject as T);
        }

        protected override bool IsAlwaysExpanded()
        {
            return true;
        }

        protected override float GetExpandedPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0;
        }

        // NOTE: It is possible to cache the target object.
        private Object LoadTargetObject(SerializedProperty property)
        {
            return Resources.Load<T>(resourcePath.stringValue);
        }

        void ClearData()
        {
            resourcePath.stringValue = string.Empty;
        }
    }
}
#endif
