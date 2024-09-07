using System.Collections;
using System.Collections.Generic;

namespace Match3.Foundation.Base.ComponentSystem
{
    public interface SpecializedEntity<T> where T : Component
    {
        void AddComponent(T component);
        U GetComponent<U>() where U : T;

        List<T> AllComponents();
    }
}