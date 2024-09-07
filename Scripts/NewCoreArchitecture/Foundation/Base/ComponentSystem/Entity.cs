
using System.Collections.Generic;

namespace Match3.Foundation.Base.ComponentSystem
{
    public interface Entity 
    {
        void AddComponent(Component component);
        T GetComponent<T>() where T : Component;

        IEnumerable<Component> AllComponents();
    }
}