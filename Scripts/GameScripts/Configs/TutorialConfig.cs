using I2.Loc;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using SeganX;
using UnityEngine.UI;

public enum Align { Center, Left, Right }

[CreateAssetMenu(menuName = "Tutorial Config")]
public class TutorialConfig : ScriptableObject
{
    public void CopyData(Popup_Tutorial popup_Tutorial)
    {
        darkRects = new Rect[popup_Tutorial.darkImages.Length];
        for (int i = 0; i < popup_Tutorial.darkImages.Length; i++)
            darkRects[i] = new Rect(popup_Tutorial.darkImages[i].anchoredPosition, popup_Tutorial.darkImages[i].sizeDelta);

        noAlphaRects = new Rect[popup_Tutorial.noAlphaImages.Length];
        for (int i = 0; i < popup_Tutorial.noAlphaImages.Length; i++)
            noAlphaRects[i] = new Rect(popup_Tutorial.noAlphaImages[i].anchoredPosition, popup_Tutorial.noAlphaImages[i].sizeDelta);

        tutorialPointers = new TutorialPointer[popup_Tutorial.pointers.Length];
        for (int i = 0; i < popup_Tutorial.pointers.Length; i++)
            tutorialPointers[i] = new TutorialPointer { position = popup_Tutorial.pointers[i].anchoredPosition };
        
        dialoguePosition = popup_Tutorial.DialogueRectTransform.anchoredPosition;

        if (hasDynamicPosition && popup_Tutorial.targetTransform != null)
        {
            expectingPosition = popup_Tutorial.targetTransform.position;
            baseAlignPosition = popup_Tutorial.alignParentRectTransform.position;
            targetObjectName = popup_Tutorial.targetTransform.gameObject.name;
        }
        
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        EditorApplication.update.Invoke();
#endif
    }

    
    public string GetLocalizedDialogue()
    {
        return dialogueLocalizedString;
    }
    
    
    #region fields

    [Header("Dynamic Position Setting")]
    public bool hasDynamicPosition;
    public Vector3 expectingPosition;
    public Vector3 baseAlignPosition;
    public string targetObjectName;

    [Space(10)]
    [PersianPreview]
    public string dialogueString;
    public LocalizedStringTerm dialogueLocalizedString;
    public Vector2 dialoguePosition;
    public int analyticId;
    public bool touchCancel = false;
    public Align align;
    //public page_names page_name;
    public TutorialPointer[] tutorialPointers;
    public Rect[] darkRects, noAlphaRects;

    public string neighborhoodMessage;
    #endregion
}

[System.Serializable]
public class TutorialPointer
{
    public Vector2 position;
    public Dir dir;
}