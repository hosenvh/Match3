
using System;
using System.Collections.Generic;

namespace Match3.Game.Gameplay.SubSystemsData.SessionData
{
    public class InputControlData : BlackBoardData
    {
        HashSet<Type> inputLocks = new HashSet<Type>();

        public void AddLockedBy<T>() where T : KeyType
        {
            inputLocks.Add(typeof(T));
        }

        public void RemoveLockedBy<T>() where T : KeyType
        {
            inputLocks.Remove(typeof(T));
        }

        public bool IsLocked()
        {
            return inputLocks.Count > 0;
        }

        public void Clear()
        {
            inputLocks.Clear();
        }

        public bool IsLockedBy<T>()
        {
            return inputLocks.Contains(typeof(T));
        }

        public HashSet<Type> CurrentLocks()
        {
            return inputLocks;
        }
    }
}