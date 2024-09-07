using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Match3.Foundation.Base.ComponentSystem
{

    public class BasicEntity : Entity
    {
        //Dictionary<Type, Component> compDic = new Dictionary<Type, Component>();
        //List<Component> compList = new List<Component>();
        Component[] compArray = new Component[0];
        List<Component> compList = new List<Component>(32);

        public void AddComponent(Component component)
        {
            compList.Add(component);
            //compList.Add(component);
            //compDic[component.GetType()] = component;
            compArray = compList.ToArray();
            OnComponentAdded(component);
        }

        public void AddComponents(params Component[] components)
        {
            var length = components.Length;
            for (int i = 0; i < length; ++i)
            {
                var component = components[i];
                compList.Add(component);
                OnComponentAdded(component);
            }
            compArray = compList.ToArray();
        }

        protected virtual void OnComponentAdded(Component component) { }

        //public void AddComponent(Component component, int index)
        //{
            
        //    compList.Add(component);
        //    compDic[component.GetType()] = component;
        //}

        public IEnumerable<Component> AllComponents()
        {
            return compArray;
        }


        public T GetComponent<T>() where T : Component
        {
            if(compArray == null) // TODO: Remove this after Rewards section where proven to be working okay.
            {
                Debug.LogError($"Trying to get component from a not initialized entity. {GetType()}");
                return default(T);
            }

            var count = compArray.Length;
            for (int i = 0; i < count; ++i)
                if (compArray[i] is T)
                    return (T)compArray[i];

            return default(T);
        }

        public T GetComponentFromEnd<T>() where T : Component
        {
            var count = compArray.Length;
            for (int i = count-1; i >= 0; --i)
                if (compArray[i] is T)
                    return (T)compArray[i];

            return default(T);
        }

        // TODO: This method can be removed. Refactor scripts using this method to handle their needs via tile caches instead. 
        public T GetComponent<T>(int index) where T : Component
        {
            return (T)compArray[index];
        }


        //// TODO: Note this does only works with exact type.
        //T DictionaryBasedGetComponent<T>() where T : Component
        //{
        //    return (T)compDic[typeof(T)];
        //}

        //T ListBasedGetComponent<T>() where T : Component
        //{
        //    var count = compDic.Count;
        //    for (int i = 0; i < count; ++i)
        //        if (compList[i] is T)
        //            return (T)compList[i];

        //    return default(T);
        //}

        T ArrayBasedGetComponent<T>() where T : Component
        {
            var count = compArray.Length;
            for (int i = 0; i < count; ++i)
                if (compArray[i] is T)
                    return (T)compArray[i];

            return default(T);
        }

        public void ReplaceComponent(Component component)
        {
            if (HasThisComponent())
            {
                ReplaceTheComponent();
                compArray = compList.ToArray();
                OnComponentAdded(component);
            }

            bool HasThisComponent() => compList.Any(x => x.GetType() == component.GetType());

            void ReplaceTheComponent()
            {
                int componentIndex = compList.FindIndex(x => x.GetType() == component.GetType());
                compList[componentIndex] = component;
            }
        }
    }

}