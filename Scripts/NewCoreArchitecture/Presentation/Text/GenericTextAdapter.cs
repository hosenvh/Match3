using UnityEngine;

namespace Match3.Presentation.TextAdapting
{
    public abstract class GenericTextAdapter<T> : TextAdapter where T: MonoBehaviour
    {
        [SerializeField] protected T target = null;

        void Awake()
        {
            TryFindTarget();
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            TryFindTarget();
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        protected void TryFindTarget()
        {
            if (target == null)
                target = GetComponent<T>();
        }

    }
}