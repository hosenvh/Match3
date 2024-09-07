using Match3.Presentation.HUD;
using SeganX;
using UnityEngine;

public class Popup_FakeHud : GameState
{
    [SerializeField]
    private HudPresentationController hudPresentationController;
    public HudPresentationController GetHudPresentationController() => hudPresentationController;
}
