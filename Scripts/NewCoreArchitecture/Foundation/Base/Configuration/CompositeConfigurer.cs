﻿using System;
using System.Collections.Generic;

namespace Match3.Foundation.Base.Configuration
{
    public class CompositeConfigurer<T> : Configurer<T>
    {
        private List<Configurer<T>> configurers = new List<Configurer<T>>();

        public void AddConfigurer(Configurer<T> configurer)
        {
            this.configurers.Add(configurer);
        }

        public void RemoveConfigurer(Predicate<Configurer<T>> match)
        {
            this.configurers.RemoveAll(match);
        }

        public void Configure(T entity)
        {
            foreach (var configurer in configurers)
                configurer.Configure(entity);
        }

        public void RegisterSelf(ConfigurationManager manager)
        {
            manager.Register(this);
        }
    }
}
