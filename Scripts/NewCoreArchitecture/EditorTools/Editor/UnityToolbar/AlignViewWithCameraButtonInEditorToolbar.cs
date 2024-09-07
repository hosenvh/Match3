using Match3.EditorTools.Editor.UnityToolbar.Base;
using Match3.EditorTools.Editor.Utility;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityToolbarExtender;
using static Base;


namespace Match3.EditorTools.Editor.UnityToolbar
{
    [InitializeOnLoad]
    public class AlignViewWithCameraButtonInEditorToolbar
    {
        static AlignViewWithCameraButtonInEditorToolbar()
        {
            ToolbarExtender.AddDrawerToLeftToolbar(priority: 2, DrawToolbarGUI);
        }

        private static void DrawToolbarGUI()
        {
            EditorUtilities.InsertButton(
                title: "",
                icon: EditorGUIUtility.IconContent(@"Camera Gizmo").image,
                style: EditorToolbarStyles.GetCommandButtonStyle(widthFactor: 0.5f),
                onClick: AlignViewWithCamera,
                tooltip: "Align View With Camera");
        }

        private static void AlignViewWithCamera()
        {
            if (IsAnyCameraAvailable())
            {
                Camera camera = GetCurrentCamera();
                AlignSceneViewWith(camera.transform);
            }
        }

        private static bool IsAnyCameraAvailable()
        {
            return GetCurrentCamera() != null;
        }

        private static Camera GetCurrentCamera()
        {
            if (IsInPrefabEditMode())
                return GetCameraInPrefab();
            if (IsInStateMap())
                return GetCameraInMap();
            return Camera.main;


            bool IsInPrefabEditMode() => PrefabStageUtility.GetCurrentPrefabStage() != null;
            Camera GetCameraInPrefab() => PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot.GetComponentInChildren<Camera>();
            bool IsInStateMap() => Application.isPlaying && ServiceLocator.Find<GameTransitionManager>().IsInMap();
            Camera GetCameraInMap() => gameManager.mapManager.CurrentMap.mapCameraController.mapCamera;
        }

        private static void AlignSceneViewWith(Transform target)
        {
            SceneView view = SceneView.lastActiveSceneView;
            view.orthographic = true;
            view.AlignViewToObject(target);
        }
    }
}