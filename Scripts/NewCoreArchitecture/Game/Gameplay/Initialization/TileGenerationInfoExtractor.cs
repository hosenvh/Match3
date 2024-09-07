using Match3.Game.Gameplay.TileGeneration;
using Match3.Game.Gameplay.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using Match3.Game.Gameplay.Tiles.Explosives;
using UnityEngine;

namespace Match3.Game.Gameplay.Initialization
{
    public class TileGenerationInfoExtractor
    {

        Dictionary<Type, Vector2Int> tileGenrationTurns = new Dictionary<Type, Vector2Int>();

        public TileGenerationInfoExtractor()
        {
            tileGenrationTurns[typeof(Lemonade)] = new Vector2Int(3, 8);
            tileGenrationTurns[typeof(Nut)] = new Vector2Int(2, 6);
            tileGenrationTurns[typeof(Butterfly)] = new Vector2Int(0, 0);
            tileGenrationTurns[typeof(RocketBox)] = new Vector2Int(3, 9);
            tileGenrationTurns[typeof(GasCylinder)] = new Vector2Int(3, 9);
            tileGenrationTurns[typeof(JamJar)] = new Vector2Int(3, 9);
            tileGenrationTurns[typeof(CatColoredBead)] = new Vector2Int(3, 9);
            tileGenrationTurns[typeof(Rocket)] = new Vector2Int(3, 9);
            tileGenrationTurns[typeof(Bomb)] = new Vector2Int(3, 9);
            tileGenrationTurns[typeof(Dynamite)] = new Vector2Int(3, 9);
            tileGenrationTurns[typeof(TNTBarrel)] = new Vector2Int(3, 9);
        }

        public List<SpecialTileGenerationInfo> ExtractFrom(BoardConfig config)
        {
            var infos = new List<SpecialTileGenerationInfo>();

            foreach (var data in config.levelConfig.tileGenerationLimits)
            {
                var type = Type.GetType(data.type);
                infos.Add(
                    new SpecialTileGenerationInfo(
                        type , data.inLevelLimit, data.inBoardLimit, tileGenrationTurns[type].x, tileGenrationTurns[type].y));
            }

            return infos;
        }
    }
}