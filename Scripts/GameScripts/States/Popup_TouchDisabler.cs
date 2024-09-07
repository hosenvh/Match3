using SeganX;
#pragma warning disable 0108

public class Popup_TouchDisabler : GameState
{
    #region methods
    public override void Back()
    {
    }

    public void Close()
    {
        base.Back();
    }
    #endregion
}