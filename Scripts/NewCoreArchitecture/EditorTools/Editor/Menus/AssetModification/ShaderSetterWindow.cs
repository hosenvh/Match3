using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using Match3.EditorTools.Editor.Base;


namespace Match3.EditorTools.Editor.Menus.AssetModification
{
    public class ShaderSetterWindow : EditorWindow
    {
        class ShaderFinder : ComponentConvertor<MeshRenderer>
        {

            string shaderName;

            public void SetShaderName(string shaderName)
            {
                this.shaderName = shaderName;
            }

            protected override string ConversionTitle()
            {
                return "Finding Mesh Shaders";
            }

            protected override void ConvertComponent(MeshRenderer component)
            {
                foreach(var material in component.sharedMaterials)
                if (material.shader.name.Equals(shaderName))
                    Debug.Log($"Component {component.name} has a material of type {shaderName}", component);
            }

            protected override List<MeshRenderer> ExtractComponentsFrom(GameObject[] gameObjects)
            {
                return BasicComponentEditorOperationUtilities.FindComponentsIn<MeshRenderer>(gameObjects);
            }
        }

        [MenuItem("Golmorad/Asset Modification/Shader Setter")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(ShaderSetterWindow));
        }

        ShaderInfo[] shaderInfos;
        int shaderIndex = 0;

        void OnGUI()
        {
            shaderInfos = ShaderUtil.GetAllShaderInfo();
            GUILayout.Label("Select Shader: ");
            shaderIndex = EditorGUILayout.Popup(shaderIndex, shaderInfos.Select(r => r.name).ToArray());
            GUILayout.Space(10);
            if (GUILayout.Button("Apply On Selected Objects"))
            {
                var s = new ShaderFinder();
                s.SetShaderName(shaderInfos[shaderIndex].name);
                s.ApplyOn(Selection.gameObjects);
            }

        }
    }
}