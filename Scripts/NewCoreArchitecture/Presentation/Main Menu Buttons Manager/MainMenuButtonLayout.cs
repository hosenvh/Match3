using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Match3.Presentation.MainMenuLayout
{
    
    public enum AlignmentSide
    {
        Left = 1, Right = -1
    }
    
    public class MainMenuButtonLayout : LayoutGroup
    {
        
        public AlignmentSide alignmentSide;
    
        public Vector2 cellSize;
        public Vector2 spacing;
    
        public int rowCount;
        public int extraRowCount;

        public float normalScale;
        public float overflowScale;
    
    
        public override void CalculateLayoutInputVertical()
        {
            base.CalculateLayoutInputHorizontal();
        
            var scale = transform.GetActiveChildCount() <= rowCount ? normalScale : overflowScale;
            rectTransform.localScale = new Vector3(scale, scale, 1);

            var totalRows = rowCount + extraRowCount;

            int column = 0;
            int row = 0;
        
            for (int i = 0; i < rectChildren.Count; i++)
            {
                column = Mathf.FloorToInt((float) i / totalRows);
                row = i - totalRows * column;
                column *= (int) alignmentSide;
                var xPos = (cellSize.x * column) + (spacing.x * column) + padding.left;
                var yPos = (cellSize.y * row) + (spacing.y * row) + padding.top;
            
                var item = rectChildren[i];
                item.localPosition = Vector3.zero;
                SetChildAlongAxis(item, 0, xPos, cellSize.x);
                SetChildAlongAxis(item, 1, yPos, cellSize.y);
            }
        }

        public override void SetLayoutHorizontal() { }
        public override void SetLayoutVertical() { }
    
    }

}

