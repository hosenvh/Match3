using System;
using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using UnityEngine;


namespace Match3.Foundation.Base.NotificationService
{

    public class NotificationActivationController
    {


        private const string NOTIFICATION_ACTIVENESS_PREFIX = "Notification_Activeness_";
        private const string NOTIFICATION_VERIFICATION_PREFIX = "Notification_Verification_";

        private readonly INotificationDataStorage dataStorage;



        public NotificationActivationController(INotificationDataStorage dataStorage)
        {
            this.dataStorage = dataStorage;
        }

        public bool IsNotificationActive(Notification notification)
        {
            return dataStorage.GetInt(NOTIFICATION_ACTIVENESS_PREFIX + notification.id,
                       notification.defaultActive ? 1 : 0) == 1;
        }

        public void SaveNotificationActiveness(Notification notification, bool active)
        {
            dataStorage.SaveInt(NOTIFICATION_ACTIVENESS_PREFIX + notification.id, active ? 1 : 0);
        }

        public bool NeedToVerifyActivation(Notification notification)
        {
            if (!notification.needToVerification) return false;
            return !dataStorage.GetBool(NOTIFICATION_VERIFICATION_PREFIX + notification.id, false);
        }



        public void VerifyToActiveFor(Notification notification, Action<bool> onResult)
        {
            global::Base.gameManager.OpenPopup<Popup_ConfirmBox>().Setup(
                notification.verificationQuestionLocalizedMessage.ToString(), ScriptLocalization.UI_General.Yes, ScriptLocalization.UI_General.No, true, result =>
                {
                    onResult(result);
                    dataStorage.SaveBool(NOTIFICATION_VERIFICATION_PREFIX + notification.id, true);
                });
        }


    }

}