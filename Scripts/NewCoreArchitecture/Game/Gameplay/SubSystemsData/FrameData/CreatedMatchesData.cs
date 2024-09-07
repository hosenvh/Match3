

using Match3.Foundation.Base.ComponentSystem;
using System.Collections.Generic;

namespace Match3.Game.Gameplay
{
    public class CreatedMatchesData : BlackBoardData
    {
        public List<Matching.Match> data = new List<Matching.Match>(16);

        public void Clear()
        {
            data.Clear();
        }
    }
}