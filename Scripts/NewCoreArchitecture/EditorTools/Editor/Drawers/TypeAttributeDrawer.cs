using PandasCanPlay.HexaWord.Utility;
using UnityEditor;
using UnityEngine;


namespace Match3.EditorTools.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(TypeAttribute))]
    public class TypeAttributeDrawer : PropertyDrawer
    {
        TypeDropdownDrawer typeDropdownDrawer;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            TypeAttribute typeAttribute = attribute as TypeAttribute;

            if (typeDropdownDrawer == null)
                typeDropdownDrawer = new TypeDropdownDrawer(typeAttribute.types, typeAttribute.typesToExclude, typeAttribute.includeAbstracts, typeAttribute.showPartialName);
            
            property.stringValue = typeDropdownDrawer.Draw(position, label.text, property.stringValue);
        }

    }
}
