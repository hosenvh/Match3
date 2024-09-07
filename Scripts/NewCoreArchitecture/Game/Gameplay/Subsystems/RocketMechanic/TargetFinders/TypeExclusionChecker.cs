using System;
using System.Collections.Generic;


namespace Match3.Game.Gameplay.SubSystems.RocketMechanic.TargetFinders
{
    public class TypeExclusionChecker
    {
        HashSet<Type> exclusions;

        public TypeExclusionChecker(HashSet<Type> exclusions)
        {
            this.exclusions = exclusions;
        }

        public bool IsNotExcluded(Type type)
        {
            return exclusions.Contains(type) == false;
        }
    }
}