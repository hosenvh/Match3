using System.Linq;
using UnityEngine;
using UnityEngine.UI;


namespace Match3.Development.Base.DevelopmentConsole
{
    public class DevelopmentCommand : MonoBehaviour
    {
        public Text commandNameText;
        public Text shortcutText;
        public RawImage colorBand;

        CommandInfo commandInfo;

        internal void Init(CommandInfo commandInfo)
        {
            this.commandInfo = commandInfo;
            commandNameText.text = commandInfo.name;
            shortcutText.text = string.Join("+", commandInfo.keyCodes.Select(k => k.ToString()));
            colorBand.color = commandInfo.color;
        }

        public void Execute()
        {
            if (commandInfo.HasNoInput())
                commandInfo.Invoke();
            else
                FindObjectOfType<DevelopmentToolManager>().OpenCommandInputPromptFor(commandInfo);
        }
    }
}