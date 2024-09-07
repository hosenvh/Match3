using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;


public class MapCameraController : Base, ISingleFingerHandler, IPinchHandler
{
    public Camera mapCamera = null;
    public Transform cameraParentTransform = null;
    [SerializeField]
    float cameraMinSize = 0, cameraMaxSize = 0;
    [SerializeField]
    Vector2 cameraMinPos = Vector2.zero, cameraMaxPos = Vector2.zero;
    Vector2 dynamicCameraMinPos, dynamicCameraMaxPos;
    Vector3 moveTargetPosition;
    float zoomTargetValue;
    bool focusing = false;
    readonly float maxMoveSpeed = 8f, maxZoomSpeed = 2.5f;
    float moveSpeed, zoomSpeed;

    [FormerlySerializedAs("CanUserControlCamera")] public bool canUserControlCamera = true;

    private const float SLIDE_THRESHOLD = 0.01f;
    private const float SLIDE_DECELERATION_FACTOR = 8;

    private Vector3 cameraMovement;
    private bool shouldSlide;
    private bool isPinching;

    public Vector3 OffsetBetweenCameraAndParent => new Vector3(
        cameraParentTransform.position.x - mapCamera.transform.position.x,
        cameraParentTransform.position.y - mapCamera.transform.position.y,
        cameraParentTransform.position.z - mapCamera.transform.position.z);

    string GetStartPosString(int vectorIndex) { return "CameraStartPos" + vectorIndex.ToString(); }
    public Vector2 CameraStartPosition
    {
        get { return new Vector2(PlayerPrefs.GetFloat(GetStartPosString(0), 155), PlayerPrefs.GetFloat(GetStartPosString(1), -35)); }
        set { PlayerPrefs.SetFloat(GetStartPosString(0), value.x); PlayerPrefs.SetFloat(GetStartPosString(1), value.y); }
    }


    private void Awake()
    {
        moveTargetPosition.y = cameraParentTransform.position.y;
        Vector2 vector2Position = CameraStartPosition;
        cameraParentTransform.position = new Vector3(vector2Position.x, 50, vector2Position.y);
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.mouseScrollDelta.y != 0)
            OnPinchZoom(Input.mouseScrollDelta.y * 10);
#endif

        if (mapCamera.orthographicSize < cameraMinSize)
            mapCamera.orthographicSize = cameraMinSize;
        else if (mapCamera.orthographicSize > cameraMaxSize)
            mapCamera.orthographicSize = cameraMaxSize;

        if (focusing)
        {
            cameraParentTransform.position = Vector3.MoveTowards(cameraParentTransform.position, moveTargetPosition, moveSpeed * Time.deltaTime);
            mapCamera.orthographicSize = Mathf.MoveTowards(mapCamera.orthographicSize, zoomTargetValue, zoomSpeed * Time.deltaTime);
        }

        Vector3 cameraPos = cameraParentTransform.localPosition;

        bool cameraPosChanged = false;
        float alpha = 3 * mapCamera.orthographicSize - 30;
        dynamicCameraMinPos.x = cameraMinPos.x + alpha;
        dynamicCameraMinPos.y = cameraMinPos.y + alpha;
        dynamicCameraMaxPos.x = cameraMaxPos.x - alpha;
        dynamicCameraMaxPos.y = cameraMaxPos.y - alpha;
        if (cameraParentTransform.position.x < dynamicCameraMinPos.x)
        {
            cameraPosChanged = true;
            cameraPos.x = dynamicCameraMinPos.x;
        }
        else if (cameraParentTransform.position.x > dynamicCameraMaxPos.x)
        {
            cameraPosChanged = true;
            cameraPos.x = dynamicCameraMaxPos.x;
        }

