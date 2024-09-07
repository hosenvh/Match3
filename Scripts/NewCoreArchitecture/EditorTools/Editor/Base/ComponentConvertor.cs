using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Match3.EditorTools.Editor.Base
{
    // TOOD: Can it be link to EditorComponentModifier?
    public abstract class ComponentConvertor<T> where T: Component
    {
        int undoGroup;

        protected float globalScaleUp = 1;

        public void SetGlobalScaleUp(float scaleUp)
        {
            this.globalScaleUp = scaleUp;
        }

        public void ApplyOn(GameObject[] gameObjects)
        {
            ConvertComponents(ExtractComponentsFrom(gameObjects));
        }

        protected abstract List<T> ExtractComponentsFrom(GameObject[] gameObjects);

        public void ConvertComponents(List<T> components)
        {
            Debug.Log($"Total found items are {components.Count}");

            foreach (var component in components)
            {
                try
                {
                    ProcessConversion(component);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error in processing ${component}:\n{e}", component);
                }
            }
        }

        private void ProcessConversion(T component)
        {
            var gameObject = component.gameObject;

            BeginUndoSession(ConversionTitle());
            ConvertComponent(component);
            EndUndoSession(gameObject);

        }

        protected abstract string ConversionTitle();

        protected abstract void ConvertComponent(T component);

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