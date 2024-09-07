using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3
{
    [Serializable]
    public class NotificationServerConfigData 
    {
        [SerializeField] private bool isScenarioNotificationActivated;
    
        public bool IsScenarioNotificationActivated => isScenarioNotificationActivated;

    }    
    
    
    [Serializable]
    public class NotificationServerCohortConfig : CohortConfigReplacer<NotificationServerConfigData>
    {
        
    }
    
    
}




