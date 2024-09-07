using System;
using Match3.CloudSave;
using I2.Loc;
using Match3;
using Match3.Foundation.Base.ServiceLocating;
using UnityEngine;


public class AccountPageGoogleCloudSaveHandler : AccountPageBaseSaveHandler
{
    
    [Space(10)]
    [SerializeField] private LocalizedStringTerm playGamesNotInstalledLocalizedMessage = default;
    [SerializeField] private LocalizedStringTerm playGamesNotSignedInLocalizedMessage = default;

    [Space(10)]
    public GameObject installPlayGamesButtonObject;
    
    
    private string playGamesDownloadLink;
    
    
    protected override void InternalStart()
    {
        playGamesDownloadLink = ServiceLocator.Find<ServerConfigManager>().data.config.playGamesDownloadLink;
    }

    public void GoToInstallPlayGames()
    {
        gameManager.ClosePopup();
        Application.OpenURL(playGamesDownloadLink);
    }


    protected override void GetAuthorizationKeyFromPlayer(Action<bool, string> onGetAuthorization)
    {
        onGetAuthorization(true, "");
    }

    protected override void InternalSetTabStatus(AuthenticationStatus status)
    {
    }

    protected override void SetInternalStatusAndMessage(AuthenticationStatus status, bool isServerAuthenticated)
    {
        var isPlayGamesInstalled = cloudSaveService.IsAllDependenciesResolved();

        saveButtonObject.SetActive(isPlayGamesInstalled && isServerAuthenticated &&
                                   status != AuthenticationStatus.AuthenticationFailed &&
                                   status != AuthenticationStatus.NotAuthenticated);
        
        loadButtonObject.SetActive(isPlayGamesInstalled && isServerAuthenticated &&
                                   status != AuthenticationStatus.AuthenticationFailed &&
                                   status != AuthenticationStatus.NotAuthenticated);
        
        installPlayGamesButtonObject.SetActive(!isPlayGamesInstalled);

        signInButtonGameObject.SetActive(isPlayGamesInstalled &&
                                              (status == AuthenticationStatus.NotAuthenticated ||
                                               status == AuthenticationStatus.AuthenticationFailed));
        signInButton.interactable = !cloudSaveService.IsAuthenticationInProgress;
        
        if (status == AuthenticationStatus.AuthenticationFailed || status == AuthenticationStatus.NotAuthenticated)
        {
            if (!isPlayGamesInstalled)
            {
                cloudSaveStatusLocalText.SetText(playGamesNotInstalledLocalizedMessage);
                return;
            }
            
            // --------------------------------------------

            if (cloudSaveService.IsAuthenticationInProgress)
            {
                cloudSaveStatusLocalText.SetText(accountSigningInLocalizedMessage);
                return;
            }
            
            // --------------------------------------------
            
            cloudSaveStatusLocalText.SetText(playGamesNotSignedInLocalizedMessage);
        }
    }
    
    
}
