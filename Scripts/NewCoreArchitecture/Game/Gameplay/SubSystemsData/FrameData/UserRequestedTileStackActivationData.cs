

using Match3.Game.Gameplay.Core;
using System.Collections.Generic;

namespace Match3.Game.Gameplay.Input
{

    public class UserRequestedTileStackActivationData : BlackBoardData
    {
        public List<TileStack> targets = new List<TileStack>(4);

        public void Clear()
        {
            targets.Clear();
        }
    }
}