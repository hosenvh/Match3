using NiceJson;
using PandasCanPlay.HexaWord.Utility;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Match3.EditorTools.Editor.Menus.Utility
{
    // NOTE: Other functionalities should be added gradually. 
    public class ServerConfigJsonModifyingWindow : EditorWindow
    {
        [MenuItem("Golmorad/Utility/ServerConfig Json Fixer Window", priority = 512)]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(ServerConfigJsonModifyingWindow));
        }

        public enum ModificationType { AddOrReplace, Remove, Duplicate};

        [System.Serializable]
        class Modification
        {
            public string keyOrIndex= "";
            public string value = "";
            public string path = "";
            public string duplicationKey = "";
            public ModificationType modificationType;
        }


        [SerializeField]
        List<Modification> modifications = new List<Modification>();
        string originalJsonString;
        string resultJsonString;

        public void OnGUI()
        {
            originalJsonString = EditorGUILayout.TextField("Server Original Json", originalJsonString);

            SerializedObject so = new SerializedObject(this);
            SerializedProperty modificationsProperty = so.FindProperty(nameof(modifications));
            EditorGUILayout.PropertyField(modificationsProperty, true); 
            so.ApplyModifiedProperties(); 

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Apply"))
                ApplyModifications();
            EditorGUILayout.EndHorizontal();

            resultJsonString = EditorGUILayout.TextField("Result", resultJsonString);
        }

        // TODO: Change this to a command pattern
        private void ApplyModifications()
        {
            var originalJson = JsonNode.ParseJsonString(originalJsonString);
            foreach (var modification in modifications)
            {
                switch(modification.modificationType)
                {
                    case ModificationType.AddOrReplace:
                        ApplyAddOrReplace(modification, originalJson);
                        break;
                    case ModificationType.Remove:
                        ApplyRemoval(modification, originalJson);
                        break;
                    case ModificationType.Duplicate:
                        ApplyDuplication(modification, originalJson);
                        break;
                    default:
                        break;
                }
            }

            resultJsonString = originalJson.ToJsonString();
            GeneralUtilities.CopyToClipboard(resultJsonString);
        }

        private void ApplyAddOrReplace(Modification modification, JsonNode originalJson)
        {
            var jsonNode = TryFindNodeInPath(originalJson, modification.path);

            if (jsonNode is JsonArray jsonArray)
                ReplaceOrInsert(jsonArray, int.Parse(modification.keyOrIndex), JsonNode.ParseJsonString(modification.value));
            else if (jsonNode is JsonObject)
                jsonNode[modification.keyOrIndex] = JsonNode.ParseJsonString(modification.value);
            else
                Debug.LogError($"Add or Replace is not supported on {jsonNode.GetType()}");

        }

        private void ApplyRemoval(Modification modification, JsonNode originalJson)
        {
            var jsonNode = TryFindNodeInPath(originalJson, modification.path);
            (jsonNode as JsonObject).Remove(modification.keyOrIndex);

            if (jsonNode is JsonArray jsonArray)
                jsonArray.RemoveAt(int.Parse(modification.keyOrIndex));
            else if (jsonNode is JsonObject jsonObject)
                jsonObject.Remove(modification.keyOrIndex);
            else
                Debug.LogError($"Removal is not supported on {jsonNode.GetType()}");

        }

        private JsonNode TryFindNodeInPath(JsonNode originalJson, string path)
        {
            var subPaths = path.Split(new char[] { ' ', '.' }, StringSplitOptions.RemoveEmptyEntries);
            JsonNode result = originalJson;

            foreach (var subPath in subPaths)
            {
                if (int.TryParse(subPath, out var index))
                    result = result[index];
                else
                    result = result[subPath];
            }
            
            return result;
        }

        private void ReplaceOrInsert(JsonArray jsonArray, int index, JsonNode value)
        {
            if (index < jsonArray.Count)
                jsonArray[index] = value;
            else
                jsonArray.Insert(index, value);
        }


        private void ApplyDuplication(Modification modification, JsonNode originalJson)
        {
            var jsonNode = TryFindNodeInPath(originalJson, modification.path);
            ApplyDuplicationRecursively(jsonNode, modification.keyOrIndex, modification.duplicationKey);
        }

        private void ApplyDuplicationRecursively(JsonNode node, string originNodeKey, string duplicationNodeKey)
        {
            switch(node)
            {
                case JsonObject jsonObj:
                    foreach (JsonNode n in jsonObj.Values)
                        ApplyDuplicationRecursively(n, originNodeKey, duplicationNodeKey);
                    break;
                case JsonArray jsonArr:
                    foreach (JsonNode n in jsonArr)
                        ApplyDuplicationRecursively(n, originNodeKey, duplicationNodeKey);
                    break;
            }

            if(node.ContainsKey(originNodeKey))
            {
                var originNode = node[originNodeKey];
                var duplicateNode = JsonNode.ParseJsonString(originNode.ToJsonString());
                node[duplicationNodeKey] = duplicateNode;
            }

        }
    }
}
