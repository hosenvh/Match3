using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.FactorySystem;
using Match3.Foundation.Unity.Configuration;
using Match3.Main;
using System;
using UnityEngine;

namespace Match3.Data.Configuration
{

    public abstract class GenericServiceFactoryConfigurer<T> : ScriptableConfiguration, Configurer<GenericServiceFactory<T>> where T : class
    {
        // NOTE: This is my ideal. But unfortunately this is not supported in C#
        //[TypeAttribute(typeof(T), false)]
        //public string targetType = "";
        //[TypeAttribute(typeof(T), false)]
        //public string fallbackType = "";

        public void Configure(GenericServiceFactory<T> entity)
        {
            entity.SetTypes(Type.GetType(TargetType()), Type.GetType(FallbackType()));
        }

        public override void RegisterSelf(ConfigurationManager manager)
        {
            manager.Register(this);
        }

        protected abstract string TargetType();
        protected abstract string FallbackType();

    }
}