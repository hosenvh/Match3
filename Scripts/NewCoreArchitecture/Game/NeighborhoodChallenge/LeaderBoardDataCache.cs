using System;

namespace Match3.Game.NeighborhoodChallenge
{
    public class LeaderBoardDataCache 
    {
        public const long LIFE_TIME_SECONDS = 5 * 60;

        LeaderBoardData leaderBoardData = null;

        DateTime updateTime = DateTime.MinValue;

        public bool IsValid()
        {
            return leaderBoardData != null 
                && updateTime.AddSeconds(LIFE_TIME_SECONDS) > DateTime.UtcNow;
        }

        public void SetData(LeaderBoardData data)
        {
            updateTime = DateTime.UtcNow;
            this.leaderBoardData = data;
        }

        public LeaderBoardData GetData()
        {
            return leaderBoardData;
        }
    }
}