
using System;
using System.Collections.Generic;
using Match3.LevelEditor.Presentation;

namespace Match3.Foundation.Base.ServiceLocating
{
    public class ServiceLocator
    {
        private static ServiceLocator instance;

        private List<Service> services = new List<Service>();

        public static void Init()
        {
            if (instance == null)
                instance = new ServiceLocator();
        }

        public static bool IsInited()
        {
            return instance != null;
        }

        public static void Clear()
        {
            instance = null;
        }

        public static void Register(Service service)
        {
            instance.services.Add(service);
        }

        public static void UnRegister<T>() where T : Service
        {
            var service = Find<T>();
            instance.services.Remove(service);
        }

        public static void Replace<T>(T service) where T : Service
        {
            if(Has<T>())
                UnRegister<T>();
            Register(service);
        }

        public static T Find<T>() where T : Service
        {
            foreach (var service in instance.services)
                if (service is T)
                    return (T)service;

            throw new System.Exception(string.Format("Service of type '{0}' could not be found.", typeof(T).ToString()));
        }

        public static bool Has<T>()
        {
            foreach (var service in instance.services)
                if (service is T)
                    return true;

            return false;
        }
    }
}