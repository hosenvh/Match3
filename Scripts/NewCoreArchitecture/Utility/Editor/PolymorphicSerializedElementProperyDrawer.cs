using System;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using Match3.EditorTools.Editor.Drawers;
using Match3.EditorTools.Editor.Utility;


namespace Match3.Utility.Editor
{
    [UnityEditor.CustomPropertyDrawer(typeof(PolymorphicSerializedElement))]
    public class PolymorphicSerializedElementProperyDrawer : ListSupportedPropertyDrawer
    {
        const string SERIALIZATION_WRAPPER_CONTENT_FIELD = "content";

        Dictionary<Type, Type> serializableWrapperTypesMap = new Dictionary<Type, Type>();
        TypeDropdownDrawer typeDropdownDrawer;

        protected override void DrawCustomProperty(Rect position, SerializedProperty property, GUIContent label)
        {
            if (BaseTypeOf(property) == null)
                return;

            EditorGUI.indentLevel++;

            UpdatePositionForType(ref position);
            HandleTypeDropDown(position, property);
            HandleSerializationData(position, property);

            property.serializedObject.ApplyModifiedProperties();

            EditorGUI.indentLevel--;
        }

        private void HandleTypeDropDown(Rect position, SerializedProperty rootProperty)
        {
            // NOTE: Not sure this is safe if we have different PolymorphicSerializedElement with different base types.
            if (typeDropdownDrawer == null)
                typeDropdownDrawer = new TypeDropdownDrawer(
                    targetTypes: new Type[] { BaseTypeOf(rootProperty) },
                    excludingTypes: new Type[]{},
                    includeAbstracts: false,
                    showPartialNames: true);

            var selectedTypeProperty = SelectedTypeProperty(rootProperty);
            selectedTypeProperty.stringValue = typeDropdownDrawer.Draw(position, "Type", selectedTypeProperty.stringValue);
        }

        private void HandleSerializationData(Rect position, SerializedProperty rootProperty)
        {
            if (SelectedType(rootProperty) == null)
                return;

            (var serializationSerializedObject, var serializationContentObject) = CreateSerializationWrapper(rootProperty);

            if (serializationSerializedObject == null)
                return;

            DrawSerializedObject(position, serializationSerializedObject);

            SerializationProperty(rootProperty).stringValue = JsonUtility.ToJson(serializationContentObject);
        }

        private void DrawSerializedObject(Rect position, SerializedObject serializedObject)
        {
            var field = serializedObject.GetIterator();
            field.NextVisible(true);

            Rect marchingRect = new Rect(position);

            while (field.NextVisible(false))
            {
                try
                {
                    marchingRect.y += marchingRect.height + EditorGUIUtility.standardVerticalSpacing;
                    marchingRect.height = EditorGUI.GetPropertyHeight(field, true);
                    EditorGUI.PropertyField(marchingRect, field, true);
                }
                catch (StackOverflowException)
                {
                    field.objectReferenceValue = null;
                    Debug.LogError("Detected self-nesting cauisng a StackOverflowException, avoid using the same " +
                        "object iside a nested structure.");
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        protected override float GetExpandedPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float totalHeight = 0.0f;

            totalHeight += EditorGUIUtility.singleLineHeight;

            var targetType = SelectedType(property);
            if (targetType == null)
                return totalHeight;


            (var serializationSerializedObject, var _) = CreateSerializationWrapper(property);
            if (serializationSerializedObject == null)
                return totalHeight;

            SerializedProperty field = serializationSerializedObject.GetIterator();
            field.NextVisible(true);
            while (field.NextVisible(false))
                totalHeight += EditorGUI.GetPropertyHeight(field, true) + EditorGUIUtility.standardVerticalSpacing;


            return totalHeight;
        }

        private (SerializedObject serializedObject, object content) CreateSerializationWrapper(SerializedProperty property)
        {

            var targetType = SelectedType(property);

            if(serializableWrapperTypesMap.TryGetValue(targetType, out var serializableWrapperType) == false)
            {
                serializableWrapperType = CreateSerialiableWrapperType(targetType);
                serializableWrapperTypesMap[targetType] = serializableWrapperType;
            }

            var scriptableObject = ScriptableObject.CreateInstance(serializableWrapperType);

            var serialziationProperty = SerializationProperty(property);
            var deserializedValue = JsonUtility.FromJson(serialziationProperty.stringValue, targetType);

            var contentField = serializableWrapperType.GetField(SERIALIZATION_WRAPPER_CONTENT_FIELD);
            contentField.SetValue(scriptableObject, deserializedValue);
            
            return (new SerializedObject(scriptableObject), contentField.GetValue(scriptableObject));
        }

        private Type SelectedType(SerializedProperty rootProperty)
        {
            return ReflectionEditorUtilities.GetType(SelectedTypeProperty(rootProperty).stringValue);
        }

        private Type BaseTypeOf(SerializedProperty rootProperty)
        {
            try
            {
                return ReflectionEditorUtilities.GetType(rootProperty.FindPropertyRelative(nameof(PolymorphicSerializedElement.baseType)).stringValue);
            }
            catch(Exception)
            {
                return null;
            }
        }

        private SerializedProperty SerializationProperty(SerializedProperty rootProperty)
        {
            return rootProperty.FindPropertyRelative(nameof(PolymorphicSerializedElement.serialization));
        }

        private SerializedProperty SelectedTypeProperty(SerializedProperty rootProperty)
        {
            return rootProperty.FindPropertyRelative(nameof(PolymorphicSerializedElement.type));
        }

        private Type CreateSerialiableWrapperType(Type contentType)
        {
            var builder = RuntimeTypeBuilder.GetTypeBuilder("CustomSerializableType_"+ contentType.Name, addRandomTokenToName: true);
            builder.SetParent(typeof(ScriptableObject));
            RuntimeTypeBuilder.CreateField(builder, SERIALIZATION_WRAPPER_CONTENT_FIELD, contentType, FieldAttributes.Public);

            return RuntimeTypeBuilder.CompileResultType(builder);
        }

        private Rect UpdatePositionForType(ref Rect position)
        {
            position.y += EditorGUIUtility.singleLineHeight;
            position.height = EditorGUIUtility.singleLineHeight;
            return position;
        }

    }

}
