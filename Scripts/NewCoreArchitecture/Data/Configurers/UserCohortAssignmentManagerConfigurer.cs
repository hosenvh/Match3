


using Match3.Foundation.Base.CohortManagement;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Unity.CohortManagement;
using Match3.Foundation.Unity.Configuration;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Data.Configuration
{
    [CreateAssetMenu(menuName = "Medrick/CohortManagemenent/CohortConfigSuiteDatabase")]
    public class UserCohortAssignmentManagerConfigurer : ScriptableConfiguration, Configurer<UserCohortAssignmentManager>
    {
        [AutoFillAssetArray(nameof(cohortConfigs))]
        public string path;


        public List<CohortConfigurationMaster> availableCohortConfigsForOfflineAssignment;

        public CohortConfigurationMaster[] cohortConfigs;

        public void Configure(UserCohortAssignmentManager entity)
        {
            foreach (var cohortConfig in cohortConfigs)
                entity.AddCohortConfigSuite(cohortConfig);

            foreach (var cohortConfig in availableCohortConfigsForOfflineAssignment)
                entity.AddAvailableOfflineCohortConfigSuite(cohortConfig.ID());
            
        }

        public override void RegisterSelf(ConfigurationManager manager)
        {
            manager.Register(this);
        }
    }
}