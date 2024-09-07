using System;
using SeganX;


namespace Match3.Presentation.Map
{
    public class Popup_MapItemPreview : GameState
    {
        private Popup_TouchDisabler touchDisabler;
        
        void Start()
        {
            touchDisabler = gameManager.OpenPopup<Popup_TouchDisabler>();
        }

        private void OnDestroy()
        {
            gameManager.ClosePopup(touchDisabler);
        }
    }    
}


