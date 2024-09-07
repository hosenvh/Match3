using Match3.Presentation.TextAdapting;
using SeganX;
using UnityEngine;


public class Popup_VideoBlackUnderlay : GameState
{
    public Canvas renderCanvas;

    public Popup_VideoBlackUnderlay Setup(Camera camera)
    {
        renderCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        renderCanvas.worldCamera = camera;
        return this;
    }
}
