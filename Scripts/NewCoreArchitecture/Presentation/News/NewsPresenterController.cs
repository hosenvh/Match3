using System.Collections;
using System.Collections.Generic;
using SeganX;
using UnityEngine;
using UnityEngine.UI;



namespace Match3.Presentation.NewsSpace
{

    public class NewsPresenterController : MonoBehaviour
    {

        public GameObject newTag;
        public LocalText titleText;
        public LocalText descriptionText;
        public Image iconImage;
        public Button button;
        public RectTransform layoutRectTransform;
        private string link;



        public void Setup(string title, string description, Sprite icon, bool isNew, string link = "")
        {
            if (string.IsNullOrEmpty(title))
                titleText.gameObject.SetActive(false);
            else
            {
                titleText.gameObject.SetActive(true);
                titleText.SetText(title);
            }

            if (string.IsNullOrEmpty(description))
                descriptionText.gameObject.SetActive(false);
            else
            {
                descriptionText.gameObject.SetActive(true);
                descriptionText.SetText(description);
            }

            iconImage.sprite = icon;

            newTag.SetActive(isNew);
            
            this.link = link;
            button.interactable = !string.IsNullOrEmpty(link);
            
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRectTransform);
        }


        public void OpenLink()
        {
            Application.OpenURL(link);
        }
        
    }

    
}