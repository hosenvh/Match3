using System;
using UnityEngine;
using Match3.Presentation.TextAdapting;

namespace Match3.Presentation.Advertisement
{
    public class ParrotAdvertisementHolder : MonoBehaviour
    {
        [SerializeField] GameObject winBadge;
        [SerializeField] GameObject loseBadge;
        [SerializeField] TextAdapter loseBadgeText;

        public TextAdapter messageText;

        Action onClickedAction;

        public void SetupForWin(string message, Action onClickedAction)
        {
            NeutralSetup(message, onClickedAction);
            loseBadge.SetActive(false);
            winBadge.SetActive(true);
        }

        public void SetupForLose(string message, int extraMoves, Action onClickedAction)
        {
            NeutralSetup(message, onClickedAction);
            winBadge.SetActive(false);
            loseBadge.SetActive(true);
            loseBadgeText.SetText("+" + extraMoves.ToString());
        }

        void NeutralSetup(string message, Action onClickedAction)
        {
            messageText.SetText(message);
            this.onClickedAction = onClickedAction;
        }

        public void OnClicked()
        {
            Base.gameManager.fxPresenter.PlayClickAudio();
            onClickedAction.Invoke();
        }
    }
}