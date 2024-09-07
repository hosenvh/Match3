using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A console to display Unity's debug logs in-game.
/// </summary>
public class MyConsoleGUI : MonoBehaviour
{
    MyConsole myConsole;
    const int margin = 20;
    Rect windowRect = new Rect(margin, margin, .8f * Screen.width - (margin * 2), .8f * Screen.height - (margin * 2));
    void Awake()
    {
        myConsole = GetComponent<MyConsole>();
    }
    void OnGUI()
    {
        windowRect = GUILayout.Window(123456, windowRect, myConsole.ConsoleWindow, "Console");
    }
}