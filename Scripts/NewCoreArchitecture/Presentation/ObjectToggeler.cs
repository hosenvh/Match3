using UnityEngine;

namespace Match3.Presentation
{
    public class ObjectToggeler : MonoBehaviour
    {
        public void Toggle()
        {
            this.gameObject.SetActive(!this.gameObject.activeSelf);
        }
    }
}