using System;
using UnityEngine;
using UnityEngine.Scripting;


namespace Match3.Development.Base.DevelopmentConsole
{
    public enum DevOptionColor
    {
        None, Red, Blue, Magenta, White, Cyan, Green, Yellow
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class DevOptionAttribute : PreserveAttribute
    {
        public readonly string commandName;
        public readonly bool ignore;
        public readonly bool shouldAutoClose;
        private readonly DevOptionColor color;

        public DevOptionAttribute(string commandName, bool ignore = false, bool shouldAutoClose = false, DevOptionColor color = DevOptionColor.None)
        {
            this.commandName = commandName;
            this.ignore = ignore;
            this.shouldAutoClose = shouldAutoClose;
            this.color = color;
        }

        public Color Color()
        {
            switch (color)
            {
                case DevOptionColor.None:
                    return UnityEngine.Color.clear;
                case DevOptionColor.Red:
                    return UnityEngine.Color.red;
                case DevOptionColor.Blue:
                    return UnityEngine.Color.blue;
                case DevOptionColor.Magenta:
                    return UnityEngine.Color.magenta;
                case DevOptionColor.White:
                    return UnityEngine.Color.white;
                case DevOptionColor.Cyan:
                    return UnityEngine.Color.cyan;
                case DevOptionColor.Green:
                    return UnityEngine.Color.green;
                case DevOptionColor.Yellow:
                    return UnityEngine.Color.yellow;
            }
            return UnityEngine.Color.clear;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DevOptionGroupAttribute : PreserveAttribute
    {
        public readonly string groupName;
        public readonly int priority;

        public DevOptionGroupAttribute(string groupName, int priority)
        {
            this.groupName = groupName;
            this.priority = priority;
        }
    }
}