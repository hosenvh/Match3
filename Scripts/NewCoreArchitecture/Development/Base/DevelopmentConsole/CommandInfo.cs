using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


namespace Match3.Development.Base.DevelopmentConsole
{
    public class CommandGroupInfo
    {
        public readonly string name;
        public int priority;
        public List<CommandInfo> commands;

        public CommandGroupInfo(string name, int priority, List<CommandInfo> commands)
        {
            this.name = name;
            this.priority = priority;
            this.commands = commands;
        }
    }

    public class CommandInfo
    {
        public readonly string name;
        public MethodInfo methodInfo;
        public KeyCode[] keyCodes = new KeyCode[0];
        public bool shouldAutoClose;
        public Color color;

        Action<CommandInfo> onExecuted;

        public CommandInfo(string name, MethodInfo methodInfo, bool shouldAutoClose, Color color, Action<CommandInfo> onExecuted)
        {
            this.name = name;
            this.methodInfo = methodInfo;
            this.shouldAutoClose = shouldAutoClose;
            this.color = color;
            this.onExecuted = onExecuted;
        }

        public void Invoke()
        {
            methodInfo.Invoke(null, null);
            onExecuted.Invoke(this);
        }

        public void Invoke(object[] inputs)
        {
            methodInfo.Invoke(null, inputs);
            onExecuted.Invoke(this);
        }

        public bool HasNoInput()
        {
            return methodInfo.GetParameters().Length == 0;
        }

        public void SetShortcut(KeyCode[] keyCodes)
        {
            this.keyCodes = keyCodes;
        }
    }
}