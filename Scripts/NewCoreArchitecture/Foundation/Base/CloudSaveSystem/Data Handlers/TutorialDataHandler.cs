using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.NeighborhoodChallenge;
using UnityEngine;


namespace Match3.CloudSave
{

    public class TutorialDataHandler : ICloudDataHandler
    {

        private const string TutorialMapStartKey = "tutorialMapStart";
        private const string TutorialMapTasksKey = "tutorialMapTask";
        private const string TutorialMapItemsKey = "tutorialMapItem";
        private const string TutorialMapItemsOkayKey = "tutorialMapItemOkay";
        private const string TutorialPlayLevelKey = "tutorialPlayLevel";
        private const string TutorialMapItemUserSelectKey = "tutorialMapItemUserSelect";
        private const string TutorialSpinnerKey = "tutorialSpinner";

        private const string TutorialNeighborhoodChallenge = "tutorialNeghborhoodChallenge_";
        
        public void CollectData(ICloudDataStorage cloudStorage)
        {
            var tutorialManager = Base.gameManager.tutorialManager;
            
            cloudStorage.SetInt(TutorialMapStartKey, tutorialManager.IsTutorialShowed(0) ? 1:0);
            cloudStorage.SetInt(TutorialMapTasksKey, tutorialManager.IsTutorialShowed(1) ? 1:0);
            cloudStorage.SetInt(TutorialMapItemsKey, tutorialManager.IsTutorialShowed(2) ? 1:0);
            cloudStorage.SetInt(TutorialMapItemsOkayKey, tutorialManager.IsTutorialShowed(3) ? 1:0);
            cloudStorage.SetInt(TutorialPlayLevelKey, tutorialManager.IsTutorialShowed(4) ? 1:0);
            cloudStorage.SetInt(TutorialMapItemUserSelectKey, tutorialManager.IsTutorialShowed(22) ? 1:0);
            cloudStorage.SetInt(TutorialSpinnerKey, tutorialManager.IsTutorialShowed(27) ? 1:0);


            var ncTutorialManager = ServiceLocator.Find<NeighborhoodChallengeTutorialManager>();
            var ncTutorialSequence = ncTutorialManager.GetTutorialSequence();
            for (var i = 0; i<ncTutorialSequence.Count; ++i)
            {
                cloudStorage.SetInt(TutorialNeighborhoodChallenge + i, tutorialManager.IsTutorialShowed(ncTutorialSequence[i]) ? 1:0);
            }

        }

        public void SpreadData(ICloudDataStorage cloudStorage)
        {
            var tutorialManager = Base.gameManager.tutorialManager;
            
            tutorialManager.SetTutorialState(0, cloudStorage.GetInt(TutorialMapStartKey));
            tutorialManager.SetTutorialState(1, cloudStorage.GetInt(TutorialMapTasksKey));
            tutorialManager.SetTutorialState(2, cloudStorage.GetInt(TutorialMapItemsKey));
            tutorialManager.SetTutorialState(3, cloudStorage.GetInt(TutorialMapItemsOkayKey));
            tutorialManager.SetTutorialState(4, cloudStorage.GetInt(TutorialPlayLevelKey));
            tutorialManager.SetTutorialState(22, cloudStorage.GetInt(TutorialMapItemUserSelectKey));
            tutorialManager.SetTutorialState(27, cloudStorage.GetInt(TutorialSpinnerKey));
            
            var ncTutorialManager = ServiceLocator.Find<NeighborhoodChallengeTutorialManager>();
            var ncTutorialSequence = ncTutorialManager.GetTutorialSequence();
            for (var i = 0; i<ncTutorialSequence.Count; ++i)
            {
                tutorialManager.SetTutorialState(ncTutorialSequence[i], cloudStorage.GetInt(TutorialNeighborhoodChallenge + i, 0));
            }
        }
        
    }

}