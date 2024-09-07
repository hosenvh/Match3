using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystems.RocketMechanic;
using Match3.Game.Gameplay.SubSystems.RocketMechanic.TargetFinders;
using PandasCanPlay.HexaWord.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Data.Configuration.TargetFinders
{

    [CreateAssetMenu(menuName = "Gameplay/Target Finders/Specific cell type finder")]
    public class SpecificCellTypeFinderConfig : TargetFinderConfig
    {
        [TypeAttribute(typeof(Cell), false)]
        public string cellType;

        public TileExclusionConfig excludedTopTiles;

        public override TargetFinder CreateTargetFinder()
        {
            return new SpecificCellTypeFinder(System.Type.GetType(cellType), excludedTopTiles.TypeExclusionChecker());
        }
    }

}
