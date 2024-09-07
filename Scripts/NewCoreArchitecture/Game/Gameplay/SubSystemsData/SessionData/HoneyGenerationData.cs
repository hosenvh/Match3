

namespace Match3.Game.Gameplay.SubSystemsData.SessionData
{
    public class HoneyGenerationData : BlackBoardData
    {
        public int honeyTilesBeingGenerated;

        public void Clear()
        {
            honeyTilesBeingGenerated = 0;
        }
    }
}