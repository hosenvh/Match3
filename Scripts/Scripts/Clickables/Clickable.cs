using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(BoxCollider))]
[ExecuteInEditMode]
public class Clickable : MonoBehaviour, IClickable
{
    
    public float clickErrorTime = 0.15f;

    public UnityEvent onClickDown;
    public UnityEvent onClickUp;
    
    private float clickDownTime;

    public void ClickDown()
    {
        clickDownTime = Time.time;
        onClickDown.Invoke();
    }
    
    public void ClickUp()
    {
        if (Time.time - clickDownTime >= clickErrorTime) return;
        onClickUp.Invoke();
    }

    
#if UNITY_EDITOR

    private void OnEnable()
    {
        gameObject.layer = ClickablesInputsController.ClickableDetectionLayer;
        var boxCollider = GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        
        var size = boxCollider.size;
        size = new Vector3(size.x, size.y, 0.3f);
        boxCollider.size = size;
    }

#endif
    
    
}
