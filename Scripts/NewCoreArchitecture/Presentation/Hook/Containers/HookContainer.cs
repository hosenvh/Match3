using System.Collections.Generic;
using UnityEngine;


namespace Match3.Presentation.Hook.Containers
{
    public abstract class HookContainer : MonoBehaviour
    {
        private readonly SortedList<int, HookItem> items = new SortedList<int, HookItem>();

        public int HookCounts => items.Count;
        
        public void AddHookItem(HookItem hookItem)
        {
            hookItem.transform.SetParent(transform, false);
            items.Add(hookItem.Priority, hookItem);
            SortContainerHookItems();
        }

        private void SortContainerHookItems()
        {
            for (int i = 0; i < items.Values.Count; i++)
                items.Values[i].transform.SetSiblingIndex(i);
        }
    }
}