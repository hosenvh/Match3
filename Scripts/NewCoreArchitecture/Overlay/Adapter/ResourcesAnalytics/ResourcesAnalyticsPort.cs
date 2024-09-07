using System.Collections.Generic;
using System.Linq;


namespace Match3.Overlay.Analytics.ResourcesAnalytics
{
    public class Port
    {
        public readonly string itemType;
        public readonly string itemId;

        public Port(string itemType, string itemId)
        {
            this.itemType = itemType;
            this.itemId = itemId;
        }

        public bool IsEqual(Port port)
        {
            return itemType.Equals(port.itemType) && itemId.Equals(port.itemId);
        }
    }

    public class PortsCollection
    {
        public readonly List<Port> elements = new List<Port>();

        public int Count => elements.Count;

        public void Push(Port port)
        {
            elements.Add(port);
        }

        public Port Peek()
        {
            return elements.Last();
        }

        public void RemoveAll(Port port)
        {
            elements.RemoveAll(element => element.IsEqual(port));
        }

        public void Clear()
        {
            elements.Clear();
        }

        public bool Contains(Port port)
        {
            return elements.Exists(element => element.IsEqual(port));
        }
    }
}