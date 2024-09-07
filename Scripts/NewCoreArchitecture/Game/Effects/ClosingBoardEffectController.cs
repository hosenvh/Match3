using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Game.Effects
{
    public class ClosingBoardEffectController : MonoBehaviour
    {
        // ----------------------------------------------------- Static Fields ----------------------------------------------------- \\

        private static readonly int AlphaSlider = Shader.PropertyToID("_AlphaSlider");
        private static readonly int AlphaMap = Shader.PropertyToID("_AlphaMap");

        // ----------------------------------------------------- Public / Serialized Fields ----------------------------------------------------- \\ 


        [SerializeField] private Camera fxCamera = default;
        [SerializeField] private GameObject effectCameraObject = default;
        [SerializeField] private GameObject boardPlaneObject = default;

        [SerializeField] private Transform boardParentTransform = default;
//    [SerializeField] private RawImage boardFxImage = default;

        [Space(10)] [SerializeField] private Material effectMaterial = default;

        [Space(10)] [SerializeField] private float closingTime = default;
        [SerializeField] private float maxAlphaSliderValue = default;

        [Space(10)] [SerializeField] private int alphaMapHorizontalCells = 12;
        [SerializeField] private int alphaMapVerticalCells = 9;
        [SerializeField] private float alphaMapXOffset = -0.25f;
        [SerializeField] private float alphaMapYOffset = 0.11f;

        [Space(10)] [SerializeField] private Canvas boardCanvas = default;
//    [SerializeField] private CanvasScaler boardCanvasScaler = default;


        // ----------------------------------------------------- Private Fields ----------------------------------------------------- \\

        private Action onCloseBoard;

        private bool isStarted = false;
        private float currentAlphaSliderValue = 0;

        private float alphaMapHalfXOffset;
        private float alphaMapHalfYOffset;

        private bool alphaMapHorizontalSizeIsEven = false;
        private bool alphaMapVerticalSizeIsEven = false;


        // ============================================================================================================================== \\

        
        #if UNITY_EDITOR
        private void Awake()
        {
            effectMaterial = Instantiate(effectMaterial);
        }
        #endif
        
        
        private void Start()
        {
            alphaMapHalfXOffset = alphaMapXOffset * 0.5f;
            alphaMapHalfYOffset = alphaMapYOffset * 0.5f;

            alphaMapHorizontalSizeIsEven = alphaMapHorizontalCells % 2 == 0;
            alphaMapVerticalSizeIsEven = alphaMapVerticalCells % 2 == 0;
        }


        public void CloseBoard(Vector2 boardSize, Action onCloseBoard)
        {
            if (isStarted) return;
            isStarted = true;

            this.onCloseBoard = onCloseBoard;

            fxCamera.orthographicSize =
                (boardPlaneObject.GetRectTransform().rect.height * boardCanvas.transform.localScale.y) * 0.5f;

            boardCanvas.renderMode = RenderMode.WorldSpace;

            effectCameraObject.SetActive(true);
            boardPlaneObject.SetActive(true);

            var position = boardParentTransform.position;
            position = new Vector3(position.x, position.y, position.z + 100);
            boardParentTransform.position = position;

            currentAlphaSliderValue = 0;
            effectMaterial.SetFloat(AlphaSlider, 0);

            Vector2 offset = Vector2.zero;

            if (alphaMapHorizontalSizeIsEven != (boardSize.x % 2 == 0))
            {
                offset.x = alphaMapHalfXOffset;
            }

            if (alphaMapVerticalSizeIsEven != (boardSize.y % 2 == 0))
            {
                offset.y = alphaMapHalfYOffset;
            }

            effectMaterial.SetTextureOffset(AlphaMap, offset);

            StartCoroutine(ClosingBoard());
        }


        IEnumerator ClosingBoard()
        {
            while (currentAlphaSliderValue < maxAlphaSliderValue)
            {
                currentAlphaSliderValue += (Time.deltaTime * maxAlphaSliderValue) / closingTime;
                currentAlphaSliderValue = Mathf.Min(currentAlphaSliderValue, maxAlphaSliderValue);
                effectMaterial.SetFloat(AlphaSlider, currentAlphaSliderValue);
                yield return null;
            }

            effectCameraObject.SetActive(false);
            boardPlaneObject.SetActive(false);
            isStarted = false;

            onCloseBoard();
        }
    }
}