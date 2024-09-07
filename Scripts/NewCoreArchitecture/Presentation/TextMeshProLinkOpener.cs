using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Match3.Presentation
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextMeshProLinkOpener : MonoBehaviour, IPointerClickHandler
    {
        Camera targetCamera;
        TextMeshProUGUI textMeshPro;

        public void Awake()
        {
            targetCamera = Camera.main;
            textMeshPro = this.GetComponent<TextMeshProUGUI>();
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(textMeshPro, Input.mousePosition, targetCamera);
            if (linkIndex != -1)
            { // was a link clicked?
                TMP_LinkInfo linkInfo = textMeshPro.textInfo.linkInfo[linkIndex];

                // open the link id as a url, which is the metadata we added in the text field
                Application.OpenURL(linkInfo.GetLinkID());
            }
        }
    }
}