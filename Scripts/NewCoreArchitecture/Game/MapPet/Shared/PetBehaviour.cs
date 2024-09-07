using System.Collections.Generic;
using Match3.Foundation.Base.DataStructures;
using UnityEngine;

namespace Match3.Game.MapPet.Shared
{
    [CreateAssetMenu(menuName = "Match3/AIBehaviours/PetBehaviour")]
    public class PetBehaviour : ScriptableObject
    {
        public string mood;
        public RangeFloat actTime;
        public float moveSpeed;
        
        [Space(10)]
        [SerializeField] public string animatorTriggerParameter = "None";
        public float animatorMoveBlendValue;

        [Space(10)]
        public List<PetBehaviour> availableBehaviours;
    } 
}