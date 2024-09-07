using UnityEngine;
using UnityEngine.UI;


public class GameVersionPrinter : MonoBehaviour
{
    public Text versionText;

    private void Start()
    {
        versionText.text = Application.version.ToString();
    }
}
