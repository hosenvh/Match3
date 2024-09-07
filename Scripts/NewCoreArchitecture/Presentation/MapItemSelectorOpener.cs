
public class MapItemSelectorOpener
{

    public void OpenAndSetup(MapItem_UserSelect mapItem_UserSelect, bool showCancel, System.Action<int> onItemSelect, System.Action<bool> onFinish)
    {
        var mapItemSelector = Base.gameManager.GetPopup<Popup_MapItemSelector>();
        if (mapItemSelector != null)
            mapItemSelector.OnConfirmClick(false);
        Base.gameManager.CurrentPopup.As<Popup_MainMenu>().Hide();
        Base.gameManager.OpenPopup<Popup_MapItemSelector>().Setup(mapItem_UserSelect, showCancel, onItemSelect,
            result =>
            {
                Base.gameManager.CurrentPopup.As<Popup_MainMenu>().Show();
                onFinish(result);
            });
    }
    
}
