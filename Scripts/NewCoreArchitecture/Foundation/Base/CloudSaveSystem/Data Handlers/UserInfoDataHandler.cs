using Match3;
using Match3.Foundation.Base.CohortManagement;
using Match3.Foundation.Base.ServiceLocating;
using UnityEngine;



namespace Match3.CloudSave
{

    public class UserInfoDataHandler : ICloudDataHandler
    {
        
        private const string PlayerIdKey = "playerId";
        private const string PlayerCohortKey = "playerCohort";

        
        public void CollectData(ICloudDataStorage cloudStorage)
        {
            var cohortManager = ServiceLocator.Find<UserCohortAssignmentManager>();
            
            cloudStorage.SetString(PlayerCohortKey, cohortManager.AssignedCohortID());
            cloudStorage.SetString(PlayerIdKey, ServiceLocator.Find<UserProfileManager>().GlobalUserId);
        }

        
        public void SpreadData(ICloudDataStorage cloudStorage)
        {
            var cohortManager = ServiceLocator.Find<UserCohortAssignmentManager>();
            
            cohortManager.SetAssignedCohortID(cloudStorage.GetString(PlayerCohortKey));
            ServiceLocator.Find<UserProfileManager>().SetGlobalUserID(cloudStorage.GetString(PlayerIdKey));
        }
    }
    
}


