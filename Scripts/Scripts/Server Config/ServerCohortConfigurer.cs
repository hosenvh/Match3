using System;
using UnityEngine;

namespace Match3
{
    public interface ServerCohortConfigurer<T>
    {
        void Configure(ref T oldConfig);
    }


    [Serializable]
    public class CohortConfigReplacer<T> : ServerCohortConfigurer<T>
    {
        [SerializeField] bool isDataValid = false;
        [SerializeField] T config;

        public void Configure(ref T oldConfig)
        {
            if(isDataValid && config != null)
                oldConfig = this.config;
        }
    }

}