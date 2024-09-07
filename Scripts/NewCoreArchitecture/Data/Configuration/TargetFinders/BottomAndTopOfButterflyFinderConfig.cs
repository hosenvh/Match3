using Match3.Game.Gameplay.SubSystems.RocketMechanic;
using Match3.Game.Gameplay.SubSystems.RocketMechanic.TargetFinders;
using UnityEngine;

namespace Match3.Data.Configuration.TargetFinders
{
    [CreateAssetMenu(menuName = "Gameplay/Target Finders/Bottom and top of butterfly finder")]
    public class BottomAndTopOfButterflyFinderConfig : TargetFinderConfig
    {
        public TileExclusionConfig excludedTopTiles;

        public override TargetFinder CreateTargetFinder()
        {
            return new BottomAndTopOfButterflyFinder(excludedTopTiles.TypeExclusionChecker());
        }
    }

}
