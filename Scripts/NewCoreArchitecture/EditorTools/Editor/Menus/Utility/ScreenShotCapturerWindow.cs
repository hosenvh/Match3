using System;
using UnityEditor;
using UnityEngine;


namespace Match3.EditorTools.Editor.Menus.Utility
{
    public class ScreenShotCapturerWindow : EditorWindow
    {
        private struct GuiError
        {
            private string message;

            public void Clear()
            {
                SetMessage(string.Empty);
            }

            public void SetMessage(string message)
            {
                this.message = message;
            }

            public void Print()
            {
                if (string.IsNullOrEmpty(message) == false)
                    EditorGUILayout.HelpBox($"Error: {message}", MessageType.Error);
            }
        }

        private ScreenShotCapturer screenShotCapturer;
        private GuiError occuredError;

        private ScreenShotCapturer ScreenShotCapturer
        {
            get
            {
                if (screenShotCapturer == null)
                    screenShotCapturer = new GameObject("ScreenShot Capturer").AddComponent<ScreenShotCapturer>();
                return screenShotCapturer;
            }
        }

        [MenuItem("Golmorad/Utility/ScreenShot Capturer", priority = 546)]
        public static void ShowWindow()
        {
            GetWindow(typeof(ScreenShotCapturerWindow));
        }

        private void OnDestroy()
        {
            if (screenShotCapturer)
                DestroyImmediate(ScreenShotCapturer.gameObject);
        }

        private void OnGUI()
        {
            ExposeFields();
            DrawButton(title: "Browse", () => ScreenShotCapturer.saveDestinationPath = AskForSaveDestinationPath());
            EditorGUILayout.Space();
            DrawButton(title: "Capture ScreenShot", () =>
            {
                occuredError.Clear();
                TryToCaptureScreenShot();
            });
            EditorGUILayout.Space();
            occuredError.Print();
        }

        private void ExposeFields()
        {
            ScreenShotCapturer.sourceCamera = (Camera) EditorGUILayout.ObjectField(nameof(ScreenShotCapturer.sourceCamera), ScreenShotCapturer.sourceCamera, typeof(Camera), allowSceneObjects: true);
            ScreenShotCapturer.distance = EditorGUILayout.FloatField($"{nameof(ScreenShotCapturer.distance)}, E.g. 70", ScreenShotCapturer.distance);
            ScreenShotCapturer.resolution = (int) EditorGUILayout.Slider($"{nameof(ScreenShotCapturer.resolution)}, E.g 6000", ScreenShotCapturer.resolution, 0, SystemInfo.maxTextureSize * SystemInfo.maxTextureSize / 1024f - 1);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"{nameof(ScreenShotCapturer.saveDestinationPath)}: {ScreenShotCapturer.saveDestinationPath}");
            ScreenShotCapturer.saveFileName = EditorGUILayout.TextField(nameof(ScreenShotCapturer.saveFileName), ScreenShotCapturer.saveFileName);
            EditorGUILayout.Space();
        }

        private void DrawButton(string title, Action onClick)
        {
            if (GUILayout.Button(title))
                onClick.Invoke();
        }

        private string AskForSaveDestinationPath()
        {
            return EditorUtility.OpenFolderPanel(title: "Save To...", folder: "", defaultName: "");
        }

        private void TryToCaptureScreenShot()
        {
            if (ScreenShotCapturer.IsItNotPossibleToCaptureScreenShot(out string reason))
                occuredError.SetMessage(reason);
            else
                ScreenShotCapturer.CaptureScreenShot();
        }
    }

    public class ScreenShotCapturer : MonoBehaviour
    {
        public Camera sourceCamera;
        public float distance;
        public int resolution;
        public string saveDestinationPath;
        public string saveFileName;

        private Camera renderCamera;
        private RenderTexture renderTexture;
        private Texture2D virtualPhoto;

        public bool IsItNotPossibleToCaptureScreenShot(out string reason)
        {
            if (sourceCamera == null)
            {
                reason = "No Camera Assigned";
                return true;
            }
            else if (string.IsNullOrEmpty(saveDestinationPath) || string.IsNullOrEmpty(saveFileName))
            {
                reason = "Destination path OR Destination file name is not set";
                return true;
            }
            else if (resolution <= 0 || distance <= 0)
            {
                reason = "Assigned values are not appropriate.";
                return true;
            }

            reason = "";
            return false;
        }

        public void CaptureScreenShot()
        {
            CreateRenderTexture();
            CreateRenderCamera();
            SetupRenderCamera();

            RenderToVirtualPhoto();
            SaveRenderedVirtualPhoto();

            DestroyImmediate(renderCamera.gameObject);
        }

        private void CreateRenderTexture()
        {
            renderTexture = new RenderTexture(resolution, resolution, depth: 24);
        }

        private void CreateRenderCamera()
        {
            renderCamera = Instantiate(sourceCamera, parent: transform);
            renderCamera.transform.position = sourceCamera.transform.position;
            renderCamera.transform.eulerAngles = sourceCamera.transform.eulerAngles;
        }

        private void SetupRenderCamera()
        {
            renderCamera.enabled = true;
            renderCamera.cameraType = CameraType.Game;
            renderCamera.forceIntoRenderTexture = true;
            renderCamera.orthographic = true;
            renderCamera.orthographicSize = distance;
            renderCamera.aspect = 1.0f;
            renderCamera.targetDisplay = 2;
            renderCamera.targetTexture = renderTexture;
        }

        private void RenderToVirtualPhoto()
        {
            virtualPhoto = new Texture2D(width: resolution, height: resolution, textureFormat: TextureFormat.RGB24, mipChain: false);
            RenderTexture.active = renderTexture;

            renderCamera.Render();
            virtualPhoto.ReadPixels(source: new Rect(0, 0, resolution, resolution), destX: 0, 0);

            RenderTexture.active = null;
        }

        private void SaveRenderedVirtualPhoto()
        {
            byte[] bytes = virtualPhoto.EncodeToTGA();
            System.IO.File.WriteAllBytes($@"{saveDestinationPath}\{saveFileName}.tga", bytes);
        }
    }
}