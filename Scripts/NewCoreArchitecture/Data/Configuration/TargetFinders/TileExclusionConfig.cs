using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystems.RocketMechanic.TargetFinders;
using PandasCanPlay.HexaWord.Utility;
using System;
using System.Collections.Generic;

namespace Match3.Data.Configuration.TargetFinders
{
    [Serializable]
    public struct TileExclusionConfig
    {
        [TypeAttribute(typeof(Tile), true)]
        public List<string> exclusions;

        public TypeExclusionChecker TypeExclusionChecker()
        {
            // NOTE: This prevents the null exception caused by incorrect deserialization of Unity.
            if (exclusions == null)
                exclusions = new List<string>();

            var excludedTypes = new HashSet<Type>();

            foreach (var typeName in this.exclusions)
                excludedTypes.Add(Type.GetType(typeName));

            return new TypeExclusionChecker(excludedTypes);
        }
    }

}
