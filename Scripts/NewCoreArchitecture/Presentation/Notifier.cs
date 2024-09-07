using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notifier : MonoBehaviour
{

    [SerializeField] private GameObject notifyObject = default;
    private Dictionary<Object, bool> notifyRequests = new Dictionary<Object, bool>();


    public void SetNotify(bool notify, Object applicator)
    {
        notifyRequests[applicator] = notify;
        if(notify) notifyObject.SetActive(true);
        else
        {
            bool allRequestsAreFalse = false;
            foreach (var notifRequest in notifyRequests)
            {
                allRequestsAreFalse = notifRequest.Value == false;
                if (!allRequestsAreFalse) break;
            }
            if(allRequestsAreFalse) notifyObject.SetActive(false);
        }
    }
    
    
    
}
