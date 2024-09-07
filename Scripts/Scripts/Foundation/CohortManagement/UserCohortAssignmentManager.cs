using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.ServiceLocating;
using System;
using System.Collections.Generic;
using Match3.Utility.GolmoradLogging;
using Match3.UserManagement.Foundation.Base;


namespace Match3.Foundation.Base.CohortManagement
{

    public interface ChorotConfiguration
    {
        string ID();
        void RegisterConfigurations(ConfigurationManager configurationManager);
    }

    // TODO: Refactor the cohort assignment flow.
    public class UserCohortAssignmentManager : Service
    {
        const string COHORT_ID_KEY = "CohortID";
        Action onAssignmentDoneCallback;
        Dictionary<string, ChorotConfiguration> allCohortsConfigSuites = new Dictionary<string, ChorotConfiguration>();
        List<string> availableOfflineCohortsConfigSuiteVersions = new List<string>();

        CohortManagementServerCommunicator communicationManager;

        string userAssignedCohortID = null;

        public UserCohortAssignmentManager(CohortManagementServerCommunicator communicationManager)
        {
            this.communicationManager = communicationManager;
            ServiceLocator.Find<ConfigurationManager>().Configure(this);
            Load();
        }

        public string AssignedCohortID()
        {
            return userAssignedCohortID;
        }

        public void SetAssignedCohortID(string id)
        {
            this.userAssignedCohortID = id;
            Save();
        }

        public void AddCohortConfigSuite(ChorotConfiguration cohortSuite)
        {
            allCohortsConfigSuites.Add(cohortSuite.ID(), cohortSuite);
        }

        public void AddAvailableOfflineCohortConfigSuite(string ID)
        {
            availableOfflineCohortsConfigSuiteVersions.Add(ID);
        }

        public void ManageUserCohort(Action onAssignmentDone)
        {
            this.onAssignmentDoneCallback = onAssignmentDone;

            if (NoCohortIsAssigned())
                TryRequestFromServer();
            else
                ActivateAssignedCohort();
            
        }

        public void OverrideCohort(string cohortID)
        {
            SetAssignedCohortID(cohortID);
            ActivateAssignedCohort();
        }

        private void ActivateAssignedCohort()
        {
            if (DoesCohortExists(AssignedCohortID()) == false)
            {
                DebugPro.LogError<UserLogTag>("Assigning an invalid Cohort : " + AssignedCohortID());
                AssignAndLoadCohortLocally();
            }
            else
                LoadCohortConfiguration();
        }

        private void LoadCohortConfiguration()
        {
            DebugPro.LogInfo<UserLogTag>("Cohort | Loading Configuration for Cohort " + userAssignedCohortID);
            var configSuite = allCohortsConfigSuites[userAssignedCohortID];

            var configManager = ServiceLocator.Find<ConfigurationManager>();

            configSuite.RegisterConfigurations(configManager);

            communicationManager.SendUserCohort(
                userAssignedCohortID,
                onSuccess: onAssignmentDoneCallback.Invoke,
                onFailure: onAssignmentDoneCallback.Invoke);

        }

        private void TryRequestFromServer()
        {
            communicationManager.RequestUserCohort(OnRequestSucceeded, OnRequestFailed);
        }

        private void OnRequestSucceeded(string cohortID)
        {
            if(DoesCohortExists(cohortID))
                AssignAndLoadCohort(cohortID);
            else
                AssignAndLoadCohortLocally();
        }

        private bool DoesCohortExists(string cohortID)
        {
            return allCohortsConfigSuites.ContainsKey(cohortID);
        }

        private void OnRequestFailed()
        {
            AssignAndLoadCohortLocally();
        }

        private void AssignAndLoadCohortLocally()
        {
            DebugPro.LogInfo<UserLogTag>("Assigning Cohort locally");
            var cohortID = availableOfflineCohortsConfigSuiteVersions.RandomOne();
            AssignAndLoadCohort(cohortID);
        }

        private void AssignAndLoadCohort(string cohortID)
        {
            SetAssignedCohortID(cohortID);
            DebugPro.LogInfo<UserLogTag>("Assigned Cohort : " + cohortID);
            LoadCohortConfiguration();
        }

        private bool NoCohortIsAssigned()
        {
            return userAssignedCohortID.IsNullOrEmpty();
        }


        public void ClearCohort()
        {
            UnityEngine.PlayerPrefs.DeleteKey(COHORT_ID_KEY);
        }

        // TODO: Move this out of here.
        public void Save()
        {
            UnityEngine.PlayerPrefs.SetString(COHORT_ID_KEY, AssignedCohortID());
        }

        public void Load()
        {
            if (UnityEngine.PlayerPrefs.HasKey(COHORT_ID_KEY))
                SetAssignedCohortID(UnityEngine.PlayerPrefs.GetString(COHORT_ID_KEY));
        }

        public List<ChorotConfiguration> AllCohorts()
        {
            return new List<ChorotConfiguration>(allCohortsConfigSuites.Values);
        }

        public void ResetCohortTo(string cohortID)
        {
            communicationManager.RequestResetCohort(cohortID);
            SetAssignedCohortID(cohortID);
        }
    }
}