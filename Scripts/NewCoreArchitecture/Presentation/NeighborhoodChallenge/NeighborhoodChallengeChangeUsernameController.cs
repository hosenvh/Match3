using System;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.NeighborhoodChallenge;
using Match3.UserManagement.Main;
using Match3.UserManagement.ProfileName;
using UnityEngine;
using UnityEngine.Events;


[Serializable]
public class UnityStringEvent : UnityEvent<string>
{
}

public class NeighborhoodChallengeChangeUsernameController : MonoBehaviour
{
    public UnityStringEvent onUsernameChanged;
    private NeighborhoodChallengeManager manager;


    public void ChangeUsername()
    {
        UserProfileNameManager profileNameManager = ServiceLocator.Find<UserManagementService>().UserProfileNameManager;
        profileNameManager.AskForUserProfileName(onSubmit: () => onUsernameChanged.Invoke(profileNameManager.CurrentUserProfileName), onCanceled: delegate { });
    }
}