using System;

public class MapItem_Element_SpriteRenderer : MapItem_Element
{
    bool isShowing = false;
    int currentStateIndex;

    private void Awake()
    {
        if (!isShowing)
            SetActiveAll(false);
    }

    public override void ShowElementState(int index, bool withAnimation, Action onFinish)
    {
        if (index > 0)
        {
            isShowing = true;
            SetActiveAll(true);
            if (withAnimation)
            {
                if (currentStateIndex == 0)
                    PlayUnityAnimation(CurrentMapState.mapItem_spriteRendererAnimationPrefab, true, onFinish);
            }
        }
        else
        {
            if (withAnimation)
            {
                PlayUnityAnimation(CurrentMapState.mapItem_spriteRendererAnimationPrefab, false, () =>
                {
                    //gameObject.SetActive(false);
                    if (onFinish != null)
                        onFinish();
                });
            }
            else
                SetActiveAll(false);
        }

        currentStateIndex = index;
    }
}
