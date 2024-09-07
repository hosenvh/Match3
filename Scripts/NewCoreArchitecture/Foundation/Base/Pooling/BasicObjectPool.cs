
using System.Collections.Generic;
using System.Linq;

namespace Match3.Foundation.Base.ObjectPooling
{
    public abstract class BasicObjectPool<T> : ObjectPool<T>
    {
        Stack<T> pooledObjects = new Stack<T>();

        public T Acquire()
        {
            T obj = default(T);

            if (pooledObjects.Count > 0)
                obj = RemoveAnObjectFromPool();
            else
                obj = CreateObject();

            ActiveObject(obj);

            return obj;
        }

        private T RemoveAnObjectFromPool()
        {
            return pooledObjects.Pop();
        }

        public void Release(T obj)
        {
            pooledObjects.Push(obj);
            DeactiveObject(obj);
        }

        public void Reserve(int count)
        {
            for (int i = 0; i < count; i++)
                Release(CreateObject());
        }

        public int Size()
        {
            return pooledObjects.Count;
        }

        protected abstract T CreateObject();

        protected abstract void DeactiveObject(T obj);
        protected abstract void ActiveObject(T obj);

    }
}