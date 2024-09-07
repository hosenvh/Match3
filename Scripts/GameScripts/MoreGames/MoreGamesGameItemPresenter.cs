using System;
using SeganX;
using UnityEngine;
using UnityEngine.UI;


namespace Match3.MoreGames
{
    public class MoreGamesGameItemPresenter : MonoBehaviour
    {
        [SerializeField] private Image gameIcon;
        [SerializeField] private LocalText gameTitle;
        [SerializeField] private Button gameButton;

        public void Setup(Sprite gameSprite, string title, Action onButtonClick)
        {
            gameIcon.sprite = gameSprite;
            gameTitle.SetText(title);
            gameButton.onClick.AddListener(onButtonClick.Invoke);
        }
    }
}