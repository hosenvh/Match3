using System;
using System.Collections.Generic;
using System.Linq;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Presentation.TransitionEffects;
using UnityEngine;


namespace Match3.Game.SkipScenario
{
    public struct SkipScenarioStartedEvent : GameEvent {}
    public struct SkipScenarioFinishedEvent : GameEvent {}

    public class SkipScenarioController
    {
        private int requirePassedLevelIndex;
        private int requireStackedStar;
        private int skipOfferInterval;
        
    
        public void SetRequireStackedStar(int requireStar)
        {
            requireStackedStar = requireStar;
        }

        public void SetRequirePassedLevelIndex(int passedLevelIndex)
        {
            requirePassedLevelIndex = passedLevelIndex;
        }

        public void SetSkipOfferInterval(int skipInterval)
        {
            skipOfferInterval = skipInterval;
        }
        
        public bool IsProperTimeToSkipScenario()
        {
            if (Base.gameManager.profiler.StarCount >= requireStackedStar)
            {
                if (GetLastRejectedOfferLevel() > 0)
                    return Base.gameManager.profiler.LastUnlockedLevel >= GetLastRejectedOfferLevel() + skipOfferInterval;

                return Base.gameManager.profiler.LastUnlockedLevel >= requirePassedLevelIndex;
            }
            
            return false;
        }

        public void SkipScenarioWithStackedStars(Action onMapReload)
        {
            EventManager eventManager = ServiceLocator.Find<EventManager>();
            eventManager.Propagate(new SkipScenarioStartedEvent(), this);

            var gameManager = Base.gameManager;
            var taskManager = gameManager.taskManager;
            var availableStars = gameManager.profiler.StarCount;
            var lastMapId = gameManager.mapManager.CurrentMapId;
            
            while (availableStars > 0)
            {
                TaskConfig firstActiveTask = taskManager.CurrentTasksList.First();
                if (availableStars >= firstActiveTask.requiremnetStars)
                {
                    availableStars -= firstActiveTask.requiremnetStars;
                    taskManager.SetTaskDone(firstActiveTask);
                
                    List<ScenarioItem> scenarioItems = taskManager.GetScenarioItems(firstActiveTask);
                    gameManager.scenarioManager.SaveScenarioStates(scenarioItems);
                    
                    foreach (var scenarioItem in scenarioItems)
                    {
                        if (scenarioItem.scenarioType == ScenarioType.ChangeMap)
                            lastMapId = scenarioItem.string_0;
                    }
                }
                else
                    break;
            }

            gameManager.profiler.SetStarCount(availableStars);
            eventManager.Propagate(new SkipScenarioFinishedEvent(), this);
            
            ServiceLocator.Find<GameTransitionManager>().GoToMap<DarkInTransitionEffect>(lastMapId, false, onMapReload);
        }


        public void RejectSkipOffer()
        {
            SaveRejectedOfferLevel(Base.gameManager.profiler.LastUnlockedLevel);
        }


        private int GetLastRejectedOfferLevel()
        {
            return PlayerPrefs.GetInt("SkipScenarioRejectLevel", 0);
        }

        private void SaveRejectedOfferLevel(int level)
        {
            PlayerPrefs.SetInt("SkipScenarioRejectLevel", level);
        }
    
    }


}