        if (cameraParentTransform.position.z < dynamicCameraMinPos.y)
        {
            cameraPosChanged = true;
            cameraPos.z = dynamicCameraMinPos.y;
        }
        else if (cameraParentTransform.position.z > dynamicCameraMaxPos.y)
        {
            cameraPosChanged = true;
            cameraPos.z = dynamicCameraMaxPos.y;
        }
        if (cameraPosChanged)
            cameraParentTransform.position = cameraPos;
    }

    public void OnSingleFingerDown(Vector2 position)
    {
        shouldSlide = false;
    }

    public void OnSingleFingerDrag(Vector2 delta)
    {
        if (!canUserControlCamera) return;

        isPinching = false;
        shouldSlide = true;

        float x = delta.x / Screen.width - delta.y / Screen.height;
        float z = delta.x / Screen.width + delta.y / Screen.height;
        float coefficient = -14 * (mapCamera.orthographicSize / 5);

        cameraMovement = new Vector3(x, 0, z) * coefficient;
        cameraParentTransform.position += cameraMovement;
    }

    public void OnSingleFingerUp(Vector2 position)
    {
        if (shouldSlide && !isPinching)
            StartCoroutine(MoveCameraToOffset());
    }

    private IEnumerator MoveCameraToOffset()
    {
        while (cameraMovement.magnitude > SLIDE_THRESHOLD)
        {
             cameraParentTransform.position += cameraMovement;
             cameraMovement.x = Mathf.Lerp(cameraMovement.x, 0, Time.deltaTime * SLIDE_DECELERATION_FACTOR);
             cameraMovement.z = Mathf.Lerp(cameraMovement.z, 0, Time.deltaTime * SLIDE_DECELERATION_FACTOR);

             if (shouldSlide == false)
                 yield break;

             yield return null;
        }
    }

    public void OnPinchStart()
    {
        isPinching = true;
    }

    public void OnPinchZoom(float delta)
    {
        if (!canUserControlCamera) return;
        isPinching = true;
        mapCamera.orthographicSize -= delta * .015f;
    }

    public void OnPinchEnd()
    {
        isPinching = true;
    }

    public void FocusOnMapItem(Transform item, Action onFinish)
    {
        canUserControlCamera = false;

        var dir = (Quaternion.Euler(30, -45, 0) * Vector3.forward).normalized;
        var x = (dir.x / dir.y) * (50 - item.position.y) + item.position.x;
        var z = (dir.z / dir.y) * (50 - item.position.y) + item.position.z;
        var mainCameraTargetPos = new Vector3(x,50, z);

        var focusPosition = mainCameraTargetPos + OffsetBetweenCameraAndParent;
        Focus(new Vector2(focusPosition.x, focusPosition.z), mapCamera.orthographicSize, ()=>
        {
            canUserControlCamera = true;
            onFinish();
        });
        moveSpeed = maxMoveSpeed + 3;
    }

    public void Focus(Vector2 moveTargetPosition, float zoomTargetValue, Action onFinish, bool shouldFocusInstantly = false)
    {
        if (shouldFocusInstantly)
        {
            this.moveTargetPosition.x = moveTargetPosition.x;
            this.moveTargetPosition.z = moveTargetPosition.y;
            this.zoomTargetValue = zoomTargetValue;

            cameraParentTransform.position = this.moveTargetPosition;
            mapCamera.orthographicSize = this.zoomTargetValue;
            onFinish();

            return;
        }

        focusing = true;


        float distance = Vector2.Distance(moveTargetPosition, new Vector2(cameraParentTransform.position.x, cameraParentTransform.position.z));
        this.moveTargetPosition.x = moveTargetPosition.x;
        this.moveTargetPosition.z = moveTargetPosition.y;

        float delta = Mathf.Abs(zoomTargetValue - mapCamera.orthographicSize);
        this.zoomTargetValue = zoomTargetValue;

        float duration = Mathf.Max(delta / maxZoomSpeed, distance / maxMoveSpeed);
        if (duration > 0)
        {
            moveSpeed = distance / duration;
            zoomSpeed = delta / duration;
        }

        DelayCall(duration, () =>
        {
            focusing = false;

            cameraParentTransform.position = this.moveTargetPosition;
            mapCamera.orthographicSize = this.zoomTargetValue;

            onFinish();
        });
    }

    public Camera GetMapCamera() { return mapCamera; }
}