using System;
using UnityEngine;


namespace Match3.Development.Base.DevelopmentConsole
{
        public class ShortCutInfo
        {
            public KeyCode[] keyCodes;
            public Action action;

            public ShortCutInfo(KeyCode[] keyCodes, Action action)
            {
                this.keyCodes = keyCodes;
                this.action = action;
            }
        }
}