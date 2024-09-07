using System.Collections.Generic;
using Match3.EditorTools.Editor.Base;
using UnityEditor;
using UnityEngine;

namespace Match3.EditorTools.Editor.Menus.AssetModification
{
    public class EndSpaceInNameFinderWindow : EditorWindow
    {
        [MenuItem("Golmorad/Asset Modification/EndSpaceInNameFinder")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(EndSpaceInNameFinderWindow));
        }

        void OnGUI()
        {
            if (GUILayout.Button("Apply On Selected Objects"))
                new EndSpaceInNameFinder().ApplyOn(Selection.gameObjects);

        }
    }
    public class EndSpaceInNameFinder : ComponentConvertor<Transform>
    {
        protected override string ConversionTitle()
        {
            return "Finding NameSpaces";
        }

        protected override void ConvertComponent(Transform component)
        {
            Debug.Log($"Object {component.name} has extra space at the end", component);
        }

        protected override List<Transform> ExtractComponentsFrom(GameObject[] gameObjects)
        {
            List<Transform> transforms = new List<Transform>();

            foreach (var obj in gameObjects)
            {
                foreach (var tr in obj.GetComponentsInChildren<Transform>())
                    if (tr.gameObject.name[tr.gameObject.name.Length - 1] == ' ')
                    transforms.Add(tr);
            }
            return transforms;
        }
    }
}