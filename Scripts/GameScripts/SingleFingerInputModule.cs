using UnityEngine;
using UnityEngine.EventSystems;


/*
ISingleFingerHandler - handles strict single-finger down-up-drag

Put this daemon ON TO the game object, with a consumer of the service.

(Note - there are many, many philosophical decisions to make when
implementing touch concepts; just some issues include what happens
when other fingers touch, can you "swap out" etc. Note that, for
example, Apple vs. Android have slightly different takes on this.
If you wanted to implement slightly different "philosophy" you'd
do that in this script.)
*/


public interface ISingleFingerHandler
{
    void OnSingleFingerDown(Vector2 position);
    void OnSingleFingerUp(Vector2 position);
    void OnSingleFingerDrag(Vector2 delta);
}

/* note, Unity chooses to have "one interface for each action"
however here we are dealing with a consistent paradigm ("dragging")
which has three parts; I feel it's better to have one interface
forcing the consumer to have the three calls (no problem if empty) */


public class SingleFingerInputModule : MonoBehaviour,
                IBeginDragHandler, IEndDragHandler, IDragHandler

{
    private ISingleFingerHandler singleFingerHandler = null;
    private const int NO_POINTER_ID = -1;

    private int currentSingleFinger = NO_POINTER_ID;
    private int fingersDownCount = 0;

    void Awake()
    {
        singleFingerHandler = GetComponent(typeof(ISingleFingerHandler)) as ISingleFingerHandler;
    }

    public void OnBeginDrag(PointerEventData data)
    {
        IncreaseFingerCountByOne();

        if (currentSingleFinger == NO_POINTER_ID && fingersDownCount == 1)
        {
            currentSingleFinger = data.pointerId;
            if (singleFingerHandler != null) singleFingerHandler.OnSingleFingerDown(data.position);
        }
    }

    public void OnEndDrag(PointerEventData data)
    {
        DecreaseFingerCountByOne();

        if (currentSingleFinger == data.pointerId)
        {
            currentSingleFinger = -1;
            if (singleFingerHandler != null) singleFingerHandler.OnSingleFingerUp(data.position);
        }
    }

    public void OnDrag(PointerEventData data)
    {
        if (currentSingleFinger == data.pointerId && fingersDownCount == 1)
        {
            if (singleFingerHandler != null)
                singleFingerHandler.OnSingleFingerDrag(data.delta);
        }
    }

    private void IncreaseFingerCountByOne() => fingersDownCount += 1;
    private void DecreaseFingerCountByOne() => fingersDownCount -= 1;

}