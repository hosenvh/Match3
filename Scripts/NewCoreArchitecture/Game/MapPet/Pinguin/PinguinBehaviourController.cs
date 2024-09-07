using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using Match3.Game.MapPet.Shared;
using UnityEngine;


namespace Match3.Game.MapPet.Pinguin
{
    // ATTENTION: This script and its relateds will delete or heavily refactor at future 
    // ATTENTION: This script and its relateds will delete or heavily refactor at future 
    // ATTENTION: This script and its relateds will delete or heavily refactor at future 
    
    public class PinguinBehaviourController : PetBehaviourController
    {
        protected override void Start()
        {
            currentBehaviour = startBehaviours.RandomOne();
            pathFollower.SetPath(catPaths.RandomOne(catPaths[0]).Path);
            pathFollower.MovePetToPositionAtPath(16.25593f);
            SetBehaviour(currentBehaviour);
        }

        protected override PetBehaviour GetNextBehaviour(PetBehaviour behaviour)
        {
            return behaviour.availableBehaviours.RandomOne();
        }

        public override void SetBehaviour(PetBehaviour behaviour)
        {
            animator.ResetTrigger(currentBehaviour.animatorTriggerParameter);
            if (behaviour.mood == "Idle")
                    pathFollower.MovePetToPositionAtPath(16.25593f);
            base.SetBehaviour(behaviour);
        }
    }
}


