#if UNITY_EDITOR

using UnityEditor;
using System.Text.RegularExpressions;

namespace Match3.Utility.Editor
{
    public static class SerializedPropertyExtensions
    {
        public static SerializedProperty FindParent(this SerializedProperty serializedProperty)
        {
            var propertyPaths = serializedProperty.propertyPath.Split('.');
            if (propertyPaths.Length <= 1)
            {
                return default;
            }

            var parentSerializedProperty = serializedProperty.serializedObject.FindProperty(propertyPaths[0]);
            for (var index = 1; index < propertyPaths.Length - 1; index++)
            {
                if (propertyPaths[index] == "Array")
                {
                    if (index + 1 == propertyPaths.Length - 1)
                    {
                        // reached the end
                        break;
                    }
                    if (propertyPaths.Length > index + 1 && Regex.IsMatch(propertyPaths[index + 1], "^data\\[\\d+\\]$"))
                    {
                        var match = Regex.Match(propertyPaths[index + 1], "^data\\[(\\d+)\\]$");
                        var arrayIndex = int.Parse(match.Groups[1].Value);
                        parentSerializedProperty = parentSerializedProperty.GetArrayElementAtIndex(arrayIndex);
                        index++;
                    }
                }
                else
                {
                    parentSerializedProperty = parentSerializedProperty.FindPropertyRelative(propertyPaths[index]);
                }
            }

            return parentSerializedProperty;
        }
    }
}
#endif
