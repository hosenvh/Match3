using System;
using Match3.Development.Base;
using Match3.Development.Base.DevelopmentConsole;
using Match3.Foundation.Base.CohortManagement;
using Match3.Foundation.Base.ServiceLocating;
using UnityEngine;
using static Base;


namespace Match3.Development.DevOptions
{
    [DevOptionGroup(groupName: "User", priority: 1)]
    public class UserManagementDevOptions : DevelopmentOptionsDefinition
    {
        private static UserProfileManager UserProfileManager => Application.isPlaying ? ServiceLocator.Find<UserProfileManager>() : new UserProfileManager();

        [DevOption(commandName: "Auto Change UserId", color: DevOptionColor.Red)]
        public static void ChangeUserIdRandomly()
        {
            SetUserId(id: GenerateNewUserIdBasedOnTime());

            string GenerateNewUserIdBasedOnTime() => $"Debug_Golmorad_{DateTime.Now.Ticks - new DateTime(2000, 1, 1).Ticks}";
        }

        [DevOption(commandName: "Reset To Original UserId")]
        public static void ResetUserIdToOriginal()
        {
            UserProfileManager.FetchUserIdBasedOnDevice(onComplete: SetUserId);
        }

        [DevOption(commandName: "Set UserId")]
        public static void SetUserId(string id)
        {
            UserProfileManager.SetGlobalUserID(id);
            if (Application.isPlaying)
                ServerDevOptions.ForceUpdateServerConfig();
            LogUserIds();
        }

        [DevOption(commandName: "Log UserIds")]
        public static void LogUserIds()
        {
            UserProfileManager.FetchUserIdBasedOnDevice(
                onComplete: originalUserId =>
                    Log(originalUserId, currentUserId: UserProfileManager.GlobalUserId));

            void Log(string originalUserId, string currentUserId)
            {
                LogInfo(message: $"Current UserId: {currentUserId}");
                LogInfo(message: $"Original UserId: {originalUserId}");
                LogInfo(message: $"Original And Current UserId Equality: {originalUserId == currentUserId}");
            }
        }

        [DevOption(commandName: "Log DeviceId")]
        public static void LogDeviceId()
        {
            LogInfo(message: $"Device id: {SystemInfo.deviceUniqueIdentifier} ");
        }

        private static void LogInfo(string message)
        {
            Debug.Log($"UserState | {message}");
        }

        [DevOption(commandName: "Set As Paying User")]
        public static void SetAsPayingUser()
        {
            if (gameManager.profiler.PurchaseCount <= 0)
                gameManager.profiler.PurchaseCount = 1;
        }

        [DevOption(commandName: "Set As NonPaying User")]
        public static void SetAsNonPayingUser()
        {
            gameManager.profiler.PurchaseCount = 0;
        }

        [DevOption(commandName: "Set Cohrot + Change UserID")]
        public static void SetCohort(string id)
        {
            ChangeCohort();
            ConvertUserToNewUserToPreventServerFromRejectingTheNewAssignedCohortOnTheNextSession();
            UpdateServerConfig();

            void ConvertUserToNewUserToPreventServerFromRejectingTheNewAssignedCohortOnTheNextSession()
            {
                ChangeUserIdRandomly();
            }

            void ChangeCohort()
            {
                ServiceLocator.Find<UserCohortAssignmentManager>().SetAssignedCohortID(id);
            }

            void UpdateServerConfig()
            {
                ServerDevOptions.ClearServerConfig();
                ServerDevOptions.ForceUpdateServerConfig();
            }
        }

        [DevOption(commandName: "Clear Cohrot")]
        public static void ClearCohort()
        {
            ServiceLocator.Find<UserCohortAssignmentManager>().ClearCohort();
            ServerDevOptions.ClearServerConfig();
            ServerDevOptions.ForceUpdateServerConfig();
        }
    }
}