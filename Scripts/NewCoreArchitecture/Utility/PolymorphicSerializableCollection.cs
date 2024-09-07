using System;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Utility
{
    [Serializable]
    public class PolymorphicSerializedElement
    {
        public string type;
        public string serialization;

        // NOTE: This is an ad-hoc solution to access the base types for elements
        public string baseType;
    }

    [Serializable]
    public class PolymorphicSerializableCollection<CollectionType, ElementType> : ISerializationCallbackReceiver where CollectionType : ICollection<ElementType>
    {
        [SerializeField] List<PolymorphicSerializedElement> serializedElements = new List<PolymorphicSerializedElement>();
        CollectionType collection;

        public PolymorphicSerializableCollection(CollectionType collection)
        {
            this.collection = collection;
        }

        public CollectionType Collection()
        {
            return collection;
        }

        public void OnAfterDeserialize()
        {
            for (int i = 0; i < serializedElements.Count; ++i)
            {
                try
                {
                    serializedElements[i].baseType = typeof(ElementType).AssemblyQualifiedName;

                    var element = (ElementType)JsonUtility.FromJson(serializedElements[i].serialization, Type.GetType(serializedElements[i].type));

                    if (element == null)
                        throw new NullReferenceException($"Element for type {serializedElements[i].type} is null.");
                    
                    collection.Add(element) ;
                }
                catch (Exception e)
                {
                    Debug.LogError($"[Polymorphic Serialization] Error in deserializing rewards\n{e.Message}\n{e.StackTrace}");
                }
            }
        }

        public void OnBeforeSerialize()
        {
        }

        public void ForceSerialize()
        {
            serializedElements.Clear();
            serializedElements.Capacity = collection.Count;

            foreach (var element in collection)
            {
                if (element == null)
                    continue;

                serializedElements.Add(
                    new PolymorphicSerializedElement()
                    {
                        serialization = JsonUtility.ToJson(element),
                        type = element.GetType().AssemblyQualifiedName,
                        baseType = typeof(ElementType).AssemblyQualifiedName
                    });
            }
        }

        public Type TargetType()
        {
            return typeof(ElementType);
        }
    }
}