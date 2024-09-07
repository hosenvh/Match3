using Match3.Foundation.Unity;
using UnityEngine;



namespace Match3.Presentation.MainMenu
{
    public abstract class MainMenuButton : MonoBehaviour
    {
        public string id;
        public Transform button;
        public ResourceGameObjectAsset buttonPrefab;
        public ResourceGameObjectAsset controllerPrefab;

        public abstract Transform Create(Transform buttonParent, Transform buttonControllerParent);
        public abstract bool CreationCondition();
        public abstract MainMenuButtonSetting GetSetting();
    }
}