using System;
using System.Collections.Generic;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Utility.GolmoradLogging;


namespace Match3.Overlay.Analytics
{
    public abstract class AnalyticsAdapter : EventListener
    {
        private readonly List<AnalyticsHandler> handlers = new List<AnalyticsHandler>();

        protected AnalyticsAdapter()
        {
            ServiceLocator.Find<EventManager>().Register(this);
        }

        public void RegisterHandler(AnalyticsHandler handler)
        {
            handlers.Add(handler);
        }

        public void UnRegisterHandler<T>() where T : AnalyticsHandler
        {
            T handler = FindHandler<T>();
            if (handler != null)
                handlers.Remove(handler);
        }

        private T FindHandler<T>() where T : AnalyticsHandler
        {
            foreach (var handler in handlers)
                if (handler is T analyticsHandler)
                    return analyticsHandler;

            DebugPro.LogError<AnalyticsLogTag>($"Handler of type '{typeof(T)}' could not be found.");
            return null;
        }

        public List<T> FindAllHandler<T>() where T : AnalyticsHandler
        {
            List<T> result = new List<T>(handlers.Count);

            foreach (var handler in handlers)
                if (handler is T analyticsHandler)
                    result.Add(analyticsHandler);

            return result;
        }

        public virtual void OnEvent(GameEvent evt, object sender)
        {
            try
            {
                foreach (var handler in handlers)
                    handler.OnEvent(evt);
            }
            catch (Exception e)
            {
                DebugPro.LogException<AnalyticsLogTag>(message: "Error in AnalyticsAdapter:\n" + e);
            }
        }
    }
}