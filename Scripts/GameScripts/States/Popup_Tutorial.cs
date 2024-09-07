using System;
using Match3.Presentation.Gameplay;
using SeganX;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class Popup_Tutorial : GameState
{

    private const float acceptablePositionOffset = 5f;
    
    [ContextMenu("CopyData")]
    public void CopyData()
    {
        tutorialConfig.CopyData(this);
    }

    #region fields
    [SerializeField]
    TutorialConfig tutorialConfig = default;
    [SerializeField]
    private GameObject touchCancelGameObject = default;
    public RectTransform[] darkImages, noAlphaImages, pointers;

    [SerializeField] private RectTransform pointerSample = null,
        dialogueRectTransform = null,
        darkImageSample = null,
        noAlphaImageSample = null;
    
    public RectTransform alignParentRectTransform = null;
    
    [SerializeField]
    LocalText dialogueText = default;
    
    public Transform targetTransform;

    [SerializeField] RectTransform ncDialogueTransform = default;
    [SerializeField] LocalText ncDialogueText = default;

    Action onTouchCancelClick;
    #endregion

    #region properties
    public RectTransform DialogueRectTransform { get { return dialogueRectTransform; } }
    #endregion

    public void Setup(TutorialConfig tutorialConfig, Action onTouchCancelClick)
    {
        
        if (tutorialConfig.align == Align.Left)
        {
            alignParentRectTransform.anchorMin = new Vector2(0, .5f);
            alignParentRectTransform.anchorMax = new Vector2(0, .5f);
        }
        else if (tutorialConfig.align == Align.Right)
        {
            alignParentRectTransform.anchorMin = new Vector2(1, .5f);
            alignParentRectTransform.anchorMax = new Vector2(1, .5f);
        }

        if (tutorialConfig.hasDynamicPosition)
        {
            var currentPos = GameObject.Find(tutorialConfig.targetObjectName).transform.position;

            if (Vector2.Distance(tutorialConfig.expectingPosition, currentPos) > acceptablePositionOffset)
            {
                var offset = currentPos - tutorialConfig.expectingPosition;
                alignParentRectTransform.position = tutorialConfig.baseAlignPosition + offset;
            }
        }
        
        //TODO: I'm not sure if we should handle this in other way because of localization
        if (string.IsNullOrEmpty(tutorialConfig.GetLocalizedDialogue()))
            dialogueRectTransform.gameObject.SetActive(false);
        else
        {
            dialogueText.SetText(tutorialConfig.GetLocalizedDialogue().Replace("\\n", "\n"));
            dialogueRectTransform.anchoredPosition = tutorialConfig.dialoguePosition;
        }
        
        if (string.IsNullOrEmpty(tutorialConfig.neighborhoodMessage))
            ncDialogueTransform.gameObject.SetActive(false);
        else
        {
            ncDialogueText.SetText(tutorialConfig.neighborhoodMessage.Replace("\\n", "\n"));
            ncDialogueTransform.anchoredPosition = tutorialConfig.dialoguePosition;
        }

        darkImages = new RectTransform[tutorialConfig.darkRects.Length];
        for (int i = 0; i < tutorialConfig.darkRects.Length; i++)
        {
            Rect item = tutorialConfig.darkRects[i];
            darkImages[i] = Instantiate(darkImageSample, alignParentRectTransform);
            darkImages[i].anchoredPosition = new Vector2(item.x, item.y);
            darkImages[i].SetAnchordWidth(item.width);
            darkImages[i].SetAnchordHeight(item.height);
        }

        noAlphaImages = new RectTransform[tutorialConfig.noAlphaRects.Length];
        for (int i = 0; i < tutorialConfig.noAlphaRects.Length; i++)
        {
            Rect item = tutorialConfig.noAlphaRects[i];
            noAlphaImages[i] = Instantiate(noAlphaImageSample, alignParentRectTransform);
            noAlphaImages[i].anchoredPosition = new Vector2(item.x, item.y);
            noAlphaImages[i].SetAnchordWidth(item.width);
            noAlphaImages[i].SetAnchordHeight(item.height);
        }

        pointers = new RectTransform[tutorialConfig.tutorialPointers.Length];
        for (int i = 0; i < tutorialConfig.tutorialPointers.Length; i++)
        {
            TutorialPointer item = tutorialConfig.tutorialPointers[i];
            pointers[i] = Instantiate(pointerSample, alignParentRectTransform);
            pointers[i].anchoredPosition = item.position;
            switch (item.dir)
            {
                case Dir.Left:
                    pointers[i].rotation = Quaternion.Euler(0, 0, 270);
                    break;
                case Dir.Right:
                    pointers[i].rotation = Quaternion.Euler(0, 0, 90);
                    break;
                case Dir.Up:
                    pointers[i].rotation = Quaternion.Euler(0, 0, 180);
                    break;
            }
        }

        if (tutorialConfig.touchCancel)
            touchCancelGameObject.SetActive(true);
        this.onTouchCancelClick = onTouchCancelClick;

        TryApplyGameBoardScale();
    }

    public void OnTouchCancel()
    {
        onTouchCancelClick();
    }

    public override void Back()
    {
    }

    // NOTE: Since GameplayState may change the scale of the board, it is needed to consider this scaling for gameplay tutorials.
    private void TryApplyGameBoardScale()
    {
        if (gameManager.CurrentState is GameplayState gameplayState)
            alignParentRectTransform.Scale(gameplayState.BoardScale, gameplayState.BoardScale, 1);
    }

}