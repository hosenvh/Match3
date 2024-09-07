using System.IO;
using I2.Loc;
using Match3.CloudSave;
using Match3.Data.Unity.PersistentTypes;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Utility;
using PandasCanPlay.HexaWord.Utility;
using TMPro;
using UnityEngine;


public class SaveCodePageController : MonoBehaviour
{

    [SerializeField] private GameObject touchBlocker;

    [Space(10)]
    [SerializeField] private Animation screenshotCodeButtonCheckAnimation;
    [SerializeField] private Animation copyCodeButtonCheckAnimation;
    [SerializeField] private Animation emailCodeButtonCheckAnimation;

    [Space(10)]
    [SerializeField] private GameObject screenshotButtonCheckmark;
    [SerializeField] private GameObject copyCodeButtonCheckmark;
    [SerializeField] private GameObject sendEmailCodeButtonCheckmark;

    [Space(10)] 
    [SerializeField] private TMP_InputField emailInputField;
    
    [Space(10)] 
    public LocalizedStringTerm notAcceptableEmailMessage;
    public LocalizedStringTerm successfulSendingEmailMessage;
    public LocalizedStringTerm failureSendingEmailMessage;
    public LocalizedStringTerm repetitiveEmailMessage;


    private PersistentString sentEmail;

    private string fileName = "GolmoradCloudSaveAuthKeyBackup.png";
    private string screenshotPath;

    private bool canPlayCopyCodeButtonAnimation = true;
    private bool canPlaySendEmailCodeButtonAnimation = true;
    private string authorizationCode;

    
    
    private void Awake()
    {
        sentEmail = new PersistentString("PlayerSentEmail_MedrickCloudSave");
    }

    void OnEnable()
    {
        ResetButtonAnimations();
        InitializeScreenshot();
        InitializeCopyCode();
        InitializeSendEmailCode();
    }

    public void TakeScreenshot()
    {
        Close();
        ScreenCapture.CaptureScreenshot(screenshotPath);
    }

    public void CopyCode()
    {
        GeneralUtilities.CopyToClipboard(authorizationCode);
        if (canPlayCopyCodeButtonAnimation)
        {
            copyCodeButtonCheckAnimation.Play();
            canPlayCopyCodeButtonAnimation = false;
        }
    }

    public void SendEmail()
    {
        if (!Validator.IsEmailValid(emailInputField.text))
        {
            Base.gameManager.OpenPopup<Popup_ConfirmBox>().Setup(notAcceptableEmailMessage,
                ScriptLocalization.UI_General.Ok, "", true);
            return;
        }

        if (sentEmail.Get() == emailInputField.text)
        {
            Base.gameManager.OpenPopup<Popup_ConfirmBox>().Setup(repetitiveEmailMessage,
                ScriptLocalization.UI_General.Ok, "", true);
            return;
        }
        
        var waitPopup = Base.gameManager.OpenPopup<Popup_WaitBox>().Setup(ScriptLocalization.Message_General.Wait);
        var sendEmailRequestHandler = new MedrickCloudSaveSendEmailRequestHandler();
        sendEmailRequestHandler.SendPlayerEmail(emailInputField.text, () =>
        {
            waitPopup.Close();
            Base.gameManager.OpenPopup<Popup_ConfirmBox>().Setup(successfulSendingEmailMessage,
                    ScriptLocalization.UI_General.Nice, "", true).OnCloseEvent +=
                () =>
                {
                    sentEmail.Set(emailInputField.text);
                    if (canPlaySendEmailCodeButtonAnimation)
                    {
                        emailCodeButtonCheckAnimation.Play();
                        canPlaySendEmailCodeButtonAnimation = false;
                    }
                };
        }, () =>
        {
            Base.gameManager.OpenPopup<Popup_ConfirmBox>().Setup(failureSendingEmailMessage,
                ScriptLocalization.UI_General.Ok, "", true);
            waitPopup.Close();
        });
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }


    private void ResetButtonAnimations()
    {
        screenshotCodeButtonCheckAnimation.Stop();
        copyCodeButtonCheckAnimation.Stop();
        emailCodeButtonCheckAnimation.Stop();
    }
    
    
    private void InitializeScreenshot()
    {
        screenshotPath = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(screenshotPath))
            screenshotButtonCheckmark.SetActive(true);
    }

    private void InitializeCopyCode()
    {
        authorizationCode = ServiceLocator.Find<CloudSaveService>().GetLocalSavedAuthorizationKey();
        if (GUIUtility.systemCopyBuffer.Contains(authorizationCode))
        {
            copyCodeButtonCheckmark.SetActive(true);
            canPlayCopyCodeButtonAnimation = false;
        }
    }

    private void InitializeSendEmailCode()
    {
        var email = sentEmail.Get("");
        if (!email.IsNullOrEmpty())
        {
            emailInputField.text = email;
            sendEmailCodeButtonCheckmark.SetActive(true);
            canPlaySendEmailCodeButtonAnimation = false;
        }
    }
    
}
