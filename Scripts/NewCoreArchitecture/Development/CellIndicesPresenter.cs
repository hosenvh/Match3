using Match3.Game.Gameplay.Core;
using Match3.Presentation.Gameplay;
using Match3.Presentation.Gameplay.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Match3.Development
{
    public class CellIndicesPresenter : MonoBehaviour
    {
        public Text textPrefab;

        bool isInitialized = false;
        bool isShowing = false;

        List<Text> texts = new List<Text>();

        public void Toggle()
        {
            if (!isInitialized)
                Initialize();

            SetShowing(!isShowing);
        }

        private void SetShowing(bool show)
        {
            isShowing = show;

            foreach (var image in texts)
                image.gameObject.SetActive(show);
        }

        private void Initialize()
        {
            var gameplayController = GameObject.FindObjectOfType<GameplayState>().gameplayController;

            var cellGrid = gameplayController.GameBoard().CellStackBoard();
            var iterator = new LeftToRightTopDownGridIterator<CellStack>(cellGrid);

            foreach (var element in iterator)
            {

                var text = Instantiate(textPrefab, this.transform, false);
                text.text = string.Format("{0}\n{1} ", cellGrid.PositionToLinearIndex(element.x, element.y).ToString(), element.value.Position());
                if (element.value.GetComponent<CellStackPresenter>() != null)
                    text.transform.position = element.value.GetComponent<CellStackPresenter>().transform.position;
                texts.Add(text);
            }

            isInitialized = true;
        }

    }
}