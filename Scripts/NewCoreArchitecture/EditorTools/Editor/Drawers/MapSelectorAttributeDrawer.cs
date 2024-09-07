using System.Collections.Generic;
using Match3.Foundation.Unity;
using UnityEditor;
using UnityEngine;


namespace Match3.EditorTools.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(MapSelectorAttribute))]
    public class MapSelectorAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            MapSelectorAttribute mapSelectorAttribute = attribute as MapSelectorAttribute;

            var currentIndex = FindAndValidateSelectedValueIndex(mapSelectorAttribute.maps, property.stringValue);
            
            var chosen = EditorGUI.Popup(
                position,
                label.text,
                currentIndex, 
                mapSelectorAttribute.maps.ToArray());
            
            property.stringValue = mapSelectorAttribute.maps[chosen];
        }
        

        private int FindAndValidateSelectedValueIndex(List<string> selectionList, string selectedValue)
        {
            int currentIndex = 0;
            if(!selectedValue.IsNullOrEmpty())
            {
                currentIndex = FindValueIndexAtStringList(selectionList, selectedValue);
                if (currentIndex == -1)
                {
                    Debug.LogError($"Previous selected map id '{selectedValue}' is not valid anymore!");
                    currentIndex = 0;
                }
            }

            return currentIndex;
        }
        
        private int FindValueIndexAtStringList(List<string> list, string value)
        {
            return list.FindIndex((i) => i.Equals(value));
        }
    }
    
}