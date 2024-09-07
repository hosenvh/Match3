
namespace Match3.Foundation.Base.ObjectPooling
{
    public interface ObjectPool<T>
    {
        T Acquire();
        void Release(T obj);

        void Reserve(int count);

        int Size();

    }
}