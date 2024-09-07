using I2.Loc;

namespace Match3
{
    public class Popup_ExitMessage : Popup_ConfirmBox
    {
        public void Setup(System.Action<bool> OnResult)
        {
            this.Setup(ScriptLocalization.Message.ExitGame, ScriptLocalization.UI_General.Yes, ScriptLocalization.UI_General.No, true, OnResult);
        }
    }
}