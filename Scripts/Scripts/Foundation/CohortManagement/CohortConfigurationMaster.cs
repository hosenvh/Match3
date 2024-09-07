

using Match3.Foundation.Base.CohortManagement;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Unity.Configuration;
using UnityEngine;

namespace Match3.Foundation.Unity.CohortManagement
{
    [CreateAssetMenu(fileName = "CohortConfigurationMaster", menuName = "Medrick/CohortManagemenent/CohortConfigurationMaster")]
    public class CohortConfigurationMaster : UnityConfigurationMaster, ChorotConfiguration
    {
        public string id;

        public new string name;

        [Multiline]
        public string description;

        public string ID()
        {
            return id;
        }

        public void RegisterConfigurations(ConfigurationManager configurationManager)
        {
            this.RegisterSelf(configurationManager);
        }

    }
}