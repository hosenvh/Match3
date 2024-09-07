using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Match3.EditorTools.Editor.Base
{
    public abstract class EditorComponentModifier<T> where T : Component
    {
        int undoGroup;

        public void ApplyOn(GameObject[] gameObjects)
        {
            ModifyComponents(ExtractComponentsFrom(gameObjects));
        }

        protected abstract List<T> ExtractComponentsFrom(GameObject[] gameObjects);

        public void ModifyComponents(List<T> components)
        {
            Debug.Log($"Total found items are {components.Count}");

            foreach (var component in components)
            {
                try
                {
                    ModifiyConversion(component);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error in processing ${component}:\n{e}", component);
                }
            }
        }

        private void ModifiyConversion(T component)
        {
            var gameObject = component.gameObject;

            BeginUndoSession(OperationTitle());
            ProcessModification(component);
            EndUndoSession(gameObject);

        }

        protected abstract string OperationTitle();

        protected abstract void ProcessModification(T component);

        protected void BeginUndoSession(string title)
        {
            Undo.SetCurrentGroupName(title);
            undoGroup = Undo.GetCurrentGroup();
        }

        protected void EndUndoSession(GameObject gameObject)
        {
            Undo.CollapseUndoOperations(undoGroup);
            Undo.FlushUndoRecordObjects();
            PrefabUtility.RecordPrefabInstancePropertyModifications(gameObject);
        }
    }
}
