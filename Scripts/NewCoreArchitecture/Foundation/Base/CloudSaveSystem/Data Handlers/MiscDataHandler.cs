using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.FaultyBehaviourDetection;
using Match3.LuckySpinner.TimeBased.Game;
using Match3.Overlay.Advertisement.Placements.Implementations.MainMenuAdPlacement;
using Match3.Overlay.Advertisement.Placements.Implementations.TaskPopupAdPlacement;


namespace Match3.CloudSave
{
    public class MiscDataHandler : ICloudDataHandler
    {


        private const string MENU_AD_TIME_FROM_LAST_SAVE_KEY = "MenuAdTimerKey";
        private const string TASK_MENU_AD_TIME_FROM_LAST_SAVE_KEY = "taskMenuAdTimerKey";
        private const string LUCKY_SPINNER_TIME_FROM_LAST_SAVE_KEY = "LuckySpinnerTimerKey";

    
    public void CollectData(ICloudDataStorage cloudStorage)
    {
        var dataManager = ServiceLocator.Find<IDataManager>();
        cloudStorage.SetString(MENU_AD_TIME_FROM_LAST_SAVE_KEY, dataManager.GetSavedTimeForKey(MainMenuAdPlacement.SERVER_TIME_SAVE_KEY).ToString());
        cloudStorage.SetString(TASK_MENU_AD_TIME_FROM_LAST_SAVE_KEY, dataManager.GetSavedTimeForKey(TaskPopupAdPlacement.SERVER_TIME_SAVE_KEY).ToString());
        cloudStorage.SetString(LUCKY_SPINNER_TIME_FROM_LAST_SAVE_KEY, dataManager.GetSavedTimeForKey(TimeBasedLuckySpinnerHandler.TIMER_BASED_LUCKY_SPINNER_LAST_USED_TIME_KEY).ToString());
        cloudStorage.SetInt(FaultyBehaviourDetectionService.FAULTY_BEHAVIOUR_DETECTED_KEY , ServiceLocator.Find<FaultyBehaviourDetectionService>().IsFaultyBehaviourDetected ? 1 : 0);
        cloudStorage.SetInt(FaultyBehaviourDetectionService.FAULTY_BEHAVIOUR_DETECTION_MODE , (int) ServiceLocator.Find<FaultyBehaviourDetectionService>().DetectionMode);
    }

    
    
    public void SpreadData(ICloudDataStorage cloudStorage)
    {
        var dataManager = ServiceLocator.Find<IDataManager>();

        dataManager.SetSavedTimeForKey(MainMenuAdPlacement.SERVER_TIME_SAVE_KEY, cloudStorage.GetString(MENU_AD_TIME_FROM_LAST_SAVE_KEY, "-1"));
        dataManager.SetSavedTimeForKey(TaskPopupAdPlacement.SERVER_TIME_SAVE_KEY, cloudStorage.GetString(TASK_MENU_AD_TIME_FROM_LAST_SAVE_KEY, "-1"));
        dataManager.SetSavedTimeForKey(TimeBasedLuckySpinnerHandler.TIMER_BASED_LUCKY_SPINNER_LAST_USED_TIME_KEY, cloudStorage.GetString(LUCKY_SPINNER_TIME_FROM_LAST_SAVE_KEY, "-1"));
        ServiceLocator.Find<FaultyBehaviourDetectionService>().SetIsFaultyBehaviourDetected(cloudStorage.GetInt(FaultyBehaviourDetectionService.FAULTY_BEHAVIOUR_DETECTED_KEY, defaultValue: 0) == 1);
        ServiceLocator.Find<FaultyBehaviourDetectionService>().SetDetectionMode((FaultyBehaviourDetectionMode) cloudStorage.GetInt(FaultyBehaviourDetectionService.FAULTY_BEHAVIOUR_DETECTION_MODE, defaultValue: 0));
    }
    
    
    }
    
}

