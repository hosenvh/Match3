using System;
using UnityEngine;


namespace Match3.Development.Base.DevelopmentConsole
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ShortCutAttribute : Attribute
    {
        public readonly KeyCode[] keyCodes;

        public ShortCutAttribute(params KeyCode[] keyCodes)
        {
            this.keyCodes = keyCodes;
        }
    }

   
}