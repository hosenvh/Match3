using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Physics;
using Match3.Presentation.Gameplay;
using Match3.Presentation.Gameplay.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Development
{
    public class DownwardFlowPresenter : MonoBehaviour
    {

        public Image flowImagePrefab;

        bool isInitialized = false;
        bool isShowing = false;

        List<Image> flowImages = new List<Image>();

        public void Toggle()
        {
            if (!isInitialized)
                Initialize();

            
            SetShowing(!isShowing);



        }

        private void Update()
        {
            if (isShowing)
                UpdateColors();
        }


        private void SetShowing(bool show)
        {
            isShowing = show;

            foreach (var image in flowImages)
                image.gameObject.SetActive(show);
                
        }

        private void Initialize()
        {
            var gameplayController = GameObject.FindObjectOfType<GameplayState>().gameplayController;


            var cellGrid = gameplayController.GameBoard().CellStackBoard();

            var iterator = new LeftToRightTopDownGridIterator<CellStack>(cellGrid);

            foreach (var element in iterator)
            {

                var image = Instantiate(flowImagePrefab, this.transform, false);

                if(element.value.GetComponent<CellStackPresenter>()!= null)
                    image.transform.position = element.value.GetComponent<CellStackPresenter>().transform.position;

                flowImages.Add(image);
            }

            isInitialized = true;
        }


        private void UpdateColors()
        {
            var gameplayController = GameObject.FindObjectOfType<GameplayState>().gameplayController;
            var physicsSystem = gameplayController.GetSystem<PhysicsSystem>();

            var cellGrid = gameplayController.GameBoard().CellStackBoard();

            var iterator = new LeftToRightTopDownGridIterator<CellStack>(cellGrid);

            foreach (var element in iterator)
            {

                Color color;
                var pos = element.value.Position();
                if (physicsSystem.IsDownwardFlowStoppedFor(pos))
                    color = Color.red;
                else
                    color = Color.green;
                color.a = 0.25f;

                flowImages[cellGrid.PositionToLinearIndex(pos.x, pos.y)].color = color;
            }



        }
    }
}