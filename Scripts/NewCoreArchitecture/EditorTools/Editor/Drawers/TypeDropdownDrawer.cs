using System;
using System.Collections.Generic;
using System.Linq;
using Match3.EditorTools.Editor.Utility;
using UnityEditor;
using UnityEngine;


namespace Match3.EditorTools.Editor.Drawers
{
    public class TypeDropdownDrawer
    {
        List<Type> foundTypes = null;

        ICollection<Type> targetTypes;
        ICollection<Type> excludingTypes;
        bool includeAbstracts;
        bool showPartialNames;

        public TypeDropdownDrawer(ICollection<Type> targetTypes, ICollection<Type> excludingTypes, bool includeAbstracts, bool showPartialNames)
        {
            this.targetTypes = targetTypes;
            this.excludingTypes = excludingTypes;
            this.includeAbstracts = includeAbstracts;
            this.showPartialNames = showPartialNames;
        }

        public string Draw(Rect position, string label, string initialTypeString)
        {
            var initialType = ReflectionEditorUtilities.GetType(initialTypeString);

            if(initialType == null && initialTypeString.IsNullOrEmpty() == false)
                Debug.LogError($"[Type Dropdown] previous type {initialTypeString} is invalid.");

            return Draw(position, label, initialType).AssemblyQualifiedName;

        }

        public Type Draw(Rect position, string label, Type initialType)
        {
            if (foundTypes == null)
            {
                foundTypes = new List<Type>();

                foreach (var type in targetTypes)
                {
                    List<Type> allFoundTypes = ReflectionEditorUtilities.FindTypesOf(type, includeAbstracts);
                    foreach (Type excludingType in excludingTypes)
                        allFoundTypes.Remove(excludingType);
                    foundTypes.AddRange(allFoundTypes);
                }

                if (foundTypes.Count == 0)
                    return null;
            }

            var index = foundTypes.FindIndex(t => t.Equals(initialType));
            if (index < 0)
                index = 0;


            var chosen = EditorGUI.Popup(
                position,
                label,
                index,
                foundTypes.Select(t => TypeName(t, showPartialNames)).ToArray());

            return foundTypes[chosen];
        }

        private string TypeName(Type type, bool partial)
        {
            if (partial)
                return type.Name;
            else
                return type.FullName;
        }

    }
}
