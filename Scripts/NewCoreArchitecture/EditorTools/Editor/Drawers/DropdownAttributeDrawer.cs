using System.Linq;
using PandasCanPlay.HexaWord.Utility;
using UnityEditor;
using UnityEngine;


namespace Match3.EditorTools.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(DropdownAttribute))]
    public class DropdownAttributeDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DropdownAttribute dropdownAttribute = attribute as DropdownAttribute;

            var currentIndex = dropdownAttribute.options.FindIndex((i) => i.Equals(property.stringValue));

            var chosen = EditorGUI.Popup(
                position,
                label.text,
                currentIndex, 
                dropdownAttribute.options.Select(s => ConvertSlashToUnicodeSlash(s)).ToArray());
            if(chosen >= 0)
                property.stringValue = dropdownAttribute.options[chosen];
        }

        public static string ConvertSlashToUnicodeSlash(string text_)
        {
            return text_.Replace('/', '\u2215');
        }

        public static string ConvertUnicodeSlashToSlash(string text_)
        {
            return text_.Replace('\u2215', '/');
        }


    }
}
