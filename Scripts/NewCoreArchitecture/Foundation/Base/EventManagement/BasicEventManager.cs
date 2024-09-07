using System;
using System.Collections.Generic;


namespace Match3.Foundation.Base.EventManagement
{
    public class BasicEventManager : EventManager
    {
        private List<EventListener> listeners = new List<EventListener>();
        

        public void Register(EventListener listener)
        {
            if (listeners.Contains(listener) == false)
                listeners.Add(listener);
        }

        public void UnRegister(EventListener listener)
        {
            listeners.Remove(listener);
        }

        public void Propagate(GameEvent evt, object sender)
        {
            
            // TODO : Remove the garbage creation
            // Note : The problem made when we try propagate an event during another propagate operation 
            var listenersCopy = new List<EventListener>(listeners);

            foreach (var listener in listenersCopy)
            {
                try
                {
                    listener.OnEvent(evt, sender);
                }
                catch(Exception exp)
                {
                    UnityEngine.Debug.LogError($"[Event Manager] Exception in propagating event {evt} to {listener}\n{exp.Message}\n{exp.StackTrace}");
                }
            }
        }

        public bool Has(EventListener listener)
        {
            return listeners.Contains(listener);
        }

        public void Clear()
        {
            listeners.Clear();
        }
    }
}