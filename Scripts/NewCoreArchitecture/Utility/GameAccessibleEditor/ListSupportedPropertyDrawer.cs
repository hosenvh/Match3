
#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Event = UnityEngine.Event;

namespace Match3.Utility.Editor
{
    // Note: This is mostly inspired/duplicated from method 'DefaultPropertyField' 
    // in https://github.com/Unity-Technologies/UnityCsReference/blob/2018.3/Editor/Mono/EditorGUI.cs
    public abstract class ListSupportedPropertyDrawer : PropertyDrawer
    {
        public override sealed void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            HandlePropertyField(position, property, label);
            //EditorGUI.LabelField(position, label);
        }

        public override sealed float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = base.GetPropertyHeight(property, label);
            if (property.isExpanded)
                height += GetExpandedPropertyHeight(property, label);

            return height;

        }

        protected abstract float GetExpandedPropertyHeight(SerializedProperty property, GUIContent label);

        protected abstract void DrawCustomProperty(Rect position, SerializedProperty property, GUIContent label);

        protected virtual Object IsValidToPerformDrag(Object targetObject, SerializedProperty property) 
        { 
            return null; 
        }

        protected virtual void PerformDrag(Object targetObject, SerializedProperty property) { }

        protected virtual bool IsAlwaysExpanded() 
        { 
            return false; 
        }



        static readonly string s_ArrayMultiInfoFormatString = EditorGUIUtility.TrTextContent("This field cannot display arrays with more than {0} elements when multiple objects are selected.").text;
        static readonly GUIContent s_ArrayMultiInfoContent = new GUIContent();
        private const float kIndentPerLevel = 15;
        internal static float indent => EditorGUI.indentLevel * kIndentPerLevel;

        private bool HandlePropertyField(Rect position, SerializedProperty property, GUIContent label)
        {
            //label = BeginPropertyInternal(position, label, property);

            EditorGUI.BeginProperty(position, label, property);

            SerializedPropertyType type = property.propertyType;

            bool childrenAreExpanded = false;
            if (!HasVisibleChildFields(property))
            {
                switch (type)
                {
                    case SerializedPropertyType.ArraySize:
                        {
                            EditorGUI.BeginChangeCheck();
                            int newValue = EditorGUI.IntField(position, label, property.intValue, EditorStyles.numberField);
                            if (EditorGUI.EndChangeCheck())
                            {
                                property.intValue = newValue;
                            }
                            break;
                        }
                    default:
                        {
                            DrawCustomProperty(position, property, label);
                            //int genericID = GUIUtility.GetControlID(s_GenericField, FocusType.Keyboard, position);
                            //PrefixLabel(position, genericID, label);
                            break;
                        }
                }
            }
            // Handle Foldout
            else
            {
                Event tempEvent = new Event(Event.current);

                // Handle the actual foldout first, since that's the one that supports keyboard control.
                // This makes it work more consistent with PrefixLabel.
                childrenAreExpanded = property.isExpanded;

                bool newChildrenAreExpanded = childrenAreExpanded;
                if (IsAlwaysExpanded())
                    newChildrenAreExpanded = true;
                else
                {
                    using (new EditorGUI.DisabledScope(!property.editable))
                    {
                        GUIStyle foldoutStyle = (DragAndDrop.activeControlID == -10) ? EditorStyles.foldoutPreDrop : EditorStyles.foldout;
                        Rect boxRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                        newChildrenAreExpanded = EditorGUI.Foldout(boxRect, childrenAreExpanded, label, true, foldoutStyle);
                    }
                }


                if (childrenAreExpanded && property.isArray && property.arraySize > property.serializedObject.maxArraySizeForMultiEditing && property.serializedObject.isEditingMultipleObjects)
                {
                    Rect boxRect = position;
                    boxRect.xMin += EditorGUIUtility.labelWidth - indent;

                    s_ArrayMultiInfoContent.text = s_ArrayMultiInfoContent.tooltip = string.Format(s_ArrayMultiInfoFormatString, property.serializedObject.maxArraySizeForMultiEditing);
                    EditorGUI.LabelField(boxRect, GUIContent.none, s_ArrayMultiInfoContent, EditorStyles.helpBox);
                }

                if (newChildrenAreExpanded != childrenAreExpanded)
                {
                    // Recursive set expanded
                    if (Event.current.alt)
                    {
                        SetExpandedRecurse(property, newChildrenAreExpanded);
                    }
                    // Expand one element only
                    else
                    {
                        property.isExpanded = newChildrenAreExpanded;
                    }
                }
                childrenAreExpanded = newChildrenAreExpanded;


                // Check for drag & drop events here, to add objects to an array by dragging to the foldout.
                // The event may have already been used by the Foldout control above, but we want to also use it here,
                // so we use the event copy we made prior to calling the Foldout method.

                // We need to use last s_LastControlID here to ensure we do not break duplicate functionality (fix for case 598389)
                // If we called GetControlID here s_LastControlID would be incremented and would not longer be in sync with GUIUtililty.keyboardFocus that
                // is used for duplicating (See DoPropertyFieldKeyboardHandling)
                int id = EditorGUIUtility.GetObjectPickerControlID();
                switch (tempEvent.type)
                {
                    case EventType.DragExited:
                        if (GUI.enabled)
                        {
                            HandleUtility.Repaint();
                        }

                        break;
                    case EventType.DragUpdated:
                    case EventType.DragPerform:

                        if (position.Contains(tempEvent.mousePosition) && GUI.enabled)
                        {
                            Object[] references = DragAndDrop.objectReferences;

                            // Check each single object, so we can add multiple objects in a single drag.
                            Object[] oArray = new Object[1];
                            bool didAcceptDrag = false;
                            foreach (Object o in references)
                            {
                                oArray[0] = o;
                                Object validatedObject = IsValidToPerformDrag(o, property);
                                if (validatedObject != null)
                                {
                                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                                    if (tempEvent.type == EventType.DragPerform)
                                    {
                                        PerformDrag(validatedObject, property);
                                        //property.AppendFoldoutPPtrValue(validatedObject);
                                        didAcceptDrag = true;
                                        DragAndDrop.activeControlID = 0;
                                    }
                                    else
                                    {
                                        DragAndDrop.activeControlID = id;
                                    }
                                }
                            }
                            if (didAcceptDrag)
                            {
                                GUI.changed = true;
                                DragAndDrop.AcceptDrag();
                            }
                        }
                        break;
                }

                if(property.isExpanded)
                   DrawElement(position, property, label);
                //DrawChilds(position, property, label);
            }

            EditorGUI.EndProperty();

            return childrenAreExpanded;

        }

        private void DrawElement(Rect rect, SerializedProperty property, GUIContent label)
        {
            rect = new Rect(rect.x, rect.y, rect.width, rect.height + GetExpandedPropertyHeight(property, label));
            DrawCustomProperty(rect, property, label);

        }

        private static void SetExpandedRecurse(SerializedProperty property, bool expanded)
        {
            SerializedProperty search = property.Copy();
            search.isExpanded = expanded;

            int depth = search.depth;
            while (search.NextVisible(true) && search.depth > depth)
            {
                if (search.hasVisibleChildren)
                {
                    search.isExpanded = expanded;
                }
            }
        }

        internal static bool HasVisibleChildFields(SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Vector3:
                case SerializedPropertyType.Vector2:
                case SerializedPropertyType.Vector3Int:
                case SerializedPropertyType.Vector2Int:
                case SerializedPropertyType.Rect:
                case SerializedPropertyType.RectInt:
                case SerializedPropertyType.Bounds:
                case SerializedPropertyType.BoundsInt:
                    return false;
            }
            return property.hasVisibleChildren;
        }

    }

}


#endif