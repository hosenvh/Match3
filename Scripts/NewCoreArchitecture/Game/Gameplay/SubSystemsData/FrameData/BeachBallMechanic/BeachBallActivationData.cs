using System.Collections.Generic;

namespace Match3.Game.Gameplay.SubSystemsData.FrameData.BeachBallMechanic
{
    public class BeachBallActivationData : BlackBoardData
    {
        public List<DelayedCellHitData> delayedCellStackHits = new List<DelayedCellHitData>(64);

        public void Clear()
        {
            delayedCellStackHits.Clear();
        }
    }
}