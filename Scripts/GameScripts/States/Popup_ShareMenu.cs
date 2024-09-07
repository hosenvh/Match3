using System;
using I2.Loc;
using Match3.Foundation.Base;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.ReferralMarketing.Segments;
using Match3.Game.ReferralMarketing.Share;
using PandasCanPlay.HexaWord.Utility;
using SeganX;
using UnityEngine;



public class Popup_ShareMenu : GameState
{

    // -------------------------------------- Public Fields -------------------------------------- \\

    public LocalizedStringTerm shareDescription;

    [Space(10)]
    public GameObject rewardHintBoxObject;

    // -------------------------------------- Private Fields -------------------------------------- \\
    
    private string shareGifStreamingPath;
    private ShareData shareData;
    private ShareTarget shareTarget;

    private NativeShare.ShareResultCallback shareResult;

    private ShareSegment callerShareSegment = null;

    // ============================================================================================ \\
    private void Awake()
    {
        ServiceLocator.Find<ConfigurationManager>().Configure(this);
    }

    public void SetShareGiftStreamingPath(string shareGifStreamingPath)
    {
        this.shareGifStreamingPath = shareGifStreamingPath;
    }

    public Popup_ShareMenu Setup(string referralCode, NativeShare.ShareResultCallback onShareResult)
    {
        shareResult = onShareResult;

        if (string.IsNullOrEmpty(shareGifStreamingPath))
        {
            var downloadLink = ServiceLocator.Find<StoreFunctionalityManager>().GamePageURL();
            shareData = new ShareData() {description = string.Format(shareDescription, referralCode, downloadLink)};
        }
        else
        {
            StartCoroutine(GeneralUtilities.ExtractStreamingAssetFilePath(shareGifStreamingPath, filePath =>
            {
                var downloadLink = ServiceLocator.Find<StoreFunctionalityManager>().GamePageURL();
                shareData = new ShareData() {description = string.Format(shareDescription, referralCode, downloadLink), filesPath = new[] {filePath}};
            }));
        }


        return this;
    }

    public Popup_ShareMenu SetActiveHintBox(bool active)
    {
        rewardHintBoxObject.SetActive(active);
        return this;
    }

    public Popup_ShareMenu SetCallerSegment(ShareSegment shareSegment)
    {
        callerShareSegment = shareSegment;
        return this;
    }

    public void ShareOnWhatsapp()
    {
        shareTarget = new ShareTarget() {androidPackageName = "com.whatsapp"};
        Back();
        Share("Whatsapp");
    }

    public void ShareOnTelegram()
    {
        shareTarget = new ShareTarget() {androidPackageName = "org.telegram.messenger", className = "org.telegram.ui.LaunchActivity"};
        Back();
        Share("Telegram");
    }

    public void ShareOnInstagram()
    {
        shareTarget = new ShareTarget() {androidPackageName = "com.instagram.android"};
        Back();
        Share("Instagram");
    }

    public void GeneralShare()
    {
        shareTarget = null;
        Back();
        Share("General");
    }

    public override void Back()
    {
        AnalyticsManager.SendEvent(
            new AnalyticsData_Referral_SharingResult(false, GetSegmentCallerName(),
                "Nothing"));

        base.Back();
    }

    private void Share(string shareChannel)
    {
        SocialSharing.Share(shareData, shareTarget, (result, appName) =>
        {
            shareResult(result, appName);

            AnalyticsManager.SendEvent(
                new AnalyticsData_Referral_SharingResult(result == NativeShare.ShareResult.Shared, GetSegmentCallerName(),
                    shareChannel));
        });
    }

    private string GetSegmentCallerName()
    {
        // TODO: Refactor this to `Unknown` after using a ShareSegment for sharing from referral center
        string segmentCallerName = "ReferralCenterShareSegment";
        if (callerShareSegment != null) segmentCallerName = callerShareSegment.GetType().Name;
        return segmentCallerName;
    }
}
