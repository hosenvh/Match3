
using Match3.Foundation.Base.ObjectPooling;
using UnityEngine;

namespace Match3.Foundation.Unity.ObjectPooling
{
    // TODO: Rename this, it is too similar to UnityComponentObjectPool
    public class UnityObjectPool<T> : MonoBehaviour, ObjectPool<T> where T : Component
    {
        public T componentPrefab;
        public Transform targetTransform;
        public int initialReserve;
        public bool autoSetup;

        protected UnityComponentObjectPool<T> internalPool = new UnityComponentObjectPool<T>();

        void Awake()
        {
            if(autoSetup)
                Setup();
        }

        public void Setup()
        {
            PreSetup();
            internalPool.SetComponentPrefab(componentPrefab);
            internalPool.SetPoolTransform(targetTransform);

            internalPool.Reserve(initialReserve);
        }

        protected virtual void PreSetup()
        {

        }

        public T Acquire()
        {
            return internalPool.Acquire();
        }

        public void Release(T obj)
        {
            internalPool.Release(obj);
        }

        public void Reserve(int count)
        {
            internalPool.Reserve(count);
        }

        public int Size()
        {
            return internalPool.Size();
        }
    }
}