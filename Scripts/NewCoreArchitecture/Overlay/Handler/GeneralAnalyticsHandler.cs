using Match3.Foundation.Base;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.FaultyBehaviourDetection;
using System;

namespace Match3.Overlay.Analytics
{
    public class GeneralAnalyticsHandler : AnalyticsHandler
    {
        private const string FAULTY_BEHAVIOUR_DETECTION_EVENT_SENT = "FaultyBehaviourDetectionEventSent";
        private const string STORE_SWITCH_EVENT_SENT = "StoreSwitchEventSent";

        private readonly UserProfileManager userProfileManager;

        private bool FaultyBehaviourDetectionEventSent
        {
            get => userProfileManager.LoadData(FAULTY_BEHAVIOUR_DETECTION_EVENT_SENT, 0) != 0;
            set => userProfileManager.SaveData(FAULTY_BEHAVIOUR_DETECTION_EVENT_SENT, value ? 1 : 0);
        }

        private bool StoreSwitchEventSent
        {
            get => userProfileManager.LoadData(STORE_SWITCH_EVENT_SENT, 0) != 0;
            set => userProfileManager.SaveData(STORE_SWITCH_EVENT_SENT, value ? 1 : 0);
        }

        public GeneralAnalyticsHandler(UserProfileManager profile)
        {
            userProfileManager = profile;

            var faultyBehaviourDetectionService = ServiceLocator.Find<FaultyBehaviourDetectionService>();
            AnalyticsManager.faultyBehaviourDetectionService = faultyBehaviourDetectionService;
            if (faultyBehaviourDetectionService.IsFaultyBehaviourDetected)
                HandleFaultyBehaviorDetectionEvent(faultyBehaviourDetectionService.DetectionMode);

            if (faultyBehaviourDetectionService.IsTimeCheatDetected)
                HandleTimeCheatDetectionEvent();

            new AnalyticsStoreTracker(ServiceLocator.Find<StoreFunctionalityManager>()).
                TrackStore(onStoreIsChangedAction: HandleStoreSwitchEvent);
        }

        private void HandleFaultyBehaviorDetectionEvent(FaultyBehaviourDetectionMode detectionMode)
        {
            if (!FaultyBehaviourDetectionEventSent)
            {
                FaultyBehaviourDetectionEventSent = true;
                AnalyticsManager.SendEvent(new AnalyticsData_Flag_FaultyBehaviourDetected(detectionMode.ToString()));
            }
        }

        private void HandleTimeCheatDetectionEvent()
        {
            AnalyticsManager.SendEvent(new AnalyticsData_TimeCheatDetected());
        }

        private void HandleStoreSwitchEvent(string previousStoreName)
        {
            if (!StoreSwitchEventSent)
            {
                StoreSwitchEventSent = true;
                AnalyticsManager.SendEvent(new AnalyticsData_Flag_StoreSwitched(previousStoreName));
            }
        }

        protected override void Handle(GameEvent evt)
        {
            switch (evt)
            {
                case FaultyBehaviourDetectionEvent data:
                    HandleFaultyBehaviorDetectionEvent(data.detectionMode);
                    break;
                case TimeCheatDetectionEvent _:
                    HandleTimeCheatDetectionEvent();
                    break;
            }
        }
    }
}