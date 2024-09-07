using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.ServiceLocating;
using System;

namespace Match3.Foundation.Base.FactorySystem
{
    public class GenericServiceFactory<T>
    {
        Type targetType;
        Type fallbackType;

        public GenericServiceFactory()
        {
            ServiceLocator.Find<ConfigurationManager>().Configure(this);
        }

        public T Create()
        {
            T service;
            try
            {
                service = CreateFrom(targetType);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
                service = CreateFrom(fallbackType);
            }

            return service;
        }

        public void SetTypes(Type targetType, Type fallbackType)
        {
            this.targetType = targetType;
            this.fallbackType = fallbackType;
        }

        protected virtual T CreateFrom(Type type)
        {
            T service = (T)Activator.CreateInstance(type);

            return service;
        }
    }
}