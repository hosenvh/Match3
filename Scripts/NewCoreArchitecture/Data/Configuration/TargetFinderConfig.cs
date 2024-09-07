using Match3.Game.Gameplay.SubSystems.RocketMechanic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Data.Configuration
{

    public  abstract class TargetFinderConfig : ScriptableObject
    {
        public abstract TargetFinder CreateTargetFinder();
    }

}
