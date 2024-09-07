using System;
using System.Linq;
using I2.Loc;
using Match3.CloudSave;
using RTLTMPro;
using UnityEngine;
using UnityEngine.UI;


public class AccountPageMedrickCloudSaveHandler : AccountPageBaseSaveHandler
{

    [Space(10)]
    [SerializeField] private Text authorizationCodeText;
    [SerializeField] private CloudSaveAuthorizationInputPanelController authorizationInputPanelController = default;
    [SerializeField] private GameObject saveCodePanelGameObject = default;
    [SerializeField] private LocalizedStringTerm medrickAccountNotSignedInLocalizedMessage = default;

    [Space(10)]
    [SerializeField] private GameObject playerCodeSectionGameObject;

    public void OpenSaveCodePanel()
    {
        saveCodePanelGameObject.SetActive(true);
    }

    protected override void InternalStart()
    {
        UpdateCodeText();
    }

    private void UpdateCodeText()
    {
        authorizationCodeText.text = cloudSaveService.GetLocalSavedAuthorizationKey().ToUpper();
    }

    protected override void GetAuthorizationKeyFromPlayer(Action<bool, string> onGetAuthorization)
    {
        authorizationInputPanelController.Open(onGetAuthorization);
    }

    protected override void InternalSetTabStatus(AuthenticationStatus status)
    {
        UpdateCodeText();
    }

    protected override void SetInternalStatusAndMessage(AuthenticationStatus status, bool isServerAuthenticated)
    {
        saveButtonObject.SetActive(isServerAuthenticated &&
                                   status != AuthenticationStatus.AuthenticationFailed &&
                                   status != AuthenticationStatus.NotAuthenticated);

        loadButtonObject.SetActive(isServerAuthenticated &&
                                   status != AuthenticationStatus.AuthenticationFailed &&
                                   status != AuthenticationStatus.NotAuthenticated);

        playerCodeSectionGameObject.SetActive(isServerAuthenticated &&
                                              status != AuthenticationStatus.AuthenticationFailed &&
                                              status != AuthenticationStatus.NotAuthenticated);

        signInButtonGameObject.SetActive(status == AuthenticationStatus.NotAuthenticated ||
                                         status == AuthenticationStatus.AuthenticationFailed);

        signInButton.interactable = status == AuthenticationStatus.AuthenticationFailed || status == AuthenticationStatus.NotAuthenticated;

        if (status == AuthenticationStatus.AuthenticationFailed || status == AuthenticationStatus.NotAuthenticated)
        {
            if (cloudSaveService.IsAuthenticationInProgress)
            {
                cloudSaveStatusLocalText.SetText(accountSigningInLocalizedMessage);
                return;
            }

            // --------------------------------------------

            cloudSaveStatusLocalText.SetText(medrickAccountNotSignedInLocalizedMessage);
        }
    }
}
