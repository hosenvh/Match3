using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystems.RocketMechanic;
using Match3.Game.Gameplay.SubSystems.RocketMechanic.TargetFinders;
using PandasCanPlay.HexaWord.Utility;
using UnityEngine;

namespace Match3.Data.Configuration.TargetFinders
{
    [CreateAssetMenu(menuName = "Gameplay/Target Finders/Specific colored tile type finder")]
    public class SpecificColoredTileTypeFinderConfig : TargetFinderConfig
    {
        [TypeAttribute(typeof(Tile), false)]
        public string tileType;
        public TileColor tileColor;
        public TileExclusionConfig excludedTopTiles;

        public override TargetFinder CreateTargetFinder()
        {
            return new SpecificColoredTileTypeFinder(
                System.Type.GetType(tileType), 
                tileColor, 
                checkTopOnly: false, 
                excludedTopTiles.TypeExclusionChecker());
        }
    }

}
