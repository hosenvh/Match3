using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Match3.Presentation.Bubbles
{
    public class GridyBubble : BubbleBase
    {
        [SerializeField] private BeardyGridLayoutGroup layoutGroup;
        [SerializeField] private float verticalGridSizeOffset;
        [SerializeField] private float horizontalGridSizeOffset;
        [SerializeField] private Vector2 gridChildsPosition;

        private List<RectTransform> gridChilds;

        public GridyBubble SetGameObjectsAsChild(List<GameObject> gameObjects, float scale = 1)
        {
            if (gridChilds == null)
                gridChilds = new List<RectTransform>();

            DestroyGridChilds();
            SetGameObjectsAsGridChild();
            return this;

            void DestroyGridChilds()
            {
                foreach (RectTransform gridChild in gridChilds)
                    Destroy(gridChild.gameObject);

                gridChilds.Clear();
            }

            void SetGameObjectsAsGridChild()
            {
                foreach (GameObject gameObject in gameObjects)
                {
                    GameObject gridChild = new GameObject("GridObject", typeof(RectTransform));
                    SetGridObjectParent();
                    SetAssignedGameObjectParent();
                    gridChilds.Add(gridChild.GetRectTransform());

                    void SetGridObjectParent()
                    {
                        gridChild.transform.SetParent(bubbleMainPart);
                        gridChild.transform.localScale = Vector3.one;
                        gridChild.transform.localPosition = Vector3.zero;
                    }

                    void SetAssignedGameObjectParent()
                    {
                        gameObject.transform.SetParent(gridChild.transform);
                        gameObject.transform.localScale = Vector3.one * scale;
                        gameObject.transform.localPosition = gridChildsPosition;
                    }
                }
            }
        }

        public override BubbleBase SetAutoSize()
        {
            StartCoroutine(SetAutoSizeCoroutine());
            return this;
        }

        private IEnumerator SetAutoSizeCoroutine()
        {
            yield return new WaitForEndOfFrame();

            SetBubbleSize();

            void SetBubbleSize()
            {
                bubbleMainPart.sizeDelta = new Vector2(GetGridHorizontalSize(), GetGridVerticalSize());
            }

            float GetGridHorizontalSize()
            {
                float horizontalItemSize = layoutGroup.cellSize.x + layoutGroup.spacing.x;
                return (GetMaxRowItemsCount() + horizontalGridSizeOffset) * horizontalItemSize;
            }

            int GetMaxRowItemsCount()
            {
                return gridChilds.Count >= layoutGroup.constraintCount ? layoutGroup.constraintCount : gridChilds.Count;
            }

            float GetGridVerticalSize()
            {
                float verticalItemSize = layoutGroup.cellSize.y + layoutGroup.spacing.y;
                return (GetRowCount() + verticalGridSizeOffset) * verticalItemSize;
            }

            int GetRowCount()
            {
                return gridChilds.Count / (layoutGroup.constraintCount + 1) + 1;
            }
        }
    }
}