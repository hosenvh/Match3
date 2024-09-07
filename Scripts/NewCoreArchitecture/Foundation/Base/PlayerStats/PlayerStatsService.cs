using System;
using System.Collections.Generic;
using System.Linq;
using Match3.Foundation.Base.ServiceLocating;
using UnityEngine;



namespace Match3.Foundation.Base.PlayerStats
{


    [Serializable]
    public class PlaySession
    {
        public int number;
        public DateTime date;
        public Dictionary<string, int> intStats = new Dictionary<string, int>();
        public Dictionary<string, float> floatStats = new Dictionary<string, float>();
        public Dictionary<string, bool> boolStats = new Dictionary<string, bool>();

        public PlaySession(int number, DateTime date)
        {
            this.number = number;
            this.date = date;
        }
    }

    [Serializable]
    public class SavablePlaySession
    {
        public int number;
        public SavableDateTime date;
        
        public List<int> intStats = new List<int>();
        public List<string> intStatsKeys = new List<string>();
        
        public List<float> floatStats = new List<float>();
        public List<string> floatStatsKeys = new List<string>();
        
        public List<bool> boolStats = new List<bool>();
        public List<string> boolStatsKeys = new List<string>();

        public SavablePlaySession(int number, SavableDateTime date)
        {
            this.number = number;
            this.date = date;
        }
        
        public static implicit operator PlaySession(SavablePlaySession savablePlaySession)
        {
            var playSession = new PlaySession(savablePlaySession.number, savablePlaySession.date);
            
            for (var i = savablePlaySession.intStats.Count-1; i>=0; --i)
            {
                playSession.intStats.Add(savablePlaySession.intStatsKeys[i], savablePlaySession.intStats[i]);
            }
            
            for (var i = savablePlaySession.floatStats.Count-1; i>=0; --i)
            {
                playSession.floatStats.Add(savablePlaySession.floatStatsKeys[i], savablePlaySession.floatStats[i]);
            }

            for (var i = savablePlaySession.boolStats.Count-1; i>=0; --i)
            {
                playSession.boolStats.Add(savablePlaySession.boolStatsKeys[i], savablePlaySession.boolStats[i]);
            }
            
            return playSession;
        }

        public static implicit operator SavablePlaySession(PlaySession playSession)
        {
            var savablePlaySession = new SavablePlaySession(playSession.number, playSession.date)
            {
                intStats = playSession.intStats.Values.ToList(),
                intStatsKeys = playSession.intStats.Keys.ToList(),
                floatStats = playSession.floatStats.Values.ToList(),
                floatStatsKeys = playSession.floatStats.Keys.ToList(),
                boolStats = playSession.boolStats.Values.ToList(),
                boolStatsKeys = playSession.boolStats.Keys.ToList()
            };
            
            return savablePlaySession;
        }
    }
    
    
    [Serializable]
    public class SavableDateTime
    {
        public long value;
        
        public static implicit operator DateTime(SavableDateTime sdt) 
        {
            return DateTime.FromFileTimeUtc(sdt.value);
        }
        
        public static implicit operator SavableDateTime(DateTime dt) 
        {
            return new SavableDateTime {value = dt.ToFileTimeUtc()};
        }
    }
    
    
    public class PlayerStatsService : Service
    {

        private const string CURRENT_SESSION_KEY = "PlayerStats_CurrentSession";
        private const string SESSION_DATA_KEY = "PlayerStats_SessionData_";
        
        private PlaySession currentSession;
        private int currentSessionNumber = -1;


        public int CurrentSessionNumber
        {
            get
            {
                if(currentSessionNumber==-1) 
                    currentSessionNumber = PlayerPrefs.GetInt(CURRENT_SESSION_KEY, 0);
                return currentSessionNumber;
            }
            private set => currentSessionNumber = value;
        }


        public void OpenNewSession()
        {
            CurrentSessionNumber++;
            currentSession = new PlaySession(CurrentSessionNumber, DateTime.UtcNow);
        }


        public void RecordAdditiveStat(string statKey, int addAmount)
        {
            if (currentSession == null)
            {
                Debug.LogError("Please Open a Session Before Recording Stat");
                return;
            }
            
            if (currentSession.intStats.ContainsKey(statKey))
            {
                currentSession.intStats[statKey] += addAmount;
            }
            else
            {
                currentSession.intStats.Add(statKey, addAmount);
            }
        }
        
        
        public void SaveCurrentSession()
        {
            if (currentSession == null)
            {
                Debug.LogError("Please Open a Session Before Saving");
                return;
            }
            
            SavablePlaySession savablePlaySession = currentSession;
            var stringSession = JsonUtility.ToJson(savablePlaySession);
            PlayerPrefs.SetString(SESSION_DATA_KEY + CurrentSessionNumber, stringSession);
            PlayerPrefs.SetInt(CURRENT_SESSION_KEY, CurrentSessionNumber);
        }


        public int GetTotalStat(string statKey, int fromSession, int toSession)
        {
            if (fromSession > CurrentSessionNumber || toSession > CurrentSessionNumber || toSession < fromSession) return -1;

            var total = 0;
            var sessionNum = fromSession;
            var sessionsCount = toSession - fromSession + 1;
            
            for (var i = 0; i < sessionsCount; ++i)
            {
                PlaySession session = JsonUtility.FromJson<SavablePlaySession>(PlayerPrefs.GetString(SESSION_DATA_KEY + sessionNum, ""));
                sessionNum++;
                if (session.intStats.ContainsKey(statKey))
                    total += session.intStats[statKey];
            }

            return total;
        }
        
        
    }

}