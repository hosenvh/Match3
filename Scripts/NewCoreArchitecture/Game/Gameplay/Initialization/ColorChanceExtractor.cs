using Match3.Game.Gameplay.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Match3.Game.Gameplay.Initialization
{
    public class ColorChanceExtractor
    {
        public Dictionary<TileColor, float> ExtractGenerationChancesFrom(BoardConfig config)
        {
            var dic = new Dictionary<TileColor, float>();

            foreach (var color in Enum.GetValues(typeof(TileColor)).Cast<TileColor>())
            {
                if (color == TileColor.None)
                    continue;

                dic[color] = 1;
            }

            foreach (var entry in config.levelConfig.colorChances)
                dic[entry.color] = entry.wieght;

            return dic;
        }

        public Dictionary<TileColor, float> ExtractDirtinessChancesFrom(BoardConfig config)
        {
            var dic = new Dictionary<TileColor, float>();

            foreach (var entry in config.levelConfig.colordirtinessChances)
                dic[entry.color] = entry.wieght;

            return dic;
        }
    }
}