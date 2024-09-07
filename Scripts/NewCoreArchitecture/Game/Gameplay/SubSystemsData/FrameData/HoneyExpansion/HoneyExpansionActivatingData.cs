namespace Match3.Game.Gameplay.SubSystemsData.FrameData.HoneyExpansion
{
    public class HoneyExpansionActivatingData : BlackBoardData
    {
        public bool areThereAnyActivatingItems = false;

        public void Clear()
        {
            areThereAnyActivatingItems = false;
        }
    }
}