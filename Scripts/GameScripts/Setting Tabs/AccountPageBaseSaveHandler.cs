using System;
using Match3.CloudSave;
using I2.Loc;
using Match3.Foundation.Base.ServiceLocating;
using SeganX;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public abstract class AccountPageBaseSaveHandler : MonoBehaviour
{
    public GameObject saveButtonObject;
    public GameObject loadButtonObject;
    
    [Space(10)]
    [SerializeField] protected Color redTextColor = default;
    [SerializeField] protected Color greenTextColor = default;
    
    [Space(10)] 
    [SerializeField] protected Image  saveButtonImage = default;
    [SerializeField] protected Image  loadButtonImage = default;
    [SerializeField] protected Image  saveButtonIconImage = default;
    [SerializeField] protected Image  loadButtonIconImage = default;
    [SerializeField] protected Text   saveButtonText = default;
    [SerializeField] protected Text   loadButtonText = default;
    [SerializeField] protected Outline saveButtonTextOutline = default;
    [SerializeField] protected Outline loadButtonTextOutline = default;
    [SerializeField] protected Button saveButton = default;
    [SerializeField] protected Button loadButton = default;

    [Space(10)]
    [SerializeField] protected LocalText cloudSaveStatusLocalText = default;
    [SerializeField] protected LocalText saveStatusLocalText = default;
    [SerializeField] protected Text saveStatusText = default;
    
    [Space(10)]
    [SerializeField] protected Button signInButton = default;
    [FormerlySerializedAs("signInPlayGamesButtonObject")] [SerializeField] protected GameObject signInButtonGameObject;
    
    [Space(10)] 
    [Header("Save And Load Button DeActive Settings")]
    [SerializeField] private Sprite deActiveSaveLoadButtonSprite = default;
    public Color loadTextDeactiveColor;
    public Color saveTextDeactiveColor;
    public Color loadIconDeactiveColor;
    public Color saveIconDeactiveColor;
    
    [Space(10)] 
    [Header("Save And Load Button Active Settings")]
    [SerializeField] private Sprite activeSaveButtonSprite = default;
    [SerializeField] private Sprite activeLoadButtonSprite =default;
    public Color loadTextActiveColor;
    public Color saveTextActiveColor;
    public Color loadIconActiveColor;
    public Color saveIconActiveColor;
    
    [FormerlySerializedAs("playGamesSigningInLocalizedMessage")] [SerializeField] protected LocalizedStringTerm accountSigningInLocalizedMessage = default;
    
    [Space(10)]
    [SerializeField] private CloudSaveStatusDescription[] tabStatuses = default;
    
    protected CloudSaveService cloudSaveService;
    protected PlayerSaveBackupService playerSaveBackupService;
    protected GameManager gameManager;

    private bool onServiceAuthenticationCallBackRegistered = false;
    

    private void Start()
    {
        cloudSaveService = ServiceLocator.Find<CloudSaveService>();
        playerSaveBackupService = ServiceLocator.Find<PlayerSaveBackupService>();
        gameManager = Base.gameManager;

        SetTabStatus(cloudSaveService.Status);

        if (cloudSaveService.IsAuthenticationInProgress)
        {
            cloudSaveService.OnAuthenticateCallBack += SetTabStatus;
            onServiceAuthenticationCallBackRegistered = true;
        }

        InternalStart();
    }

    protected abstract void InternalStart();

    private void OnDisable()
    {
        if(onServiceAuthenticationCallBackRegistered)
            cloudSaveService.OnAuthenticateCallBack -= SetTabStatus;
    }
    
    public void SaveGameOnCloud()
    {
        gameManager.OpenPopup<Popup_WaitBox>().Setup(ScriptLocalization.Message_General.Wait);
        cloudSaveService.Save(status =>
        {
            gameManager.ClosePopup();
            var saveResultString = ScriptLocalization.Message_AccountTab.SaveProgressFailed;
            if (status == CloudSaveRequestStatus.Successful)
                saveResultString = ScriptLocalization.Message_AccountTab.SaveProgressSuccessfull;

            gameManager.OpenPopup<Popup_ConfirmBox>()
                .Setup(saveResultString, ScriptLocalization.UI_General.Ok, "", true, closeWithConfirm => { });

            SetTabStatus(cloudSaveService.Status);
        });
    }

    
    public void LoadGameFromCloud()
    {
        GetAuthorizationKeyFromPlayer((result, authKey) =>
        {
            if (result)
            {
                gameManager.OpenPopup<Popup_ConfirmBox>().Setup(
                    ScriptLocalization.Message_AccountTab.RestoreAccountConfirmation, ScriptLocalization.UI_General.Yes,
                    ScriptLocalization.UI_General.No,
                    true,
                    confirmed =>
                    {
                        if (!confirmed) return;

                        playerSaveBackupService.StoreNewBackup();

                        gameManager.OpenPopup<Popup_WaitBox>().Setup(ScriptLocalization.Message_General.Wait);
                        cloudSaveService.TryResolveAuthenticationAndLoad(authKey, status =>
                        {
                            gameManager.ClosePopup();
                            var loadResultString = ScriptLocalization.Message_AccountTab.LoadProgressFailed;
                            if (status == CloudSaveRequestStatus.Successful)
                                loadResultString = ScriptLocalization.Message_AccountTab.LoadProgressSuccessfull;
                            else if (status == CloudSaveRequestStatus.BadInputError)
                                loadResultString = ScriptLocalization.Message_AccountTab.LoadProgressSuccessfull; // TODO: show bad input message
                            else if (status == CloudSaveRequestStatus.NoSavedDataExists)
                                loadResultString = ScriptLocalization.Message_AccountTab.UserHasNotSaved;

                            gameManager.OpenPopup<Popup_ConfirmBox>()
                                .Setup(loadResultString, 
                                    ScriptLocalization.UI_General.Ok, "", true, closeWithConfirm =>
                                {
                                    if (status == CloudSaveRequestStatus.Successful)
                                        Application.Quit();
                                });
                        });
                    });
            }
        });
    }

    public void SignInToAccount()
    {
        signInButton.interactable = false;
        cloudSaveStatusLocalText.SetText(accountSigningInLocalizedMessage);
        gameManager.OpenPopup<Popup_WaitBox>().Setup(ScriptLocalization.Message_General.Wait);
        cloudSaveService.Authenticate(authStatus =>
        {
            gameManager.ClosePopup();
            if (this == null) return;
            SetTabStatus(authStatus);

            if (authStatus == AuthenticationStatus.AuthenticationFailed)
            {
                gameManager.OpenPopup<Popup_ConfirmBox>()
                    .Setup(messageString: ScriptLocalization.Message_CloudSave.CantLoginToCloud, 
                        confirmString: ScriptLocalization.UI_General.Ok,
                        cancelString: "",
                        closeOnConfirm: true,
                        onResult: closeWithConfirm => { });
            }
        });
    }
    
    protected abstract void GetAuthorizationKeyFromPlayer(Action<bool, string> onGetAuthorization);
    
    protected void SetTabStatus(AuthenticationStatus status)
    {
        if (!cloudSaveService.IsServiceActive)
        {
            SetActiveLoadButton(false);
            SetActiveSaveButton(false);
            cloudSaveStatusLocalText.SetText(ScriptLocalization.Message_AccountTab.CloudSaveDeactivated);
            return;
        }

        SetInternalStatusAndMessage(status, cloudSaveService.IsServerAuthenticated);
        
        if (cloudSaveService.IsStatusAcceptableToSave())
        {
            if (cloudSaveService.LastSaveRequestStatus != CloudSaveRequestStatus.NotRequested)
            {
                if(cloudSaveService.LastSaveRequestStatus == CloudSaveRequestStatus.Successful) 
                    SetSaveStatusDescription(ScriptLocalization.Message_AccountTab.GameIsSaved, greenTextColor);
                else
                    SetSaveStatusDescription(ScriptLocalization.Message_AccountTab.GameNotSaved, redTextColor);
            }
        }
        else
            ResetSaveStatusDescription();

        foreach (var tabStatus in tabStatuses)
        {
            if (status != tabStatus.forAuthStatus) continue;

            cloudSaveStatusLocalText.SetText(tabStatus.statusLocalizedDescription.ToString());
            SetActiveLoadButton(tabStatus.canLoad);
            SetActiveSaveButton(tabStatus.canSave);
            break;
        }

        InternalSetTabStatus(status);
    }

    protected abstract void InternalSetTabStatus(AuthenticationStatus status);

    protected abstract void SetInternalStatusAndMessage(AuthenticationStatus status, bool isServerAuthenticated);
    
    private void SetSaveStatusDescription(string status, Color textColor)
    {
        saveStatusLocalText.SetText(status);
        saveStatusText.color = textColor;
    }

    private void ResetSaveStatusDescription()
    {
        saveStatusLocalText.SetText("");        
    }
    
    private void SetActiveSaveButton(bool active)
    {
        saveButton.interactable = active;
        if (active)
        {
            saveButtonImage.sprite = activeSaveButtonSprite;
            saveButtonText.color = saveTextActiveColor;
            saveButtonIconImage.color = saveIconActiveColor;
        }
        else
        {
            saveButtonImage.sprite = deActiveSaveLoadButtonSprite;
            saveButtonText.color = saveTextDeactiveColor;
            saveButtonIconImage.color = saveIconDeactiveColor;
        }
        
        saveButtonTextOutline.enabled = active;
    }
    
    
    private void SetActiveLoadButton(bool active)
    {
        loadButton.interactable = active;
        if (active)
        {
            loadButtonImage.sprite = activeLoadButtonSprite;
            loadButtonText.color = loadTextActiveColor;
            loadButtonIconImage.color = loadIconActiveColor;
        }
        else
        {
            loadButtonImage.sprite = deActiveSaveLoadButtonSprite;
            loadButtonText.color = loadTextDeactiveColor;
            loadButtonIconImage.color = loadIconDeactiveColor;
        }
        
        loadButtonTextOutline.enabled = active;
    }
    
}
