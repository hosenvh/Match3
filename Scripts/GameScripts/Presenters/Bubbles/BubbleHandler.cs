using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;


namespace Match3.Presentation.Bubbles
{
    public class BubbleHandler<TBubble> where TBubble : BubbleBase
    {
        private const string PREFAB_PATH = "Prefabs/UI/";
        private TBubble bubble;

        public BubbleHandler()
        {
            bubble = InstantiateBubble();
        }

        private TBubble InstantiateBubble()
        {
            TBubble bubblePrefab = Resources.Load<TBubble>(PREFAB_PATH + typeof(TBubble).Name);
            TBubble instantiatedBubble = Object.Instantiate(bubblePrefab, Base.gameManager.canvas.transform);
            return instantiatedBubble;
        }

        public TBubble SetOpenerButton(Button openerButton)
        {
            openerButton.onClick.AddListener(() => OpenBubble());

            return bubble;
        }

        public TBubble OpenBubble()
        {
            if (bubble == null)
                bubble = InstantiateBubble();

            bubble.Open<TBubble>();
            return bubble;
        }

        public BubbleBase CloseBubble()
        {
            if (bubble != null)
            {
                bubble.Close();
            }

            return bubble;
        }
    }
}