using Match3.Game.Gameplay.Core;
using System.Collections.Generic;

namespace Match3.Game.Gameplay.SubSystemsData.FrameData.PowerUpManagement
{

    public struct HandPowerUpData
    {
        public readonly CellStack orign;
        public readonly CellStack destination;

        public HandPowerUpData(CellStack orign, CellStack destination)
        {
            this.orign = orign;
            this.destination = destination;
        }
    }

    public class PowerUpRequestData : BlackBoardData
    {
        public List<CellStack> hammerTargets = new List<CellStack>(2);
        public List<CellStack> broomTargets = new List<CellStack>(32);
        public List<HandPowerUpData> handPowerUpTargets = new List<HandPowerUpData>();

        public void Clear()
        {
            hammerTargets.Clear();
            broomTargets.Clear();
            handPowerUpTargets.Clear();
        }
    }
}