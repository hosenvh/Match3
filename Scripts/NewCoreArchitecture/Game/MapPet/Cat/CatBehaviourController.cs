using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Match3.Game.MapPet.Shared;


namespace Match3.Game.MapPet.Cat
{
    public class CatBehaviourController : PetBehaviourController
    {
        protected override PetBehaviour GetNextBehaviour(PetBehaviour behaviour)
        {
            return behaviour.availableBehaviours.RandomOne();
        }
    }
}


