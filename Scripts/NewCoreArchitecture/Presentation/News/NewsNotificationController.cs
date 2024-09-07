using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.NewsSpace;
using Match3.Presentation;
using UnityEngine;


public class NewsNotificationController : MonoBehaviour, EventListener
{

    [SerializeField] private Notifier notifier = default;


    private void OnEnable()
    {
        // TODO: This is temporary, please find a better way.
        if (ServiceLocator.Find<PresentationElementActivationStateCenter>().IsActive(PresentationElement.News) == false)
            return;

        var newsManager = ServiceLocator.Find<NewsManager>();
        
        ServiceLocator.Find<EventManager>().Register(this);
        
        SetActiveNotification(newsManager.HaveNewNews);
    }

    private void OnDisable()
    {
        ServiceLocator.Find<EventManager>().UnRegister(this); 
    }
    

    private void SetActiveNotification(bool active)
    {
        notifier.SetNotify(active, this);
    }


    public void OnEvent(GameEvent evt, object sender)
    {
        switch (evt)
        {
            case NewsesGottenOldEvent _:
                SetActiveNotification(false);
                break;
            case NewNewsesReceivedEvent _:
                SetActiveNotification(true);
                break;
        }
    }
    
    
    
}
