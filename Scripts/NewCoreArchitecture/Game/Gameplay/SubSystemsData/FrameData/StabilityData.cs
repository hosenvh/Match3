
namespace Match3.Game.Gameplay.SubSystemsData.FrameData
{
    public class StabilityData : BlackBoardData
    {
        public bool wasStableLastChecked;

        public void Clear()
        {
            wasStableLastChecked = false;
        }
    }
}