using System.Collections.Generic;
using System.Linq;

namespace Match3.Foundation.Base.ComponentSystem
{
    public class BasicSpecializedEntity<T> : SpecializedEntity<T> where T :Component
    {
        BasicEntity basicEntity = new BasicEntity();

        List<T> compList = new List<T>();

        public void AddComponent(T component)
        {
            basicEntity.AddComponent(component);
            compList.Add(component);
        }

        public List<T> AllComponents()
        {
            return compList;
        }

        public U GetComponent<U>() where U : T
        {
            return basicEntity.GetComponent<U>();
        }
    }

}