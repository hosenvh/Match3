using System;
using System.Collections.Generic;
using Match3.Utility.GolmoradLogging;
using UnityEngine;


namespace Match3.Overlay.Analytics
{
    public class AnalyticsAdaptersManager
    {
        private bool isInitialized;

        private readonly List<AnalyticsAdapter> adapters = new List<AnalyticsAdapter>();
        private readonly Dictionary<Type, List<AnalyticsHandler>> handlersToBeRegisteredIntoItsAdapterAfterInitialization = new Dictionary<Type, List<AnalyticsHandler>>();

        public void RegisterAnalyticsAdaptor(AnalyticsAdapter adapter)
        {
            adapters.Add(adapter);
        }

        public void UnRegisterAnalyticsAdaptor<TAdapter>() where TAdapter : AnalyticsAdapter
        {
            var adapter = FindAnalyticsAdaptor(typeof(TAdapter));
            if (adapter != null)
                adapters.Remove(adapter);
        }

        private AnalyticsAdapter FindAnalyticsAdaptor(Type targetAdapterType)
        {
            foreach (AnalyticsAdapter adapter in adapters)
                if (adapter.GetType() == targetAdapterType)
                    return adapter;

            DebugPro.LogError<AnalyticsLogTag>($"Adaptor of type '{targetAdapterType}' could not be found.");
            return null;
        }

        public void Initialize()
        {
            RegisterHandlersWaitingForInitialization();
            isInitialized = true;
        }

        private void RegisterHandlersWaitingForInitialization()
        {
            foreach (KeyValuePair<Type, List<AnalyticsHandler>> element in handlersToBeRegisteredIntoItsAdapterAfterInitialization)
            {
                foreach (AnalyticsHandler handler in element.Value)
                    RegisterHandler(targetAdapterType: element.Key, handler);
            }
            handlersToBeRegisteredIntoItsAdapterAfterInitialization.Clear();
        }

        public void RegisterAnalyticsHandler<TAdaptor>(AnalyticsHandler handler) where TAdaptor : AnalyticsAdapter
        {
            Type targetAdapterType = typeof(TAdaptor);
            if (isInitialized)
                RegisterHandler(targetAdapterType, handler);
            else
                AddHandlerToBeRegisteredAfterInitialization(targetAdapterType, handler);
        }

        public void UnRegisterAnalyticsHandler<TAdaptor, THandler>() where TAdaptor : AnalyticsAdapter where THandler : AnalyticsHandler
        {
            if (isInitialized)
                UnRegisterHandler<TAdaptor, THandler>();
            else
                RemoveHandlerFromBeingRegisteredAfterInitialization<TAdaptor, THandler>();
        }

        private void RegisterHandler(Type targetAdapterType, AnalyticsHandler handler)
        {
            var adapter = FindAnalyticsAdaptor(targetAdapterType);
            adapter?.RegisterHandler(handler);
        }

        private void UnRegisterHandler<TAdaptor, THandler>()  where TAdaptor : AnalyticsAdapter where THandler : AnalyticsHandler
        {
            var adapter = FindAnalyticsAdaptor(targetAdapterType: typeof(TAdaptor));
            adapter?.UnRegisterHandler<THandler>();
        }

        private void AddHandlerToBeRegisteredAfterInitialization(Type targetAdapterType, AnalyticsHandler handler)
        {
            if (handlersToBeRegisteredIntoItsAdapterAfterInitialization.ContainsKey(targetAdapterType))
                handlersToBeRegisteredIntoItsAdapterAfterInitialization[targetAdapterType].Add(handler);
            else
                handlersToBeRegisteredIntoItsAdapterAfterInitialization.Add(targetAdapterType, new List<AnalyticsHandler> {handler});
        }

        private void RemoveHandlerFromBeingRegisteredAfterInitialization<TAdaptor, THandler>()  where TAdaptor : AnalyticsAdapter where THandler : AnalyticsHandler
        {
            Type targetAdapterType = typeof(TAdaptor);
            if (handlersToBeRegisteredIntoItsAdapterAfterInitialization.ContainsKey(targetAdapterType) == false)
                return;

            List<AnalyticsHandler> waitingHandlers = handlersToBeRegisteredIntoItsAdapterAfterInitialization[targetAdapterType];
            waitingHandlers.RemoveAll(handler => handler is THandler);
        }
    }
}