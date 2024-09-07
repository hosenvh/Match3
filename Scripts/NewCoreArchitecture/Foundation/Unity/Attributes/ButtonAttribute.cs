using System;
using UnityEngine;

namespace ArmanCo.ShapeRunner.Utility
{
    [Serializable]
    public class ButtonAction
    {
        public Action action;

        public ButtonAction(Action action)
        {
            this.action = action;
        }
    }

    public class ButtonAttribute : PropertyAttribute
    {
        public string name;
        public string callBackName;

        public ButtonAttribute(string name, string callBackName)
        {
            this.name = name;
            this.callBackName = callBackName;
        }
    }
}
