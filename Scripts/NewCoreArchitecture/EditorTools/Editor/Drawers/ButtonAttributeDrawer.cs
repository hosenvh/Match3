using ArmanCo.ShapeRunner.Utility;
using UnityEditor;
using UnityEngine;


namespace Match3.EditorTools.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(ButtonAttribute))]
    public class ButtonAttributeDrawer : PropertyDrawer
    {
        static object[] emptyParamters = new object[0];

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ButtonAttribute buttonAtr = attribute as ButtonAttribute;
            var callBack = property.serializedObject.targetObject.GetType().GetMethod(buttonAtr.callBackName);
            var buttonAction = fieldInfo.GetValue(property.serializedObject.targetObject) as ButtonAction;

            if (GUI.Button(position, buttonAtr.name))
                if (callBack != null)
                    callBack.Invoke(property.serializedObject.targetObject, emptyParamters);
                else
                    Debug.LogError($"You clicked a gui button without valid callback method, maybe its callback method is not public?, method name that is trying to be found is: {buttonAtr.callBackName}");
        }
    }
}