using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;
using System.IO;
using NiceJson;

namespace Match3.EditorTools.Editor.Menus.MapItems
{
    // TODO: Remove this file later.
    public class MapItemSizeImporterAndExporter
    {
        enum ValidationType { WithID, WithName, WithNameOrID };

        class TransformMaps
        {
            public Dictionary<long, Transform> idToTransformMap = new Dictionary<long, Transform>();
            public Dictionary<string, Transform> nameToTransformMap= new Dictionary<string, Transform>();
        }


        public void Export(State_Map mapState, TextAsset outputText)
        {
            File.WriteAllText(AssetDatabase.GetAssetPath(outputText), string.Empty);

            using (StreamWriter file = new StreamWriter(AssetDatabase.GetAssetPath(outputText)))
            {
                JsonObject root = new JsonObject();

                root["states"] = ExportItem(mapState.GetComponentsInChildren<MapItem_State>(true).Select(s => s.transform));
                root["userSelects"] = ExportItem(mapState.GetComponentsInChildren<MapItem_UserSelect>(true).Select(s => s.transform));

                file.Write(root.ToJsonString());
            }
           
            EditorUtility.SetDirty(outputText);
        }

        private JsonArray ExportItem(IEnumerable<Transform> transforms)
        {
            JsonArray rootArray = new JsonArray();
            foreach (var transform in transforms)
            {
                JsonObject jsonObject = new JsonObject();

                jsonObject["data"] = CreateTransformData(transform);

                var elementJson = new JsonArray();
                jsonObject["elements"] = elementJson;
                ExportElements(transform, elementJson);

                rootArray.Add(jsonObject);
            }

            return rootArray;
        }

        private void ExportElements(Transform transform, JsonArray jsonArray)
        {
            foreach (var element in transform.GetComponentsInChildren<MapItem_Element>())
            {
                if (element.transform.parent == transform)
                    jsonArray.Add(CreateTransformData(element.transform));
            }
        }

        private JsonObject CreateTransformData(Transform transform)
        {
            var jsonObject = new JsonObject();

            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(transform.gameObject, out _, out long localId);

            jsonObject["name"] = transform.gameObject.name;
            jsonObject["id"] = localId;
            jsonObject["transform"] = EditorJsonUtility.ToJson(transform);

            return jsonObject;
        }


        public void Import(State_Map mapState, TextAsset inputFile)
        {
            var path = AssetDatabase.GetAssetPath(mapState);
            mapState = PrefabUtility.LoadPrefabContents(path).GetComponent<State_Map>();

            using (StreamReader file = new StreamReader(AssetDatabase.GetAssetPath(inputFile)))
            {
                JsonObject root = (JsonObject)JsonObject.ParseJsonString(inputFile.text);

                Import(mapState.GetComponentsInChildren<MapItem_State>(true).Select(s => s.transform), root["states"] as JsonArray);
                Import(mapState.GetComponentsInChildren<MapItem_UserSelect>(true).Select(s => s.transform), root["userSelects"] as JsonArray);
            }

            EditorUtility.SetDirty(mapState);
            PrefabUtility.SaveAsPrefabAsset(mapState.gameObject, path);
        }

        public void ImportForSelection(GameObject[] gameObjects, TextAsset inputFile)
        {

            using (StreamReader file = new StreamReader(AssetDatabase.GetAssetPath(inputFile)))
            {
                JsonObject root = (JsonObject)JsonObject.ParseJsonString(inputFile.text);

                foreach (var gameObject in gameObjects)
                {
                    Import(gameObject.GetComponentsInChildren<MapItem_State>(true).Select(s => s.transform), root["states"] as JsonArray);
                    Import(gameObject.GetComponentsInChildren<MapItem_UserSelect>(true).Select(s => s.transform), root["userSelects"] as JsonArray);

                    EditorUtility.SetDirty(gameObject);
                }
            }
        }


        private void Import(IEnumerable<Transform> transforms, JsonArray itemsJson)
        {
            var transformMaps = CreateTransformMaps(transforms);

            HashSet<Transform> transformsSet = new HashSet<Transform>(transforms);

            foreach (var itemJson in itemsJson)
            {
                var objectData = itemJson["data"];

                if (IsValid(objectData, transformMaps, ValidationType.WithNameOrID, out Transform transform))
                {
                    ImportElements(transform, itemJson["elements"] as JsonArray);

                    Undo.RecordObject(transform, "Change Transform");
                    EditorJsonUtility.FromJsonOverwrite(objectData["transform"], transform);
                    UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(transform);
                    transformsSet.Remove(transform);
                }
            }

            foreach(var transform in transformsSet)
                Debug.LogWarning($"Missing data for {transform.name}");
            
        }

