using System;
using System.Collections.Generic;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.NewsSpace;
using UnityEngine;
using UnityEngine.UI;


namespace Match3.Presentation.NewsSpace
{
    public class NewsBoxController : MonoBehaviour
    {
        public GameObject newsEmptyDescriptionObject;
        public Transform newsContainer;
        public NewsPresenterController newsPresenterPrefab;
        public Sprite[] newsIcons;
        
        public RectTransform layoutRectTransform;


        private bool opened = false;


        public void OpenNewsBox(bool forceRefresh = false)
        {
            if (opened && !forceRefresh) return;

            var newsManager = ServiceLocator.Find<NewsManager>();

            var newNewses = newsManager.GetValidNewNewses;
            var oldNewses = newsManager.GetValidOldNewses;

            if (DoesNotAnyNewsExist())
            {
                newsEmptyDescriptionObject.SetActive(true);
                return;
            }

            FillNewsBox(newNewses, oldNewses);

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRectTransform);

            newsManager.SetNewNewsesOld();

            opened = true;

            bool DoesNotAnyNewsExist() => oldNewses.Count == 0 && newNewses.Count == 0;
        }


        private void FillNewsBox(IEnumerable<News> newNewses, IEnumerable<News> oldNewses)
        {
            foreach (var news in newNewses)
            {
                var newsP = Instantiate(newsPresenterPrefab, newsContainer);
                newsP.Setup(news.title, news.description, newsIcons[news.iconIndex], true, news.link);
            }

            foreach (var news in oldNewses)
            {
                var newsP = Instantiate(newsPresenterPrefab, newsContainer);
                newsP.Setup(news.title, news.description, newsIcons[news.iconIndex], false, news.link);
            }
        }
    }
}