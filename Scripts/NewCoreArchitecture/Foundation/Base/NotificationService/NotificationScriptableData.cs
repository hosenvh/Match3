using I2.Loc;
using LocalPush;
using UnityEngine;
using UnityEngine.Serialization;


namespace Match3.Foundation.Base.NotificationService
{

    public class NotificationScriptableData : ScriptableObject 
    {
        public int id = 0;
        public LocalizedStringTerm notificationLocalizedName;

        [Space(10)] 
        public bool defaultActive = false;
        public bool canBeDisable = true;
        public bool needToVerification = false;
    
        [Space(10)] 
        public LocalizedStringTerm localizedTitle;
        public LocalizedStringTerm localizedMessage;
        public NotificationIcon smallIcon;
        public Color notifColor = new Color(.8f, 0, 0);
    
        [FormerlySerializedAs("activationQuestionMessage")] [Space(10)] 
        public LocalizedStringTerm verificationQuestionLocalizedMessage;

    }

}


