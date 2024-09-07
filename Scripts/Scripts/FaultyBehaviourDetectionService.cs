using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using UnityEngine;


namespace Match3.Game.FaultyBehaviourDetection
{
    public class PurchaseVerifiedBehaviourEvent : GameEvent
    {
    }

    public class PurchaseFaultyBehaviourEvent : GameEvent
    {
    }

    public class FaultyBehaviourDetectionEvent : GameEvent
    {
        public readonly FaultyBehaviourDetectionMode detectionMode;

        public FaultyBehaviourDetectionEvent(FaultyBehaviourDetectionMode detectionMode)
        {
            this.detectionMode = detectionMode;
        }
    }

    public class TimeCheatDetectionEvent : GameEvent
    {
    }

    public enum FaultyBehaviourDetectionMode
    {
        None = 0,
        InitialExtraResources = 1,
        NotLogicalResourcesAmount = 2,
        PurchaseVerification = 3
    }

    public class FaultyBehaviourDetectionService : Service, EventListener
    {
        public const string FAULTY_BEHAVIOUR_DETECTED_KEY = "Faulty_Behaviour_Detected_V4";
        public const string FAULTY_BEHAVIOUR_DETECTION_MODE = "Faulty_Behaviour_Detection_Mode_V4";
        private const int SERVER_TIME_MAX_DIFFERENCE = 5400; // 1.5 hours

        private readonly EventManager eventManager;
        private readonly UserProfileManager profileManager;

        public bool IsFaultyBehaviourDetected
        {
            get => profileManager.LoadData(FAULTY_BEHAVIOUR_DETECTED_KEY, defaultValue: 0) != 0;
            private set => profileManager.SaveData(FAULTY_BEHAVIOUR_DETECTED_KEY, value ? 1 : 0);
        }

        public FaultyBehaviourDetectionMode DetectionMode
        {
            get => (FaultyBehaviourDetectionMode) profileManager.LoadData(key: FAULTY_BEHAVIOUR_DETECTION_MODE, defaultValue: 0);
            private set => profileManager.SaveData(key: FAULTY_BEHAVIOUR_DETECTION_MODE, (int) value);
        }

        public bool IsTimeCheatDetected { get; private set; }

        public FaultyBehaviourDetectionService(
            EventManager eventManager,
            UserProfileManager profileManager,
            ITimeManager timeManager,
            GameProfiler gameProfiler,
            bool isFirstSession)
        {
            this.eventManager = eventManager;
            this.profileManager = profileManager;

            eventManager.Register(this);

            if (IsFaultyBehaviourDetected)
            {
                eventManager.Propagate(new FaultyBehaviourDetectionEvent(DetectionMode), this);
                return;
            }

            if (IsServerTimeDifferenceMoreThanLogical(timeManager))
                HandleTimeCheatDetected();

            if (isFirstSession && CheckFirstSessionResources(gameProfiler))
                FaultyBehaviourDetected(FaultyBehaviourDetectionMode.InitialExtraResources);
            else if (CheckResourcesTotalAmount(gameProfiler))
                FaultyBehaviourDetected(FaultyBehaviourDetectionMode.NotLogicalResourcesAmount);
        }

        public void OnEvent(GameEvent evt, object sender)
        {
            if (evt is PurchaseVerifiedBehaviourEvent)
                TryResetFaultyBehaviorAfterVerifiedPurchase();
            if (evt is PurchaseFaultyBehaviourEvent && ShouldCheckPurchaseFaultyBehavior())
                FaultyBehaviourDetected(FaultyBehaviourDetectionMode.PurchaseVerification);
        }

        private void TryResetFaultyBehaviorAfterVerifiedPurchase()
        {
            if (IsFaultyBehaviourDetected && DetectionMode == FaultyBehaviourDetectionMode.PurchaseVerification)
                ResetFaultyBehaviorDetection();

            void ResetFaultyBehaviorDetection()
            {
                IsFaultyBehaviourDetected = false;
                SetDetectionMode(FaultyBehaviourDetectionMode.None);
            }
        }

        private void FaultyBehaviourDetected(FaultyBehaviourDetectionMode mode)
        {
            if (IsFaultyBehaviourDetected)
                return;
            SetDetectionMode(mode);
            IsFaultyBehaviourDetected = true;

            eventManager.Propagate(new FaultyBehaviourDetectionEvent(DetectionMode), this);
        }

        // NOTE: For now time cheating is not consider a faulty behavior.
        private void HandleTimeCheatDetected()
        {
            IsTimeCheatDetected = true;
            eventManager.Propagate(new TimeCheatDetectionEvent(), this);
        }

        private bool IsServerTimeDifferenceMoreThanLogical(ITimeManager timeManager)
        {
            return Mathf.Abs(timeManager.ServerTimeDifference) > SERVER_TIME_MAX_DIFFERENCE;
        }

        private bool CheckFirstSessionResources(GameProfiler profile)
        {
            return profile.LastUnlockedLevel != 0 || profile.CoinCount > 2000 || profile.StarCount > 2 || profile.KeyCount > 2 || profile.LifeCount > 5;
        }

        private bool CheckResourcesTotalAmount(GameProfiler profile)
        {
            return profile.CoinCount >= 10000000 || profile.StarCount >= 10000 || profile.KeyCount >= 10000 || profile.LifeCount >= 50 * 3600;
        }

        private bool ShouldCheckPurchaseFaultyBehavior()
        {
            var configuration = ServiceLocator.Find<ServerConfigManager>().data;
            return configuration == null || configuration.config.faultyBehaviourDetection == null || configuration.config.faultyBehaviourDetection.checkPaymentFaultyBehaviour;
        }

        public void SetIsFaultyBehaviourDetected(bool isFaultyBehaviourDetected)
        {
            IsFaultyBehaviourDetected = isFaultyBehaviourDetected;
        }

        public void SetDetectionMode(FaultyBehaviourDetectionMode detectionMode)
        {
            DetectionMode = detectionMode;
        }
    }
}