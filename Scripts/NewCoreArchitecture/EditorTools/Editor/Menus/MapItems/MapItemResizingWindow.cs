using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Match3.EditorTools.Editor.Base;
using static Match3.EditorTools.Editor.Base.BasicComponentEditorOperationUtilities;

namespace Match3.EditorTools.Editor.Menus.MapItems
{
    public class MapItemResizingWindow : EditorWindow
    {
        [MenuItem("Golmorad/Map Items/Map Item Resizing Window")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(MapItemResizingWindow));
        }

        public class MapItemResizer : EditorComponentModifier<MapItem_Element>
        {

            float relativeScale;

            public MapItemResizer(float relativeScale)
            {
                this.relativeScale = relativeScale;
            }

            protected override List<MapItem_Element> ExtractComponentsFrom(GameObject[] gameObjects)
            {
                return FindComponentsIn<MapItem_Element>(gameObjects);
            }

            protected override string OperationTitle()
            {
                return "Resizing";
            }

            protected override void ProcessModification(MapItem_Element component)
            {
                Scale(component.transform, relativeScale);
            }
        }

        float relativeScale;

        public void OnGUI()
        {
            relativeScale = EditorGUILayout.FloatField(nameof(relativeScale), relativeScale);
            if (GUILayout.Button("Apply To Selection"))
                new MapItemResizer(relativeScale).ApplyOn(Selection.gameObjects);
        }
    }

}
