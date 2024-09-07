using I2.Loc;
using Match3.Utility.Editor;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using FileTypeInfo = System.Collections.Generic.KeyValuePair<string, long>;

namespace Match3.EditorTools.Editor.Menus.AssetModification
{

	// WARNING: This is not tested well enough.
    public class AssetDependencyReplacingWindow : EditorWindow
	{
		UnityEngine.Object target;
		UnityEngine.Object oldDependency;
		UnityEngine.Object newDependency;

		static List<FileTypeInfo> fileTypes = new List<FileTypeInfo> { new FileTypeInfo("FontAsset", 11400000 ) };

		int selectedFiletype = 0;

		[MenuItem("Golmorad/Asset Modification/Replace Asset Dependency")]
		public static void ShowWindow()
		{
			EditorWindow.GetWindow(typeof(AssetDependencyReplacingWindow));
		}

		void OnGUI()
		{
			target = EditorGUILayout.ObjectField("Target", target, typeof(UnityEngine.Object), allowSceneObjects: false);
			oldDependency = EditorGUILayout.ObjectField("Old Dependecny", oldDependency, typeof(UnityEngine.Object), allowSceneObjects: false);
			newDependency = EditorGUILayout.ObjectField("New Dependency", newDependency, typeof(UnityEngine.Object), allowSceneObjects: false);
			selectedFiletype = EditorGUILayout.Popup("File Type", selectedFiletype, fileTypes.Select(f => f.Key).ToArray());

			if (GUILayout.Button("Replace"))
				 AssetDependencyReplacer.Replace(target, oldDependency, newDependency, fileTypes[selectedFiletype].Value);
		}
	}

}
