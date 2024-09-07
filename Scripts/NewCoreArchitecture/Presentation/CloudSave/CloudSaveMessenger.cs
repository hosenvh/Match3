using System;
using Match3.CloudSave;
using I2.Loc;
using Match3;
using Match3.Foundation.Base.ServiceLocating;
using UnityEngine;
using UnityEngine.Serialization;


public class CloudSaveMessenger : MonoBehaviour
{

    [FormerlySerializedAs("useGeneralDescription")] 
    [SerializeField] private bool useGeneralDescriptionForConflict = default;

    [SerializeField] private LocalizedStringTerm authenticationConflictGeneralLocalizedDescription = default;
    [SerializeField] private LocalizedStringTerm authenticationConflictOldDeviceLocalizedDescription;

    [FormerlySerializedAs("descriptions")] 
    [SerializeField] private CloudSaveStatusDescription[] conflictDescriptions = default;

    [SerializeField] private LocalizedStringTerm authenticationFailedLocalizedDescription = default;
    
    private CloudSaveService cloudSaveService;

    private string downloadPlayGamesLink;

    public bool EverShowInstallPlayGamesMessage
    {
        get => PlayerPrefs.GetInt("showInstallPlayGamesMessage", 0) == 1;
        set => PlayerPrefs.SetInt("showInstallPlayGamesMessage", value ? 1:0);
    }


    private bool ShowedCurrentConflictMessage
    {
        get => PlayerPrefs.GetInt("showThisCloudConflictMessage", 0) == 1;
        set => PlayerPrefs.SetInt("showThisCloudConflictMessage", value ? 1 : 0);
    }

    private AuthenticationStatus LastAuthenticationConflictStatus
    {
        get => (AuthenticationStatus)PlayerPrefs.GetInt("LastAuthenticationConflictStatus", 0);
        set => PlayerPrefs.SetInt("LastAuthenticationConflictStatus", (int)value);
    }

    private bool ShowedSignInProblemMessage
    {
        get => PlayerPrefs.GetInt("ShowSignInProblemMessage", 0) == 1;
        set => PlayerPrefs.SetInt("ShowSignInProblemMessage", value ? 1 : 0);
    }
    

    private void Awake()
    {
        cloudSaveService = ServiceLocator.Find<CloudSaveService>();
        downloadPlayGamesLink = ServiceLocator.Find<ServerConfigManager>().data.config.playGamesDownloadLink;
    }


    public bool IsInstallPlayGamesMessageExists()
    {
        return (cloudSaveService.IsServiceActive && !cloudSaveService.IsAllDependenciesResolved() && !EverShowInstallPlayGamesMessage);
    }
    
    public void ShowInstallPlayGamesMessage(Action onConfirm, Action onCancel)
    {
        OpenConfirmBox(ScriptLocalization.Message_CloudSave.InstallPlayGamesToSave, ScriptLocalization.UI_General.IWant,
            ScriptLocalization.UI_General.LeaveIt,
            () =>
            {
                onConfirm();
                Application.OpenURL(downloadPlayGamesLink);
            }, onCancel);
        EverShowInstallPlayGamesMessage = true;
    }


    public bool IsAuthenticationFailedMessageExists()
    {
        return (cloudSaveService.IsServiceActive && !cloudSaveService.IsTryingToAuthenticateAcceptable() && !ShowedSignInProblemMessage);
    }
    
    public void ShowAuthenticationFailedMessage(Action onConfirm)
    {
        ShowedSignInProblemMessage = true;

        OpenConfirmBox(authenticationFailedLocalizedDescription, ScriptLocalization.UI_General.Settings, "",
            ()=>
            {
                onConfirm();
                OpenAccountSettings();
            },
            delegate { });
    }

    public bool HasDeviceConflict()
    {
        return cloudSaveService.Status == AuthenticationStatus.DifferentDeviceWithFullLocal ||
               cloudSaveService.Status == AuthenticationStatus.DifferentDeviceWithEmptyLocal;
    }
    
    
    public bool IsConflictMessageExists()
    {
        if (cloudSaveService.HasSwitchedCloudServer)
            return true;
        if (cloudSaveService.IsServiceActive 
            && cloudSaveService.Status != AuthenticationStatus.NotAuthenticated
            && cloudSaveService.Status != AuthenticationStatus.AuthenticationFailed
            && cloudSaveService.Status != AuthenticationStatus.Successful)
        {
            if (!ShowedCurrentConflictMessage || cloudSaveService.Status != LastAuthenticationConflictStatus)
            {
                return true;
            }
        }

        return false;
    }
    
    
    public void ShowConflictMessage(Action onConfirm)
    {
        ShowedCurrentConflictMessage = true;
        LastAuthenticationConflictStatus = cloudSaveService.Status;

        if (useGeneralDescriptionForConflict)
            OpenConfirmBox(authenticationConflictGeneralLocalizedDescription, ScriptLocalization.UI_General.Settings,
                "", ()=>
                {
                    onConfirm();
                    OpenAccountSettings();
                }, delegate {  });
        else
            FindAndShowMessage(cloudSaveService.Status, onConfirm);
    }

    public void ShowDeviceConflictMessage(Action onConfirm)
    {
        ShowedCurrentConflictMessage = true;
        LastAuthenticationConflictStatus = cloudSaveService.Status;

        OpenConfirmBox(authenticationConflictOldDeviceLocalizedDescription, ScriptLocalization.UI_General.Settings,
            "", ()=>
            {
                onConfirm();
                OpenAccountSettings();
            }, delegate {  });
    }

    
    private void FindAndShowMessage(AuthenticationStatus status, Action onConfirm)
    {
        foreach (var describe in conflictDescriptions)
        {
            if(describe.forAuthStatus!=status) continue;
            OpenConfirmBox(describe.statusLocalizedDescription.ToString(), ScriptLocalization.UI_General.Settings, "",
                ()=>
                {
                    onConfirm();
                    OpenAccountSettings();
                }, delegate {  });
            break;
        }
    }

    private void OpenConfirmBox(string desc, string confirmButton, string cancelButton, Action onConfirm, Action onCancel)
    {
        var gameManager = Base.gameManager;
        var confirmPopup = gameManager.OpenPopup<Popup_ConfirmBox>();
        confirmPopup.Setup(desc, confirmButton, cancelButton, true,
            isConfirmed =>
            {
                if (isConfirmed)
                    onConfirm();
                else
                    onCancel();
            });
    }

    private void OpenAccountSettings()
    {
        Base.gameManager.OpenPopup<Popup_Settings>().OpenAccountTab();
    }
    
}
