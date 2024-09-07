using Match3.Foundation.Base.NotificationService;
using Match3.Foundation.Base.ServiceLocating;
using UnityEngine;
using UnityEngine.UI;


public class NotificationSettingsController : MonoBehaviour
{


    public Toggle fullLifeNotifToggle;
    public Toggle luckySpinnerNotifToggle;
    public Toggle ncResultNotifToggle;

    [Space(10)] 
    public Notification fullLifeNotification;
    public Notification luckySpinnerNotification;
    public Notification ncResultNotification;
    
    
    private NotificationService notificationService;

    
    
    void Start()
    {
        notificationService = ServiceLocator.Find<NotificationService>();

        fullLifeNotifToggle.isOn = notificationService.CheckActive(fullLifeNotification);
        luckySpinnerNotifToggle.isOn = notificationService.CheckActive(luckySpinnerNotification);
        ncResultNotifToggle.isOn = notificationService.CheckActive(ncResultNotification);
    }


    public void OnFullLifeToggleChange(bool isOn)
    {
        notificationService.SetActive(fullLifeNotification, isOn);
    }
    

    public void OnLuckySpinnerToggleChange(bool isOn)
    {
        notificationService.SetActive(luckySpinnerNotification, isOn);
    }
    
    
    public void OnNcResultToggleChange(bool isOn)
    {
        notificationService.SetActive(ncResultNotification, isOn);
    }
    
}
