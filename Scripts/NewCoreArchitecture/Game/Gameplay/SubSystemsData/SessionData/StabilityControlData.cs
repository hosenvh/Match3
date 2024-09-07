

namespace Match3.Game.Gameplay.SubSystemsData.SessionData
{
    public class StabilityControlData : BlackBoardData
    {
        public bool shouldForceStablize;

        public void Clear()
        {
            shouldForceStablize = false;
        }
    }
}