        private void ImportElements(Transform transform, JsonArray elementsJson)
        {
            var elementsTransforms = new HashSet<Transform>(transform.GetComponentsInChildren<MapItem_Element>(true).Where(e => e.transform.parent == transform).Select(e => e.transform));
            var transformMaps = CreateTransformMaps(elementsTransforms);

            foreach (var element in elementsJson)
            {
                if (IsValid(element, transformMaps, ValidationType.WithName, out transform))
                {
                    Undo.RecordObject(transform, "Change Transform");
                    EditorJsonUtility.FromJsonOverwrite(element["transform"], transform);
                    UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(transform);

                    elementsTransforms.Remove(transform);
                }
            }

            foreach (var elementTransform in elementsTransforms)
                Debug.LogError($"Missing data for {elementTransform.name}");

        }


        TransformMaps CreateTransformMaps(IEnumerable<Transform> transforms)
        {
            var maps = new TransformMaps();

            HashSet<string> conflictedNames = new HashSet<string>();

            foreach (var transform in transforms)
            {
                if(LocalIdOf(transform.gameObject) != 0)
                    maps.idToTransformMap.Add(LocalIdOf(transform.gameObject), transform);

                var name = transform.gameObject.name;
                if (maps.nameToTransformMap.ContainsKey(name))
                    conflictedNames.Add(name);

                maps.nameToTransformMap[transform.gameObject.name] = transform;
            }

            foreach (var name in conflictedNames)
            {
                Debug.LogError($"Conflicted name {name}");
                maps.nameToTransformMap.Remove(name);
            }

            return maps;
        }


        private bool IsValid(JsonNode transformData, TransformMaps transformMaps, ValidationType validationType, out Transform transform)
        {
            long id = transformData["id"];
            var name = transformData["name"];
            transform = null;

            switch (validationType)
            {
                case ValidationType.WithID:
                    if(transformMaps.idToTransformMap.ContainsKey(id))
                    {
                        transform = transformMaps.idToTransformMap[id];
                        return true;
                    }
                    else 
                    {
                        Debug.LogWarning($"Could not import item {name} with id {id}");
                        return false;
                    }
                case ValidationType.WithName:
                    if (transformMaps.nameToTransformMap.ContainsKey(name))
                    {
                        transform = transformMaps.nameToTransformMap[name];
                        return true;
                    }
                    else
                    {
                        Debug.LogWarning($"The names of imported object with id {id} and name {name} doesn't match");
                        return false;
                    }
                case ValidationType.WithNameOrID:
                    if (transformMaps.nameToTransformMap.ContainsKey(name))
                    {
                        transform = transformMaps.nameToTransformMap[name];
                        return true;
                    }
                    if (transformMaps.idToTransformMap.ContainsKey(id))
                    {
                        transform = transformMaps.idToTransformMap[id];
                        return true;
                    }
                    else
                    {
                        Debug.LogWarning($"The names of imported object with id {id} and name {name} doesn't match");
                        return false;
                    }
            }

            return true;

        }

        private long LocalIdOf(GameObject gameObject)
        {
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(gameObject, out _, out long localId);
            return localId;
        }

    }


    public class MapItemSizeImportingExportingWindow : EditorWindow
    {
        [MenuItem("Golmorad/Map Items/Map Item Size Exporting and Importing")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(MapItemSizeImportingExportingWindow));
        }

        TextAsset textAsset;

        State_Map mapState;


        public void OnGUI()
        {
            textAsset = (TextAsset)EditorGUILayout.ObjectField("Data File", textAsset, typeof(TextAsset), false);
            mapState = (State_Map)EditorGUILayout.ObjectField("Map State", mapState, typeof(State_Map), false);

            if (GUILayout.Button("Export"))
                new MapItemSizeImporterAndExporter().Export(mapState, textAsset);


            if (GUILayout.Button("Import For Selection"))
                new MapItemSizeImporterAndExporter().ImportForSelection(Selection.gameObjects, textAsset);

            if (GUILayout.Button("Import"))
                new MapItemSizeImporterAndExporter().Import(mapState, textAsset);
        }

    }

}
