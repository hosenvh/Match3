using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystems.RocketMechanic;
using Match3.Game.Gameplay.SubSystems.RocketMechanic.TargetFinders;
using PandasCanPlay.HexaWord.Utility;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Match3.Data.Configuration.TargetFinders
{
    [CreateAssetMenu(menuName = "Gameplay/Target Finders/Below lemonade finder")]
    public class BelowLemonadeFinderConfig : TargetFinderConfig
    {

        public TileExclusionConfig excludedTopTiles;

        public override TargetFinder CreateTargetFinder()
        {
            return new BelowLemonadeFinder(excludedTopTiles.TypeExclusionChecker());
        }
    }

}
