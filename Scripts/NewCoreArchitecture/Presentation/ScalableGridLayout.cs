using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



namespace Match3.Presentation
{
    [RequireComponent(typeof(VerticalLayoutGroup))]
    public class ScalableGridLayout : MonoBehaviour
    {
        public HorizontalLayoutGroup row;
        public GameObject cell;
        public int narrowScreenMaxColumn;
        public int wideScreenMaxColumn;
        
        [Space(10)] 
        public bool scaling;
        public float narrowScreenScale;
        public float wideScreenScale;

        private int maxColumn;
        
        private List<HorizontalLayoutGroup> rows = new List<HorizontalLayoutGroup>();
        private Dictionary<int, List<Transform>> gridCells = new Dictionary<int, List<Transform>>();
        
        private bool isNarrowScreen;
        
        
        void Awake()
        {
            isNarrowScreen = IsNarrowScreen();
            maxColumn = isNarrowScreen ? narrowScreenMaxColumn : wideScreenMaxColumn;
            if (scaling)
                InitScale();
        }
        
        public void AddElementToGrid(Transform element)
        {
            var newCell = GetNewCell();
            element.SetParent(newCell);
            element.position = newCell.position;
        }
        

        public Transform GetNewCell()
        {
            if (rows.Count == 0)
            {
                rows.Add(Instantiate(row, transform));
                gridCells.Add(0, new List<Transform>());
            }
            
            var lastRowIndex = rows.Count - 1;
            var lastRowCells = gridCells[lastRowIndex];
            var generatedCell = Instantiate(cell).transform;

            if (lastRowCells.Count < maxColumn)
            {
                lastRowCells.Add(generatedCell);
            }
            else
            {
                lastRowIndex++;
                rows.Add(Instantiate(row, transform));
                gridCells.Add(lastRowIndex, new List<Transform>());
            }
            
            generatedCell.SetParent(rows.LastOne().transform, false);
            
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.AsRectTransform());
            LayoutRebuilder.ForceRebuildLayoutImmediate(rows.LastOne().transform.AsRectTransform());
            
            return generatedCell;
        }

        private bool IsNarrowScreen()
        {
            var screenHeight = Screen.height;
            var screenWidth = Screen.width;
            var screenAspectRatio = screenHeight / screenWidth;
            return Math.Abs(screenAspectRatio - 4.0f / 3.0f) < 0.01f;
        }

        private void InitScale()
        {
            if (isNarrowScreen)
                transform.localScale = Vector3.one * narrowScreenScale;
            else
                transform.localScale = Vector3.one * wideScreenScale; 
        }

    }    
}


