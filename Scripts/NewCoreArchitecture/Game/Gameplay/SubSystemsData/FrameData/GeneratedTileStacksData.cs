

using Match3.Game.Gameplay.Core;
using System.Collections.Generic;

namespace Match3.Game.Gameplay
{
    // TODO: Try merge this with GeneratedObjectsData.
    public class GeneratedTileStacksData : BlackBoardData
    {
        public List<TileStack> tileStacks = new List<TileStack>(32);


        public void Clear()
        {
            tileStacks.Clear();
        }
    }
}