﻿using System;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.NeighborhoodChallenge;
using UnityEngine;



namespace Match3.Presentation.MainMenu
{

    public class ActiveNCMenuButton : MainMenuButton, EventListener
    {
        public override Transform Create(Transform buttonParent, Transform buttonControllerParent)
        {
            button.SetParent(buttonParent);
            
            var neighborhoodChallengeManager = ServiceLocator.Find<NeighborhoodChallengeManager>();
            button.gameObject.SetActive(neighborhoodChallengeManager.IsEnabled() && neighborhoodChallengeManager.IsUnlocked());
            
            ServiceLocator.Find<EventManager>().Register(this);
            
            return button;
        }

        public override bool CreationCondition()
        {
            return true;
        }

        public override MainMenuButtonSetting GetSetting()
        {
            return MainMenuButtonsSettings.EnabledNeighbourhoodChallenge;
        }

        private void OnDisable()
        {
            ServiceLocator.Find<EventManager>().UnRegister(this);
        }
        
        public void EnterNeighborHoodChallenge()
        {
            var NCManager = ServiceLocator.Find<NeighborhoodChallengeManager>();
            NCManager.GetController<NCLobbyEnteringController>().EnterLobby();
        }
        
        public void OnEvent(GameEvent evt, object sender)
        {
            if (evt is NeighborhoodChallengeUnlockedEvent)
                button.gameObject.SetActive(true);
        }
    }

}