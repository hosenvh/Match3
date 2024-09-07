using Spine.Unity;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Match3.EditorTools.Editor.Menus.MapItems
{
    public class MapSpineItemsLoopingStateDeterminationWindow : EditorWindow
    {
        [MenuItem("Golmorad/Map Items/Map Item Spines Looping State")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(MapSpineItemsLoopingStateDeterminationWindow));
        }

        void OnGUI()
        {
            if (GUILayout.Button("Auto Determine All"))
                AutoDetermineLoopingStatesForAll();
            if (GUILayout.Button("Auto Determine Selection + Children"))
                AutoDetermineLoopingStatesForSelection();
        }

        private void AutoDetermineLoopingStatesForSelection()
        {
            var spineItems = new List<MapItem_Element_Spine>();
            foreach (var root in Selection.gameObjects)
                spineItems.AddRange(root.GetComponentsInChildren<MapItem_Element_Spine>(true));

            DetermineLoopingState(spineItems);
        }

        private void AutoDetermineLoopingStatesForAll()
        {
            var spineItems = new List<MapItem_Element_Spine>();
            foreach(var root in EditorSceneManager.GetActiveScene().GetRootGameObjects())
                spineItems.AddRange(root.GetComponentsInChildren<MapItem_Element_Spine>(true));

            DetermineLoopingState(spineItems);
        }

        private void DetermineLoopingState(List<MapItem_Element_Spine> spineItems)
        {
            Debug.Log($"Total found spines items are {spineItems.Count}");

            foreach (var spineItem in spineItems)
                DetermineLoopingState(spineItem);
        }

        private void DetermineLoopingState(MapItem_Element_Spine spineItem)
        {
            var skeletonGraphic = spineItem.GetComponent<SkeletonGraphic>();

            if (skeletonGraphic == null)
            {
                Debug.LogError($"Item {spineItem.name} doesn't have skeleton graphic", spineItem);
                return;
            }

            if (skeletonGraphic.SkeletonData == null)
            {
                Debug.LogError($"Item {spineItem.name} doesn't have skeleton graphic data", spineItem);
                return;
            }

            bool shouldLoop = false;
            foreach (var animation in skeletonGraphic.SkeletonData.Animations)
                if(IsStateAnimation(animation) && IsAnimationLengthNotZero(animation))
                {
                    shouldLoop = true;
                    break;
                }


            if(shouldLoop)
            {
                spineItem.shouldLoopAnimation = true;
                EditorUtility.SetDirty(spineItem);
                Debug.Log($"Setting animation looping for item {spineItem.name}", spineItem);
            }
        }

        private bool IsAnimationLengthNotZero(Spine.Animation animation)
        {
            return animation.Duration > 0;
        }

        private bool IsStateAnimation(Spine.Animation animation)
        {
            // NOTE: For now only state animations have lenght of 1
            return animation.Name.Length == 1;
        }
    }
}