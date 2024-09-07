using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.CloudSave
{
    public enum CloudSaveRequestStatus
    {
        Successful = 1,
        TimeOutError = -1, 
        InternalError = -2, 
        AuthenticationError = -3,
        BadInputError = -4,
        OperationFailedError = -5,
        NotRequested = -6,
        NoSavedDataExists = -7
    }
    
    
    public enum AuthenticationStatus
    {
        NotAuthenticated = 0,
        AuthenticationFailed = 1,
        Successful = 2,
        SameEmptyAccount = 3,
        DifferentDeviceWithEmptyLocal = 4,
        DifferentDeviceWithFullLocal = 5,
        DifferentEmptyAccount = 6,
        DifferentFullAccount = 7,
        DataNotSync = 8,
        ServiceDeactivated = 9,
        UnMatchedSaveIteration = 10,
        olderAppVersion = 11
    }
    
    
}
