
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Match3.Presentation
{
    public class UIDragabler : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
    {

        public void OnBeginDrag(PointerEventData eventData)
        {
            Button button = this.transform.GetComponent<Button>();
            if (button != null) button.enabled = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            this.transform.position = eventData.position;
            eventData.Use();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Button button = this.transform.GetComponent<Button>();
            if (button != null) button.enabled = true; ;
        }

    }
}