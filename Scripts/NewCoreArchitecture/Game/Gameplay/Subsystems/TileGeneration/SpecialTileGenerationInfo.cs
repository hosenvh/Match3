

using System;

namespace Match3.Game.Gameplay.TileGeneration
{

    public struct SpecialTileGenerationInfo
    {
        public readonly Type tileType;
        public readonly int inLevelLimit;
        public readonly int inBoardLimit;
        public readonly int minGenerationTurn;
        public readonly int maxGenerationTurn;

        public SpecialTileGenerationInfo(Type tileType, int inLevelLimit, int inBoardLimit, int minGenerationTurn, int maxGenerationTurn)
        {
            this.tileType = tileType;
            this.inLevelLimit = inLevelLimit;
            this.inBoardLimit = inBoardLimit;
            this.minGenerationTurn = minGenerationTurn;
            this.maxGenerationTurn = maxGenerationTurn;
        }
    }
    
}