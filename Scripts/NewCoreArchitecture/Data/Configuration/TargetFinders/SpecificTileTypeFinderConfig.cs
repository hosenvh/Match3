using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystems.RocketMechanic;
using Match3.Game.Gameplay.SubSystems.RocketMechanic.TargetFinders;
using PandasCanPlay.HexaWord.Utility;
using UnityEngine;

namespace Match3.Data.Configuration.TargetFinders
{
    [CreateAssetMenu(menuName = "Gameplay/Target Finders/Specific tile type finder")]
    public class SpecificTileTypeFinderConfig : TargetFinderConfig
    {
        [TypeAttribute(typeof(Tile), false)]
        public string tileType;
        public TileExclusionConfig excludedTopTiles;

        public override TargetFinder CreateTargetFinder()
        {
            return new SpecificTileTypeFinder(
                System.Type.GetType(tileType),
                checkTopOnly: false,
                excludedTopTiles.TypeExclusionChecker());
        }
    }

}
