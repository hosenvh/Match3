
using System;
using System.Collections.Generic;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Newtonsoft.Json.Utilities;
using UnityEngine;


namespace Match3.Game.NewsSpace
{

    public class NewsManager : Service, EventListener
    {
        
        private News[] allNewses;
        private List<News> allValidNewses = new List<News>();
        private List<News> newValidNewses = new List<News>();
        private List<News> oldValidNewses = new List<News>();


        public News[] GetAllNewses => allNewses;
        public List<News> GetAllValidNewses => allValidNewses;
        public List<News> GetValidNewNewses => newValidNewses;
        public List<News> GetValidOldNewses => oldValidNewses;

        public bool HaveNewNews => newValidNewses.Count > 0;
        
        
        public NewsManager()
        {
            ServiceLocator.Find<EventManager>().Register(this);
        }

        
        public void Initialize(News[] newses)
        {
            var appVersion = float.Parse(Application.version);
            
            allNewses = newses;
            allValidNewses.Clear();
            
            foreach (var news in newses)
            {
                if(news.specificVersion.Equals(appVersion) || news.lessVersion>=appVersion || (news.specificVersion<=0 && news.lessVersion<=0)) 
                    allValidNewses.Add(news);
            }
            SeparateNewAndOldNewses(allValidNewses);
        }

        
        private void SeparateNewAndOldNewses(IEnumerable<News> newses)
        {
            newValidNewses.Clear();
            oldValidNewses.Clear();
            
            foreach (var news in newses)
            {
                if(IsNewsOld(news.id))
                    oldValidNewses.Add(news);
                else
                    newValidNewses.Add(news);
            }
            
            if(newValidNewses.Count>0)
                ServiceLocator.Find<EventManager>().Propagate(new NewNewsesReceivedEvent(), this);
        }


        private bool IsNewsOld(int id)
        {
            return PlayerPrefs.HasKey($"News_Id_{id}");
        }
        

        public void SetNewNewsesOld()
        {
            if(newValidNewses.Count<=0) return;
            
            SaveNewsesId(newValidNewses);
            var oldNews = new List<News>(newValidNewses);
            oldNews.AddRange(oldValidNewses);
            oldValidNewses = oldNews;
            newValidNewses.Clear();
            
            ServiceLocator.Find<EventManager>().Propagate(new NewsesGottenOldEvent(), this);
        }


        private void SaveNewsesId(IEnumerable<News> newses)
        {
            foreach (var news in newses)
            {
                PlayerPrefs.SetInt($"News_Id_{news.id}", news.id);
            }
        }
        

        public void OnEvent(GameEvent evt, object sender)
        {
            if (evt is ServerConfigEvent serverConfig)
            {
                Initialize(serverConfig.serverConfigData.config.newses);
            }
        }

    }

}