using UnityEngine;


namespace Match3.Presentation.Hook
{
    public abstract class HookItem : MonoBehaviour
    {
        public int Priority { get; protected set; }

        public void SetHookPriority(int priority)
        {
            Priority = priority;
        }
    }
}