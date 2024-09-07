using Match3.Game.Gameplay.Tiles;
using System.Collections.Generic;

namespace Match3.Game.Gameplay.SubSystems.PadlockMechanic
{
    public class PadlockHitData : BlackBoardData
    {
        public List<Padlock> padlocks = new List<Padlock>(32);

        public void Clear()
        {
            padlocks.Clear();
        }
    }
}