using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Match3.EditorTools.Editor.Base
{
    public abstract class EditorAssetModifier<T> where T : UnityEngine.Object
    {
        int undoGroup;

        public void ApplyOn(string[] paths)
        {
            ModifyAssets(ExtractAssetsFrom(paths));
        }

        protected abstract List<T> ExtractAssetsFrom(string[] paths);

        public void ModifyAssets(List<T> assets)
        {
            Debug.Log($"Total found items are {assets.Count}");

            foreach (var asset in assets)
            {
                try
                {
                    ModifiyConversion(asset);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error in processing ${asset}:\n{e}", asset);
                }
            }
        }

        private void ModifiyConversion(T asset)
        {

            BeginUndoSession(OperationTitle());
            ProcessModification(asset);
            EndUndoSession(asset);

        }

        protected abstract string OperationTitle();

        protected abstract void ProcessModification(T component);

        protected void BeginUndoSession(string title)
        {
            Undo.SetCurrentGroupName(title);
            undoGroup = Undo.GetCurrentGroup();
        }

        protected void EndUndoSession(UnityEngine.Object asset)
        {
            Undo.CollapseUndoOperations(undoGroup);
            Undo.FlushUndoRecordObjects();
            PrefabUtility.RecordPrefabInstancePropertyModifications(asset);
            EditorUtility.SetDirty(asset);
        }
    }
}
