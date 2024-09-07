using DG.DemiEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.EditorTools.Editor.Base
{
    public static class BasicComponentEditorOperationUtilities
    {
        public static List<T> FindComponentsIn<T>(GameObject[] gameObjects, Predicate<T> condition) where T : Component
        {
            var compoents = new List<T>();
            foreach (var go in gameObjects)
                foreach (var comp in go.GetComponentsInChildren<T>(true))
                    if (condition.Invoke(comp) == true)
                        compoents.Add(comp);
            return compoents;
        }

        public static List<T> FindComponentsIn<T>(GameObject[] gameObjects) where T : Component
        {
            var compoents = new List<T>();
            foreach (var go in gameObjects)
                compoents.AddRange(go.GetComponentsInChildren<T>(true));
                        
            return compoents;
        }

        public static void ChangeScriptTo<T> (GameObject ownerGameObject, MonoBehaviour component) where T : MonoBehaviour
        {
            Undo.RecordObject(component, "Changing Script");

            var newComponent = ownerGameObject.AddComponent<T>();
            MonoScript replacementScript = MonoScript.FromMonoBehaviour(newComponent);
            GameObject.DestroyImmediate(newComponent);


            SerializedObject so = new SerializedObject(component);
            SerializedProperty scriptProperty = so.FindProperty("m_Script");
            so.Update();
            scriptProperty.objectReferenceValue = replacementScript;
            so.ApplyModifiedProperties();
        }


        public static bool HasComponent<T>(GameObject gameObject)
        {
            return gameObject.GetComponent<T>() != null;
        }

        public static bool HasComponentInChilderen<T>(GameObject gameObject)
        {
            return gameObject.GetComponentInChildren<T>() != null;
        }


        public static void SetPivot(RectTransform target, Vector2 pivot)
        {
            Undo.RecordObject(target, "Setting Pivot");
            if (!target) return;
            var offset = pivot - target.pivot;
            offset.Scale(target.rect.size);
            var wordlPos = target.position + target.TransformVector(offset);
            target.pivot = pivot;
            target.position = wordlPos;
        }

        public static void Scale(Transform target, float scaleCoefficient)
        {
            Undo.RecordObject(target, "Scaling");

            var scale = target.localScale;
            scale *= scaleCoefficient;
            target.localScale = scale;
        }

        public static void Scale(Transform target, Vector2 scaleCoefficient)
        {
            Undo.RecordObject(target, "Scaling");

            var scale = target.localScale;

            scale = Vector3.Scale(scale, new Vector3(scaleCoefficient.x, scaleCoefficient.y, 1));

            target.localScale = scale;
        }

        public static void ConvertImageToSpriteRenderer(Image image, float scaleUp)
        {
            var ownerGameObject = image.gameObject;

            var color = image.color;
            var sprite = image.sprite;

            Undo.RecordObject(ownerGameObject, "Converting Image To SpriteRenderer");

            SetPivot(image.rectTransform, new Vector2(0.5f, 0.5f));

            var size = image.rectTransform.sizeDelta;
            image.SetNativeSize();
            var nativeSize = image.rectTransform.sizeDelta;

            Scale(image.transform, (size / nativeSize) * scaleUp);

            Undo.DestroyObjectImmediate(image);
            Undo.DestroyObjectImmediate(ownerGameObject.GetComponent<CanvasRenderer>());

            var renderer = Undo.AddComponent<SpriteRenderer>(ownerGameObject);
            renderer.sprite = sprite;
            renderer.color = color;

            //Undo.DestroyObjectImmediate(ownerGameObject.transform.As<RectTransform>());
        }


        public static List<string> ExtractPathsFromSelection()
        {
            var paths = new List<string>();

            var objs = Selection.GetFiltered<UnityEngine.Object>(SelectionMode.Assets);
            foreach (var obj in objs)
            {
                string path = AssetDatabase.GetAssetPath(obj);

                if (!string.IsNullOrEmpty(path) && (Directory.Exists(path) || File.Exists(path)))
                    paths.Add(path);

            }

            return paths;
        }

        public static void ModifyComponentsInPrefabs<T>(Predicate<T> condition, Action<T> modificationAction) where T : UnityEngine.Behaviour
        {
            var prefabsGuids = AssetDatabase.FindAssets($"t:Prefab");
            foreach (var guid in prefabsGuids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = PrefabUtility.LoadPrefabContents(assetPath);

                bool modified = false;
                foreach(var comp in prefab.GetComponentsInChildren<T>(true))
                {
                    if (condition.Invoke(comp))
                    {
                        PrefabUtility.RecordPrefabInstancePropertyModifications(comp);
                        modificationAction.Invoke(comp);
                        modified = true;
                    }
                }
                if(modified)
                    PrefabUtility.SaveAsPrefabAsset(prefab, assetPath);

                PrefabUtility.UnloadPrefabContents(prefab);
            }
        }

        public static void ModifyComponentsInScenes<T>(Predicate<T> condition, Action<T> modificationAction) where T : UnityEngine.Behaviour
        {
            var prefabsGuids = AssetDatabase.FindAssets($"t:Scene");
            foreach (var guid in prefabsGuids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var scene = EditorSceneManager.GetSceneByPath(assetPath);

                bool modified = false;

                foreach (var obj in scene.GetRootGameObjects())
                {
                    foreach (var comp in obj.GetComponentsInChildren<T>(true))
                    {
                        if (condition.Invoke(comp))
                        {
                            EditorSceneManager.MarkSceneDirty(scene);
                            modificationAction.Invoke(comp);
                            modified = true;
                        }
                    }
                }
                if (modified)
                    EditorSceneManager.MarkSceneDirty(scene);

            }
        }

    }
}