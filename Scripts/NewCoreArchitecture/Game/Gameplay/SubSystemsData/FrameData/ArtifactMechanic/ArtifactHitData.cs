
using Match3.Game.Gameplay.Core;
using System.Collections.Generic;

namespace Match3.Game.Gameplay.SubSystemsData.FrameData.ArtifactMechanic
{
    public class ArtifactHitData : BlackBoardData
    {
        public List<CellStack> cellStackToHitIfColored = new List<CellStack>(16);

        public void Clear()
        {
            cellStackToHitIfColored.Clear();
        }
    }
}