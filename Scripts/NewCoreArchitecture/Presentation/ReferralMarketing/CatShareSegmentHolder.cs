using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace Match3.Presentation.ReferralMarketing
{
    
    public class CatShareSegmentHolder : MonoBehaviour
    {
        public Button shareButton;

        public void Setup(UnityAction onButtonClick)
        {
            shareButton.onClick.AddListener(onButtonClick);
        }

    }
    
}
