
using Match3.Foundation.Base.ObjectPooling;
using System;
using UnityEngine;

namespace Match3.Foundation.Unity.ObjectPooling
{
    public class UnityComponentObjectPool<T> : BasicObjectPool<T> where T : Component
    {
        protected Transform targetTransform;
        protected T componentPrefab;

        Action<T> creationPostProcess = delegate { };

        public void SetCreationPostProcess(Action<T> action)
        {
            this.creationPostProcess = action;
        }

        public void SetPoolTransform(Transform transform)
        {
            this.targetTransform = transform;
        }

        public void SetComponentPrefab(T prefab)
        {
            this.componentPrefab = prefab;
        }

        protected override void ActiveObject(T obj)
        {
            obj.gameObject.SetActive(true);
        }

        protected override void DeactiveObject(T obj)
        {
            obj.gameObject.transform.SetParent(targetTransform, false);
            obj.gameObject.SetActive(false);
        }

        protected override T CreateObject()
        {
            var obj = UnityEngine.Object.Instantiate(componentPrefab, targetTransform, false);

            creationPostProcess.Invoke(obj);

            return obj;
        }

    }
}