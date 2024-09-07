

using Match3.Game.Gameplay.Core;
using System.Collections.Generic;

namespace Match3.Game.Gameplay.SubSystemsData.FrameData.LevelEnding
{
    public class LevelWinningData : BlackBoardData
    {
        public List<Tile> calculatedScoringTiles = new List<Tile>(64);
        public bool isWinSequenceEnded = false;

        public void Clear()
        {
            calculatedScoringTiles.Clear();
            isWinSequenceEnded = false;
        }
    }
}