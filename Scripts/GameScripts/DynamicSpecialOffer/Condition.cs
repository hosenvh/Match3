using UnityEngine;

namespace DynamicSpecialOfferSpace
{
    
    [System.Serializable]
    public abstract class Condition : MonoBehaviour
    {
         public abstract bool ResolveCondition();
    }

}