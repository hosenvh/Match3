using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

/*
IPinchHandler - strict two sequential finger pinch Handling

Put this daemon ON TO the game object, with a consumer of the service.

(Note, as always, the "philosophy" of a glass gesture is up to you.
There are many, many subtle questions; eg should extra fingers block,
can you 'swap primary' etc etc etc - program it as you wish.)
*/


public interface IPinchHandler
{
    void OnPinchStart();
    void OnPinchEnd();
    void OnPinchZoom(float gapDelta);
}

/* note, Unity chooses to have "one interface for each action"
however here we are dealing with a consistent paradigm ("pinching")
which has three parts; I feel it's better to have one interface
forcing the consumer to have the three calls (no problem if empty) */


public class PinchInputModule : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler

{
    private IPinchHandler pinchHandler = null;

    private const int NO_POINTER_ID = -1;
    private int firstFingerID = NO_POINTER_ID;
    private int secondFingerID = NO_POINTER_ID;
    private int fingersDownCount = 0;
    private bool isPinching = false;

    private Vector2 firstFingerPosition = Vector2.zero;
    private Vector2 secondFingerPosition = Vector2.zero;
    private float previousDistance = 0f;
    private float delta = 0f;

    void Awake()
    {
        pinchHandler = GetComponent(typeof(IPinchHandler)) as IPinchHandler;
    }

    public void OnBeginDrag(PointerEventData data)
    {
        IncreaseFingerCountByOne();

        if (DidPlayerPutDownTheirFirstFinger())
        {
            firstFingerID = data.pointerId;
            firstFingerPosition = data.position;

            return;
        }

        if (DidPlayerPutDownAnotherFinger())
        {
            secondFingerID = data.pointerId;
            secondFingerPosition = data.position;

            CalculateDelta();
            StartPinching();
        }

        bool DidPlayerPutDownTheirFirstFinger() => firstFingerID == NO_POINTER_ID && fingersDownCount == 1;
        bool DidPlayerPutDownAnotherFinger() => firstFingerID != NO_POINTER_ID && secondFingerID == NO_POINTER_ID && fingersDownCount == 2;

        void StartPinching()
        {
            isPinching = true;
            if (pinchHandler != null) pinchHandler.OnPinchStart();
        }
    }

    public void OnEndDrag(PointerEventData data)
    {
        DecreaseFingerCountByOne();

        if (firstFingerID == data.pointerId)
        {
            firstFingerID = NO_POINTER_ID;
            StopPinching();
        }

        if (secondFingerID == data.pointerId)
        {
            secondFingerID = NO_POINTER_ID;
            StopPinching();
        }

        void StopPinching()
        {
            if (isPinching)
            {
                isPinching = false;
                if (pinchHandler != null) pinchHandler.OnPinchEnd();
            }
        }
    }

    public void OnDrag(PointerEventData data)
    {

        if (firstFingerID == data.pointerId)
        {
            firstFingerPosition = data.position;
            CalculateDelta();
        }

        if (secondFingerID == data.pointerId)
        {
            secondFingerPosition = data.position;
            CalculateDelta();
        }

        if (ShouldZoom())
            pinchHandler.OnPinchZoom(delta);

        bool ShouldZoom()
        {
            return isPinching && (data.pointerId == firstFingerID || data.pointerId == secondFingerID) && fingersDownCount == 2 && pinchHandler != null;
        }
    }

    private void IncreaseFingerCountByOne() => fingersDownCount += 1;
    private void DecreaseFingerCountByOne() => fingersDownCount -= 1;

    private void CalculateDelta()
    {
        float newDistance = Vector2.Distance(firstFingerPosition, secondFingerPosition);
        delta = newDistance - previousDistance;
        previousDistance = newDistance;
    }
}