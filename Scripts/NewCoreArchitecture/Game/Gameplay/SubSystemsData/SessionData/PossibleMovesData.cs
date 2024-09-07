
namespace Match3.Game.Gameplay.SubSystemsData.SessionData.General
{
    public class PossibleMovesData : BlackBoardData
    {
        public bool isPossibleMoveAvailable; 
        public void Clear()
        {
            isPossibleMoveAvailable = false;
        }
    }
}